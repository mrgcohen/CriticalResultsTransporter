using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Diagnostics;
using System.Net.Mail;
using CriticalResults.Properties;

namespace CriticalResults
{
	/// <summary>
	/// This is the default SMTP Transport service for Critical Results.
	/// 
	/// Modified: 2009-09-08, Jeremy Richardson
	///		-Fixed major defects in the email subsystem.
	/// </summary>
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class SmtpTransportService : ITransportService, ISystemMessageSender
	{
		private TraceSource _Trace;
		private string _Subject;
		SimpleEmailer _Emailer;
		
		private enum Events
		{
			SendError,
			SendRequest,
			SendSuccess
		}
		
		public SmtpTransportService()
		{
			string traceName = this.GetType().FullName;
			Trace.WriteLine(string.Format("Creating TraceSource {0}.  Please register a listener for detailed information.", traceName));
			_Trace = new TraceSource(traceName);

			InitializeSmtpClient();
		}
		private void InitializeSmtpClient()
		{
            string host = Settings.Default.SmtpServerIP;
            int port = Settings.Default.SmtpServerPort;

            string userName = Settings.Default.SmtpAccountUserName;
            string password = Settings.Default.SmtpAccountPassword;
            string domain = Settings.Default.SmtpAccountDomain;
            bool useSsl = Settings.Default.UseSsl;

            string fromAddress = Settings.Default.SmtpServerFromAddress;
            string fromName = Settings.Default.SmtpServerFromName;

            _Subject = Settings.Default.EmailSubject;

			_Emailer = new SimpleEmailer(host, port, userName, password, domain, useSsl, fromAddress, fromName);
		}
		
		#region ITransportService Members

		public bool RequestNotification(string notificationUid, string address, string value)
		{
			_Trace.TraceEvent(TraceEventType.Verbose, (int)Events.SendRequest, string.Format("RequestNotification( {0}, {1}, {2} )", notificationUid, address, value));

			MailAddress to = new MailAddress(address);
			MailMessage message = new MailMessage(_Emailer.From, to);
			message.Subject = _Subject;
			message.Body = value;
			try
			{
				_Emailer.Client.Send(message);
			}
			catch (System.Net.Mail.SmtpFailedRecipientException ex)
			{
				_Trace.TraceEvent(TraceEventType.Error, (int)Events.SendError, string.Format("{0}\r\n{1}", notificationUid, ex.ToString()));
				return false;
			}
			catch (System.Net.Mail.SmtpException ex)
			{
				_Trace.TraceEvent(TraceEventType.Error, (int)Events.SendError, string.Format("{0}\r\n{1}", notificationUid, ex.ToString()));
				return false;
			}
			_Trace.TraceEvent(TraceEventType.Verbose, (int)Events.SendSuccess, notificationUid);
			return true;
		}
		#endregion

		[WebInvoke(Method = "POST", UriTemplate = "SendSystemMessage/{address}", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
		public bool SendSystemMessage(string address, string value)
		{
			_Trace.TraceEvent(TraceEventType.Verbose, (int)Events.SendRequest, string.Format("SendSystemMessage( {0}, {1} )", address, value));

			MailAddress to = new MailAddress(address);
			MailMessage message = new MailMessage(_Emailer.From, to);
			message.Subject = _Subject;
			message.Body = value;
			try
			{
				_Emailer.Client.Send(message);
			}
			catch (System.Net.Mail.SmtpFailedRecipientException ex)
			{
				_Trace.TraceEvent(TraceEventType.Error, (int)Events.SendError, string.Format("{0}\r\n{1}", "System Message", ex.ToString()));
				return false;
			}
			catch (System.Net.Mail.SmtpException ex)
			{
				_Trace.TraceEvent(TraceEventType.Error, (int)Events.SendError, string.Format("{0}\r\n{1}", "System Message", ex.ToString()));
				return false;
			}
			_Trace.TraceEvent(TraceEventType.Verbose, (int)Events.SendSuccess, "System Message");
			return true;
		}

		[WebInvoke(Method = "POST", UriTemplate = "SendSystemMessage/{address}/json", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public bool SendSystemMessage_Json(string address, string value)
		{
			return SendSystemMessage(address, value);
		}
	}
}
