using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CriticalResults
{
	[DataContract]
	public class Rating
	{
		private int _Value;
		[DataMember]
		public int Value
		{
			get { return _Value; }
			set { _Value = value; }
		}
		private string _Comments;
		[DataMember]
		public string Comments
		{
			get { return _Comments; }
			set { _Comments = value; }
		}

		private Result _Result;
		[DataMember]
		public Result Result
		{
			get { return _Result; }
			set { _Result = value; }
		}
		private User _User;
		[DataMember]
		public User User
		{
			get { return _User; }
			set { _User = value; }
		}

		private RatingEntity _Entity;
		public Rating() { }

		public Rating(RatingEntity e)
		{
			_Entity = e;
			_Value = e.value;
			_Comments = e.comments;
		}
		public bool ResolveUser()
		{
			if (_Entity.User != null)
			{
				_User = new User(_Entity.User);
			}
			else
			{
				return false;
			}
			return true;
		}
		public bool ResolveResult()
		{
			if (_Entity.Result != null)
			{
				_Result = new Result(_Entity.Result);
			}
			else
			{
				return false;
			}
			return true;
		}
	}
}
