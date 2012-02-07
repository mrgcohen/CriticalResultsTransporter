using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CriticalResults
{
	[DataContract]
	public class Acknowledgment
	{
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

		private string _Notes;
		[DataMember]
		public string Notes
		{
			get { return _Notes; }
			set { _Notes = value; }
		}

		private User _User;
		[DataMember]
		public User User
		{
			get { return _User; }
			set { _User = value; }
		}
		private Result _Result;
		[DataMember]
		public Result Result
		{
			get { return _Result; }
			set { _Result = value; }
		}

		private AcknowledgmentEntity _Entity;
		public Acknowledgment() { }
		public Acknowledgment(AcknowledgmentEntity e)
		{
			_Entity = e;
			_CreationTime = e.CreationTime;
			_Notes = e.Notes;
		}
		public bool ResolveUser()
		{
			if (_Entity == null)
				return false;
			if (_Entity.User == null)
				return false;

			_User = new User(_Entity.User);
			return true;
		}
		public bool ResolveResult()
		{
			if (_Entity == null)
				return false;
			if (_Entity.Result == null)
				return false;

			_Result = new Result(_Entity.Result);			
			return true;
		}

	}
}
