using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CriticalResults
{
    public partial class _default : System.Web.UI.Page
    {
        private string[] MobileUserAgents = new string[]{
	"iPhone",
	"iPod",
	"BlackBerry",
	"Android",
	"Windows CE",
	"Palm"};
	
        protected void Page_Load(object sender, EventArgs e)
        {
            string userAgent = Request.ServerVariables.GetValues("HTTP_USER_AGENT")[0].ToString();
            string mobileRedirect = Properties.Settings.Default.MobileRedirectTarget;
            if(!String.IsNullOrEmpty(mobileRedirect))
            {
                foreach (string agent in MobileUserAgents)
                {
                    if (userAgent.Contains(agent))
                    {
                        Response.Redirect(mobileRedirect);
                        return;
                    }
                }
            }
            string qs = "";
            if (!string.IsNullOrEmpty(Request.QueryString.ToString()))
            {
                qs = "?" + Request.QueryString.ToString();
            }
            Response.Redirect("Default.htm" + qs);

        }
    }
}
