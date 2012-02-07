using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Diagnostics;

namespace CriticalResults
{
	public class SystemEmailer
	{
		/// <summary>
		///	SystemEmailer provides services to send system emails.
		///	
		/// Note: this is done independently of the SmtpTransport, which follows the Transport plug-in model and should not be directly included.
		///	
		/// Created: 2009-08-06, John Morgan
		/// Modified: 2009-09-08, Jeremy Richardson
		///		-Reworked base email classes to fix numerous defects
		///		-Changed to use composition over inheritance
		///		-Renamed class
		///		
		/// TODO: UseSSL -> UseSsl for consistency
		/// TODO: Add FromAddress, FromName
		/// TODO: Add "Smtp" to various settings to disambiguate.  For Example: "AccountName" becomes "SmtpAccountName"
		/// </summary>

        TraceSource _TraceSource;
		SimpleEmailer _Mailer;

		public SystemEmailer()
		{
			string traceName = this.GetType().FullName;
			Trace.WriteLine(string.Format("Creating TraceSource {0}.  Please register a listener for detailed information.", traceName));
			_TraceSource = new TraceSource(traceName);

			LoadSettingsFromDatabase();
		}

		public bool SendMessage(string to, string subject, string message)
		{
			try
			{
				MailMessage mailMessage = new MailMessage(_Mailer.From.Address.ToString(), to, subject, message);
				_Mailer.Client.Send(mailMessage);
                _TraceSource.TraceInformation("Message sent successfully.");
			}
			catch (System.Net.Mail.SmtpException ex)
			{
                _TraceSource.TraceEvent(TraceEventType.Error, -100, ex.Message);
                if (ex.InnerException != null)
                {
                    _TraceSource.TraceEvent(TraceEventType.Error, -100, ex.InnerException.Message);
                }
				return false;
			}
			return true;
		}

		public void LoadSettingsFromDatabase()
		{
			CriticalResultsEntityManager manager = new CriticalResultsEntityManager();
			SettingEntity[] settings = manager.GetSettings("System");

			string host = null;
			int port = 587;

			string userName = "";
			string password = "";
			string domain = "";
			bool useSsl = false;
			string fromAddress = null;
			string fromName = "Critical Results";

			foreach (SettingEntity setting in settings)
			{
				switch (setting.EntryKey)
				{
					case "SMTP_AccountName":
						if (!String.IsNullOrEmpty(setting.Value))
						{
							userName = setting.Value;
						}
						break;
					case "SMTP_AccountPassword":
						if (!String.IsNullOrEmpty(setting.Value))
						{
							password = setting.Value;
						}
						break;
					case "SMTP_AccountDomain":
						if (!String.IsNullOrEmpty(setting.Value))
						{
							domain = setting.Value;
						}
						break;
					case "SMTP_UseSSL":
						if (!String.IsNullOrEmpty(setting.Value))
						{
							bool.TryParse(setting.Value, out useSsl);
						}
						break;
					case "SMTP_ServerIP":
						if (!String.IsNullOrEmpty(setting.Value))
						{
							host = setting.Value;
						}
						break;
					case "SMTP_ServerPort":
						if (!String.IsNullOrEmpty(setting.Value))
						{
							port = Int32.Parse(setting.Value);
						}
						break;
					case "SMTP_FromAddress":
						if (!String.IsNullOrEmpty(setting.Value))
						{
							fromAddress = setting.Value;
						}
						break;
					case "SMTP_FromName":
						if (!String.IsNullOrEmpty(setting.Value))
						{
							fromName = setting.Value;
						}
						break;
				}
			}
				//_Mailer = new SimpleEmailer(host, port, userName, password, domain, useSsl, fromAddress, fromName);
			_Mailer = new SimpleEmailer(host, port, userName, password, domain, useSsl, fromAddress, fromName);

		}
	}
}
