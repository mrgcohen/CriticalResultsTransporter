using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using System.Timers;
using CriticalResults.Properties;
using CriticalResults.ResultsServiceReference;
using CriticalResults.TransportServiceReference;
using System.IO;
using System.Collections;
using System.ServiceModel;

namespace CriticalResults
{
	/// <summary>
	/// The HybridAgent only uses web services for Transport communication.
	/// Database access is done through the Entities layer.
	/// </summary>
	public class HybridAgent
	{
		private const string DEFAULT_MESSAGE_FILE = "message.txt";
		private const string REMINDER_MESSAGE_FILE_PART = "reminder.";
		private const string ESCALATION_MESSAGE_FILE_PART = "escalated.";
		static string MIN_DATE_STRING = "2008-01-01 00:00:01";
		const string DATE_FORMAT_STRING = "yyyy-MM-dd HH:mm:ss";

		private TraceSource _Tracer;

		private System.Threading.Timer _Timer;

		CriticalResultsEntityManager _Manager;

		Dictionary<Guid, LevelEntity> _Levels;
		Dictionary<string, UserEntity> _ClinicalAdministrators;
		Dictionary<string, TransportServiceClient> _TransportServices;

		private bool EnableNotification { get { return Settings.Default.EnableNotification; } }

		private bool EnableEscalation { get { return Settings.Default.EnableEscalation; } }
		private string EscalationTransportName { get { return Settings.Default.EscalationTransportName; } }
		private TimeSpan EscalationRepeatTimeout { get { return Settings.Default.EscalationRepeatTimeout; } }
		
		Dictionary<Guid, ResultEntity> _UnacknowledgedResults;
		DateTime _LastEscalationCheck = DateTime.MinValue;
		
		private bool EnableDigestNotification { get { return Settings.Default.EnableReminder; } }
		private string ReminderNotificationTransportName { get { return Settings.Default.ReminderTransportName; } }
		private DateTime ReminderTime { get { return Settings.Default.ReminderTime; } }
		
		private Dictionary<string, int> _ReminderEntries;	//User address, count
		private DateTime _LastReminderNotification;

		private const int MAX_RETRIES = 1;

        private long _TimerInterval = (long)Settings.Default.TimerInterval.TotalMilliseconds;
		
		private enum AgentEvents
		{
			SendError,
			SendSuccess,
			SendReminders,
			SendReminderSuccess,
			SendReminderError,

			EscalatedAlert,
			
			DefaultMessageTemplateUsed,
			NotificationLoop,
			NoPendingNotifications,

			UnhandledException,

			NotificationDisabled,
			ReminderDisabled,
			EscalationDisabled,

			GeneralEvent,
		}
		
		/// <summary>
		///	The Agent checks for pending notifications and delivers them to the proper Transport.
		/// Hybrid Agent uses DB (entity) access for the data model.
		/// Transport calls are made by web service.
		/// 
		/// Modifed: 2009-11-1, Jeremy Richardson
		///		-Fixed issues with local FS when running as a service
		///		-Added extra exception handling/logging for more robust Service operation
        ///		
        /// Modified: 2010-08-30, John Morgan
        ///     -Changed agent to utilize System.Threading.Timer as opposed to System.Timers.Timer due to exception swallowing
		/// </summary>
		public HybridAgent()
		{
			string traceName = this.GetType().FullName;
			Trace.WriteLine(string.Format("Registering Trace Source {0}.", traceName));
			_Tracer = new TraceSource(traceName);

			System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
			_Tracer.TraceInformation("Current Directory Set To {0}", System.IO.Directory.GetCurrentDirectory().ToString());

			if (DateTime.Now.TimeOfDay < Settings.Default.ReminderTime.TimeOfDay)
				_LastReminderNotification = DateTime.Now - new TimeSpan(1, 0, 0, 0);
			else
				_LastReminderNotification = DateTime.Now;

			_Tracer.TraceInformation(this.GetType().Assembly.GetName().Version.ToString());
				string configValues = "\r\n---Configuration---";
			configValues += string.Format("Enable Escalation: {0}\r\n", Settings.Default.EnableEscalation);
			configValues += string.Format("Enable Notification: {0}\r\n", Settings.Default.EnableNotification);
			configValues += string.Format("Enable Reminder: {0}\r\n", Settings.Default.EnableReminder);
			configValues += string.Format("Escalation Repeat Timeout: {0}\r\n", Settings.Default.EscalationRepeatTimeout);
			configValues += string.Format("Escalation Transport Name: {0}\r\n", Settings.Default.EscalationTransportName);
			configValues += string.Format("Critical Results URI: {0}\r\n", Settings.Default.CriticalResultsUri);
			configValues += string.Format("Reminder Time: {0}\r\n", Settings.Default.ReminderTime);
			configValues += string.Format("Reminder Transport: {0}\r\n", Settings.Default.ReminderTransportName);
			configValues += string.Format("Timer Interval: {0}\r\n", Settings.Default.TimerInterval);
			configValues += "---Configuration End---";
			_Tracer.TraceInformation(configValues);


            
            System.Threading.TimerCallback timerCallback = this.OnTimerElapsed;
            _Timer = new System.Threading.Timer(timerCallback, null, 0, _TimerInterval);
            //_Timer.Interval = Settings.Default.TimerInterval.TotalMilliseconds;
            //_Timer.Elapsed += new ElapsedEventHandler(OnTimerElapsed);
            //_Timer.Start();


		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			_Tracer.TraceInformation("Unhandled Exception\r\nExiting{1}\r\nException Follows\r\n{0}", e.ExceptionObject.ToString(), e.IsTerminating);
			if (!e.IsTerminating)
			{
				System.Environment.Exit(-1);
			}
		}

		public void GetTransports()
		{
			_TransportServices = new Dictionary<string, TransportServiceClient>();
			TransportEntity[] transports = _Manager.GetAllTransports();
			foreach (TransportEntity transport in transports)
			{
				TransportServiceClient service = new TransportServiceClient("WSHttpBinding_ITransportService", transport.TransportURI);
				_TransportServices.Add(transport.Name, service);
			}
		}

		/// <summary>
		/// Modified: 2009-08-06, Jeremy Richardson
		///		-Changed role name literal to item in ApplicationRoles enumeration.
		/// </summary>
		public void GetClinicalAdministrators()
		{
			string queryString = "";

			_ClinicalAdministrators = new Dictionary<string, UserEntity>();
			UserEntity[] users = _Manager.QueryUserEntity(queryString, null, null);
			foreach (UserEntity user in users)
			{
				if (!user.Roles.IsLoaded)
				{
					user.Roles.Load();
				}

				var query = from roles in user.Roles
							where roles.Name == ApplicationRoles.clinAdmin.ToString()
							select roles;
				if (query.Count() > 0)
				{
					_ClinicalAdministrators.Add(user.UserName, user);
				}
			}
		}
		public void GetLevels()
		{
			_Levels = new Dictionary<Guid, LevelEntity>();
			LevelEntity[] levels = _Manager.GetLevels();
			foreach (LevelEntity level in levels)
			{
				_Levels.Add(level.Uuid, level);
			}
		}

		private int _ConsecutiveErrors = 0;

		/// <summary>
		/// Modified: 2009-11-1, Jeremy Richardson
		///		-Catch Generic exception for logging
		///		-Expose OnException event to pass to object host
		///		-Throw Generic exception
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnTimerElapsed(object stateInfo)
		{
			try
			{
                _Timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
				Console.Write(".");
				_Manager = new CriticalResultsEntityManager();

				GetClinicalAdministrators();
				GetTransports();
				GetLevels();

				RunBuildEscalationAndReminders();
				RunNotify();
				RunReminder();

                _Timer.Change(0, _TimerInterval);

			}
			catch (Exception ex)
			{
				_Tracer.TraceEvent(TraceEventType.Critical, (int)AgentEvents.UnhandledException, ex.ToString());
				throw (ex);
			}
		}

		private void RunReminder()
		{
			if (!Settings.Default.EnableReminder)
			{
				_Tracer.TraceEvent(TraceEventType.Verbose, 0, "Reminders disabled.");
				return;
			}
			if (ShouldGenerateReminder())
			{
				GenerateReminder();
			}
		}

		private bool ShouldGenerateReminder()
		{
			if ((DateTime.Now.Day > _LastReminderNotification.Day || DateTime.Now.Month != _LastReminderNotification.Month) && DateTime.Now.TimeOfDay > ReminderTime.TimeOfDay)
			{
				_Tracer.TraceEvent(TraceEventType.Information, (int)AgentEvents.SendReminders, "Send Reminders.  Now: {0}, Last Reminder: {1}", DateTime.Now.ToString(), _LastReminderNotification.ToString());
				return true;
			}
			_Tracer.TraceEvent(TraceEventType.Verbose, 0, "Reminders not sent.  Now: {0}, Last Reminder: {1}", DateTime.Now.ToString(), _LastReminderNotification.ToString());
			return false;

		}

		private void AddReminderEntry(UserEntity user)
		{
			UserTransportEntity transport = _Manager.GetUserTransport(user.UserName, ReminderNotificationTransportName);

			if (!_ReminderEntries.ContainsKey(transport.Address))
			{
				if (string.IsNullOrEmpty(transport.Address))
				{
					_Tracer.TraceEvent(TraceEventType.Warning, (int)AgentEvents.SendReminderError, "Address for reminder was missing.");
				}
				else
				{
					_ReminderEntries.Add(transport.Address, 1);
				}
			}
			else
			{
				_ReminderEntries[transport.Address]++;
			}
		}

		private void GenerateReminder()
		{
			string reminderStatusMessage;
			
			_LastReminderNotification = DateTime.Now;

			reminderStatusMessage = string.Format("Reminder Notification run at {0}\r\n", _LastReminderNotification);

			foreach (string address in _ReminderEntries.Keys)
			{
					bool ok = true;
					Guid g = Guid.NewGuid();

					string message = File.ReadAllText(ReminderNotificationTransportName + "." + REMINDER_MESSAGE_FILE_PART + DEFAULT_MESSAGE_FILE);

					string notificationInfo = string.Format("{0}: Generating reminder message for '{1}'.", DateTime.Now, address);

					try
					{
						ok = AdvancedRequestNotification(ReminderNotificationTransportName, g.ToString(), address, message);
						
						if (ok)
						{
							notificationInfo = string.Format("{0} {1}", AgentEvents.SendReminderSuccess, notificationInfo);
							_Tracer.TraceEvent(TraceEventType.Information, (int)AgentEvents.SendReminderSuccess, notificationInfo);
						}
						else
						{
							notificationInfo = string.Format("{0} {1}", AgentEvents.SendReminderError, notificationInfo);
							_Tracer.TraceEvent(TraceEventType.Information, (int)AgentEvents.SendReminderError, notificationInfo);
						}
					}
					catch (Exception ex)
					{
						reminderStatusMessage = ex.ToString() + reminderStatusMessage;
						_Tracer.TraceEvent(TraceEventType.Error, (int)AgentEvents.SendReminderError, notificationInfo + "\r\n\t***Reminder notification failed.***\r\n");
						throw (ex);
					}
				
			}
		}

		/// <summary>
		/// Runs to escalate any unacknowledged Critical Result Alerts
		/// -Send to sender
		/// Send at:
		/// -Escalation time
		/// Created: 2009-08-07, Jeremy Richardson
		/// Modified: 2010-02-10, Jeremy Richardson
		/// - To Sender only
		/// - At escalation time only
		/// - If direct contact required: escalate
		/// - If no direct contact required: add to digest list
		/// </summary> 
		void RunBuildEscalationAndReminders()
		{
			_ReminderEntries = new Dictionary<string, int>();

			if (_LastEscalationCheck == DateTime.MinValue)
			{
				_LastEscalationCheck = DateTime.Parse(MIN_DATE_STRING);
			}

			string queryString = string.Format("it.AcknowledgmentTime IS NULL AND it.CreationTime > DATETIME'{0}'", _LastEscalationCheck.ToString(DATE_FORMAT_STRING));
			ResultListEntity[] currentResultsArray = _Manager.QueryResultList(queryString, null, null);
			
			Dictionary<Guid, ResultEntity> currentResultDict = new Dictionary<Guid,ResultEntity>(currentResultsArray.Length);
			
			foreach (ResultListEntity entry in currentResultsArray)
			{
				LevelEntity level = _Levels[entry.LevelUuid];

				ResultEntity result;

				result = _Manager.GetResultEntity(entry.ResultUuid);

				currentResultDict.Add(result.Uuid, result);

				if (ShouldBeEscalated(result))
				{
					if (EnableEscalation)
					{
						_Tracer.TraceEvent(TraceEventType.Verbose, 0, "{0}: Escalating, ID:{1}, UUID:{2}, Created:{3}, Escalated:{4}, Due:{5}", DateTime.Now, result.Id, result.Uuid, result.CreationTime, result.EscalationTime, result.DueTime);
						Escalate(result, EscalationTransportName);
					}
					else
					{
						_Tracer.TraceEvent(TraceEventType.Verbose, 0, "{0}: Escalation Disabled, ID:{1}, UUID:{2}, Created:{3}, Escalated:{4}, Due:{5}", DateTime.Now, result.Id, result.Uuid, result.CreationTime, result.EscalationTime, result.DueTime);
					}
				}

				
			}			
			_UnacknowledgedResults = currentResultDict;
		}

		/// <summary>
		/// Checks whether the alert should be escalated.
		/// Adds alert to the reminder list.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private bool ShouldBeEscalated(ResultEntity result)
		{
			DateTime lastNotification = DateTime.MinValue;

			if (DateTime.Now < result.EscalationTime)
			{
				_Tracer.TraceEvent(TraceEventType.Verbose, 0, "{0}: Not Escalating, Escalation Time Not Met, ID:{1}, UUID:{2}, Created:{3}, Escalated:{4}, Due:{5}", DateTime.Now, result.Id, result.Uuid, result.CreationTime, result.EscalationTime, result.DueTime);
				return false;
			}
			
			AddReminderEntry(result.Sender);
			AddReminderEntry(result.Receiver);

			if (!result.LevelReference.IsLoaded)
				result.LevelReference.Load();

			if (!result.Level.DirectContactRequired)
			{
				_Tracer.TraceEvent(TraceEventType.Verbose, 0, "{0}: Not Escalating, Direct Contact Not Specified, ID:{1}, UUID:{2}, Created:{3}, Escalated:{4}, Due:{5}", DateTime.Now, result.Id, result.Uuid, result.CreationTime, result.EscalationTime, result.DueTime);
				return false;
			}
			
			result.Notifications.Load(System.Data.Objects.MergeOption.OverwriteChanges);
			if (result.Notifications.Count() == 0)
			{
				_Tracer.TraceEvent(TraceEventType.Warning, 0, "{0}: Escalating, No Notifications, ID:{1}, UUID:{2}, Created:{3}, Escalated:{4}, Due:{5}", DateTime.Now, result.Id, result.Uuid, result.CreationTime, result.EscalationTime, result.DueTime);
				return true;
			}

			foreach (NotificationEntity notification in result.Notifications)
			{
				if (lastNotification == DateTime.MinValue)
					lastNotification = notification.CreationTime;
				else if (notification.CreationTime > lastNotification)
					lastNotification = notification.CreationTime;
			}
			if (lastNotification < result.EscalationTime)
			{
				return true;
			}
			else
			{
				_Tracer.TraceEvent(TraceEventType.Verbose, 0, "{0}: Not Escalating, last notification was at {6}, after escalation time, ID:{1}, UUID:{2}, Created:{3}, Escalated:{4}, Due:{5}", DateTime.Now, result.Id, result.Uuid, result.CreationTime, result.EscalationTime, result.DueTime, lastNotification.ToString());
				return false;
			}
		}
		private void Escalate(ResultEntity result, string transportName)
		{
			_Tracer.TraceEvent(TraceEventType.Information, (int)AgentEvents.EscalatedAlert, "{0}: Sender: {1} Result: {2} Escalation Time: {3}", AgentEvents.EscalatedAlert.ToString(), result.Sender.UserName, result.Id, result.EscalationTime);
			_Manager.CreateResultNotification(result.Sender.UserName, transportName, result.Uuid, "Escalate", Notification.NotificationState.New_Escalated.ToString());
		}

		/// <summary>
		/// Notes: 
		/// Created: 2009-07-18, Jeremy Richardson
		/// Modified: 2009-08-06, Jeremy Richardson
		/// -Added functionality: send to configured transports; send on escalated state
		/// </summary>
		void RunNotify()
		{
			Dictionary<string, string> valueDict;
			
			string queryString = string.Format("it.State = '{0}' OR it.State = '{1}'", Notification.NotificationState.New.ToString(), Notification.NotificationState.New_Escalated.ToString());
			NotificationEntity[] notifications = _Manager.QueryNotificationEntity(queryString, null, null);

			_Tracer.TraceEvent(TraceEventType.Verbose, (int)AgentEvents.NotificationLoop, "Notification Loop: {0} pending notifications", notifications.Count());
			
			foreach (NotificationEntity notification in notifications)
			{
				_Tracer.TraceEvent(TraceEventType.Information, (int)AgentEvents.NotificationLoop, "Id: {0} Uuid: {1} Created: {2} Now: {3}", notification.Id, notification.Uuid, notification.CreationTime, DateTime.Now);
				
				valueDict = new Dictionary<string, string>();
				
				Guid g = Guid.NewGuid();

				string messageFileName = DEFAULT_MESSAGE_FILE;

				string successState;
				if (notification.State == Notification.NotificationState.New_Escalated.ToString())
				{
					successState = Notification.NotificationState.Sent_Escalated.ToString();
					messageFileName = ESCALATION_MESSAGE_FILE_PART + messageFileName;
				}
				else
				{
					successState = Notification.NotificationState.Sent.ToString();
				}

				messageFileName = notification.UserTransport.Transport.Name + "." + messageFileName;
				if (!File.Exists(messageFileName))
				{
					_Tracer.TraceEvent(TraceEventType.Warning, (int)AgentEvents.DefaultMessageTemplateUsed, "{0}: Please provide a custom template named: {1}", AgentEvents.DefaultMessageTemplateUsed.ToString(), messageFileName);
					messageFileName = DEFAULT_MESSAGE_FILE;
				}
				string message = File.ReadAllText(messageFileName);

				if (!notification.Result.SenderReference.IsLoaded)
				{
					_Tracer.TraceEvent(TraceEventType.Warning, 0, "Loading Result Sender Reference");
					notification.Result.SenderReference.Load();					
				}
				if (!notification.Result.ReceiverReference.IsLoaded)
				{
					_Tracer.TraceEvent(TraceEventType.Warning, 0, "Loading Result Receiver Reference");
					notification.Result.ReceiverReference.Load();
				}
				if (!notification.UserTransport.UserReference.IsLoaded)
				{
					_Tracer.TraceEvent(TraceEventType.Warning, 0, "Loading Notification User Reference");
					notification.UserTransport.UserReference.Load();
				}
				if (!notification.Result.ResultContexts.IsLoaded)
				{
					_Tracer.TraceEvent(TraceEventType.Warning, 0, "Loading Result Context Reference");
					notification.Result.ResultContexts.Load();
				}
				
				string senderName = string.Format("{0} {1}, {2}", notification.Result.Sender.FirstName, notification.Result.Sender.LastName, notification.Result.Sender.Credentials);
				string receiverName = string.Format("{0} {1}, {2}", notification.Result.Receiver.FirstName, notification.Result.Receiver.LastName, notification.Result.Receiver.Credentials);
				string recipientName = string.Format("{0} {1}, {2}", notification.UserTransport.User.FirstName, notification.UserTransport.User.LastName, notification.UserTransport.User.Credentials);
								
				string resultContextJson = notification.Result.ResultContexts.First().JsonValue;

				string [] notificationCommentArray = notification.Notes.Replace("\"", "").Split('\r', '\n');
				string comment1 = "";
				if (notificationCommentArray.Length > 0)
					comment1 = System.Web.HttpUtility.UrlDecode(notificationCommentArray[0]);
				string comment2 = "";
				if (notificationCommentArray.Length > 1)
					comment2 = System.Web.HttpUtility.UrlDecode(notificationCommentArray[notificationCommentArray.Length-1]);

				string alertUri = Settings.Default.CriticalResultsUri;
				alertUri = string.Format(alertUri + "&Action=open&ResultUuid={0}", notification.Result.Uuid);
				string secureAlertUri = Settings.Default.CriticalResultsSSLUri;
				secureAlertUri = string.Format(secureAlertUri + "&Action=open&ResultUuid={0}", notification.Result.Uuid);

				valueDict.Add("ALERT_URI", alertUri);
				valueDict.Add("SECURE_ALERT_URI", secureAlertUri);
				valueDict.Add("SENDER", senderName);
				valueDict.Add("RECEIVER", receiverName);
				valueDict.Add("RECIPIENT", recipientName);
				valueDict.Add("CURRENT_TIME", DateTime.Now.ToString());
				valueDict.Add("ALERT_CREATED_TIME", notification.Result.CreationTime.ToString());
				valueDict.Add("ALERT_DUE_TIME", notification.Result.DueTime.ToString());
				valueDict.Add("ALERT_ESCALATION_TIME", notification.Result.EscalationTime.ToString());
				valueDict.Add("LEVEL_NAME", notification.Result.Level.Name);
				valueDict.Add("LEVEL_DESCRIPTION", notification.Result.Level.Description);
				valueDict.Add("CALLBACK_INFO", comment1.Trim());
				valueDict.Add("COMMENTS", comment2.Trim());

				Dictionary<string, object> contextTable = ContextParser.ParseContext(resultContextJson); 
				foreach (string key in contextTable.Keys)
				{
					try
					{
						Dictionary<string, object> entry = (Dictionary<string, object>)contextTable[key];
						valueDict.Add(key, (string)entry["value"].ToString());
					}
					catch (Exception ex)
					{
						throw (ex);
					}
				}

				message = CreateMessage(message, valueDict);

				string transportName = notification.UserTransport.Transport.Name;
				string receiverAddress = notification.UserTransport.Address;
				
				string notificationStatusMessage = string.Format("\r\n---BEGIN MESSAGE---\r\nHEADER\r\nResult ID: {0}\r\nTo: {1} Notification ID:{2}, ---BODY---\r\n{3}\r\n---END MESSAGE---", g, notification.Id, receiverAddress, message);
				bool ok = true;

				try
				{
					if (EnableNotification)
					{
						ok = AdvancedRequestNotification(transportName, g.ToString(), receiverAddress, message);
					}
					else
					{
						_Tracer.TraceEvent(TraceEventType.Warning, (int)AgentEvents.NotificationDisabled, "-- {0} --", AgentEvents.NotificationDisabled.ToString(), message);
						ok = true;
					}
				}
				catch (Exception ex)
				{
					notificationStatusMessage = ex.ToString() + notificationStatusMessage;
					_Tracer.TraceEvent(TraceEventType.Error, (int)AgentEvents.SendError, notificationStatusMessage);
					_Manager.UpdateNotificationEntity(notification.Uuid, Notification.NotificationState.Error.ToString(), notification.Notes + "\r\n" + notificationStatusMessage);
					throw (ex);					
				}

				string timeStamp = ", " + DateTime.Now.ToString();

				if (ok)
				{
					_Tracer.TraceEvent(TraceEventType.Information, (int)AgentEvents.SendSuccess, notificationStatusMessage);
					_Manager.UpdateNotificationEntity(notification.Uuid, successState, notification.Notes + "\r\n" + notificationStatusMessage);
				}
				else
				{
					_Tracer.TraceEvent(TraceEventType.Error, (int)AgentEvents.SendError, notificationStatusMessage);
					_Manager.UpdateNotificationEntity(notification.Uuid, Notification.NotificationState.Error.ToString(), notification.Notes + "\r\n" + notificationStatusMessage);
				}
			}
		}
		public string CreateMessage(string messageTemplate, Dictionary<string, string> values)
		{
			foreach (string key in values.Keys)
			{
				if (values[key] is string)
				{
					messageTemplate = messageTemplate.Replace("{" + key.Trim() + "}", (string)values[key]);
				}
			}

			return messageTemplate;
		}

		/// <summary>
		/// AdvancedRequestNotification calls RequestNotification() for the provided service
		/// Timeout exceptions are handled by waiting for 30 seconds and then retrying until getting a response or reaching the maximum number of retries, at which point false is returned.
		/// </summary>
		/// <param name="transportName"></param>
		/// <param name="messageGuid"></param>
		/// <param name="address"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool AdvancedRequestNotification(string transportName, string messageGuid, string address, string message)
		{
			return AdvancedRequestNotification(transportName, messageGuid, address, message, 0);
		}
		public bool AdvancedRequestNotification(string transportName, string messageGuid, string address, string message, int retry)
		{
			if (retry > MAX_RETRIES)
			{
				_Tracer.TraceEvent(TraceEventType.Error, (int)AgentEvents.UnhandledException, "Failed to send message {0} to {1} after {2} attempts", messageGuid, address, retry--);
				return false;
			}
			
			TransportServiceClient service = _TransportServices[transportName];

			try
			{
				bool ok = service.RequestNotification(messageGuid, address, message);
				return ok;
			}
			catch (System.TimeoutException timeoutEx)
			{
				_Tracer.TraceEvent(TraceEventType.Error, (int)AgentEvents.UnhandledException, "Retrying send message {0} to {1}, attempt {2} after exception: {3}", messageGuid, address, retry + 1, timeoutEx.ToString());
				_ConsecutiveErrors++;
				System.Threading.Thread.Sleep(30000);
				GetTransports();
				return AdvancedRequestNotification(transportName, messageGuid, address, message, retry++);
			}

		}
	}
}
