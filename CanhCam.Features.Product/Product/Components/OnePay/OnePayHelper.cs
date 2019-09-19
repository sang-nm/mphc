/// Author:			        Tran Quoc Vuong - itqvuong@gmail.com - tqvuong263@yahoo.com
/// Created:				2016-06-07
/// Last Modified:			2016-06-07

using System;
using log4net;
using CanhCam.Web.Framework;
using CanhCam.Business;
using System.Net;
using System.IO;
using System.Text;
using System.Web;

namespace CanhCam.Web.ProductUI
{
    public static class OnePayHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OnePayHelper));

        public static string ProcessCallbackData(string hashcode)
        {
            string hashvalidateResult = string.Empty;
            // Khoi tao lop thu vien
            VPCRequest conn = new VPCRequest("http://onepay.vn");
            conn.SetSecureSecret(hashcode);
            // Xu ly tham so tra ve va kiem tra chuoi du lieu ma hoa
            hashvalidateResult = conn.Process3PartyResponse(System.Web.HttpContext.Current.Request.QueryString);

            // Lay gia tri tham so tra ve tu cong thanh toan
            string vpc_TxnResponseCode = conn.GetResultField("vpc_TxnResponseCode", "Unknown");
            string txnResponseCode = vpc_TxnResponseCode;

            //string merchTxnRef = conn.GetResultField("vpc_MerchTxnRef", "0");
            string orderInfo = conn.GetResultField("vpc_OrderInfo", "");

            Order order = null;
            if (orderInfo.Length > 0)
                order = Order.GetOrderByCode(orderInfo);
            if (order == null || order.OrderId < 0 || order.IsDeleted)
                order = null;

            // Sua lai ham check chuoi ma hoa du lieu
            if (hashvalidateResult == "CORRECTED" && txnResponseCode.Trim() == "0") // "Transaction was paid successful"
            {
                if (order != null)
                {
                    order.PaymentStatus = (int)OrderPaymentMethod.Successful;
                    if (order.OrderNote.Length == 0)
                        order.OrderNote = "Transaction was paid successful";
                    order.Save();
                }

                return SiteUtils.GetZoneUrl(ConfigHelper.GetIntProperty("OnePAY.ZoneID.TransactionSuccessful", 1));
            }
            else if (hashvalidateResult == "INVALIDATED" && txnResponseCode.Trim() == "0") // "Transaction is pending"
            {
                if (order != null)
                {
                    order.PaymentStatus = (int)OrderPaymentMethod.Pending;
                    if (order.OrderNote.Length == 0)
                        order.OrderNote = "Transaction is pending";
                    order.Save();
                }

                return SiteUtils.GetZoneUrl(ConfigHelper.GetIntProperty("OnePAY.ZoneID.TransactionPending", 1));
            }
            else // "Transaction was not paid successful"
            {
                if (order != null)
                {
                    order.PaymentStatus = (int)OrderPaymentMethod.NotSuccessful;
                    if (order.OrderNote.Length == 0)
                        order.OrderNote = "Transaction was not paid successful";
                    order.Save();
                }

                return SiteUtils.GetZoneUrl(ConfigHelper.GetIntProperty("OnePAY.ZoneID.TransactionNotSuccessful", 1));
            }
        }

        public static string GetPaymentUrlIfNeeded(Order order)
        {
            if (order == null || order.OrderId <= 0)
                return string.Empty;

            if (order.PaymentMethod == ProductConfiguration.OnePAYNoiDiaPaymentMethodId || order.PaymentMethod == ProductConfiguration.OnePAYQuocTePaymentMethodId) // Onepay
            {
                //https://mtf.onepay.vn/developer/?page=modul_noidia
                //https://mtf.onepay.vn/developer/?page=modul_quocte
                // Thông số tài khoản cổng thanh toán: 
                string urlPayment = string.Empty;
                string hashcode = string.Empty;
                string merchantId = string.Empty;
                string accesscode = string.Empty;
                string returnUrl = string.Empty;
                string queryDRUser = string.Empty;
                string queryDRPassword = string.Empty;

                if (GetOnePayParameter(order.PaymentMethod, out urlPayment, out hashcode, out merchantId, out accesscode, out returnUrl, out queryDRUser, out queryDRPassword))
                {
                    // Khoi tao lop thu vien va gan gia tri cac tham so gui sang cong thanh toan
                    VPCRequest conn = new VPCRequest(urlPayment);
                    conn.SetSecureSecret(hashcode);
                    // Add the Digital Order Fields for the functionality you wish to use
                    // Core Transaction Fields
                    if (order.PaymentMethod == ProductConfiguration.OnePAYQuocTePaymentMethodId)
                        conn.AddDigitalOrderField("AgainLink", "http://onepay.vn");
                    conn.AddDigitalOrderField("Title", "onepay paygate");
                    conn.AddDigitalOrderField("vpc_Locale", "vn");//Chon ngon ngu hien thi tren cong thanh toan (vn/en)
                    conn.AddDigitalOrderField("vpc_Version", "2");
                    conn.AddDigitalOrderField("vpc_Command", "pay");
                    conn.AddDigitalOrderField("vpc_Merchant", merchantId);
                    conn.AddDigitalOrderField("vpc_AccessCode", accesscode);
                    conn.AddDigitalOrderField("vpc_MerchTxnRef", Guid.NewGuid().ToString());
                    conn.AddDigitalOrderField("vpc_OrderInfo", order.OrderCode.ToString());
                    conn.AddDigitalOrderField("vpc_Amount", Convert.ToDouble(order.OrderTotal * 100).ToString()); //*100?
                    if (order.PaymentMethod == ProductConfiguration.OnePAYNoiDiaPaymentMethodId)
                        conn.AddDigitalOrderField("vpc_Currency", "VND");
                    conn.AddDigitalOrderField("vpc_ReturnURL", returnUrl);
                    // Thong tin them ve khach hang. De trong neu khong co thong tin
                    //conn.AddDigitalOrderField("vpc_SHIP_Street01", order.BillingAddress);
                    //conn.AddDigitalOrderField("vpc_SHIP_Provice", billingProvinceName);
                    //conn.AddDigitalOrderField("vpc_SHIP_City", billingDistrictName);
                    //conn.AddDigitalOrderField("vpc_SHIP_Country", "Vietnam");
                    conn.AddDigitalOrderField("vpc_Customer_Phone", order.BillingPhone);
                    conn.AddDigitalOrderField("vpc_Customer_Email", order.BillingEmail);
                    if (order.UserGuid != Guid.Empty)
                        conn.AddDigitalOrderField("vpc_Customer_Id", order.UserGuid.ToString());
                    // Dia chi IP cua khach hang
                    conn.AddDigitalOrderField("vpc_TicketNo", order.CreatedFromIP);
                    // Chuyen huong trinh duyet sang cong thanh toan
                    return conn.Create3PartyQueryString();
                }
            }

            return string.Empty;
        }

        private static bool GetOnePayParameter(int paymentMethod, out string urlPayment, out string hashcode, out string merchantId, out string accesscode, out string returnUrl, out string queryDRUser, out string queryDRPassword)
        {
            urlPayment = string.Empty;
            hashcode = string.Empty;
            merchantId = string.Empty;
            accesscode = string.Empty;
            returnUrl = string.Empty;
            queryDRUser = string.Empty;
            queryDRPassword = string.Empty;

            if (paymentMethod == ProductConfiguration.OnePAYNoiDiaPaymentMethodId) // Nội địa
            {
                urlPayment = ConfigHelper.GetStringProperty("OnePAYNoiDia.URLPayment", string.Empty);
                hashcode = ConfigHelper.GetStringProperty("OnePAYNoiDia.Hashcode", string.Empty);
                merchantId = ConfigHelper.GetStringProperty("OnePAYNoiDia.MerchantID", string.Empty);
                accesscode = ConfigHelper.GetStringProperty("OnePAYNoiDia.Accesscode", string.Empty);
                queryDRUser = ConfigHelper.GetStringProperty("OnePAYNoiDia.QueryDR.User", string.Empty);
                queryDRPassword = ConfigHelper.GetStringProperty("OnePAYNoiDia.QueryDR.Password", string.Empty);
                returnUrl = SiteUtils.GetNavigationSiteRoot() + "/product/onepaycallback.aspx";
            }
            else
            {
                urlPayment = ConfigHelper.GetStringProperty("OnePAYQuocTe.URLPayment", string.Empty);
                hashcode = ConfigHelper.GetStringProperty("OnePAYQuocTe.Hashcode", string.Empty);
                merchantId = ConfigHelper.GetStringProperty("OnePAYQuocTe.MerchantID", string.Empty);
                accesscode = ConfigHelper.GetStringProperty("OnePAYQuocTe.Accesscode", string.Empty);
                queryDRUser = ConfigHelper.GetStringProperty("OnePAYQuocTe.QueryDR.User", string.Empty);
                queryDRPassword = ConfigHelper.GetStringProperty("OnePAYQuocTe.QueryDR.Password", string.Empty);
                returnUrl = SiteUtils.GetNavigationSiteRoot() + "/product/onepaycallbackqt.aspx";
            }

            return (hashcode.Length > 0 && urlPayment.Length > 0 && merchantId.Length > 0 && accesscode.Length > 0);
        }

        public static string DoQueryDR(int paymentMethod)
        {
            string responseFromServer = string.Empty;

            //https://mtf.onepay.vn/developer/?page=modul_noidia
            //https://mtf.onepay.vn/developer/?page=modul_quocte
            // Thông số tài khoản cổng thanh toán: 
            string urlPayment = string.Empty;
            string hashcode = string.Empty;
            string merchantId = string.Empty;
            string accesscode = string.Empty;
            string returnUrl = string.Empty;
            string queryDRUser = string.Empty;
            string queryDRPassword = string.Empty;
            
            if (GetOnePayParameter(paymentMethod, out urlPayment, out hashcode, out merchantId, out accesscode, out returnUrl, out queryDRUser, out queryDRPassword))
            {
                string postData = string.Empty;
                string seperator = string.Empty;
                int paras = 7;

                string[,] array =
			    {
			        {"vpc_AccessCode", accesscode},
			        {"vpc_Command", "queryDR"},           
			        {"vpc_MerchTxnRef", Guid.NewGuid().ToString()},			
                    {"vpc_Merchant", merchantId},						
                    {"vpc_Password", queryDRPassword},
			        {"vpc_User", queryDRUser},
			        {"vpc_Version", "1"}
			    };
                for (int i = 0; i < paras; i++)
                {
                    postData = postData + seperator + HttpUtility.UrlEncode(array[i, 0]) + "=" + HttpUtility.UrlEncode(array[i, 1]);
                    seperator = "&";
                }

                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlPayment);

                    //  WebRequest request = WebRequest.Create(vpc_Host);
                    request.Method = "POST";

                    // Create POST data and convert it to a byte array.
                    //string postData = "This is a test that posts this string to a Web server.";
                    byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                    // Set the ContentType property of the WebRequest.         
                    request.UserAgent = "HTTP Client";
                    request.ContentType = "application/x-www-form-urlencoded";
                    // Set the ContentLength property of the WebRequest.
                    request.ContentLength = byteArray.Length;
                    // Get the request stream.
                    Stream dataStream = request.GetRequestStream();
                    // Write the data to the request stream.
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    // Close the Stream object.
                    dataStream.Close();
                    // Get the response.
                    //    WebResponse response = request.GetResponse();
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    // Display the status.
                    //   Response.WriteLine(((HttpWebResponse)response).StatusDescription);
                    // Get the stream containing content returned by the server.
                    dataStream = response.GetResponseStream();
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    responseFromServer = reader.ReadToEnd();
                    // Display the content.
                    //  Response.WriteLine(responseFromServer);
                    // Clean up the streams.
                    reader.Close();
                    dataStream.Close();
                    response.Close();
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
            }

            return responseFromServer;
        }
    }
}