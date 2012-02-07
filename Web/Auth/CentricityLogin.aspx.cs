using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Data.Entity;
using CriticalResults;

namespace CentricityAuthExt
{
	public partial class _Default : System.Web.UI.Page
	{
		public static TraceSource _Trace;
		public string CentricityUserName = "";
		public string UserName = "";
		public string Token = "";
		public const string AuthExtName = "Centricity";

        static _Default()
        {
            string traceName = "CentricityAuthExt.CentricityLogin";
            System.Diagnostics.Trace.WriteLine(string.Format("Creating TraceSource( \"{0}\" ).  Configure a TraceListener to log information.", traceName));
            _Trace = new TraceSource(traceName);
        }

		protected void Page_Load(object sender, EventArgs e)
		{
            if (Request.IsSecureConnection)
            {
                if (!CriticalResults.Properties.Settings.Default.CentricityLoginHTTPS)
                {
                    throw new CriticalResults.ResultsService.SecurityException();
                }
            }
			CentricityUserName = DecodeBase64(Request.QueryString["CentricityUserName"]);
            string message;
            Auth.Authenticate(AuthExtName, CentricityUserName, null, Request.UserHostAddress, _Trace, out UserName, out Token, out message);
		}

		private string DecodeBase64(string str)
		{
			byte[] decbuff = Convert.FromBase64String(str);
			return System.Text.Encoding.UTF8.GetString(decbuff);
		}
	}
}
