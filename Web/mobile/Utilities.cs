using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

namespace Mobile
{
	public static class Utilities
	{
		public static string GetIp(HttpRequest request)
		{
			string iplist = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
			string ip = "";
			if (!string.IsNullOrEmpty(iplist))
			{
				string[] ips = iplist.Split(new char[] { ',' });
				ip = ips[ips.Length - 1];
			}
			else
			{
				ip = request.ServerVariables["REMOTE_ADDR"];
			}
			return ip;
		}

        public static string GetIP4Address()
        {
            foreach (IPAddress IPA in Dns.GetHostAddresses(HttpContext.Current.Request.UserHostAddress))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                    return (IPA.ToString());

            }
            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                    return IPA.ToString();
            }

            return "";
        }
	}
}
