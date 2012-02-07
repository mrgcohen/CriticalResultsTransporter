using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Diagnostics;
using System.Xml;
using System.ServiceModel.Channels;
using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography;

namespace CriticalResults
{
	/// <summary>
	/// This is the primary service for Critical Results.
	/// 
	/// Note(s) on RESTFul URI's:
	/// By default, IIS assumes PUT and DELETE are related to FS actions, so write/delete fs permissions are required.  For this reason, RESTFul deletes are actually implemented as POST's with "delete" in the URI.
	/// </summary>
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class ResultsService : IResultsService
	{
		private const string EMAIL_TRANSPORT = "SMTP Transport";        
        private SystemEmailer SystemMailer;

		private TraceSource _Trace;
		private TraceSource _AuditTrace;

        private enum ResultEvents
		{
			GetResult,
			AuditEvent,
			LogEvent,
		}

		public class SecurityException : Exception
		{
			public SecurityException() { }
		}

		public ResultsService()
        {
			string traceName = this.GetType().FullName;
			Trace.WriteLine(string.Format("Registering Trace Source {0}.", traceName));
			_Trace = new TraceSource(traceName);

			traceName = traceName + ".Audit";
			Trace.WriteLine(string.Format("Registering Trace Source {0}.", traceName));
			_AuditTrace = new TraceSource(traceName);
		}

		private static TimeSpan TOKEN_LIFESPAN = new TimeSpan(0, 60, 0);
        
		#region IResultsService Members
		[WebInvoke(Method="GET", UriTemplate="Echo/{message}", ResponseFormat=WebMessageFormat.Xml)]
		public string Echo(string message)
		{
			_Trace.TraceInformation("Echo:" + message);
			AuditEvent("Echo", message);

			return message;
		}
		
		[WebInvoke(Method = "PUT", UriTemplate = "DoubleEcho/", BodyStyle=WebMessageBodyStyle.Wrapped)]
		public string DoubleEcho(string message, string secondMessage)
		{
			return message + ":" + secondMessage;
		}
		
		[WebInvoke(Method="POST", UriTemplate="Echo/")]
		public string EchoPost(string message)
		{
			return message;
		}

		[WebInvoke(Method="GET", UriTemplate="result/{uuid}")]
        [HTTPBasicCheck]
        public Result GetResult(string uuid)
		{
			_Trace.TraceEvent(TraceEventType.Verbose, (int)ResultEvents.GetResult, ResultEvents.GetResult.ToString());

			ResultEntity e = new CriticalResultsEntityManager().GetResultEntity(new Guid(uuid));
			Result r = new Result(e);
			bool ok;
			ok = r.ResolveSender();
			ok = r.ResolveReceiver();
			ok = r.ResolveLevel();
			ok = r.Level.ResolveTransports();
			ok = r.ResolveContext();
			return r;
		}
		[WebInvoke(Method = "GET", UriTemplate = "result/{uuid}/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck]
		public Result GetResult_Json(string uuid)
		{
			return GetResult(uuid);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <remarks>
		/// </remarks>
		[WebInvoke(Method = "POST", UriTemplate = "audit/json", BodyStyle=WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public void AuditEvent(string eventName, string eventDescription)
		{
			HTTPBasicAuthenticationHeader header = HTTPBasicAuthenticationHeader.GetFromWCF();
			string userName = "";
			if(header != null) //TODO: Get username of mobile user!
				userName = header.UserName;

            string message = string.Format(", {0}, \"{1}\", \"{2}\", \"{3}\", \"{4}\"", DateTime.Now, WcfHelper.GetIPv4FromWCF(), userName, eventName, eventDescription);
			_Trace.TraceEvent(TraceEventType.Verbose, (int)ResultEvents.AuditEvent, ResultEvents.AuditEvent.ToString());
			_AuditTrace.TraceInformation(message);
		}

		[WebInvoke(Method = "POST", UriTemplate = "audit/json", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public void LogEvent(int eventCode, int logLevel, string message)
		{
			TraceEventType eventType = TraceEventType.Information;

			switch (logLevel)
			{
				case 1:
					eventType = TraceEventType.Verbose;
					break;
				case 2:
					eventType = TraceEventType.Information;
					break;
				case 3:
					eventType = TraceEventType.Warning;
					break;
				case 4:
					eventType = TraceEventType.Error;
					break;
				default:
					eventType = TraceEventType.Information;
					break;
			}

			message = string.Format("{0} {1}", message, "user");

			_Trace.TraceEvent(eventType, eventCode, message);
		}

		[WebInvoke(Method="POST", UriTemplate="result/query", BodyStyle=WebMessageBodyStyle.WrappedRequest)]
        [HTTPBasicCheck(RefreshToken=false)]
		public Result[] QueryResults(string queryString, int? pageSize, int? pageNumber)
		{
			_Trace.TraceEvent(TraceEventType.Verbose, -1, "QueryResults");
			
			ResultEntity[] entities = new CriticalResultsEntityManager().QueryResultEntity(queryString, pageSize, pageNumber);
			List<Result> results = new List<Result>();
			foreach (ResultEntity e in entities)
			{
				Result r = new Result(e);
				results.Add(r);
			}
			return results.ToArray();
		}

		[WebInvoke(Method = "POST", UriTemplate = "result/query/json", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false)]
		public Result[] QueryResults_Json(string queryString, int? pageSize, int? pageNumber)
		{
			ResultEntity[] entities = new CriticalResultsEntityManager().QueryResultEntity(queryString, pageSize, pageNumber);
			List<Result> results = new List<Result>();
			foreach (ResultEntity e in entities)
			{
				Result r = new Result(e);
				results.Add(r);
			}
			return results.ToArray();
		}

		[WebInvoke(Method = "POST", UriTemplate = "resultlist/", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [HTTPBasicCheck(RefreshToken = false)]
		public ResultViewEntry [] QueryResultList(string queryString, int? pageSize, int? pageNumber)
		{
			_Trace.TraceEvent(TraceEventType.Verbose, -1, "QueryResultList");
			
			ResultListEntity [] entities = new CriticalResultsEntityManager().QueryResultList(queryString, pageSize, pageNumber);
			List<ResultViewEntry> results = new List<ResultViewEntry>();
			foreach (ResultListEntity entity in entities)
			{
				ResultViewEntry result = new ResultViewEntry(entity);
				results.Add(result);
			}
			return results.ToArray();
		}
		[WebInvoke(Method = "POST", UriTemplate = "resultlist/json", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false)]
        public ResultViewEntry[] QueryResultList_Json(string queryString, int? pageSize, int? pageNumber)
		{
			return QueryResultList(queryString, pageSize, pageNumber);
		}

		[WebInvoke(Method = "POST", UriTemplate = "result/")]
        [HTTPBasicCheck(RefreshToken = false)]
        public Result CreateResult(Result result)
		{
			_Trace.TraceEvent(TraceEventType.Information, -1, "CreateResult");
			
			bool ok = result.CreateEntity();

			if (ok)
				AuditEvent("CreateResult:Success", result.Uuid.ToString());
			else
				AuditEvent("CreateResult:Failure", "");

			return result;
		}
		[WebInvoke(Method = "POST", UriTemplate = "result/json", RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false)]
        public Result CreateResult_Json(Result result)
		{
			return CreateResult(result);
		}

		/// <summary>
		/// TODO: URI template should be rewritten to result/{uuid}/receiver/{userName} 
		/// TODO: why is everything being passed back with the result?  it seems unnecessary.
		/// Created:
		/// Modified:
		/// </summary>
		/// <param name="uuid"></param>
		/// <param name="userName"></param>
		/// <returns></returns>
		[WebInvoke(Method = "POST", UriTemplate = "result/{uuid}/updatereceiver/{userName}", ResponseFormat = WebMessageFormat.Xml, RequestFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(RefreshToken = false)]
        public Result UpdateResultReceiver(string uuid, string userName)
		{
			_Trace.TraceEvent(TraceEventType.Information, -1, "UpdateResultReceiver");
						
			Guid Uuid = new Guid(uuid);
			ResultEntity result = new CriticalResultsEntityManager().UpdateResultReceiver(Uuid, userName);
			Result r = new Result(result);
			r.ResolveAcknowledgment();
			r.ResolveContext();
			r.ResolveEntity();
			r.ResolveNotifications();
			r.ResolveRatings();
			r.ResolveReceiver();
			r.ResolveSender();
			r.ResolveSenderProxy();
			r.ResolveTags();
			r.ResolveLevel();
			return r;
		}

		[WebInvoke(Method = "POST", UriTemplate = "result/{uuid}/updatereceiver/{userName}/json", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false)]
        public Result UpdateResultReceiver_Json(string uuid, string userName)
		{
			return UpdateResultReceiver(uuid, userName);
		}


		[WebInvoke(Method = "GET", UriTemplate = "level/", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck]
        public Level[] GetAllLevels()
		{
			return GetLevels();
		}

        [WebInvoke(Method = "GET", UriTemplate = "level/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck]
        public Level[] GetAllLevels_Json()
        {
            return GetLevels();
        }

		public Level[] GetLevels()
		{
			_Trace.TraceEvent(TraceEventType.Verbose, -1, "GetAllLevels");

			LevelEntity[] entities = new CriticalResultsEntityManager().GetLevels();
			List<Level> levels = new List<Level>();
			foreach (LevelEntity entity in entities)
			{
				Level level = new Level(entity);
				level.ResolveTransports();
				levels.Add(level);
			}
			return levels.ToArray();

		}

        /// <summary>
        /// Modified: 2009-10-01: John Morgan
        ///     Altered to add selected transports
        /// Modified: 2010-06-28: John Morgan
        ///     Altered to add usertransportlevels on new level creation
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "level/create/")]
        [HTTPBasicCheck(RefreshToken = false, Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public Level CreateLevel(Level level)
        {
			_Trace.TraceEvent(TraceEventType.Verbose, -1, "CreateLevel");
						
            CriticalResultsEntityManager manager = new CriticalResultsEntityManager();
            LevelEntity entity = manager.CreateLevel(level.Name, level.Description, level.ShortDescription, level.ColorValue, level.EscalationTimeout, level.DueTimeout, level.DirectContactRequired);
            if (level.Transports != null)
            {
                foreach(Transport t in level.Transports){
                    bool mandatory = false;
                    foreach(Transport mT in level.MandatoryTransports)
                    {
                        if(mT.Name == t.Name)
                        {
                            mandatory = true;
                        }
                    }
                    manager.CreateTransportLevelEntity(entity.Uuid, t.Name, mandatory);
                }
                UserEntity[] ues = manager.GetAllUsers();
                entity.TransportLevels.Load();
                foreach (UserEntity ue in ues)
                {
                    ue.UserTransports.Load();
                   
                    foreach (UserTransportEntity te in ue.UserTransports)
                    {
                        
                        foreach (TransportLevelEntity t in entity.TransportLevels)
                        {
                            if (te.Transport != null)
                            {
                                if (t.Transport.Name == te.Transport.Name)
                                {
                                    te.Levels.Add(entity);
                                }
                            }
                        }
                    }
                }
                manager.SaveChanges();
            }
            return new Level(entity);
        }
        
        [WebInvoke(Method = "POST", UriTemplate = "level/create/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false, Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public Level CreateLevel_Json(Level level)
        {
            return CreateLevel(level);
        }

        /// <summary>
        ///Modified: 2009-09-31 - John Morgan
        /// Altered to accept and update level transports
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "level/update")]
        [HTTPBasicCheck(RefreshToken = false, Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public Level UpdateLevel(Level level)
        {
			_Trace.TraceEvent(TraceEventType.Verbose, -1, "UpdateLevel");
						
			CriticalResultsEntityManager manager = new CriticalResultsEntityManager();
            LevelEntity entity = manager.UpdateLevel(level.Uuid, level.Name, level.Description, level.ShortDescription, level.ColorValue, level.EscalationTimeout, level.DueTimeout, level.DirectContactRequired);
            if (level.Transports != null)
            {
                List<TransportLevelEntity> transportsToRemove = new List<TransportLevelEntity>();
                transportsToRemove.AddRange(entity.TransportLevels);
                foreach(TransportLevelEntity tle in transportsToRemove)
                {
                    if (!tle.TransportReference.IsLoaded)
                    {
                        tle.TransportReference.Load();
                    }
                    manager.RemoveTransportFromLevel(level.Uuid, tle.Transport.Name);
                }

                foreach (Transport t in level.Transports)
                {
                    bool mandatory = false;
                    foreach (Transport mT in level.MandatoryTransports)
                    {
                        if (t.Name == mT.Name)
                        {
                            mandatory = true;
                        }
                    }
                    manager.CreateTransportLevelEntity(level.Uuid, t.Name, mandatory);
                }

            }
            return new Level(entity);
        }
        [WebInvoke(Method = "POST", UriTemplate = "level/update/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false)]
        public Level UpdateLevel_Json(Level level)
        {
            return UpdateLevel(level);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="levelUuid"></param>
		/// <param name="transportName"></param>
		/// <returns></returns>
		/// <remarks>
		///	Future: This should be a "PUT"
		/// Change: 2009-05-31, Jeremy R
		///		Changed UriTemplate.  This is supposed to be a RESTFul service...
		/// </remarks>
		[WebInvoke(Method = "POST", UriTemplate = "level/{levelUuid}/transport/{transportName}/{mandatoryString}")]
        [HTTPBasicCheck(RefreshToken = false, Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public bool CreateTransportLevel(string levelUuid, string transportName, string mandatoryString)
        {
			_Trace.TraceEvent(TraceEventType.Verbose, -1, "CreateTransportLevel");
						
			Guid levelGuid = new Guid(levelUuid);
			
			bool mandatory = false;
			bool ok = bool.TryParse(mandatoryString, out mandatory);

            return new CriticalResultsEntityManager().CreateTransportLevelEntity(levelGuid, transportName, mandatory);
        }

		[WebInvoke(Method = "POST", UriTemplate = "level/{levelUuid}/transport/{transportName}/{mandatoryString}/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false, Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public bool CreateTransportLevel_Json(string levelUuid, string transportName, string mandatoryString)
        {
            return CreateTransportLevel(levelUuid, transportName, mandatoryString);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="levelUuid"></param>
		/// <param name="transportName"></param>
		/// <returns></returns>
		/// <remarks>
		/// Created: 2009-05-31, Jeremy R - Add ability to modify Transport Level
		/// </remarks>
		[WebInvoke(Method = "POST", UriTemplate = "level/{levelUuid}/transport/{mandatoryString}/{transportName}")]
        [HTTPBasicCheck(RefreshToken = false, Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public bool ModifyTransportLevel(string levelUuid, string transportName, string mandatoryString)
		{
			_Trace.TraceEvent(TraceEventType.Verbose, -1, "ModifyTransportLevel");
						
			Guid levelGuid = new Guid(levelUuid);

			bool mandatory = false;
			bool ok = bool.TryParse(mandatoryString, out mandatory);

			return new CriticalResultsEntityManager().CreateTransportLevelEntity(levelGuid, transportName, false);
		}

		[WebInvoke(Method = "POST", UriTemplate = "level/{levelUuid}/transport/{transportName}/{mandatoryString}/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false, Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public bool ModifyTransportLevel_Json(string levelUuid, string transportName, string mandatoryString)
		{
			return ModifyTransportLevel(levelUuid, transportName, mandatoryString);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="levelUuid"></param>
		/// <param name="transportName"></param>
		/// <returns></returns>
		/// <remarks>
		/// Future: 
		/// --this is not RESTFul. 
		/// --Method should be "DELETE"
		/// --"removetransport" should not be in the URI
		/// </remarks>
        [WebInvoke(Method = "POST", UriTemplate = "level/removetransport/{levelUuid}")]
        [HTTPBasicCheck(RefreshToken = false, Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public bool RemoveTransportFromLevel(string levelUuid, string transportName)
        {
			_Trace.TraceEvent(TraceEventType.Verbose, -1, "RemoveTransportFromLevel");
			
            Guid level = new Guid(levelUuid);
            return new CriticalResultsEntityManager().RemoveTransportFromLevel(level, transportName);
        }
        
        [WebInvoke(Method = "POST", UriTemplate = "level/removetransport/{levelUuid}/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false, Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public bool RemoveTransportFromLevel_Json(string levelUuid, string transportName)
        {
            return RemoveTransportFromLevel(levelUuid, transportName);
        }

		[WebInvoke(Method = "GET", UriTemplate = "user/", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck]
        public User[] GetAllUsers()
		{
			_Trace.TraceEvent(TraceEventType.Verbose, -1, "GetAllUsers");
			
			UserEntity[] entities = new CriticalResultsEntityManager().GetAllUsers();
			List<User> users = new List<User>();
			foreach (UserEntity e in entities)
			{
				User u = new User(e);
				u.ResolveRoles();
				users.Add(u);
			}
			return users.ToArray();
		}
		
        [WebInvoke(Method = "GET", UriTemplate = "user/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck]
        public User[] GetAllUsers_Json()
		{
			return GetAllUsers();
		}

		[WebInvoke(Method = "GET", UriTemplate = "user/{userName}", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck]
		public User GetUser(string userName) 
		{
			_Trace.TraceEvent(TraceEventType.Verbose, -1, "GetUser");

			UserEntity e = new CriticalResultsEntityManager().GetUser(userName);
			User u = new User(e);
			u.ResolveRoles();
			u.ResolveTransports();
			u.ResolveProxied();
			u.ResolveProxies();
			u.ResolveEntries();
			foreach (UserTransport t in u.Transports)
			{
				t.ResolveLevels();
			}
			return u;
		}

		public User GetUser_NonWeb(string userName)
		{
			_Trace.TraceEvent(TraceEventType.Verbose, -1, "GetUser_NonWeb");

			UserEntity e = new CriticalResultsEntityManager().GetUser(userName);
			if (e == null)
				return null;
			User u = new User(e);
			u.ResolveRoles();
			u.ResolveTransports();
			u.ResolveProxied();
			u.ResolveProxies();
			u.ResolveEntries();
			foreach (UserTransport t in u.Transports)
			{
				t.ResolveLevels();
			}
			return u;
		}

		/// <summary>
		/// Modified: 2009-10-01, John Morgan
		/// Changed to include Roles
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		[WebInvoke(Method = "GET", UriTemplate = "user/{userName}/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck]
        public User GetUser_Json(string userName) { return GetUser(userName); }

		[WebInvoke(Method = "POST", UriTemplate = "user/", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(RefreshToken=false)]
        public User[] QueryUsers(string queryString, int? pageSize, int? pageNumber) 
		{
			_Trace.TraceEvent(TraceEventType.Verbose, -1, "QueryUsers");
						
			UserEntity[] entities = new CriticalResultsEntityManager().QueryUserEntity(queryString, pageSize, pageNumber);
			List<User> users = new List<User>();
			foreach (UserEntity e in entities)
			{
				if (!e.Roles.IsLoaded)
					e.Roles.Load();
				User u = new User(e);
				u.ResolveRoles();
				users.Add(u);
			}
			return users.ToArray();
		}
		
		//{"queryString":"1==1","pageSize":null}
		[WebInvoke(Method = "POST", UriTemplate = "user/json", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false)]
        public User[] QueryUsers_Json(string queryString, int? pageSize, int? pageNumber) 
		{ 
			return QueryUsers(queryString, null, null); 
		}

		[WebInvoke(Method = "GET", UriTemplate = "contextType/", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck]
        public ContextType[] GetAllContextTypes()
		{
			_Trace.TraceEvent(TraceEventType.Verbose, -1, "GetAllContextTypes");
			
			ContextTypeEntity[] entities = new CriticalResultsEntityManager().GetAllContextTypes();
			List<ContextType> types = new List<ContextType>();
			foreach (ContextTypeEntity e in entities)
			{
				ContextType t = new ContextType(e);
				types.Add(t);
			}
			return types.ToArray();
		}

		//{"queryString":"1==1","pageSize":null}
		[WebInvoke(Method = "GET", UriTemplate = "contextType/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck]
        public ContextType[] GetAllContextTypes_Json()
		{
			return GetAllContextTypes();
		}


		[WebInvoke(Method = "GET", UriTemplate = "result/{resultUuid}/Acknowledgment", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck]
        public Acknowledgment GetResultAcknowledgment(string resultUuid)
		{
			_Trace.TraceEvent(TraceEventType.Verbose, -1, "GetResultAcknowledgment");
			
			Guid guid = new Guid(resultUuid);
			CriticalResultsEntityManager manager = new CriticalResultsEntityManager();
			AcknowledgmentEntity e = manager.GetResultAcknowledgment(guid);
			if (e == null)
				return new Acknowledgment();
			Acknowledgment ack = new Acknowledgment(e);
			ack.ResolveUser();
			ack.ResolveResult();
			return ack;
		}
		
        [WebInvoke(Method = "GET", UriTemplate = "result/{resultUuid}/Acknowledgment/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck]
        public Acknowledgment GetResultAcknowledgment_Json(string resultUuid)
		{
			return GetResultAcknowledgment(resultUuid);
		}

		[WebInvoke(Method = "GET", UriTemplate = "result/{resultUuid}/notifications", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck]
        public Notification[] GetResultNotifications(string resultUuid)
		{
			_Trace.TraceEvent(TraceEventType.Verbose, -1, "GetResultNotifications");
					
			Guid guid = new Guid(resultUuid);
			NotificationEntity[] entities = new CriticalResultsEntityManager().GetResultNotifications(guid);
			List<Notification> notifications = new List<Notification>();
			foreach (NotificationEntity entity in entities)
			{
				Notification n = new Notification(entity);
				n.ResolveUserTransport();
				n.UserTransport.ResolveTransport();
				notifications.Add(n);
			}
			return notifications.ToArray();
		}
		[WebInvoke(Method = "GET", UriTemplate = "result/{resultUuid}/notifications/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public Notification[] GetResultNotifications_Json(string resultUuid)
		{
			return GetResultNotifications(resultUuid);
		}

		[WebInvoke(Method = "POST", UriTemplate = "result/notitications/{selectedTransport}", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(RefreshToken=false)]
        public Notification CreateResultNotification(Notification notification, string selectedTransport)
        {
			_Trace.TraceEvent(TraceEventType.Verbose, -1, "CreateResultNotification");
					
			CriticalResultsEntityManager crm = new CriticalResultsEntityManager();
			NotificationEntity e = new NotificationEntity();
			e.Notes = notification.Notes;
			e.CreationTime = DateTime.Now;
			e.State = notification.State;
			e.Result = crm.GetResultEntity(notification.Result.Uuid);
			e.UserTransport = crm.GetUserTransport(notification.Result.Receiver.UserName, selectedTransport);
			NotificationEntity ne = crm.CreateResultNotification(e);
			return new Notification(ne);
		}

		[WebInvoke(Method = "POST", UriTemplate = "result/notification/{selectedTransport}/json", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false)]
        public Notification CreateResultNotification_Json(Notification notification, string selectedTransport)
		{
			return CreateResultNotification(notification, selectedTransport);
		}
		
		/// <summary>
		/// Created: 2009-07-18, Jeremy Richardson
		///		Created to support NotifyAgent functionality
		/// </summary>
		/// <param name="queryString"></param>
		/// <param name="pageSize"></param>
		/// <param name="pageNumber"></param>
		/// <returns></returns>
		[WebInvoke(Method = "POST", UriTemplate = "notitication", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(RefreshToken = false)]
        public Notification[] QueryNotifications(string queryString, int? pageSize, int? pageNumber)
		{
			NotificationEntity[] entities = new CriticalResultsEntityManager().QueryNotificationEntity(queryString, pageSize, pageNumber);
			List<Notification> results = new List<Notification>();
			foreach (NotificationEntity e in entities)
			{
				Notification n = new Notification(e);				
			}
			return results.ToArray();
		}

		/// <summary>
		/// Gets pending notifications.
		/// 
		/// Created: 2009-07-18, Jeremy Richardson
		///		Created to support NotifyAgent functionality
		/// </summary>
		/// <returns></returns>
		//[WebInvoke(Method = "POST", UriTemplate = "notification/state/new", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck]
        public Notification[] GetPendingNotifications()
		{
			_Trace.TraceEvent(TraceEventType.Verbose, -1, "GetPendingNotifications");
						
			string queryString = string.Format("it.State = '{0}'", Notification.NotificationState.New);
			Notification [] notifications = QueryNotifications(queryString, null, null);
			foreach (Notification n in notifications)
			{
				n.ResolveResult();			
				n.Result.ResolveSender();
				n.ResolveUserTransport();
				n.UserTransport.ResolveTransport();
				n.UserTransport.ResolveUser();
			}
			return notifications;
		}
		
		/// <summary>
		/// Modified: 2010-01-01, JM, fixed datetime bugs in query.  Moved call to entity manager to this method instead of call QueryNotifications to avoid CheckHTTPBasicAuth() call.
		/// </summary>
		/// <param name="notificationStateName"></param>
		/// <param name="startDateRangeString"></param>
		/// <param name="endDateRangeString"></param>
		/// <returns></returns>
		[WebInvoke(Method = "POST", UriTemplate = "notification/state/{notificationStateName}", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public Notification[] GetNotifications(string notificationStateName, string startDateRangeString, string endDateRangeString)
		{
			//CheckHTTPBasicAuth();
			_Trace.TraceEvent(TraceEventType.Verbose, -1, "GetNotifications");

			string queryString = string.Format("it.State = '{0}'", notificationStateName);

			if (!string.IsNullOrEmpty(startDateRangeString))
			{
				DateTime startDateRange = DateTime.Parse(startDateRangeString);
				queryString += string.Format(" AND it.CreationTime > DATETIME'{0}'", startDateRange.ToString("yyyy-MM-dd hh:mm"));
			}
			if (!string.IsNullOrEmpty(endDateRangeString))
			{
				DateTime endDateRange = DateTime.Parse(endDateRangeString);
				queryString += string.Format(" AND it.CreationTime < DATETIME'{0}'", endDateRange.ToString("yyyy-MM-dd hh:mm"));
			}

			NotificationEntity[] entities = new CriticalResultsEntityManager().QueryNotificationEntity(queryString, null, null);
			List<Notification> results = new List<Notification>();

			foreach (NotificationEntity entity in entities)
			{
				Notification n = new Notification(entity);
				n.ResolveResult();
				n.Result.ResolveSender();
				n.ResolveUserTransport();
				n.UserTransport.ResolveTransport();
				n.UserTransport.ResolveUser();
				results.Add(n);
			}
			return results.ToArray() ;
		}

        //[WebInvoke(Method = "GET", UriTemplate = "notification/all/xml", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        //public Notification[] GetLastDaysNotifications()
        //{
        //    TimeSpan day = new TimeSpan(1, 0, 0, 0);
        //    string queryString = string.Format("it.CreationTime > DATETIME'{0}'", (DateTime.Now - day).ToString("yyyy-MM-dd HH:mm:ss"));
        //    NotificationEntity[] entities = new CriticalResultsEntityManager().QueryNotificationEntity(queryString, null, null);
        //    List<Notification> notifications = new List<Notification>();
        //    foreach (NotificationEntity e in entities)
        //    {
        //        Notification n = new Notification(e);
        //        n.ResolveResult();
        //        n.Result.ResolveSender();
        //        n.ResolveUserTransport();
        //        n.UserTransport.ResolveTransport();
        //        n.UserTransport.ResolveUser();
        //        notifications.Add(n);
        //    }
        //    Notification [] notificationArray = notifications.ToArray();
        //    return notificationArray;
        //}

		/// <summary>
		/// Created: 2009-07-18, Jeremy Richardson
		///		Created to support NotifyAgent functionality
		/// </summary>
		/// <param name="notificationUuid"></param>
		/// <param name="state"></param>
		/// <param name="notes"></param>
		/// <returns></returns>
		[WebInvoke(Method = "POST", UriTemplate = "notification/{notificationUuid}", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(RefreshToken=false)]
        public Notification UpdateNotification(string notificationUuid, string state, string notes)
		{
			_Trace.TraceEvent(TraceEventType.Verbose, -1, "UpdateNotification");
						
			NotificationEntity e = new CriticalResultsEntityManager().UpdateNotificationEntity(new Guid(notificationUuid), state, notes);
			Notification n = new Notification(e);
			return n;
		}

		[WebInvoke(Method = "POST", UriTemplate = "result/{resultUuid}/acknowledgment/{userName}", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(RefreshToken = false)]
        public Acknowledgment CreateResultAcknowledgment(string resultUuid, string userName, string notes)
		{
			_Trace.TraceEvent(TraceEventType.Verbose, -1, "CreateResultAcknowledgment");
						
			AcknowledgmentEntity e = new CriticalResultsEntityManager().CreateAcknowledgment(new Guid(resultUuid), userName, notes);
			Acknowledgment ack = new Acknowledgment(e);
			ack.ResolveResult();
			ack.ResolveUser();

			AuditEvent("AcknowledgeResult:Success", string.Format("{0}; {1}", resultUuid, userName)); 

			return ack;
		}
        [WebInvoke(Method = "POST", UriTemplate = "result/{resultUuid}/acknowledgment/{userName}/json", BodyStyle=WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false)]
        public Acknowledgment CreateResultAcknowledgment_Json(string resultUuid, string userName, string notes)
		{
			return CreateResultAcknowledgment(resultUuid, userName, notes);
		}

        [WebInvoke(Method = "GET", UriTemplate = "transport/")]
        [HTTPBasicCheck]
        public Transport[] GetAllTransports()
        {
			return GetTransports();
        }

        [WebInvoke(Method = "GET", UriTemplate = "transport/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck]
        public Transport[] GetAllTransports_Json()
        {
            return GetTransports();
        }

        public Transport[] GetTransports()
		{
			TransportEntity[] entities = new CriticalResultsEntityManager().GetAllTransports();
            List<Transport> transports = new List<Transport>();
            foreach (TransportEntity entity in entities)
            {
				Transport transport = new Transport(entity);
				transport.ResolveLevels();
                transports.Add(transport);
            }
            return transports.ToArray();
		}


		/// <summary>
		/// Modified: 2009-07-28, John Morgan
		/// Call Added to add new transport to all users
		/// </summary>
		/// <param name="name"></param>
		/// <param name="transportUri"></param>
		/// <param name="friendlyName"></param>
		/// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "transport/{name}/create", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [HTTPBasicCheck(RefreshToken = false, Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
		public Transport CreateTransport(string name, string transportUri, string friendlyName)
        {
			TransportEntity entity = new CriticalResultsEntityManager().CreateTransport(name, transportUri, friendlyName);
            Transport transport = new Transport(entity);
			AddUsersToTransport(transport.Name);
            return transport;
        }
        [WebInvoke(Method = "POST", UriTemplate = "transport/{name}/create/json", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false, Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public Transport CreateTransport_Json(string name, string transportUri, string friendlyName)
        {
            return CreateTransport(name, transportUri, friendlyName);
        }

		//User Management
		/// <summary>
		/// Modified: 2009-07-28, John Morgan
		///		- Call added to AddTransportsToUser to add all available transports to a new user.
		/// Modified: 2009-08-12, John Morgan
		///		- Added levels to newly created user transport
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		[WebInvoke(Method = "POST", UriTemplate = "user/create", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(RefreshToken = false)]
        public User CreateUser(User user)
		{
			return CreateANCRUser(user);
		}

        [WebInvoke(Method = "POST", UriTemplate = "user/create/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false)]
        public User CreateUser_Json(User user)
        {
            return CreateANCRUser(user);
        }

        public User CreateANCRUser(User user)
		{
			CriticalResultsEntityManager manager = new CriticalResultsEntityManager();
			UserEntity userEntity = manager.CreateUserEntity(user.ToEntity());

			AuditEvent("CreateUser:Success", user.UserName);

			LevelEntity[] levels = manager.GetLevels();
			List<string> levelNames = new List<string>();
			foreach (LevelEntity level in levels)
			{
				levelNames.Add(level.Name);
			}

			TransportEntity[] transports = manager.GetAllTransports();
			foreach (TransportEntity transport in transports)
			{
				bool addTransport = true;
				foreach (UserTransportEntity userTransport in userEntity.UserTransports)
				{
					if (userTransport.Transport.Name == transport.Name)
					{
						addTransport = false;
					}
				}
				if (addTransport)
				{
					if (user.Transports != null)
					{
						bool transportFound = false;
						for (int i = 0; i < user.Transports.Length; i++)
						{
							if (user.Transports[i].Transport.Name == transport.Name)
							{
								if (user.Transports[i].Address == null)
								{
									user.Transports[i].Address = "";
								}
								manager.CreateUserTransport(userEntity.UserName, transport.Name, user.Transports[i].Address);
								manager.UpdateLevelsToUserTransport(user.UserName, transport.Name, user.Transports[i].Address, levelNames.ToArray());
								transportFound = true;

							}
						}
						if (!transportFound)
						{
							manager.CreateUserTransport(userEntity.UserName, transport.Name, "");
							manager.UpdateLevelsToUserTransport(user.UserName, transport.Name, "", levelNames.ToArray());
						}
					}
					else
					{
						manager.CreateUserTransport(userEntity.UserName, transport.Name, "");
						manager.UpdateLevelsToUserTransport(user.UserName, transport.Name, "", levelNames.ToArray());
					}
				}
			}

			userEntity = manager.GetUser(user.UserName);

			//bool passwordFound = false;
			//foreach (UserEntryEntity uee in e.UserEntries)
			//{
			//    if (uee.Type == "AuthExt" && uee.Key == "ANCR")
			//        passwordFound = true;
			//}
			//if(!passwordFound)
			//    GeneratePassword(user.UserName);

			User u = new User(userEntity);
			u.ResolveRoles();
			u.ResolveTransports();
			u.ResolveEntries();
			return u;
		}

		/// <summary>
		/// Created: 2009-10-05, John Morgan
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		[WebInvoke(Method = "POST", UriTemplate = "user/{username}/confirmation", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(RefreshToken = false)]
        public bool SendAccountConfirmation(string username)
		{
			return AccountConfirmation(username);
		}

        [WebInvoke(Method = "POST", UriTemplate = "user/{username}/confirmation/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false)]
        public bool SendAccountConfirmation_Json(string username)
        {
            return AccountConfirmation(username);
        }

        public bool AccountConfirmation(string username)
		{
			CriticalResultsEntityManager manager = new CriticalResultsEntityManager();
			UserEntity ue = manager.GetUser(username);
			if (!ue.UserTransports.IsLoaded)
			{
				ue.UserTransports.Load();
			}
			foreach (UserTransportEntity ute in ue.UserTransports)
			{
				if (!ute.TransportReference.IsLoaded)
				{
					ute.TransportReference.Load();
				}
                if (ute.Transport.Name == EMAIL_TRANSPORT && !String.IsNullOrEmpty(ute.Address))
				{
					SystemMailer = new SystemEmailer();
					SettingEntity[] settings = new CriticalResultsEntityManager().GetSettings("System");
					string message = "";
					foreach (SettingEntity setting in settings)
					{
						if (setting.EntryKey == "AccountConfirmationMessage")
						{
							message = setting.Value;
						}
					}
					message = String.Format(message, ue.UserName);
					BackgroundWorker bgw = new BackgroundWorker();
					bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
					bgw.RunWorkerAsync(message + "|" + ute.Address);
				}
			}
			return true;
		}

		void bgw_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;

			string[] parameters = ((string)e.Argument).Split(new char[] { '|' });
			bool ret = SystemMailer.SendMessage(parameters[1], "Critical Results Account", parameters[0]);
		}

	
		/// <summary>
		/// Adds all available transports to a user with address to be filled out later.
		/// Created: 2009-07-28, John Morgan
		/// Modified: 2009-08-13, John Morgan
		/// Altered to only add transports that do not exist
		/// </summary>
		/// <param name="userName"></param>
		/// <returns>UserEntity</returns>
		private UserEntity AddTransportsToUser(string userName)
		{
			CriticalResultsEntityManager manager = new CriticalResultsEntityManager();
			TransportEntity[] transports = manager.GetAllTransports();
			UserEntity user = manager.GetUser(userName);
			foreach (TransportEntity transport in transports)
			{
				bool transportFound = false;
				foreach (UserTransportEntity t in user.UserTransports)
				{
					if (t.Transport.Name == transport.Name)
						transportFound = true;
				}
				if(!transportFound)
					manager.CreateUserTransport(userName, transport.Name, "");
			}
			return manager.GetUser(userName);
		}

		[WebInvoke(Method = "POST", UriTemplate = "user/update", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(RefreshToken = false, Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
		public User UpdateUser(User user)
		{
			UserEntity e = new CriticalResultsEntityManager().UpdateUserEntity(user.ToEntity());
			return new User(e);
		}

		[WebInvoke(Method = "POST", UriTemplate = "user/Update/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false, Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public User UpdateUser_Json(User user)
		{
			return UpdateUser(user);
		}

		/// <summary>
		/// Modified: 2009-08-14, john Morgan - RESTful uri names
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="roleNames"></param>
		/// <returns></returns>
		[WebInvoke(Method = "POST", UriTemplate = "user/{userName}/roles", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(RefreshToken = false, Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public bool UpdateUserRoles(string userName, string[] roleNames)
		{
			CriticalResultsEntityManager manager = new CriticalResultsEntityManager();
			manager.UpdateUserRoles(userName, roleNames);
			return true;
		}

		[WebInvoke(Method = "POST", UriTemplate = "user/{userName}/roles/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false)]
        public bool UpdateUserRoles_Json(string userName, string[] roleName)
		{
			return UpdateUserRoles(userName, roleName);
		}

		/// <summary>
		/// Created: 2009-08-14, John Morgan
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="roleName"></param>
		[WebInvoke(Method = "POST", UriTemplate = "user/{userName}/role", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(RefreshToken = false, Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public bool AddUserToRole(string userName, string roleName)
		{
			return AssignUserToRole(userName, roleName);
		}

		public bool AssignUserToRole(string userName, string roleName)
		{
			new CriticalResultsEntityManager().AddUserToRole(userName, roleName);
			return true;
		}

		[WebInvoke(Method = "POST", UriTemplate = "user/{userName}/role/json", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false, Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public bool AddUserToRole_Json(string userName, string roleName)
		{
			return AddUserToRole(userName, roleName);
		}

        [WebInvoke(Method = "POST", UriTemplate = "transport/update", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [HTTPBasicCheck(RefreshToken = false, Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public Transport UpdateTransport(string origName, string origUri, string newName, string newUri, string friendlyName)
        {
			TransportEntity entity = new CriticalResultsEntityManager().UpdateTransport(origName, origUri, newName, newUri, friendlyName);
            Transport transport = new Transport(entity);
            return transport;
        }
        
        [WebInvoke(Method = "POST", UriTemplate = "transport/update/json", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false, Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public Transport UpdateTransport_Json(string origName, string origUri, string newName, string newUri, string friendlyName)
        {
            return UpdateTransport(origName, origUri, newName, newUri, friendlyName);
        }

        [WebInvoke(Method = "POST", UriTemplate = "transport/{name}/delete")]
        [HTTPBasicCheck(RefreshToken = false, Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public bool DeleteTransport(string name, string transportUri)
        {
            return new CriticalResultsEntityManager().DeleteTransport(name, transportUri);
        }
        
        [WebInvoke(Method = "POST", UriTemplate = "transport/{name}/delete/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false, Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public bool DeleteTransport_Json(string name, string transportUri)
        {
            return DeleteTransport(name, transportUri);
        }        

		/// <summary>
		/// 
		/// </summary>
		/// <param name="levelUuid"></param>
		/// <returns></returns>
		/// <remarks>
		/// Change: 2009-05-31, Jeremy R.
		///		Changed to use CriticalResultsEntityManager.GetTransportsForLevel()
		/// </remarks>
        [WebInvoke(Method="GET", UriTemplate = "level/{levelUuid}/transport")]
        [HTTPBasicCheck]
        public Transport[] GetTransportsForLevel(string levelUuid)
        {
            Guid level = new Guid(levelUuid);
			TransportEntity [] entities = new CriticalResultsEntityManager().GetTransportsForLevel(level);
			List<Transport> transports = new List<Transport>();
			foreach (TransportEntity e in entities)
			{
				Transport t = new Transport(e);
				t.ResolveLevels();
				transports.Add(t);
			}
			return transports.ToArray();
        }

        [WebInvoke(Method = "GET", UriTemplate = "level/{levelUuid}/transport/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck]
        public Transport[] GetTransportsForLevel_Json(string levelUuid)
        {
            return GetTransportsForLevel(levelUuid);
        }
		
		/// <summary>
		/// TODO: The CreateUserEntry and CreateUserEntry_Json methods can be factored out and 
		/// replaced with CreateUpdateUserEntry or CreateUpdateUserEntry_Json
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="type"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="xmlValue"></param>
		/// <returns></returns>
		[WebInvoke(Method = "POST", UriTemplate = "user/{userName}/entry", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(RefreshToken=false)]
		public User CreateUserEntry(string userName, string type, string key, string value, string xmlValue)
		{
			return UserEntry(userName, type, key, value, xmlValue);
		}

		public User UserEntry(string userName, string type, string key, string value, string xmlValue)
		{
			UserEntryEntity uee = new CriticalResultsEntityManager().CreateUserEntryEntity(userName, type, key, value, xmlValue, false);
			User u = GetUser_NonWeb(userName);
			return u;
		}

		[WebInvoke(Method = "POST", UriTemplate = "user/{userName}/entry/json", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false)]
        public User CreateUserEntry_Json(string userName, string type, string key, string value, string xmlValue)
		{
			return CreateUserEntry(userName, type, key, value, xmlValue);
		}

		/// <summary>
		/// Created: 2009-10-07, John Morgan
		/// Added Method to Update or Create a UserEntry
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="type"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="xmlValue"></param>
		/// <returns></returns>
		[WebInvoke(Method = "POST", UriTemplate = "user/{userName}/userentry", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(RefreshToken = false)]
        public UserEntry CreateUpdateUserEntry(string userName, string type, string key, string value, string xmlValue)
		{
            HTTPBasicCheckAttribute.AdditionalCheck(userName, HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin);
            UserEntryEntity ue = new CriticalResultsEntityManager().CreateUpdateUserEntryEntity(userName, type, key, value, xmlValue, false);
			return new UserEntry(ue);
		}

		[WebInvoke(Method = "POST", UriTemplate = "user/{userName}/userentry/json", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false)]
        public UserEntry CreateUpdateUserEntry_Json(string userName, string type, string key, string value, string xmlValue)
		{
			return CreateUpdateUserEntry(userName, type, key, value, xmlValue);
		}


		[WebInvoke(Method = "POST", UriTemplate = "user/{userName}/entry/delete", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(RefreshToken = false)]
        public bool DeleteUserEntry(string userName, string type, string key)
		{
            HTTPBasicCheckAttribute.AdditionalCheck(userName, HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin);
            return new CriticalResultsEntityManager().DeleteUserEntryEntity(userName, type, key);
		}

		[WebInvoke(Method = "POST", UriTemplate = "user/{userName}/entry/delete/json", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false)]
        public bool DeleteUserEntry_Json(string userName, string type, string key)
		{
			return DeleteUserEntry(userName, type, key);
		}

		[WebInvoke(Method = "POST", UriTemplate = "user/entry", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [HTTPBasicCheck(RefreshToken = false)]
        public UserEntry[] QueryUserEntries(string queryString, int? pageSize, int? pageNumber)
		{
			UserEntryEntity[] entities = new CriticalResultsEntityManager().QueryUserEntryEntities(queryString, pageSize, pageNumber);
			List<UserEntry> entries = new List<UserEntry>();
			foreach (UserEntryEntity entity in entities)
			{
				UserEntry entry = new UserEntry(entity);
				entry.ResolveUser();
				entries.Add(entry);
			}
			return entries.ToArray();
		}

		[WebInvoke(Method = "POST", UriTemplate = "user/entry/json", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false)]
        public UserEntry[] QueryUserEntries_Json(string queryString, int? pageSize, int? pageNumber)
		{
			return QueryUserEntries(queryString, pageSize, pageNumber);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		/// <remarks>
		/// Change: 2009-05-31, Jeremy R.
		///		Changed Uri to be RESTFul
		/// </remarks>
		[WebInvoke(Method="GET", UriTemplate = "role", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck]
        public Role[] GetAllRoles()
		{
			RoleEntity[] roleEntities = new CriticalResultsEntityManager().GetAllRoles();
			List<Role> roles = new List<Role>();
			foreach (RoleEntity entity in roleEntities)
			{
				roles.Add(new Role(entity));
			}
			return roles.ToArray();
		}
		
        [WebInvoke(Method = "GET", UriTemplate = "role/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck]
        public Role[] GetAllRoles_Json()
		{
			return GetAllRoles();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		/// <remarks>
		/// Change: 2009-05-31, Jeremy R.
		///		Changed Uri to be RESTFul
		/// </remarks>
		[WebInvoke(Method = "GET", UriTemplate = "user/{userName}/transport", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck]
        public UserTransport[] GetUserTransports(string userName)
		{
            HTTPBasicCheckAttribute.AdditionalCheck(userName, HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin);
            UserTransportEntity[] tes = new CriticalResultsEntityManager().GetUserTransports(userName);
			List<UserTransport> transports = new List<UserTransport>();
			foreach (UserTransportEntity te in tes)
			{
				UserTransport transport = new UserTransport(te);
				transport.ResolveLevels();
				transport.ResolveTransport();
				transports.Add(transport);
			}
			return transports.ToArray();
		}
		
        [WebInvoke(Method = "GET", UriTemplate = "user/{userName}/transport/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck]
        public UserTransport[] GetUserTransports_Json(string userName)
		{
			return GetUserTransports(userName);
		}

		[WebInvoke(Method = "POST", UriTemplate = "user/createTransport", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(RefreshToken=false)]
        public UserTransport CreateUserTransport(string userName, string transportName, string address)
		{
            HTTPBasicCheckAttribute.AdditionalCheck(userName, HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin);
            UserTransportEntity ute = new CriticalResultsEntityManager().CreateUserTransport(userName, transportName, address);
			return new UserTransport(ute);
		}

        [WebInvoke(Method = "POST", UriTemplate = "user/createTransport/json", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false)]
        public UserTransport CreateUserTransport_Json(string userName, string transportName, string address)
		{
			return CreateUserTransport(userName, transportName, address);
		}

		/// <summary>
		/// Add a transport to all users
		/// Created: 2009-07-27 John Morgan
		/// </summary>
		/// <param name="transportName"></param>
		/// <returns></returns>
		private bool AddUsersToTransport(string transportName)
		{
			CriticalResultsEntityManager manager = new CriticalResultsEntityManager();
			UserEntity[] users = manager.GetAllUsers();
			foreach (UserEntity user in users)
			{
				manager.CreateUserTransport(user.UserName, transportName, "");
			}
			return true;
		}

		[WebInvoke(Method = "POST", UriTemplate = "user/addLevelsToTransport", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(RefreshToken = false)]
        public UserTransport AddLevelsToUserTransport(string userName, string transportName, string address, string[] levelNames)
		{
            HTTPBasicCheckAttribute.AdditionalCheck(userName, HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin);
            return new UserTransport(new CriticalResultsEntityManager().UpdateLevelsToUserTransport(userName, transportName, address, levelNames));		
		}

        [WebInvoke(Method = "POST", UriTemplate = "user/addLevelsToTransport/json", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false)]
        public UserTransport AddLevelsToUserTransport_Json(string userName, string transportName, string address, string[] levelNames)
		{
            HTTPBasicCheckAttribute.AdditionalCheck(userName, HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin);
			return AddLevelsToUserTransport(userName, transportName, address, levelNames);
		}

		[WebInvoke(Method = "POST", UriTemplate = "user/updateTransport", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(RefreshToken = false)]
        public UserTransport UpdateUserTransport(string userName, string transportName, string originalAddress, string address)
		{
            HTTPBasicCheckAttribute.AdditionalCheck(userName, HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin); 
            UserTransportEntity ute = new CriticalResultsEntityManager().UpdateUserTransport(userName, transportName, originalAddress, address);
			UserTransport ut = new UserTransport(ute);
			ut.ResolveTransport();
			ut.ResolveLevels();
			return ut;
		}
		
        [WebInvoke(Method = "POST", UriTemplate = "user/updateTransport/json", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false)]
        public UserTransport UpdateUserTransport_Json(string userName, string transportName, string originalAddress, string address)
		{
			return UpdateUserTransport(userName, transportName, originalAddress, address);
		}

		[WebInvoke(Method="POST", UriTemplate="user/{masterUserName}/proxy/{proxyUserName}/{relationshipDescription}", RequestFormat= WebMessageFormat.Xml, ResponseFormat=WebMessageFormat.Xml)]
        [HTTPBasicCheck(RefreshToken = false)]
        public ProxyEntity CreateProxy(string masterUserName, string proxyUserName, string relationshipDescription)
		{
			return new CriticalResultsEntityManager().CreateProxy(masterUserName, proxyUserName, relationshipDescription);
		}
		
        [WebInvoke(Method = "POST", UriTemplate = "user/{masterUserName}/proxy/{proxyUserName}/{relationshipDescription}/json", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(RefreshToken = false)]
        public ProxyEntity CreateProxy_Json(string masterUserName, string proxyUserName, string relationshipDescription)
		{
			return CreateProxy(masterUserName, proxyUserName, relationshipDescription);
		}

		[WebInvoke(Method = "DELETE", UriTemplate = "user/{masterUserName}/proxy/{proxyUserName}", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(RefreshToken = false)]
        public void DeleteProxy(string masterUserName, string proxyUserName)
		{
			new CriticalResultsEntityManager().DeleteProxy(masterUserName, proxyUserName);
		}

        [WebInvoke(Method = "DELETE", UriTemplate = "user/{masterUserName}/proxy/{proxyUserName}/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false)]
        public void DeleteProxy_Json(string masterUserName, string proxyUserName)
		{
			DeleteProxy(masterUserName, proxyUserName);
		}

		[WebInvoke(Method = "GET", UriTemplate = "user/{proxyUserName}/proxy", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck]
        public User[] GetUsersForProxy(string proxyUserName)
		{
			UserEntity[] entities = new CriticalResultsEntityManager().GetUsersForProxy(proxyUserName);
			List<User> users = new List<User>();
			foreach (UserEntity e in entities)
			{
				User u = new User(e);
				users.Add(u);
			}
			return users.ToArray();
		}
		
        [WebInvoke(Method = "GET", UriTemplate = "user/{proxyUserName}/proxy/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck]
        public User[] GetUsersForProxy_Json(string proxyUserName)
		{
			return GetUsersForProxy(proxyUserName);
		}

        [WebInvoke(Method = "GET", UriTemplate = "settings", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck]
        public Setting[] GetAllSettings()
        {
            SettingEntity[] entities = new CriticalResultsEntityManager().GetAllSettings();
            List<Setting> settings = new List<Setting>(); 
            foreach (SettingEntity e in entities)
            {
                settings.Add(new Setting(e));
            }
            return settings.ToArray();
        }

        [WebInvoke(Method = "GET", UriTemplate = "settings/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck]
        public Setting[] GetAllSettings_Json()
        {
            return GetAllSettings();
        }
        
        /// <summary>
		/// Created: 2009-07-29, John Morgan
		/// Modified:
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="entryKey"></param>
		/// <returns></returns>
        [WebInvoke(Method = "GET", UriTemplate = "settings/{owner}", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck]
        public Setting[] GetSettings(string owner)
		{
			return GetSettings_NonWeb(owner);
		}

		public Setting[] GetSettings_NonWeb(string owner)
		{
			SettingEntity[] entities = new CriticalResultsEntityManager().GetSettings(owner);
			List<Setting> s = new List<Setting>();
			foreach (SettingEntity e in entities)
			{
				s.Add(new Setting(e));
			}
			return s.ToArray();
		}

		[WebInvoke(Method = "GET", UriTemplate = "settings/{owner}/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck]
        public Setting[] GetSettings_Json(string owner)
		{
			return GetSettings(owner);
		}

		/// <summary>
		/// Created: 2009-10-01, John Morgan
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="uuid"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		[WebInvoke(Method = "POST", UriTemplate = "settings/{owner}", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(Roles=HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public Setting UpdateSetting(string owner, string uuid, string value)
		{
            return SettingsManager.UpdateSetting(owner, uuid, value);
		}

		[WebInvoke(Method = "POST", UriTemplate = "settings/{owner}/json", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public Setting UpdateSetting_Json(string owner, string uuid, string value)
		{
			return UpdateSetting(owner, uuid, value);
		}

		[WebInvoke(Method = "POST", UriTemplate = "user/{userName}/password", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck]
        public bool SetPassword(string userName, string currentPassword, string newPassword)
		{
            string currentPasswordHash = BitConverter.ToString(HashAlgorithm.Create("SHA256").ComputeHash(Encoding.ASCII.GetBytes(currentPassword))).ToLower().Replace("-", "");
            string newPasswordHash = BitConverter.ToString(HashAlgorithm.Create("SHA256").ComputeHash(Encoding.ASCII.GetBytes(newPassword))).ToLower().Replace("-", "");
			CriticalResultsEntityManager manager = new CriticalResultsEntityManager();
			UserEntity ue = manager.GetUser(userName);
			UserEntryEntity passwordEntity = null;
			foreach (UserEntryEntity uee in ue.UserEntries)
			{
				if (uee.Type == "AuthExt" && uee.Key == "ANCR")
				{
					passwordEntity = uee;
				}
			}
			if (passwordEntity == null)
				return false;
			else
			{
				if (passwordEntity.Value == currentPasswordHash)
				{
                    return manager.UpdateUserEntryEntity(userName, "AuthExt", "ANCR", newPasswordHash, true);
				}
				return false;
			}
		}
		[WebInvoke(Method = "POST", UriTemplate = "user/{userName}/password/json", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck]
        public bool SetPassword_Json(string userName, string currentPassword, string newPassword)
		{
			return SetPassword(userName, currentPassword, newPassword);
		}

		public bool CreatePassword(string userName)
		{
			string password = Guid.NewGuid().ToString().Substring(0, 6);
            string passwordHash = BitConverter.ToString(HashAlgorithm.Create("SHA256").ComputeHash(Encoding.ASCII.GetBytes(password))).ToLower().Replace("-", "");
            bool setPassSuccess = new CriticalResultsEntityManager().UpdateUserEntryEntity(userName, "AuthExt", "ANCR", passwordHash, true);
            if (setPassSuccess)
            {
                UserEntity user = new CriticalResultsEntityManager().GetUser(userName);
                foreach (UserTransportEntity transport in user.UserTransports)
                {
                    if (transport.Transport.Name == EMAIL_TRANSPORT && !String.IsNullOrEmpty(transport.Address))
                    {
                        SystemMailer = new SystemEmailer();
                        BackgroundWorker worker = new BackgroundWorker();
                        worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                        worker.RunWorkerAsync(transport.Address + "|" + password);
                    }
                }
            }
			return setPassSuccess;
		}

		[WebInvoke(Method = "POST", UriTemplate = "user/{userName}/newpassword", RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public bool GeneratePassword(string userName)
		{
			return CreatePassword(userName);
		}

		void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			string[] parameters = ((string)e.Argument).Split(new char[] { '|' });
			SystemMailer.SendMessage(parameters[0], "New Account Confirmation", "Your Critical Results account password is " + parameters[1]);
		}
		
        [WebInvoke(Method = "POST", UriTemplate = "user/{userName}/newpassword/json", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(Roles = HTTPCheckRoles.sysAdmin | HTTPCheckRoles.clinAdmin)]
        public bool GeneratePassword_Json(string userName)
		{
			return GeneratePassword(userName);
		}

		/// <summary>
		/// Modified:2009-08-07, John Morgan
		/// - Added password check
		/// Modified: 2009-11-23, Jeremy R
		/// - Added CheckUser() to expire tokens.
		/// Modified:2009-12-03, Jeremy R
		///	- Added check for user enabled.  If user is not enabled, no token is returned.
		///	Modified: 2009-12-22, Jeremy R
		/// - Simplified logic
		/// - Removed redundancies added to CheckUser
		///	- Logic allowed returning a null token when a user logs in from another IP.
        ///	Modified: 2009-12-24, Jeremy R.
        ///	- Removed call to ExpireTokens
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="passwordHash"></param>
		/// <returns></returns>
		[WebInvoke(Method = "POST", UriTemplate = "user/{userName}/token", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        public Guid? GetToken(string userName, string passwordHash)
		{
            return GetToken(userName, passwordHash, WcfHelper.GetIPv4FromWCF());
		}

		/// <summary>
		/// Created: 2010-06-18: John Morgan
		///		- created to get token for appications not addressing as wcf service
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="passwordHash"></param>
		/// <param name="ipAddress"></param>
		/// <returns></returns>
		public Guid? GetToken(string userName, string passwordHash, string ipAddress)
		{
            string token;
            string username;
            string message;
            if (!Auth.Authenticate("ANCR", passwordHash, userName, ipAddress, _Trace, out username, out token, out message))
            {
                throw new Exception(message);
            }
            Guid retVal = new Guid(token);
            return retVal;
		}

		[WebInvoke(Method = "POST", UriTemplate = "user/{userName}/token/json", BodyStyle=WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public Guid? GetToken_Json(string userName, string passwordHash)
		{
			return GetToken(userName, passwordHash);
		}

        /// <summary>
        /// Added: 2009-08-19, John Morgan, Added to handle feedback submission.
        /// </summary>
        /// <param name="resultUuid"></param>
        /// <param name="userName"></param>
        /// <param name="rating"></param>
        /// <param name="comments"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "result/{resultUuid}/feedback", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [HTTPBasicCheck(RefreshToken = false)]
        public bool LeaveFeedback(string resultUuid, string userName, int rating, string comments)
        {
			Guid uuid = new Guid(resultUuid);
            RatingEntity re = new CriticalResultsEntityManager().CreateRating(uuid, userName, rating, comments);
            if (re == null)
                return false;
            else
                return true;
        }
        
        [WebInvoke(Method = "POST", UriTemplate = "result/{resultUuid}/feedback/json", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public bool LeaveFeedback_Json(string resultUuid, string userName, int rating, string comments)
        {
            return LeaveFeedback(resultUuid, userName, rating, comments);
        }

		[WebInvoke(Method = "POST", UriTemplate = "/AuthExt/User/json", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck(RefreshToken = false)]
        public User CheckAuthExtUsage(string AuthExtKey, string AuthExtValue)
		{
			return GetUserByAuthExt(AuthExtKey, AuthExtValue);
		}

		#endregion

		public User GetUserByAuthExt(string key, string value)
		{
			UserEntity ue = new CriticalResultsEntityManager().GetUserByAuthExt(key, value);
			if (ue == null)
				return null;
			else
				return new User(ue);
		}
	}
}
