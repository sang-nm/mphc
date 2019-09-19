/// Created:				2014-07-22
/// Last Modified:			2014-07-22

using System;
using CanhCam.Web.Framework;
using CanhCam.Business;
using CanhCam.Business.WebHelpers;
using Resources;
using Telerik.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace CanhCam.Web.ProductUI
{
    public partial class OrderListPage : CmsNonBasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OrderListPage));

        RadGridSEOPersister gridPersister;
        protected Double timeOffset = 0;
        protected TimeZoneInfo timeZone = null;
        //private static State State = new State();
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
            PopulateLabels();

            if (!WebUser.IsAdmin)
            {
                btnDelete.Visible = false;
                lblFromDate.Visible = false;
                dpFromDate.Visible = false;
                lblEndDate.Visible = false;
                dpToDate.Visible = false;
                //lblOrderStatus.Visible = false;
                //ddOrderStatus.Visible = false;
                btnExportCustomers.Visible = false;
            }

            if (!Page.IsPostBack)
            {
                PopulateControls();
            }
        }

        #region "RadGrid Event"

        void grid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            bool isAdmin = WebUser.IsAdmin;
            int status = Convert.ToInt32(ddOrderStatus.SelectedValue);
            DateTime? fromdate = null;
            DateTime? todate = null;

            if (dpFromDate.Text.Trim().Length > 0)
            {
                DateTime localTime = DateTime.Parse(dpFromDate.Text);
                localTime = new DateTime(localTime.Year, localTime.Month, localTime.Day, 0, 0, 0);

                if (timeZone != null)
                    fromdate = localTime.ToUtc(timeZone);
                else
                    fromdate = localTime.AddHours(-timeOffset);
            }
            if (dpToDate.Text.Trim().Length > 0)
            {
                DateTime localTime = DateTime.Parse(dpToDate.Text);
                localTime = new DateTime(localTime.Year, localTime.Month, localTime.Day, 23, 59, 59);

                if (timeZone != null)
                    todate = localTime.ToUtc(timeZone);
                else
                    todate = localTime.AddHours(-timeOffset);
            }
            int iCount = 0;
            bool isApplied = gridPersister.IsAppliedSortFilterOrGroup;
            //if (State.isSearch)
            //{
            //    status = State.OldStatus;
            //    txtKeyword.Text = State.OldKeyword;
            //}
            if (status != -1 || txtKeyword.Text.Trim() != string.Empty)
            {
                iCount = Order.GetCount(siteSettings.SiteId, -1, status, -1, -1, fromdate, todate, null, null, null, txtKeyword.Text.Trim());
            }
            //else if (OldState.OldStatus != -1 || OldState.OldKeyword != string.Empty)
            //{
            //    iCount = Order.GetCount(siteSettings.SiteId, -1, OldState.OldStatus, -1, -1, fromdate, todate, null, null, null, OldState.OldKeyword);
            //}
            else
                iCount = Order.GetCountNotAdmin(siteSettings.SiteId, -1, status, -1, -1, fromdate, todate, null, null, null, txtKeyword.Text.Trim(), isAdmin);

            int startRowIndex = isApplied ? 1 : grid.CurrentPageIndex + 1;
            int maximumRows = isApplied ? iCount : grid.PageSize;

            grid.VirtualItemCount = iCount;
            grid.AllowCustomPaging = !isApplied;
            grid.PagerStyle.EnableSEOPaging = !isApplied;

            if (status != -1 || txtKeyword.Text.Trim() != string.Empty)
                grid.DataSource = Order.GetPage(siteSettings.SiteId, -1, status, -1, -1, fromdate, todate, null, null, null, txtKeyword.Text.Trim(), startRowIndex, maximumRows);
            else
                grid.DataSource = Order.GetPageNotAdmin(siteSettings.SiteId, -1, status, -1, -1, fromdate, todate, null, null, null, txtKeyword.Text.Trim(), startRowIndex, maximumRows, isAdmin);
        }

        void grid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is Telerik.Web.UI.GridDataItem)
            {
                Telerik.Web.UI.GridDataItem item = (Telerik.Web.UI.GridDataItem)e.Item;
                HyperLink lnkQuickView = (HyperLink)item.FindControl("lnkQuickView");
                DropDownList ddOrderStatus = (DropDownList)item.FindControl("ddOrderStatus");
                int orderId = Convert.ToInt32(item.GetDataKeyValue("OrderId"));
                int orderStatus = Convert.ToInt32(item.GetDataKeyValue("OrderStatus"));

                RadToolTipManager1.TargetControls.Add(lnkQuickView.ClientID, orderId.ToString(), true);

                PopulateOrderStatus(ddOrderStatus, false);
                ListItem li = ddOrderStatus.Items.FindByValue(orderStatus.ToString());
                if (li != null)
                {
                    ddOrderStatus.ClearSelection();
                    li.Selected = true;
                }
            }
        }

        protected string GetCustomer(string firstName, string lastName)
        {
            string format = ProductResources.OrderCustomerFirstLastFormat;

            format = format.Replace("{FirstName}", firstName);
            format = format.Replace("{LastName}", lastName);

            return Server.HtmlEncode(format);
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

        protected void OnAjaxUpdate(object sender, ToolTipUpdateEventArgs e)
        {
            OrderDetailToolTipControl ctrl = Page.LoadControl("/Product/Controls/OrderDetailToolTipControl.ascx") as OrderDetailToolTipControl;
            ctrl.ID = "UcOrderDetail1";
            ctrl.OrderId = Convert.ToInt32(e.Value);
            e.UpdatePanel.ContentTemplateContainer.Controls.Add(ctrl);
        }

        #endregion

        #region Event

        void btnSearch_Click(object sender, EventArgs e)
        {
            //State.isSearch = true;
            //State.OldKeyword = txtKeyword.Text.Trim();
            //State.OldStatus = Convert.ToInt32(ddOrderStatus.SelectedValue);
            //Response.Redirect(Request.Url.LocalPath);
            grid.Rebind();
        }

        void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                int iRecordDeleted = 0;
                foreach (Telerik.Web.UI.GridDataItem data in grid.SelectedItems)
                {
                    int orderId = Convert.ToInt32(data.GetDataKeyValue("OrderId"));
                    Order order = new Order(orderId);

                    if (order != null && order.OrderId > 0 && order.SiteId == siteSettings.SiteId && !order.IsDeleted)
                    {
                        ContentDeleted.Create(siteSettings.SiteId, order.OrderId.ToString(), "Order", typeof(OrderDeleted).AssemblyQualifiedName, order.OrderId.ToString(), Page.User.Identity.Name);

                        order.IsDeleted = true;
                        order.Save();

                        iRecordDeleted += 1;
                    }
                }

                if (iRecordDeleted > 0)
                {
                    LogActivity.Write("Delete " + iRecordDeleted.ToString() + " order(s)", "Order");
                    message.SuccessMessage = ResourceHelper.GetResourceString("Resource", "DeleteSuccessMessage");

                    grid.Rebind();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        protected void ddOrderStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DropDownList ddOrderStatus = (DropDownList)sender;
                if (ddOrderStatus != null && ddOrderStatus.SelectedValue.Length > 0)
                {
                    GridDataItem dataItem = (GridDataItem)ddOrderStatus.NamingContainer;
                    int orderId = Convert.ToInt32(dataItem.GetDataKeyValue("OrderId"));

                    Order order = new Order(orderId);
                    if (order != null && order.OrderId > 0)
                    {
                        order.OrderStatus = Convert.ToInt32(ddOrderStatus.SelectedValue);
                        order.Save();

                        LogActivity.Write("Update order's status", order.OrderId.ToString());
                        message.SuccessMessage = ProductResources.OrderStatusUpdateSuccessMessage;
                        grid.Rebind();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            DataTable dt = GetOrdersForExport();
            string fileName = "orders-data-export-" + DateTimeHelper.GetDateTimeStringForFileName() + ".xls";

            ExportHelper.ExportDataTableToExcel(HttpContext.Current, dt, fileName);
            message.ErrorMessage = ResourceHelper.GetResourceString("Resource", "GridViewNoData");

        }

        protected void btnExportByProduct_Click(object sender, EventArgs e)
        {
            DataTable dt = GetOrderByProductForExport();

            string fileName = "orders-data-export-" + DateTimeHelper.GetDateTimeStringForFileName() + ".xls";

            ExportHelper.ExportDataTableToExcel(HttpContext.Current, dt, fileName);
        }

        protected void btnExportCustomers_Click(object sender, EventArgs e)
        {
            DataTable dt = GetCustomersForExport();

            string fileName = "customers-data-export-" + DateTimeHelper.GetDateTimeStringForFileName() + ".xls";

            ExportHelper.ExportDataTableToExcel(HttpContext.Current, dt, fileName);
            message.ErrorMessage = ResourceHelper.GetResourceString("Resource", "GridViewNoData");
        }

        #endregion

        private DataTable GetOrderByProductForExport()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Number", typeof(int));
            dt.Columns.Add("OrderCode", typeof(string));
            dt.Columns.Add("OrderStatus", typeof(string));
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("CreatedTime", typeof(string));
            dt.Columns.Add("ProductCode", typeof(string));
            dt.Columns.Add("ProductName", typeof(string));
            dt.Columns.Add("Quantity", typeof(int));
            dt.Columns.Add("Price", typeof(double));
            dt.Columns.Add("SubTotal", typeof(double));
            dt.Columns.Add("OrderDiscount", typeof(double));
            dt.Columns.Add("OrderShipping", typeof(double));
            dt.Columns.Add("OrderTotal", typeof(double));

            List<ShippingMethod> lstShippingMethods = ShippingMethod.GetByActive(siteSettings.SiteId, 1);
            List<PaymentMethod> lstPaymentMethods = PaymentMethod.GetByActive(siteSettings.SiteId, 1);
            if (lstShippingMethods.Count > 0)
                dt.Columns.Add("ShippingMethod", typeof(string));
            if (lstPaymentMethods.Count > 0)
                dt.Columns.Add("PaymentMethod", typeof(string));

            dt.Columns.Add("FirstName", typeof(string));
            dt.Columns.Add("LastName", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("Phone", typeof(string));
            dt.Columns.Add("Address", typeof(string));

            int status = Convert.ToInt32(ddOrderStatus.SelectedValue);
            DateTime? fromdate = null;
            DateTime? todate = null;
            if (dpFromDate.Text.Trim().Length > 0)
            {
                DateTime localTime = DateTime.Parse(dpFromDate.Text);
                localTime = new DateTime(localTime.Year, localTime.Month, localTime.Day, 0, 0, 0);

                if (timeZone != null)
                    fromdate = localTime.ToUtc(timeZone);
                else
                    fromdate = localTime.AddHours(-timeOffset);
            }
            if (dpToDate.Text.Trim().Length > 0)
            {
                DateTime localTime = DateTime.Parse(dpToDate.Text);
                localTime = new DateTime(localTime.Year, localTime.Month, localTime.Day, 23, 59, 59);

                if (timeZone != null)
                    todate = localTime.ToUtc(timeZone);
                else
                    todate = localTime.AddHours(-timeOffset);
            }

            var iCount = OrderItem.GetCountBySearch(siteSettings.SiteId, -1, status, -1, -1, fromdate, todate, null, null, null, txtKeyword.Text.Trim());
            var lstOrderItems = OrderItem.GetPageBySearch(siteSettings.SiteId, -1, status, -1, -1, fromdate, todate, null, null, null, txtKeyword.Text.Trim(), 1, iCount);
            if (lstOrderItems.Count > 0)
            {
                List<Guid> lstProductGuids = new List<Guid>();
                List<Guid> lstProvinceGuids = new List<Guid>();
                List<Guid> lstDistrictGuids = new List<Guid>();
                foreach (OrderItem orderItem in lstOrderItems)
                {
                    if (!lstProductGuids.Contains(orderItem.ProductGuid))
                        lstProductGuids.Add(orderItem.ProductGuid);
                    if (!lstProvinceGuids.Contains(orderItem.Order.BillingProvinceGuid))
                        lstProvinceGuids.Add(orderItem.Order.BillingProvinceGuid);
                    if (!lstDistrictGuids.Contains(orderItem.Order.BillingDistrictGuid))
                        lstDistrictGuids.Add(orderItem.Order.BillingDistrictGuid);
                }

                List<GeoZone> lstProvinces = new List<GeoZone>();
                List<GeoZone> lstDistricts = new List<GeoZone>();
                if (lstProvinceGuids.Count > 0)
                    lstProvinces = GeoZone.GetByGuids(string.Join(";", lstProvinceGuids.ToArray()), 1);
                if (lstDistrictGuids.Count > 0)
                    lstDistricts = GeoZone.GetByGuids(string.Join(";", lstDistrictGuids.ToArray()), 1);
                if (lstProvinces.Count > 0)
                    dt.Columns.Add("Province", typeof(string));
                if (lstDistricts.Count > 0)
                    dt.Columns.Add("District", typeof(string));

                int i = 1;
                List<Product> lstProducts = Product.GetByGuids(siteSettings.SiteId, string.Join(";", lstProductGuids.ToArray()), -1);
                foreach (OrderItem orderItem in lstOrderItems)
                {
                    foreach (Product product in lstProducts)
                    {
                        if (orderItem.ProductId == product.ProductId)
                        {
                            DataRow row = dt.NewRow();

                            row["Number"] = i;
                            row["OrderCode"] = orderItem.Order.OrderCode;
                            row["OrderStatus"] = ProductHelper.GetOrderStatus(orderItem.Order.OrderStatus);
                            row["Date"] = ProductHelper.FormatDate(orderItem.Order.CreatedUtc, timeZone, timeOffset, "dd/MM/yyyy");
                            row["CreatedTime"] = ProductHelper.FormatDate(orderItem.Order.CreatedUtc, timeZone, timeOffset, "HH:mm");

                            row["ProductCode"] = product.Code;
                            row["ProductName"] = product.Title;
                            row["Quantity"] = orderItem.Quantity;
                            row["Price"] = Convert.ToDouble(orderItem.Price);

                            row["SubTotal"] = Convert.ToDouble(orderItem.Order.OrderSubtotal);
                            row["OrderDiscount"] = Convert.ToDouble(orderItem.Order.OrderDiscount);
                            row["OrderShipping"] = Convert.ToDouble(orderItem.Order.OrderShipping);
                            row["OrderTotal"] = Convert.ToDouble(orderItem.Order.OrderTotal);

                            if (lstShippingMethods.Count > 0)
                            {
                                string name = string.Empty;
                                if (orderItem.Order.ShippingMethod > 0)
                                {
                                    foreach (ShippingMethod method in lstShippingMethods)
                                    {
                                        if (method.ShippingMethodId == orderItem.Order.ShippingMethod)
                                        {
                                            name = method.Name;
                                            break;
                                        }
                                    }
                                }

                                row["ShippingMethod"] = name;
                            }
                            if (lstPaymentMethods.Count > 0)
                            {
                                string name = string.Empty;
                                if (orderItem.Order.PaymentMethod > 0)
                                {
                                    foreach (PaymentMethod method in lstPaymentMethods)
                                    {
                                        if (method.PaymentMethodId == orderItem.Order.PaymentMethod)
                                        {
                                            name = method.Name;
                                            break;
                                        }
                                    }
                                }

                                row["PaymentMethod"] = name;
                            }

                            row["FirstName"] = orderItem.Order.BillingFirstName;
                            row["LastName"] = orderItem.Order.BillingLastName;
                            row["Email"] = orderItem.Order.BillingEmail;
                            row["Phone"] = orderItem.Order.BillingPhone;
                            row["Address"] = orderItem.Order.BillingAddress;

                            if (lstProvinces.Count > 0)
                            {
                                string name = string.Empty;
                                if (orderItem.Order.BillingProvinceGuid != Guid.Empty)
                                {
                                    foreach (GeoZone gz in lstProvinces)
                                    {
                                        if (gz.Guid == orderItem.Order.BillingProvinceGuid)
                                        {
                                            name = gz.Name;
                                            break;
                                        }
                                    }
                                }

                                row["Province"] = name;
                            }
                            if (lstDistricts.Count > 0)
                            {
                                string name = string.Empty;
                                if (orderItem.Order.BillingDistrictGuid != Guid.Empty)
                                {
                                    foreach (GeoZone gz in lstDistricts)
                                    {
                                        if (gz.Guid == orderItem.Order.BillingDistrictGuid)
                                        {
                                            name = gz.Name;
                                            break;
                                        }
                                    }
                                }

                                row["District"] = name;
                            }

                            i += 1;
                            dt.Rows.Add(row);

                            break;
                        }
                    }
                }
            }

            return dt;
        }

        private DataTable GetOrdersForExport()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Number", typeof(int));
            dt.Columns.Add("OrderCode", typeof(string));
            dt.Columns.Add("OrderStatus", typeof(string));
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("CreatedTime", typeof(string));
            dt.Columns.Add("Products", typeof(string));
            dt.Columns.Add("SubTotal", typeof(double));
            dt.Columns.Add("OrderDiscount", typeof(double));
            dt.Columns.Add("OrderShipping", typeof(double));
            dt.Columns.Add("OrderTax", typeof(double));
            dt.Columns.Add("OrderTotal", typeof(double));

            List<ShippingMethod> lstShippingMethods = ShippingMethod.GetByActive(siteSettings.SiteId, 1);
            List<PaymentMethod> lstPaymentMethods = PaymentMethod.GetByActive(siteSettings.SiteId, 1);
            if (lstShippingMethods.Count > 0)
                dt.Columns.Add("ShippingMethod", typeof(string));
            if (lstPaymentMethods.Count > 0)
                dt.Columns.Add("PaymentMethod", typeof(string));

            dt.Columns.Add("FirstName", typeof(string));
            dt.Columns.Add("LastName", typeof(string));
            dt.Columns.Add("Customer Code", typeof(string));

            int status = Convert.ToInt32(ddOrderStatus.SelectedValue);
            DateTime? fromdate = null;
            DateTime? todate = null;
            if (dpFromDate.Text.Trim().Length > 0)
            {
                DateTime localTime = DateTime.Parse(dpFromDate.Text);
                localTime = new DateTime(localTime.Year, localTime.Month, localTime.Day, 0, 0, 0);

                if (timeZone != null)
                    fromdate = localTime.ToUtc(timeZone);
                else
                    fromdate = localTime.AddHours(-timeOffset);
            }
            if (dpToDate.Text.Trim().Length > 0)
            {
                DateTime localTime = DateTime.Parse(dpToDate.Text);
                localTime = new DateTime(localTime.Year, localTime.Month, localTime.Day, 23, 59, 59);

                if (timeZone != null)
                    todate = localTime.ToUtc(timeZone);
                else
                    todate = localTime.AddHours(-timeOffset);
            }

            var iCount = Order.GetCount(siteSettings.SiteId, -1, status, -1, -1, fromdate, todate, null, null, null, txtKeyword.Text.Trim());
            var lstOrders = Order.GetPage(siteSettings.SiteId, -1, status, -1, -1, fromdate, todate, null, null, null, txtKeyword.Text.Trim(), 1, iCount);
            if (lstOrders.Count > 0)
            {
                List<Guid> lstProvinceGuids = new List<Guid>();
                List<Guid> lstDistrictGuids = new List<Guid>();
                foreach (Order o in lstOrders)
                {
                    if (!lstProvinceGuids.Contains(o.BillingProvinceGuid))
                        lstProvinceGuids.Add(o.BillingProvinceGuid);
                    if (!lstDistrictGuids.Contains(o.BillingDistrictGuid))
                        lstDistrictGuids.Add(o.BillingDistrictGuid);
                }

                List<GeoZone> lstProvinces = new List<GeoZone>();
                List<GeoZone> lstDistricts = new List<GeoZone>();
                if (lstProvinceGuids.Count > 0)
                    lstProvinces = GeoZone.GetByGuids(string.Join(";", lstProvinceGuids.ToArray()), 1);
                if (lstDistrictGuids.Count > 0)
                    lstDistricts = GeoZone.GetByGuids(string.Join(";", lstDistrictGuids.ToArray()), 1);
                if (lstProvinces.Count > 0)
                    dt.Columns.Add("Province", typeof(string));
                if (lstDistricts.Count > 0)
                    dt.Columns.Add("District", typeof(string));

                int i = 1;
                foreach (Order o in lstOrders)
                {
                    DataRow row = dt.NewRow();

                    row["Number"] = i;
                    row["OrderCode"] = o.OrderCode;
                    row["OrderStatus"] = ProductHelper.GetOrderStatus(o.OrderStatus);
                    row["Date"] = ProductHelper.FormatDate(o.CreatedUtc, timeZone, timeOffset, "dd/MM/yyyy");
                    row["CreatedTime"] = ProductHelper.FormatDate(o.CreatedUtc, timeZone, timeOffset, "HH:mm");

                    var lstProducts = Product.GetByOrder(siteSettings.SiteId, o.OrderId);
                    string products = string.Empty;
                    string sepa = string.Empty;
                    foreach (Product product in lstProducts)
                    {
                        products += sepa + product.Title;
                        sepa = ";" + System.Environment.NewLine;
                    }
                    row["Products"] = products;

                    row["SubTotal"] = Convert.ToDouble(o.OrderSubtotal);
                    row["OrderDiscount"] = Convert.ToDouble(o.OrderDiscount);
                    row["OrderShipping"] = Convert.ToDouble(o.OrderShipping);
                    row["OrderTax"] = Convert.ToDouble(o.OrderTax);
                    row["OrderTotal"] = Convert.ToDouble(o.OrderTotal);

                    if (lstShippingMethods.Count > 0)
                    {
                        string name = string.Empty;
                        if (o.ShippingMethod > 0)
                        {
                            foreach (ShippingMethod method in lstShippingMethods)
                            {
                                if (method.ShippingMethodId == o.ShippingMethod)
                                {
                                    name = method.Name;
                                    break;
                                }
                            }
                        }

                        row["ShippingMethod"] = name;
                    }
                    if (lstPaymentMethods.Count > 0)
                    {
                        string name = string.Empty;
                        if (o.PaymentMethod > 0)
                        {
                            foreach (PaymentMethod method in lstPaymentMethods)
                            {
                                if (method.PaymentMethodId == o.PaymentMethod)
                                {
                                    name = method.Name;
                                    break;
                                }
                            }
                        }

                        row["PaymentMethod"] = name;
                    }

                    row["FirstName"] = o.BillingFirstName;
                    row["LastName"] = o.BillingLastName;
                    var user = new SiteUser(siteSettings, o.UserGuid);
                    row["Customer Code"] = user.Name;
                    if (lstProvinces.Count > 0)
                    {
                        string name = string.Empty;
                        if (o.BillingProvinceGuid != Guid.Empty)
                        {
                            foreach (GeoZone gz in lstProvinces)
                            {
                                if (gz.Guid == o.BillingProvinceGuid)
                                {
                                    name = gz.Name;
                                    break;
                                }
                            }
                        }

                        row["Province"] = name;
                    }
                    if (lstDistricts.Count > 0)
                    {
                        string name = string.Empty;
                        if (o.BillingDistrictGuid != Guid.Empty)
                        {
                            foreach (GeoZone gz in lstDistricts)
                            {
                                if (gz.Guid == o.BillingDistrictGuid)
                                {
                                    name = gz.Name;
                                    break;
                                }
                            }
                        }

                        row["District"] = name;
                    }

                    i += 1;
                    dt.Rows.Add(row);
                }
            }

            return dt;
        }

        private DataTable GetCustomersForExport()
        {
            DataTable dt = new DataTable();

            //dt.Columns.Add("FirstName", typeof(string));
            //dt.Columns.Add("LastName", typeof(string));
            dt.Columns.Add("FullName", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("Phone", typeof(string));
            dt.Columns.Add("Address", typeof(string));

            int status = Convert.ToInt32(ddOrderStatus.SelectedValue);
            DateTime? fromdate = null;
            DateTime? todate = null;
            if (dpFromDate.Text.Trim().Length > 0)
            {
                DateTime localTime = DateTime.Parse(dpFromDate.Text);
                localTime = new DateTime(localTime.Year, localTime.Month, localTime.Day, 0, 0, 0);

                if (timeZone != null)
                    fromdate = localTime.ToUtc(timeZone);
                else
                    fromdate = localTime.AddHours(-timeOffset);
            }
            if (dpToDate.Text.Trim().Length > 0)
            {
                DateTime localTime = DateTime.Parse(dpToDate.Text);
                localTime = new DateTime(localTime.Year, localTime.Month, localTime.Day, 23, 59, 59);

                if (timeZone != null)
                    todate = localTime.ToUtc(timeZone);
                else
                    todate = localTime.AddHours(-timeOffset);
            }

            var iCount = Order.GetCount(siteSettings.SiteId, -1, status, -1, -1, fromdate, todate, null, null, null, txtKeyword.Text.Trim());
            var lstOrders = Order.GetPage(siteSettings.SiteId, -1, status, -1, -1, fromdate, todate, null, null, null, txtKeyword.Text.Trim(), 1, iCount);
            if (lstOrders.Count > 0)
            {
                List<Guid> lstProvinceGuids = new List<Guid>();
                List<Guid> lstDistrictGuids = new List<Guid>();
                foreach (Order o in lstOrders)
                {
                    if (!lstProvinceGuids.Contains(o.BillingProvinceGuid))
                        lstProvinceGuids.Add(o.BillingProvinceGuid);
                    if (!lstDistrictGuids.Contains(o.BillingDistrictGuid))
                        lstDistrictGuids.Add(o.BillingDistrictGuid);
                }

                List<GeoZone> lstProvinces = new List<GeoZone>();
                List<GeoZone> lstDistricts = new List<GeoZone>();
                if (lstProvinceGuids.Count > 0)
                    lstProvinces = GeoZone.GetByGuids(string.Join(";", lstProvinceGuids.ToArray()), 1);
                if (lstDistrictGuids.Count > 0)
                    lstDistricts = GeoZone.GetByGuids(string.Join(";", lstDistrictGuids.ToArray()), 1);
                if (lstProvinces.Count > 0)
                    dt.Columns.Add("Province", typeof(string));
                if (lstDistricts.Count > 0)
                    dt.Columns.Add("District", typeof(string));

                int i = 1;
                foreach (Order o in lstOrders)
                {
                    DataRow row = dt.NewRow();

                    //row["FirstName"] = o.BillingFirstName;
                    //row["LastName"] = o.BillingLastName;
                    row["FullName"] = o.BillingFirstName;
                    row["Email"] = o.BillingEmail;
                    row["Phone"] = o.BillingPhone;
                    row["Address"] = o.BillingAddress;

                    if (lstProvinces.Count > 0)
                    {
                        string name = string.Empty;
                        if (o.BillingProvinceGuid != Guid.Empty)
                        {
                            foreach (GeoZone gz in lstProvinces)
                            {
                                if (gz.Guid == o.BillingProvinceGuid)
                                {
                                    name = gz.Name;
                                    break;
                                }
                            }
                        }

                        row["Province"] = name;
                    }
                    if (lstDistricts.Count > 0)
                    {
                        string name = string.Empty;
                        if (o.BillingDistrictGuid != Guid.Empty)
                        {
                            foreach (GeoZone gz in lstDistricts)
                            {
                                if (gz.Guid == o.BillingDistrictGuid)
                                {
                                    name = gz.Name;
                                    break;
                                }
                            }
                        }

                        row["District"] = name;
                    }

                    i += 1;
                    dt.Rows.Add(row);
                }
            }

            return dt;
        }

        #region Populate

        private void PopulateLabels()
        {
            Title = SiteUtils.FormatPageTitle(siteSettings, ProductResources.OrderAdminTitle);
            heading.Text = ProductResources.OrderAdminTitle;

            UIHelper.AddConfirmationDialog(btnDelete, ProductResources.OrderDeleteMultiWarning);
        }

        private void PopulateControls()
        {
            PopulateOrderStatus(ddOrderStatus, true);
        }

        private void PopulateOrderStatus(DropDownList ddOrderStatus, bool addAll)
        {
            ddOrderStatus.Items.Clear();

            if (addAll)
                ddOrderStatus.Items.Add(new ListItem(ProductResources.OrderStatusAll, "-1"));

            ddOrderStatus.Items.Add(new ListItem(ProductResources.OrderStatusNew, "0"));
            ddOrderStatus.Items.Add(new ListItem(ProductResources.OrderStatusProcessing, "1"));
            if (WebUser.IsAdmin)
            {
                ddOrderStatus.Items.Add(new ListItem(ProductResources.OrderStatusComplete, "2"));
                ddOrderStatus.Items.Add(new ListItem(ProductResources.OrderStatusCancelled, "4"));
            }
            //ddOrderStatus.Items.Add(new ListItem(ProductResources.OrderStatusPending, "3"));
            //ddOrderStatus.Items.Add(new ListItem(ProductResources.OrderStatusCancelled, "4"));
            //ddOrderStatus.Items.Add(new ListItem(ProductResources.OrderStatusRequestCancelled, "5"));
        }

        #endregion

        #region LoadSettings

        private void LoadSettings()
        {
            timeOffset = SiteUtils.GetUserTimeOffset();
            timeZone = SiteUtils.GetUserTimeZone();
        }

        #endregion

        #region OnInit

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Load += new EventHandler(this.Page_Load);

            btnSearch.Click += new EventHandler(btnSearch_Click);
            btnDelete.Click += new EventHandler(btnDelete_Click);
            grid.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grid_NeedDataSource);

            grid.ItemDataBound += new GridItemEventHandler(grid_ItemDataBound);

            gridPersister = new RadGridSEOPersister(grid);
        }

        #endregion
    }
}


namespace CanhCam.Web.ProductUI
{
    public class OrderDeleted : IContentDeleted
    {
        public bool RestoreContent(string orderId)
        {
            try
            {
                Order order = new Order(Convert.ToInt32(orderId));

                if (order != null && order.OrderId > 0)
                {
                    order.IsDeleted = false;
                    order.Save();
                }
            }
            catch (Exception) { return false; }

            return true;
        }

        public bool DeleteContent(string orderId)
        {
            try
            {
                Order.Delete(Convert.ToInt32(orderId));
            }
            catch (Exception) { return false; }

            return true;
        }

    }
}

//namespace CanhCam.Web.ProductUI
//{
//    public class State
//    {
//        private int _OldStatus = -1;

//        public int OldStatus
//        {
//            get { return _OldStatus; }
//            set { _OldStatus = value; }
//        }

//        private string _OldKeyword = string.Empty;

//        public string OldKeyword
//        {
//            get { return _OldKeyword; }
//            set { _OldKeyword = value; }
//        }

//        private bool _isSearch;

//        public bool isSearch
//        {
//            get { return _isSearch; }
//            set { _isSearch = value; }
//        }

//    }
//}