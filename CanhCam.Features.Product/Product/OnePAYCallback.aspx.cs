using log4net;
using CanhCam.Business;
using CanhCam.Business.WebHelpers;
using CanhCam.Web.Framework;
using CanhCam.SearchIndex;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Web.UI.WebControls;
using CanhCam.Web.UI;
using System.Xml;

namespace CanhCam.Web.ProductUI
{

    public partial class OnePAYCallback : System.Web.UI.Page
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(OnePAYCallback));

        #region OnInit

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Load += new EventHandler(this.Page_Load);
        }

        #endregion

        private void Page_Load(object sender, EventArgs e)
        {
            string hashcode = ConfigHelper.GetStringProperty("OnePAYNoiDia.Hashcode", string.Empty);
            var redirectUrl = OnePayHelper.ProcessCallbackData(hashcode);
            WebUtils.SetupRedirect(this, redirectUrl);
        }

    }
}