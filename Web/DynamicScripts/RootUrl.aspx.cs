using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CriticalResults.DynamicScripts
{
    public partial class RootUrl : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/x-javascript";
            Response.Write(string.Format("DefaultDomain = '{0}';", Properties.Settings.Default.DefaultDomain));
            Response.Write(string.Format("AdminEmailAddress = '{0}';", Properties.Settings.Default.AdminEmailAddress));
            Response.Write(string.Format("RootUrl = '{0}';", ResolveUrl("~/")));
        }
    }
}
