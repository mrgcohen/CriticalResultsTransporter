using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data.Entity;
using System.Diagnostics;
using CriticalResults;
using System.Net;

namespace Mobile
{
	public partial class WindowsLogin : System.Web.UI.Page
	{
		public string WindowsUser = "";
		public string UserName = "";
		public string TokenGuid = "";
		public string PageMessage = "";
		public const string AuthExtName = "Windows";

        private string[] MobileUserAgents = new string[]{
	            "iPhone",
	            "iPod",
	            "BlackBerry",
	            "Android",
	            "Windows CE",
	            "Palm"};

        protected void Page_Load(object sender, EventArgs e)
		{

			WindowsUser = Request.LogonUserIdentity.Name;
			string queryString = string.Format("it.Type='AuthExt' AND it.Key='{0}' AND it.Value='{1}'", AuthExtName, WindowsUser);
			CriticalResults.CriticalResultsEntityManager manager = new CriticalResultsEntityManager();
			CriticalResults.UserEntryEntity [] entries = manager.QueryUserEntryEntities(queryString, null, null);

            string userHostAddress = Utilities.GetIP4Address();

			if (entries.Count() == 1)
			{
				if (entries.First().User.Enabled == true)
				{
					UserName = entries.First().User.UserName;
					Session["UserName"] = UserName;
					CriticalResults.TokenEntity[] currentTokens = manager.GetTokensForUser(UserName);
					foreach (CriticalResults.TokenEntity t in currentTokens)
					{
						if (t.Ipv4 == userHostAddress)
						{
							TokenGuid = t.Token.ToString();
							Session["Token"] = TokenGuid.ToString();
						}
					}
					if (TokenGuid == "")
					{

						CriticalResults.TokenEntity token = manager.CreateToken(entries.First().User, userHostAddress);
						TokenGuid = token.Token.ToString();
						Session["Token"] = TokenGuid.ToString();
					}
                    Response.AddHeader("REFRESH", "5;URL=ResultList.aspx");

				}
				else
				{
					PageMessage = "The ANCR account associated with this Windows Login " + WindowsUser + " is disabled.  Please contact your System Administrator.";
					message.InnerText = PageMessage;
				}

			}
			else if(entries.Count() > 1)
			{
				PageMessage = "Multiple ANCR accounts resolved to this Windows User.  Until this is resolved you may not login with your Windows User Account.  Please contact your System Administrator.";
				message.InnerText = PageMessage;
			}
            else
            {
                PageMessage = "No ANCR account can be found for " + WindowsUser + ".  Please contact your System Administrator.";
                message.InnerText = PageMessage;
            }
		}

 
	}
}
