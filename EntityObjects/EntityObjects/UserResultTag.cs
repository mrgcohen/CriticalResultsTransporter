using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CriticalResults
{
	[DataContract]
	public class UserResultTag
	{
		private Guid _Uuid;
		[DataMember]
		public Guid Uuid
		{
			get { return _Uuid; }
			set { _Uuid = value; }
		}
		private string _Tag;
		[DataMember]
		public string Tag
		{
			get { return _Tag; }
			set { _Tag = value; }
		}
		private string _Description;
		[DataMember]
		public string Description
		{
			get { return _Description; }
			set { _Description = value; }
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

		private UserResultTagEntity _Entity;

		public UserResultTag() { }

		public UserResultTag(UserResultTagEntity e)
		{
			_Entity = e;
			_Uuid = e.Uuid;
			_Tag = e.Tag;
			_Description = e.Description;
		}
		public bool ResolveUser()
		{
			if (_Entity != null)
			{
				if (_Entity.User != null)
				{
					_User = new User(_Entity.User);
				}
			}
			else
			{
				return false;
			}
			return true;
		}
		public bool ResolveResult()
		{
			if (_Entity != null)
			{
				if (_Entity.Result != null)
				{
					_Result = new Result(_Entity.Result);
				}
			}
			else
			{
				return false;
			}
			return true;
		}
	}
}
