/// Author:			        Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2015-08-21
/// Last Modified:			2015-08-21
/// 2015-10-23: Enable shopping cart with product attributes

using System;
using CanhCam.Web.Framework;
using CanhCam.Business;
using CanhCam.Business.WebHelpers;
using Resources;
using log4net;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Linq;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;
using System.Data;
using CanhCam.Web.Editor;
using System.Web.Script.Serialization;
using System.Web;
using System.Web.Hosting;
using System.Globalization;
using System.IO;

namespace CanhCam.Web.ProductUI
{
    public partial class OrderEditPage : CmsNonBasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OrderEditPage));

        private Order order;
        private List<Product> lstProducts = new List<Product>();
        private List<OrderItem> lstOrderItems = new List<OrderItem>();

        List<ProductProperty> productProperties = new List<ProductProperty>();
        List<CustomField> customFields = new List<CustomField>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                SiteUtils.RedirectToLoginPage(this);
                return;
            }

            if (!WebUser.IsAdminOrContentAdmin && !ProductPermission.CanManageOrders)
            {
                SiteUtils.RedirectToAccessDeniedPage(this);
                return;
            }

            LoadSettings();

            if (order == null)
            {
                SiteUtils.RedirectToAccessDeniedPage(this);
                return;
            }

            if (order.CouponCode.Length > 0)
            {
                var col = grid.Columns.FindByUniqueNameSafe("DiscountAmount");
                if (col != null)
                    col.Visible = true;
            }

            PopulateLabels();

            if (!Page.IsPostBack)
                PopulateControls();
        }

        private void HideControls()
        {
            btnUpdate.Visible = false;
            btnDelete.Visible = false;
            ddOrderStatus.Visible = false;
            litOrderStatus.Visible = true;

            txtBillingFirstName.Visible = false;
            txtBillingLastName.Visible = false;
            txtBillingEmail.Visible = false;
            txtBillingPhone.Visible = false;
            txtBillingAddress.Visible = false;
            ddBillingProvince.Visible = false;
            ddBillingDistrict.Visible = false;

            litBillingFirstName.Visible = true;
            litBillingLastName.Visible = true;
            litBillingEmail.Visible = true;
            litBillingPhone.Visible = true;
            litBillingAddress.Visible = true;
            litBillingProvince.Visible = true;
            litBillingDistrict.Visible = true;
            litOrderStatus.Visible = true;

            txtShippingFirstName.Visible = false;
            txtShippingLastName.Visible = false;
            txtShippingEmail.Visible = false;
            txtShippingPhone.Visible = false;
            txtShippingAddress.Visible = false;
            ddShippingProvince.Visible = false;
            ddShippingDistrict.Visible = false;

            litShippingFirstName.Visible = true;
            litShippingLastName.Visible = true;
            litShippingEmail.Visible = true;
            litShippingPhone.Visible = true;
            litShippingAddress.Visible = true;
            litShippingProvince.Visible = true;
            litShippingDistrict.Visible = true;
        }

        #region "RadGrid Event"

        void grid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (order != null)
            {
                lstOrderItems = OrderItem.GetByOrder(order.OrderId);
                grid.DataSource = lstOrderItems;

                if (lstOrderItems.Count > 1)
                {
                    if ((order.OrderStatus == (int)OrderStatus.New) || (order.OrderStatus == (int)OrderStatus.Processing))
                        grid.MasterTableView.Columns.FindByUniqueName("Delete").Visible = true;
                }

                lstProducts = Product.GetByOrder(siteSettings.SiteId, order.OrderId);

                if (ProductConfiguration.EnableShoppingCartAttributes)
                {
                    var lstProductIds = lstProducts.Select(x => x.ProductId).Distinct().ToList();
                    customFields = CustomField.GetActiveForCart(siteSettings.SiteId, Product.FeatureGuid);
                    if (customFields.Count > 0 && lstProductIds.Count > 0)
                        productProperties = ProductProperty.GetPropertiesByProducts(lstProductIds);
                }

                var codeColumn = grid.MasterTableView.Columns.FindByUniqueNameSafe("ProductCode");
                if (codeColumn != null) codeColumn.Visible = displaySettings.ShowProductCode;

                var priceColumn = grid.MasterTableView.Columns.FindByUniqueNameSafe("OrderPrice");
                if (priceColumn != null) priceColumn.Visible = displaySettings.ShowPrice;

                var totalPriceColumn = grid.MasterTableView.Columns.FindByUniqueNameSafe("OrderTotalPrice");
                if (totalPriceColumn != null) totalPriceColumn.Visible = displaySettings.ShowPrice;
            }
        }

        void grid_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is Telerik.Web.UI.GridDataItem)
            {
                LinkButton btnDelete = e.Item.FindControl("btnDelete") as LinkButton;
                if (btnDelete != null)
                    UIHelper.AddConfirmationDialog(btnDelete, "Bạn có chắc chắn muốn hủy sản phẩm này?");

                Telerik.Web.UI.GridDataItem item = (Telerik.Web.UI.GridDataItem)e.Item;
                Literal litProductCode = (Literal)item.FindControl("litProductCode");
                Literal litProductName = (Literal)item.FindControl("litProductName");
                int productId = Convert.ToInt32(item.GetDataKeyValue("ProductId"));
                litProductCode.Visible = displaySettings.ShowProductCode;

                Product product = ProductHelper.GetProductFromList(lstProducts, productId);
                if (product != null)
                {
                    litProductCode.Text = product.Code;
                    litProductName.Text = string.Format("<a href='{0}'>{1}</a>", ProductHelper.FormatProductUrl(product.Url, product.ProductId, product.ZoneId), product.Title);

                    //if (lstAppliedItems.Count > 0 && coupon != null)
                    //{
                    //    CouponAppliedToItem appliedToItem = CouponAppliedToItem.FindFromList(lstAppliedItems, product.ZoneId, (int)CouponAppliedType.ToCategories);
                    //    if (appliedToItem == null)
                    //        appliedToItem = CouponAppliedToItem.FindFromList(lstAppliedItems, product.ProductId, (int)CouponAppliedType.ToProducts);

                    //    if (appliedToItem != null)
                    //    {
                    //        Literal litGiftProducts = (Literal)item.FindControl("litGiftProducts");
                    //        decimal discountAmount = Convert.ToDecimal(item.GetDataKeyValue("DiscountAmount"));
                    //        string detail = string.Empty;
                    //        string percentage = string.Empty;
                    //        if (appliedToItem.Discount > 0)
                    //        {
                    //            if (appliedToItem.UsePercentage)
                    //                percentage = "<strong>" + ProductHelper.FormatPrice(appliedToItem.Discount, true) + "%</strong> (";
                    //        }
                    //        else
                    //        {
                    //            if (coupon.DiscountType == (int)CouponDiscountType.PercentagePerProduct)
                    //                percentage = "<strong>" + ProductHelper.FormatPrice(coupon.Discount, true) + "%</strong> (";
                    //        }

                    //        detail += "<div><strong>Khuyến mãi: </strong></div>";
                    //        if (discountAmount > 0)
                    //            detail += "<div style='font-size:12px;font-style:italic'> - Ưu đãi: " + percentage + ProductHelper.FormatPrice(discountAmount, ProductHelper.VNDCurrency) + (percentage.Length > 0 ? ")" : "") + "</div>";

                    //        if (
                    //            appliedToItem.GiftCustomProducts.Length > 0
                    //            || appliedToItem.GiftProducts.Length > 0
                    //        )
                    //        {
                    //            detail += "<div style='font-size:12px;font-style:italic'> - Quà tặng: ";

                    //            if (appliedToItem.GiftCustomProducts.Length > 0)
                    //                detail += appliedToItem.GiftCustomProducts;

                    //            if (appliedToItem.GiftProducts.Length > 0)
                    //            {
                    //                var lstGiftProducts = Product.GetPageBySearch(1, 10, siteSettings.SiteId, string.Empty, -1, -1, -1, -1, null, null, -1, -1, null, appliedToItem.GiftProducts, null);
                    //                if (lstGiftProducts.Count > 0)
                    //                    foreach (Product productTmp in lstGiftProducts)
                    //                    {
                    //                        productTmp.OpenInNewWindow = true;
                    //                        detail += "<div>" + ProductHelper.BuildProductLink(productTmp) + "</div>";
                    //                    }
                    //            }

                    //            detail += "</div>";
                    //        }

                    //        litGiftProducts.Text = detail;
                    //    }
                    //}

                    Literal litAttributes = (Literal)item.FindControl("litAttributes");
                    if (litAttributes != null)
                    {
                        string attributesXml = item.GetDataKeyValue("AttributesXml").ToString();
                        string results = string.Empty;

                        if (!string.IsNullOrEmpty(attributesXml))
                        {
                            var attributes = ProductAttributeParser.ParseProductAttributeMappings(customFields, attributesXml);
                            if (attributes.Count > 0)
                            {
                                foreach (var a in attributes)
                                {
                                    var values = ProductAttributeParser.ParseValues(attributesXml, a.CustomFieldId);
                                    if (values.Count > 0)
                                    {
                                        productProperties.ForEach(property =>
                                        {
                                            if (property.ProductId == productId
                                                && property.CustomFieldId == a.CustomFieldId
                                                && values.Contains(property.CustomFieldOptionId))
                                                results += string.Format("<div><span>{0}</span>: {1}</div>", a.Name, property.OptionName);
                                        });
                                    }
                                }
                            }
                        }

                        if (results.Length > 0)
                            litAttributes.Text = string.Format("<div class='attributes'>{0}</div>", results);
                    }
                }
            }
        }

        #endregion

        #region Event

        void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    order.BillingFirstName = txtBillingFirstName.Text.Trim();
                    order.BillingLastName = txtBillingLastName.Text.Trim();
                    order.BillingEmail = txtBillingEmail.Text.Trim();
                    order.BillingAddress = txtBillingAddress.Text.Trim();
                    order.BillingPhone = txtBillingPhone.Text.Trim();
                    if (ddBillingProvince.SelectedValue.Length == 36)
                        order.BillingProvinceGuid = new Guid(ddBillingProvince.SelectedValue);
                    if (ddBillingDistrict.SelectedValue.Length == 36)
                        order.BillingDistrictGuid = new Guid(ddBillingDistrict.SelectedValue);

                    order.ShippingFirstName = txtShippingFirstName.Text.Trim();
                    order.ShippingLastName = txtShippingLastName.Text.Trim();
                    order.ShippingEmail = txtShippingEmail.Text.Trim();
                    order.ShippingAddress = txtShippingAddress.Text.Trim();
                    order.ShippingPhone = txtShippingPhone.Text.Trim();
                    if (ddShippingProvince.SelectedValue.Length == 36)
                        order.ShippingProvinceGuid = new Guid(ddShippingProvince.SelectedValue);
                    if (ddShippingDistrict.SelectedValue.Length == 36)
                        order.ShippingDistrictGuid = new Guid(ddShippingDistrict.SelectedValue);

                    order.OrderNote = txtOrderNote.Text.Trim();

                    bool sendEmail = false;
                    bool updateUserPoints = false;
                    if (order.OrderStatus != Convert.ToInt32(ddOrderStatus.SelectedValue))
                    {
                        sendEmail = true;

                        //if (ddOrderStatus.SelectedValue == "2" && order.UserPoint == 0 && order.UserPointDiscount == 0)
                        updateUserPoints = true;
                    }

                    order.OrderStatus = Convert.ToInt32(ddOrderStatus.SelectedValue);

                    order.ShippingMethod = -1;
                    order.OrderShipping = decimal.Zero;
                    if (rdbListShippingMethod.SelectedIndex > -1)
                    {
                        order.ShippingMethod = Convert.ToInt32(rdbListShippingMethod.SelectedValue);
                        order.OrderShipping = ProductHelper.GetShippingPrice(order.ShippingMethod, order.OrderSubtotal, lstOrderItems.GetTotalWeights(lstProducts), lstOrderItems.GetTotalProducts(), ProductHelper.GetShippingGeoZoneGuidsByOrderSession(order));
                    }

                    order.PaymentMethod = -1;
                    if (rdbListPaymentMethod.SelectedIndex > -1)
                        order.PaymentMethod = Convert.ToInt32(rdbListPaymentMethod.SelectedValue);

                    BindTotal();
                    if (order.Save())
                    {
                        //if (updateUserPoints && order.UserGuid != Guid.Empty)
                        //{
                        //    SiteUser siteUser = new SiteUser(siteSettings, order.UserGuid);
                        //    if (siteUser != null && siteUser.UserId > 0)
                        //        SiteUserEx.UpdateUserPoints(siteUser.UserGuid, siteUser.TotalPosts + (int)(order.OrderSubtotal / 1000));
                        //}

                        if (updateUserPoints)
                            ProductHelper.ProcessUserPoint(siteSettings, order);
                    }

                    LogActivity.Write("Update order's status", order.OrderCode);
                    message.SuccessMessage = ResourceHelper.GetResourceString("Resource", "UpdateSuccessMessage");

                    if (sendEmail)
                    {
                        string orderStatusName = GetOrderStatusName(order.OrderStatus);
                        if (orderStatusName.Length > 0)
                        {
                            string billingProvinceName = string.Empty;
                            string billingDistrictName = string.Empty;
                            string shippingProvinceName = string.Empty;
                            string shippingDistrictName = string.Empty;
                            if (order.BillingProvinceGuid != Guid.Empty)
                            {
                                var province = new GeoZone(order.BillingProvinceGuid);
                                if (province != null && province.Guid != Guid.Empty)
                                    billingProvinceName = province.Name;
                            }
                            if (order.BillingDistrictGuid != Guid.Empty)
                            {
                                var province = new GeoZone(order.BillingDistrictGuid);
                                if (province != null && province.Guid != Guid.Empty)
                                    billingDistrictName = province.Name;
                            }
                            if (order.ShippingProvinceGuid != Guid.Empty)
                            {
                                var province = new GeoZone(order.ShippingProvinceGuid);
                                if (province != null && province.Guid != Guid.Empty)
                                    shippingProvinceName = province.Name;
                            }
                            if (order.ShippingDistrictGuid != Guid.Empty)
                            {
                                var province = new GeoZone(order.ShippingDistrictGuid);
                                if (province != null && province.Guid != Guid.Empty)
                                    shippingDistrictName = province.Name;
                            }

                            string toEmail = order.BillingEmail.Trim();
                            if (
                                order.ShippingEmail.Length > 0
                                && !string.Equals(toEmail, order.ShippingEmail, StringComparison.CurrentCultureIgnoreCase)
                                )
                                toEmail += "," + order.ShippingEmail;

                            //order.OrderStatus
                            lstOrderItems = OrderItem.GetByOrder(order.OrderId);
                            lstProducts = Product.GetByOrder(order.SiteId, order.OrderId);
                            string templateName = "Order" + orderStatusName + ".CustomerNotification";

                            if (ProductHelper.SendOrderPlacedNotification(siteSettings, order, lstProducts, lstOrderItems, templateName, billingProvinceName, billingDistrictName, shippingProvinceName, shippingDistrictName, toEmail))
                                WebTaskManager.StartOrResumeTasks();
                        }
                    }

                    WebUtils.SetupRedirect(this, Request.RawUrl);
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
            }
        }


        void txtOrderDiscountPercentage_TextChanged(object sender, EventArgs e)
        {
            bool yes = false;
            decimal amount = decimal.Zero;
            if (decimal.TryParse(txtOrderDiscountPercentage.Text.Trim(), out amount))
            {
                if (amount >= 0 && amount <= 100)
                {
                    yes = true;
                    txtOrderDiscount.Text = ProductHelper.FormatPrice(order.OrderSubtotal * amount * 0.01M);
                    BindTotal();
                }
            }

            if (!yes)
                txtOrderDiscountPercentage.Text = "0";
        }

        void txtOrderShippingPercentage_TextChanged(object sender, EventArgs e)
        {
            bool yes = false;
            decimal amount = decimal.Zero;
            if (decimal.TryParse(txtOrderShippingPercentage.Text.Trim(), out amount))
            {
                if (amount >= 0 && amount <= 100)
                {
                    yes = true;
                    txtOrderShipping.Text = ProductHelper.FormatPrice(order.OrderSubtotal * amount * 0.01M);
                    BindTotal();
                }
            }

            if (!yes)
                txtOrderShippingPercentage.Text = "0";
        }

        //void txtOrderTaxPercentage_TextChanged(object sender, EventArgs e)
        //{
        //    bool yes = false;
        //    decimal amount = decimal.Zero;
        //    if (decimal.TryParse(txtOrderTaxPercentage.Text.Trim(), out amount))
        //    {
        //        if (amount >= 0 && amount <= 100)
        //        {
        //            yes = true;
        //            txtOrderTax.Text = ProductHelper.FormatPrice(order.OrderSubtotal * amount * 0.01M);
        //            BindTotal();
        //        }
        //    }

        //    if (!yes)
        //        txtOrderTaxPercentage.Text = "0";
        //}

        void txtOrderDiscount_TextChanged(object sender, EventArgs e)
        {
            BindTotal();
        }

        void txtOrderShipping_TextChanged(object sender, EventArgs e)
        {
            BindTotal();
        }

        //void txtOrderTax_TextChanged(object sender, EventArgs e)
        //{
        //    BindTotal();
        //}

        //private void wordbind(Order order)
        //{
        //    try
        //    {
                
        //        if (order != null && !Request.Browser.IsMobileDevice)
        //        {
        //            object missing = Missing.Value;
        //            object EndOfDoc = "\\endofdoc"; /* \endofdoc is a predefined bookmark */
        //            object fileName = "bill-export-" + DateTimeHelper.GetDateTimeStringForFileName() + ".docx";

        //            //Start Word and create a new document.
        //            Word._Application wordApp;
        //            Word._Document FileDoc = null, NewDoc = null, TargetDoc = null;

        //            wordApp = new Word.Application();
        //            wordApp.Visible = false;

        //            //FileDoc = FileWord.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);
        //            string path = HostingEnvironment.MapPath("~/Data/Sites/1/WordTemplate/");
        //            object FileTemplate = path + "thongtindonhang.dotx";


        //            //** Directory
        //            string directory = Directory.GetCurrentDirectory();
        //            string target = @"c:\temp";
        //            if (!Directory.Exists(target))
        //            {
        //                Directory.CreateDirectory(target);
        //            }

        //            // Change the current directory.
        //            Environment.CurrentDirectory = (target);

        //            string targetpath = directory;
        //            object targetname = targetpath + "\\" + fileName;

        //            if (File.Exists((string)FileTemplate))
        //            {
        //                object oTemplate = (FileTemplate);
        //                FileDoc = wordApp.Documents.Add(ref FileTemplate, ref missing, ref missing, ref missing);
        //                object readOnly = false;
        //                object isVisible = false;
        //                //FileDoc = wordApp.Documents.Open(FileTemplate, Type.Missing, true);
        //                //Word.Range oRange = FileDoc.Content;
        //                //oRange.Copy();

        //                //NewDoc = wordApp.Documents.Add();
        //                //NewDoc.Content.PasteSpecial();
        //                //NewDoc.SaveAs(newfile);

        //                //TargetDoc = wordApp.Documents.Add(ref newfile, ref missing, ref missing, ref missing);
        //                decimal number;
        //                //writefield
        //                foreach (Word.Field field in FileDoc.Fields)
        //                {
        //                    string fieldName = field.Code.Text.Substring(11, field.Code.Text.IndexOf("\\") - 12).Trim();
        //                    var properties = order.GetType().GetProperties();

        //                    foreach (var property in properties)
        //                    {
        //                        string value = Convert.ToString(order.GetType().GetProperty(property.Name).GetValue(order, null));
        //                        if (property.Name == fieldName)
        //                        {
        //                            field.Select();

        //                            if (value != string.Empty)
        //                            {
        //                                if (decimal.TryParse(value, out number))
        //                                    wordApp.Selection.TypeText(number.ToString("0"));
        //                                else
        //                                    wordApp.Selection.TypeText(value);
        //                            }
        //                            else
        //                                wordApp.Selection.TypeText("#");
        //                        }
        //                    }
        //                }

        //                DataTable dtOrder = GetOrderForExport(order);

        //                //writetable
        //                Word.Table table = FileDoc.Tables[1];
        //                int lenrow = dtOrder.Rows.Count;
        //                int lencol = dtOrder.Columns.Count;
        //                for (int i = 0; i < lenrow; ++i)
        //                {
        //                    object ob = Missing.Value;
        //                    table.Rows.Add(ref ob);
        //                    for (int j = 0; j < lencol; ++j)
        //                    {
        //                        if (decimal.TryParse(Convert.ToString(dtOrder.Rows[i][j]), out number))
        //                            table.Cell(i + 2, j + 1).Range.Text = number.ToString("0");
        //                        else
        //                            table.Cell(i + 2, j + 1).Range.Text = dtOrder.Rows[i][j].ToString();
        //                    }
        //                }
        //                FileDoc.SaveAs2(targetname);
        //                wordApp.Documents.Close();
        //                wordApp.Quit();

        //                FileInfo file = new FileInfo((string)targetname);
        //                Response.Clear();
        //                Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
        //                Response.AddHeader("Content-Length", file.Length.ToString());
        //                //Response.ContentType = "application/octet-stream";
        //                //Response.ContentType = "application/vnd.openxmlformats - officedocument.wordprocessingml.document";
        //                Response.ContentType = "application/ms-word";
        //                Response.WriteFile(file.FullName);
        //                Response.Flush();
        //                Response.Clear();
        //                file.Delete();
        //                //Response.End();
        //            }
        //            else
        //            {
        //                log.Error("This file does not exist.");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message);
        //    }
        //}

        private DataTable GetOrderForExport(Order order)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SẢN PHẨM", typeof(string));
            dt.Columns.Add("ĐƠN GIÁ", typeof(string));
            dt.Columns.Add("SỐ LƯỢNG", typeof(string));
            dt.Columns.Add("THÀNH TIỀN", typeof(string));

            if (order != null)
            {
                List<OrderItem> lstOrderItems = OrderItem.GetByOrder(order.OrderId);
                foreach (OrderItem oItem in lstOrderItems)
                {
                    Product product = new Product(siteSettings.SiteId, oItem.ProductId);

                    DataRow row = dt.NewRow();

                    row["SẢN PHẨM"] = product.Title;
                    row["ĐƠN GIÁ"] = oItem.Price;
                    row["SỐ LƯỢNG"] = oItem.Quantity;
                    row["THÀNH TIỀN"] = oItem.Price * oItem.Quantity;
                    dt.Rows.Add(row);
                }
            }
            return dt;
        }

        void btnConfirm_Click(object sender, EventArgs e)
        {

            BindTotal();
            order.Save();
            LogActivity.Write("Confirm order", order.OrderCode);
            message.SuccessMessage = "Đã xác nhận đơn hàng & Gửi email cho Khách hàng thành công.";

            string billingProvinceName = string.Empty;
            string billingDistrictName = string.Empty;
            string shippingProvinceName = string.Empty;
            string shippingDistrictName = string.Empty;
            if (order.BillingProvinceGuid != Guid.Empty)
            {
                var province = new GeoZone(order.BillingProvinceGuid);
                if (province != null && province.Guid != Guid.Empty)
                    billingProvinceName = province.Name;
            }
            if (order.BillingDistrictGuid != Guid.Empty)
            {
                var province = new GeoZone(order.BillingDistrictGuid);
                if (province != null && province.Guid != Guid.Empty)
                    billingDistrictName = province.Name;
            }
            if (order.ShippingProvinceGuid != Guid.Empty)
            {
                var province = new GeoZone(order.ShippingProvinceGuid);
                if (province != null && province.Guid != Guid.Empty)
                    shippingProvinceName = province.Name;
            }
            if (order.ShippingDistrictGuid != Guid.Empty)
            {
                var province = new GeoZone(order.ShippingDistrictGuid);
                if (province != null && province.Guid != Guid.Empty)
                    shippingDistrictName = province.Name;
            }

            string toEmail = order.BillingEmail.Trim();
            if (
                order.ShippingEmail.Length > 0
                && !string.Equals(toEmail, order.ShippingEmail, StringComparison.CurrentCultureIgnoreCase)
                )
                toEmail += "," + order.ShippingEmail;
            
            lstOrderItems = OrderItem.GetByOrder(order.OrderId);
            lstProducts = Product.GetByOrder(order.SiteId, order.OrderId);
            string templateName = "OrderPlacedCustomerNotification";

            if (ProductHelper.SendOrderPlacedNotification(siteSettings, order, lstProducts, lstOrderItems, templateName, billingProvinceName, billingDistrictName, shippingProvinceName, shippingDistrictName, toEmail))
                WebTaskManager.StartOrResumeTasks();
            //wordbind(order);
            WebUtils.SetupRedirect(this, Request.RawUrl);
        }

        void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (!WebUser.IsAdmin)
                    return;

                ContentDeleted.Create(siteSettings.SiteId, order.OrderId.ToString(), "Order", typeof(OrderDeleted).AssemblyQualifiedName, order.OrderId.ToString(), Page.User.Identity.Name);

                order.IsDeleted = true;
                order.Save();

                LogActivity.Write("Delete order", order.OrderId.ToString());

                message.SuccessMessage = ResourceHelper.GetResourceString("Resource", "DeleteSuccessMessage");

                WebUtils.SetupRedirect(this, SiteRoot + "/Product/AdminCP/OrderList.aspx");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        void grid_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            try
            {
                if (grid.Items.Count <= 1)
                    return;

                string s = e.CommandArgument.ToString();
                Guid itemGuid = Guid.Empty;
                if (s.Length == 36)
                {
                    try
                    {
                        itemGuid = new Guid(s);
                    }
                    catch (FormatException) { }
                }

                switch (e.CommandName)
                {
                    case "Delete":

                        if (itemGuid != Guid.Empty)
                        {
                            OrderItem orderItem = new OrderItem(itemGuid);
                            if (orderItem != null)
                            {
                                order.OrderDiscount -= orderItem.DiscountAmount;
                                if (order.OrderSubtotal < 0)
                                    order.OrderDiscount = 0;
                                order.OrderSubtotal -= (orderItem.Price * orderItem.Quantity - orderItem.DiscountAmount);
                                if (order.OrderSubtotal < 0)
                                    order.OrderSubtotal = 0;
                                order.OrderTotal = order.OrderSubtotal - order.OrderDiscount;
                                if (order.OrderTotal < 0)
                                    order.OrderTotal = 0;

                                OrderItem.Delete(itemGuid);

                                if (order.CouponCode.Length > 0)
                                {
                                    lstOrderItems = OrderItem.GetByOrder(order.OrderId);
                                    bool hasCoupon = false;
                                    foreach (OrderItem item in lstOrderItems)
                                    {
                                        if (item.DiscountAmount > 0)
                                        {
                                            hasCoupon = true;
                                            break;
                                        }
                                    }
                                    if (!hasCoupon)
                                        order.CouponCode = string.Empty;
                                }

                                order.Save();

                                message.SuccessMessage = ResourceHelper.GetResourceString("Resource", "DeleteSuccessMessage");
                            }
                        }

                        break;
                }

                WebUtils.SetupRedirect(this, Request.RawUrl);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }

        #endregion

        #region Populate

        private void PopulateLabels()
        {
            Title = SiteUtils.FormatPageTitle(siteSettings, ProductResources.OrderDetailTitle);
            heading.Text = ProductResources.OrderDetailTitle;

            UIHelper.AddConfirmationDialog(btnDelete, ProductResources.OrderDeleteMultiWarning);
            UIHelper.AddConfirmationDialog(btnConfirm, ProductResources.OrderConfirmWarning);
        }

        private void PopulateControls()
        {
            BindOrderStatus();
            BindShippingMethod();
            BindPaymentMethod();
            BindProvince();

            txtOrderNote.Text = order.OrderNote;

            txtBillingFirstName.Text = order.BillingFirstName;
            txtBillingLastName.Text = order.BillingLastName;
            txtBillingAddress.Text = order.BillingAddress;
            txtBillingPhone.Text = order.BillingPhone;
            txtBillingEmail.Text = order.BillingEmail;

            litBillingFirstName.Text = Server.HtmlEncode(order.BillingFirstName);
            litBillingLastName.Text = Server.HtmlEncode(order.BillingLastName);
            litBillingAddress.Text = Server.HtmlEncode(order.BillingAddress);
            litBillingPhone.Text = Server.HtmlEncode(order.BillingPhone);
            litBillingEmail.Text = Server.HtmlEncode(order.BillingEmail);

            ListItem item = ddBillingProvince.Items.FindByValue(order.BillingProvinceGuid.ToString());
            if (item != null)
            {
                ddBillingProvince.ClearSelection();
                item.Selected = true;
                BindDistrict(ddBillingProvince, ddBillingDistrict);

                litBillingProvince.Text = ddBillingProvince.SelectedItem.Text;
            }

            item = ddBillingDistrict.Items.FindByValue(order.BillingDistrictGuid.ToString());
            if (item != null)
            {
                ddBillingDistrict.ClearSelection();
                item.Selected = true;

                litBillingDistrict.Text = ddBillingDistrict.SelectedItem.Text;
            }

            txtShippingFirstName.Text = order.ShippingFirstName;
            txtShippingLastName.Text = order.ShippingLastName;
            txtShippingAddress.Text = order.ShippingAddress;
            txtShippingPhone.Text = order.ShippingPhone;
            txtShippingEmail.Text = order.ShippingEmail;

            litShippingFirstName.Text = Server.HtmlEncode(order.ShippingFirstName);
            litShippingLastName.Text = Server.HtmlEncode(order.ShippingLastName);
            litShippingAddress.Text = Server.HtmlEncode(order.ShippingAddress);
            litShippingPhone.Text = Server.HtmlEncode(order.ShippingPhone);
            litShippingEmail.Text = Server.HtmlEncode(order.ShippingEmail);

            item = ddShippingProvince.Items.FindByValue(order.ShippingProvinceGuid.ToString());
            if (item != null)
            {
                ddShippingProvince.ClearSelection();
                item.Selected = true;
                BindDistrict(ddShippingProvince, ddShippingDistrict);

                litShippingProvince.Text = ddShippingProvince.SelectedItem.Text;
            }

            item = ddShippingDistrict.Items.FindByValue(order.ShippingDistrictGuid.ToString());
            if (item != null)
            {
                ddShippingDistrict.ClearSelection();
                item.Selected = true;
            }

            item = ddOrderStatus.Items.FindByValue(order.OrderStatus.ToString());
            if (item != null)
            {
                ddOrderStatus.ClearSelection();
                item.Selected = true;

                litOrderStatus.Text = ddOrderStatus.SelectedItem.Text;
                litOrderStatus.ForeColor = GetForeColor(order.OrderStatus);
            }

            item = rdbListShippingMethod.Items.FindByValue(order.ShippingMethod.ToString());
            if (item != null)
            {
                rdbListShippingMethod.ClearSelection();
                item.Selected = true;
            }

            item = rdbListPaymentMethod.Items.FindByValue(order.PaymentMethod.ToString());
            if (item != null)
            {
                rdbListPaymentMethod.ClearSelection();
                item.Selected = true;
            }

            //txtOrderTax.Text = ProductHelper.FormatPrice(order.OrderTax, false);
            txtOrderShipping.Text = ProductHelper.FormatPrice(order.OrderShipping, false);
            txtOrderDiscount.Text = ProductHelper.FormatPrice(order.OrderDiscount, false);

            litOrderCode.Text = order.OrderCode;
            litSubTotal.Text = ProductHelper.FormatPrice(order.OrderSubtotal, true);
            litOrderTotal.Text = ProductHelper.FormatPrice(order.OrderTotal, true);
            litUserPointDiscount.Text = ProductHelper.FormatPrice(order.UserPointDiscount, true);
            litCreatedOn.Text = DateTimeHelper.Format(order.CreatedUtc, SiteUtils.GetUserTimeZone(), Resources.ProductResources.OrderCreatedDateFormat, SiteUtils.GetUserTimeOffset());

            if (order.UserGuid != Guid.Empty)
            {
                SiteUser user = new SiteUser(siteSettings, order.UserGuid);
                if (user != null && user.UserId > 0)
                {
                    divUsers.Visible = true;
                    litUserCode.Text = user.Name;
                    litUserEmail.Text = user.Email;
                    litUserPoints.Text = user.TotalPosts.ToString();
                }
            }
        }

        protected System.Drawing.Color GetForeColor(int orderStatus)
        {
            switch ((OrderStatus)orderStatus)
            {
                case OrderStatus.New:
                    return System.Drawing.Color.Red;
                case OrderStatus.Processing:
                    return System.Drawing.Color.Blue;
                case OrderStatus.Complete:
                    return System.Drawing.Color.Green;
                case OrderStatus.OutOfStock:
                    return System.Drawing.Color.Gray;
                case OrderStatus.Cancelled:
                    return System.Drawing.Color.Gray;
            }

            return System.Drawing.Color.Red;
        }

        private string GetOrderStatusName(int orderStatus)
        {
            switch ((OrderStatus)orderStatus)
            {
                case OrderStatus.New:
                    return "New";
                case OrderStatus.Processing:
                    return "Processing";
                case OrderStatus.Complete:
                    return "Complete";
                case OrderStatus.OutOfStock:
                    return "OutOfStock";
                case OrderStatus.Cancelled:
                    return "Cancelled";
            }

            return string.Empty;
        }

        private void BindOrderStatus()
        {
            ddOrderStatus.Items.Clear();

            ddOrderStatus.Items.Add(new ListItem(ProductResources.OrderStatusNew, "0"));
            ddOrderStatus.Items.Add(new ListItem(ProductResources.OrderStatusProcessing, "1"));
            ddOrderStatus.Items.Add(new ListItem(ProductResources.OrderStatusComplete, "2"));
            ddOrderStatus.Items.Add(new ListItem(ProductResources.OrderStatusOutOfStock, "3"));
            ddOrderStatus.Items.Add(new ListItem(ProductResources.OrderStatusCancelled, "4"));
        }

        private void BindShippingMethod()
        {
            var lstShippingMethods = ShippingMethod.GetByActive(siteSettings.SiteId, 1);
            if (lstShippingMethods.Count > 0)
            {
                rdbListShippingMethod.DataSource = lstShippingMethods;
                rdbListShippingMethod.DataBind();
            }
            else
            {
                System.Web.UI.HtmlControls.HtmlGenericControl divShippingMethod = (System.Web.UI.HtmlControls.HtmlGenericControl)upMethod.FindControl("divShippingMethod");
                if (divShippingMethod != null)
                    divShippingMethod.Visible = false;
            }
        }

        private void BindPaymentMethod()
        {
            var lstPaymentMethods = PaymentMethod.GetByActive(siteSettings.SiteId, 1);
            if (lstPaymentMethods.Count > 0)
            {
                rdbListPaymentMethod.DataSource = lstPaymentMethods;
                rdbListPaymentMethod.DataBind();
            }
            else
            {
                System.Web.UI.HtmlControls.HtmlGenericControl divPaymentMethod = (System.Web.UI.HtmlControls.HtmlGenericControl)upMethod.FindControl("divPaymentMethod");
                if (divPaymentMethod != null)
                    divPaymentMethod.Visible = false;
            }
        }

        private void BindProvince()
        {
            ddBillingProvince.DataSource = GeoZone.GetByCountry(siteSettings.DefaultCountryGuid);
            ddBillingProvince.DataBind();

            ddShippingProvince.DataSource = ddBillingProvince.DataSource;
            ddShippingProvince.DataBind();
        }

        private void BindDistrict(ListControl ddProvince, ListControl dd)
        {
            dd.Items.Clear();
            if (ddProvince.SelectedValue.Length == 36)
            {
                dd.DataSource = GeoZone.GetByListParent(ddProvince.SelectedValue, 1);
                dd.DataBind();
            }

            dd.Items.Insert(0, new ListItem(ProductResources.OrderSelectLabel, ""));
        }

        private string GetListGeoZoneGuid()
        {
            string listGeoZoneGuid = string.Empty;
            if (!string.IsNullOrEmpty(ddShippingProvince.SelectedValue))
            {
                listGeoZoneGuid = ddShippingProvince.SelectedValue;
                var lstDistrict = GeoZone.GetByListParent(listGeoZoneGuid, 1);
                foreach (GeoZone item in lstDistrict)
                    listGeoZoneGuid += ";" + item.Guid.ToString();
            }

            return listGeoZoneGuid;
        }

        void ddBillingProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindDistrict(ddBillingProvince, ddBillingDistrict);
        }

        void ddShippingProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindDistrict(ddShippingProvince, ddShippingDistrict);
        }

        void rdbListShippingMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            decimal shippingPrice = ProductHelper.GetShippingPrice(Convert.ToInt32(rdbListShippingMethod.SelectedValue), order.OrderSubtotal, lstOrderItems.GetTotalWeights(lstProducts), lstOrderItems.GetTotalProducts(), ProductHelper.GetShippingGeoZoneGuidsByOrderSession(order));
            decimal total = CartHelper.GetCartTotal(order.OrderSubtotal, shippingPrice);
            litOrderTotal.Text = ProductHelper.FormatPrice(total, true);
        }

        void BindTotal()
        {
            decimal price = decimal.Zero;
            //decimal.TryParse(txtOrderTax.Text.Trim(), out price);
            if (price > 0)
                order.OrderTax = price;
            else
                order.OrderTax = decimal.Zero;

            decimal.TryParse(txtOrderShipping.Text.Trim(), out price);
            if (price > 0)
                order.OrderShipping = price;
            else
                order.OrderShipping = decimal.Zero;

            decimal.TryParse(txtOrderDiscount.Text.Trim(), out price);
            if (price > 0)
                order.OrderDiscount = price;
            else
                order.OrderDiscount = decimal.Zero;

            order.OrderTotal = CartHelper.GetCartTotal(subTotal: order.OrderSubtotal, shippingTotal: order.OrderShipping, discountTotal: order.OrderDiscount, orderTax: order.OrderTax, pointDiscount: order.UserPointDiscount);
            litOrderTotal.Text = ProductHelper.FormatPrice(order.OrderTotal, true);
        }

        #endregion

        #region LoadSettings

        private void LoadSettings()
        {
            int orderId = WebUtils.ParseInt32FromQueryString("orderid", -1);
            if (orderId > 0)
            {
                order = new Order(orderId);

                if (order == null
                    || order.OrderId == -1
                    || order.SiteId != siteSettings.SiteId
                    || order.IsDeleted)
                    order = null;
            }

            btnDelete.Visible = WebUser.IsAdmin;
        }

        #endregion

        #region OnInit

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Load += new EventHandler(this.Page_Load);

            grid.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grid_NeedDataSource);
            grid.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grid_ItemDataBound);
            grid.ItemCommand += new Telerik.Web.UI.GridCommandEventHandler(grid_ItemCommand);

            btnUpdate.Click += new EventHandler(btnUpdate_Click);
            btnDelete.Click += new EventHandler(btnDelete_Click);
            ddBillingProvince.SelectedIndexChanged += new EventHandler(ddBillingProvince_SelectedIndexChanged);
            ddShippingProvince.SelectedIndexChanged += new EventHandler(ddShippingProvince_SelectedIndexChanged);
            rdbListShippingMethod.SelectedIndexChanged += new EventHandler(rdbListShippingMethod_SelectedIndexChanged);

            btnConfirm.Click += new EventHandler(btnConfirm_Click);

            //txtOrderTaxPercentage.TextChanged += new EventHandler(txtOrderTaxPercentage_TextChanged);
            txtOrderShippingPercentage.TextChanged += new EventHandler(txtOrderShippingPercentage_TextChanged);
            txtOrderDiscountPercentage.TextChanged += new EventHandler(txtOrderDiscountPercentage_TextChanged);
            //txtOrderTax.TextChanged += new EventHandler(txtOrderTax_TextChanged);
            txtOrderShipping.TextChanged += new EventHandler(txtOrderShipping_TextChanged);
            txtOrderDiscount.TextChanged += new EventHandler(txtOrderDiscount_TextChanged);

            //Button1.Click += new EventHandler(Button1_Click);
        }

        #endregion

        //private void Button1_Click(object sender, EventArgs e)
        //{
        //    wordbind(order);
        //}
    }
}
