using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Mail;

using System.Diagnostics;

namespace CriticalResults
{
	/// <summary>
	/// Mailer provides quick and easy initialization of the .Net SmtpClient object
	/// </summary>
	public class SimpleEmailer
	{
		private TraceSource _Trace;
		private string _Subject = "Critical Radiology Result Alert";

		public string Subject
		{
			get { return _Subject; }
			set { _Subject = value; }
		}
		
		private SmtpClient _Client;
		public SmtpClient Client
		{
			get { return _Client; }
		}

		private MailAddress _From;
		public MailAddress From
		{
			get { return _From; }
		}
		
		public SimpleEmailer(string host, int port, string userName, string password, string domain, bool useSsl, string fromAddress, string fromName)
		{
			string traceName = this.GetType().FullName;
			Trace.WriteLine(string.Format("Creating TraceSource {0}.  Please register a listener for detailed information.", traceName));
			_Trace = new TraceSource(traceName);
			
			_Trace.TraceInformation("Creating SmtpTransport: UserName:{0}, Password: -, Domain:{1}, Host:{2}, Port:{3}, SSL:{4}", userName, domain, host, port, useSsl);
			
			_Client = new SmtpClient(host,port);
			_Trace.TraceInformation("Created SMTP Client - {0}:{1}", host, port);
			
			_Client.EnableSsl = useSsl;

			_From = new MailAddress(fromAddress, fromName);
			_Trace.TraceInformation("From address set to {0} [{1}]", _From.Address, _From.DisplayName);

			if (!string.IsNullOrEmpty(userName))
			{
				_Client.UseDefaultCredentials = false;
				_Client.Credentials = new System.Net.NetworkCredential(userName, password, domain);
				_Trace.TraceInformation("Created credentials for {0}\\{1}", domain, userName);
			}
			else
			{
				_Client.UseDefaultCredentials = true;
				_Trace.TraceInformation("Using default credentials");
			}
		}
	}
}
