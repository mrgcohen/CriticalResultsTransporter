using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CriticalResults;

namespace Mobile
{
	public partial class _Default : System.Web.UI.Page
	{
		ResultsService _ResultService;
		Guid? _Token = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			_ResultService = new ResultsService();
		}

		protected void mobileLogin_Authenticate(object sender, AuthenticateEventArgs e)
		{
			string ip = Utilities.GetIp(Request);
			_Token = _ResultService.GetToken(mobileLogin.UserName, mobileLogin.Password, ip);
			if (_Token == null)
			{
				e.Authenticated = false;
			}
			else
			{
				e.Authenticated = true;
				Session["Token"] = _Token.ToString();
				Session["UserName"] = mobileLogin.UserName;
				Response.Redirect("ResultList.aspx");
			}
		}
	}
}
