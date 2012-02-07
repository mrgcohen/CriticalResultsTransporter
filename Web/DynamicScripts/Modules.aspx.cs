using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CriticalResults.DynamicScripts
{
    public partial class Modules : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.JSExtenstions != null)
            {
                foreach (string module in Properties.Settings.Default.JSExtenstions)
                {
                    Response.ContentType = "application/x-javascript";
                    Response.WriteFile("..\\" + module);
                    Response.Write("\n");
                }
            }
        }
    }
}
