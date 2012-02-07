using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CriticalResults
{
	[DataContract]
	public class Role
	{
		private string _Name;
		[DataMember]
		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}
		private string _Description;
		[DataMember]
		public string Description
		{
			get { return _Description; }
			set { _Description = value; }
		}

		private List<User> _Users;
		[DataMember]
		public User[] Users
		{
			get 
			{ 
				if( _Users != null )
					return _Users.ToArray();
				return null;
			}
			set 
			{
				if (value != null)
					_Users = new List<User>(value);
			}
		}

		private RoleEntity _Entity;

		public Role()
		{

		}
		
		public Role(RoleEntity e)
		{
			_Entity = e;
			_Name = e.Name;
			_Description = e.Description;

		}

		public bool ResolveUsers()
		{
			if (_Entity.Users != null)
			{
				_Users = new List<User>();
				foreach(UserEntity entity in _Entity.Users)
				{
					User u = new User(entity);
					_Users.Add(u);
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
