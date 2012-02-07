using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CriticalResults
{
	/// <summary>
	/// 
	/// </summary>
	public class HTTPBasicAuthenticationHeader
	{

		
		//public const string BASIC_AUTH_HEADER_NAME = "Authorization";
		//public const string BASIC_AUTH_HEADER_TYPE = "Basic";

		public const string ANCR_AUTH_HEADER_NAME = "ANCR";
		public const string ANCR_AUTH_HEADER_TYPE = "Token";

		public string UserName { get; set; }
		public string Password { get; set; }
        public string Method { get; set; }

		public HTTPBasicAuthenticationHeader(string authHeader, string method)
		{
			UserName = null;
			Password = null;
            Method = method;

			if (!string.IsNullOrEmpty(authHeader))
			{
				string[] headerArr = authHeader.Split(' ');
				if (headerArr[0] == ANCR_AUTH_HEADER_TYPE)
				{
					string decodedAuth = DecodeBase64(headerArr[1]);
					string[] valArr = decodedAuth.Split(':');
					UserName = valArr[0];
					Password = valArr[1];
				}
			}
		}

		public static HTTPBasicAuthenticationHeader GetFromWCF()
		{
            return GetFromWCF(System.ServiceModel.Web.WebOperationContext.Current);
		}

        public static HTTPBasicAuthenticationHeader GetFromWCF(System.ServiceModel.Web.WebOperationContext context)
        {
            if (context != null)
            {
                string authHeader = context.IncomingRequest.Headers[ANCR_AUTH_HEADER_NAME];
                string method = context.IncomingRequest.Method;
                return new HTTPBasicAuthenticationHeader(authHeader, method);
            }
            else
            {
                return null;
            }
        }

        private string DecodeBase64(string str)
		{
			byte[] decbuff = Convert.FromBase64String(str);
			return System.Text.Encoding.UTF8.GetString(decbuff);
		}
	}
}
