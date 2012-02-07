using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data.Entity;
using System.Diagnostics;
using CriticalResults;

namespace WindowsAuthExt
{
	public partial class WindowsLogin : System.Web.UI.Page
	{
		public static TraceSource _Trace;
		
		public string WindowsUser = "";
		public string UserName = "";
		public string TokenGuid = "";
		public string PageMessage = "";

		public const string AuthExtName = "Windows";

		static WindowsLogin()
		{
			string traceName = "WindowsAuthExt.WindowsLogin";
			System.Diagnostics.Trace.WriteLine(string.Format("Creating TraceSource( \"{0}\" ).  Configure a TraceListener to log information.", traceName));
			_Trace = new TraceSource(traceName);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			WindowsUser = Request.LogonUserIdentity.Name;
            Auth.Authenticate(AuthExtName, WindowsUser, null, Request.UserHostAddress, _Trace, out UserName, out TokenGuid, out PageMessage);
		}
	}
}
