using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

[assembly: ContractNamespace("http://partners.org/brigham/criticalresults/", ClrNamespace = "CriticalResults")]
namespace CriticalResults
{
	[DataContract]
	public class Result
	{
		private string _Message;
		[DataMember]
		public string Message
		{
			get { return _Message; }
			set { _Message = value; }
		}
		private DateTime? _CreationTime;
		[DataMember]
		public DateTime? CreationTime
		{
			get { return _CreationTime; }
			set { _CreationTime = value; }
		}
		private Guid _Uuid;
		[DataMember]
		public Guid Uuid
		{
			get { return _Uuid; }
			set { _Uuid = value; }
		}

		private Level _Level = null;
		[DataMember]
		public Level Level
		{
			get { return _Level; }
			set { _Level = value; }
		}
		private List<ResultContext> _Context = null;
		[DataMember]
		public ResultContext[] Context
		{
			get 
			{
				if (_Context != null)
					return _Context.ToArray();
				return null;
			}
			set 
			{
				if (value != null)
					_Context = new List<ResultContext>(value);
			}
		}
		private User _Sender = null;
		[DataMember]
		public User Sender
		{
			get { return _Sender; }
			set { _Sender = value; }
		}
		private User _Receiver = null;
		[DataMember]
		public User Receiver
		{
			get { return _Receiver; }
			set { _Receiver = value; }
		}
		private User _SenderProxy = null;
		[DataMember]
		public User SenderProxy
		{
			get { return _SenderProxy; }
			set { _SenderProxy = value; }
		}
		private DateTime? _EscalationTime;
		[DataMember]
		public DateTime? EscalationTime
		{
			get { return _EscalationTime; }
			set { _EscalationTime = value; }
		}

		[DataMember]
		public string EscalationTimeFormatted
		{
			get
			{
				if (_EscalationTime == null)
					return null;
				TimeSpan delta = _EscalationTime.Value - DateTime.Now;
				string prefix = "";
				if (delta < TimeSpan.Zero)
					prefix = "-";
				return string.Format("{2} {0}h {1}m", Math.Abs((int)delta.TotalHours), Math.Abs(delta.Minutes), prefix);
			}
			set { }
		}


		private List<Acknowledgment> _Acknowledgments = null;
		[DataMember]
		public Acknowledgment[] Acknowledgment
		{
			get 
			{
				if (_Acknowledgments != null)
					return _Acknowledgments.ToArray();
				return null;
			}
			set 
			{
				if (_Acknowledgments != null)
					_Acknowledgments = new List<Acknowledgment>(value);
			}
		}
		private List<Notification> _Notifications = null;
		[DataMember]
		public Notification[] Notifications
		{
			get 
			{
				if (_Notifications != null)
					return _Notifications.ToArray();
				return null;
			}
			set 
			{ 
				if (value != null)
					_Notifications = new List<Notification>(value); 
			}
		}
		private List<Rating> _Ratings = null;
		[DataMember]
		public Rating[] Rating
		{
			get 
			{
				if (_Ratings != null)
					return _Ratings.ToArray();
				return null;
			}
			set 
			{
				if (value != null)
					_Ratings = new List<Rating>(value);
			}
		}
		private List<UserResultTag> _Tags = null;
		[DataMember]
		public UserResultTag[] Tags
		{
			get {
				if (_Tags != null)
					return _Tags.ToArray();
				return null;
			}
			set 
			{
				if (value != null)
					_Tags = new List<UserResultTag>(value);
			}
		}

		private ResultEntity _Entity = null;

		public Result() { }		
		public Result(ResultEntity e)
		{
			_Entity = e;
			ApplyEntity();
		}

		public bool EntityResolved
		{
			get
			{
				if (_Entity == null)
					return true;
				return false;
			}
		}

		private void ApplyEntity()
		{
			_Message = _Entity.Message;
			_CreationTime = _Entity.CreationTime;
			_Uuid = _Entity.Uuid;
			_EscalationTime = _Entity.EscalationTime;
		}

		public bool ResolveEntity()
		{
			if (_Entity != null)
			{
				return true;
			}
			_Entity = new CriticalResultsEntityManager().GetResultEntity(_Uuid);

			if (_Entity != null)
			{
				ApplyEntity();
				return true;
			}
			else
				return false;
		}

		public bool CreateEntity()
		{
			CriticalResultsEntityManager manager = new CriticalResultsEntityManager();
			
			ResultEntity result = new ResultEntity();
			result.Message = _Message;
			if (_CreationTime == null)
				result.CreationTime = DateTime.Now;
			else
				result.CreationTime = _CreationTime.Value;
			result.Uuid = Guid.NewGuid();

			result.Sender = manager.GetUser(_Sender.UserName);
			result.Receiver = manager.GetUser(_Receiver.UserName);
			result.Level = manager.GetLevel(_Level.Name);

			result.EscalationTime = result.CreationTime + result.Level.EscalationTimespan;
			result.DueTime = result.CreationTime + result.Level.DueTimespan;

			if (result.SenderProxy != null)
			{
				result.SenderProxy = manager.GetUser(_SenderProxy.UserName);
			}

			_Entity = manager.CreateResultEntity(result);

			foreach (ResultContext context in _Context)
			{
				ContextTypeEntity contextTypeEntity = manager.GetContextType(context.ContextType.Name);
				ResultContextEntity resultEntity = manager.CreateResultContextEntity(_Entity, contextTypeEntity, context.JsonValue, context.XmlValue, context.PatientKey, context.ExamKey);
				_Entity.ResultContexts.Add(resultEntity);
			}

			ApplyEntity();

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// Change: 2009-05-31, Jeremy R
		///		Removed internal call to automatically resolve any level transports.  This doesn't belong in the Result.  
		///		This can be done by calling Result.ResolveLevel() followed by Result.Levels.ResolveTransport();
		/// 
		/// </remarks>
		public bool ResolveLevel()
		{
			if(_Entity == null )
			{
				return false;
			}
			else if( _Entity.Level == null )
			{
				return false;
			}

			_Level = new Level(_Entity.Level);
			
			return true;
		}
		public bool ResolveContext()
		{
			if (_Entity == null)
				return false;
			else if (_Entity.ResultContexts == null)
				return false;

			_Context = new List<ResultContext>();
			foreach (ResultContextEntity e in _Entity.ResultContexts)
			{
				ResultContext r = new ResultContext(e);
				_Context.Add(r);
			}
			return true;
		}
		public bool ResolveSender()
		{
			if (_Entity == null)
				return false;
			if (_Entity.Sender == null)
				return false;
			
			_Sender = new User(_Entity.Sender);
			return true;
		}
		public bool ResolveReceiver()
		{
			if (_Entity == null)
				return false;
			if (_Entity.Receiver == null)
				return false;
			_Receiver = new User(_Entity.Receiver);
			return true;
		}
		public bool ResolveSenderProxy()
		{
			if (_Entity == null)
				return false;
			if (_Entity.SenderProxy == null)
				return false;
			_SenderProxy = new User(_Entity.SenderProxy);
			return true;
		}
		public bool ResolveAcknowledgment()
		{
			if (_Entity == null)
				return false;
			if (_Entity.Acknowledgments == null)
				return false;
			 
			_Acknowledgments = new List<Acknowledgment>();
			foreach (AcknowledgmentEntity e in _Entity.Acknowledgments)
			{
				Acknowledgment a = new Acknowledgment(e);
				_Acknowledgments.Add(a);
			}
			return true;
		}
		public bool ResolveNotifications()
		{
			if (_Entity == null)
				return false;
			if (_Entity.Notifications != null)
				return false;

			_Notifications = new List<Notification>();
			foreach (NotificationEntity e in _Entity.Notifications)
			{
				Notification notification = new Notification(e);
				_Notifications.Add(notification);
			}
			return true;
		}
		public bool ResolveRatings()
		{
			if (_Entity == null)
				return false;
			if (_Entity.Ratings == null)
			{
				return false;
			}

			_Ratings = new List<Rating>();
			foreach (RatingEntity e in _Entity.Ratings)
			{
				Rating r = new Rating(e);
				_Ratings.Add(r);
			}
			return true;
		}
		public bool ResolveTags()
		{
			if (_Entity == null)
				return false;
			if (_Entity.UserResultTags != null)
				return false;

			_Tags = new List<UserResultTag>();
			foreach (UserResultTagEntity entity in _Entity.UserResultTags)
			{
				UserResultTag tag = new UserResultTag(entity);
				_Tags.Add(tag);
			}
			return true;
		}
	}
}
