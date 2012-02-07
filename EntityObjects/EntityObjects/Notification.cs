using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CriticalResults
{
	[DataContract]
	public class Notification
	{
		/// <summary>
		/// Allowed notification states
		/// </summary>
		public enum NotificationState
		{
			Manual,
			New,
			Processing,
			Error,
			Sent,
			New_Escalated,
			Sent_Escalated,
		}
		
		private DateTime _CreationTime;
		[DataMember]
		public DateTime CreationTime
		{
			get { return _CreationTime; }
			set { _CreationTime = value; }
		}
		[DataMember]
		public string CreationTimeFormatted
		{
			get { return _CreationTime.ToString("MM-dd-yyyy hh:mm tt"); }
			set { }
		}
		private Guid _Uuid;
		[DataMember]
		public Guid Uuid
		{
			get { return _Uuid; }
			set { _Uuid = value; }
		}

		private Result _Result;
		[DataMember]
		public Result Result
		{
			get { return _Result; }
			set { _Result = value; }
		}
		private UserTransport _UserTransport;
		[DataMember]
		public UserTransport UserTransport
		{
			get { return _UserTransport; }
			set { _UserTransport = value; }
		}

		private string _Notes;
		[DataMember]
		public string Notes
		{
			get { return _Notes; }
			set { _Notes = value; }
		}

		private string _State;
		[DataMember]
		public string State
		{
			get { return _State; }
			set { _State = value; }
		}
		
		private NotificationEntity _Entity;
		public Notification() { }
		public Notification(NotificationEntity e)
		{
			_Entity = e;
			_CreationTime = e.CreationTime;
			_Uuid = e.Uuid;
			_State = e.State;
			_Notes = e.Notes;
		}
		public bool ResolveResult()
		{
			if( _Entity == null )
				return false;
			if (_Entity.Result == null)
				return false;

			_Result = new Result(_Entity.Result);

			return true;
		}
		public bool ResolveUserTransport()
		{
			if (_Entity == null)
				return false;
			if( _Entity.UserTransport == null )
				return false;

			_UserTransport = new UserTransport(_Entity.UserTransport);
			return true;
		}
	}
}
