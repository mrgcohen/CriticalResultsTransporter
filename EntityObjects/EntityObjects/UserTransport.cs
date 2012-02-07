using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CriticalResults
{
	[DataContract]
	public class UserTransport
	{
		private string _Address;
		[DataMember]
		public string Address
		{
			get { return _Address; }
			set { _Address = value; }
		}

		private Transport _Transport;
		[DataMember]
		public Transport Transport
		{
			get { return _Transport; }
			set { _Transport = value; }
		}
		private User _User;
		[DataMember]
		public User User
		{
			get { return _User; }
			set { _User = value; }
		}
		private List<Notification> _Notifications;
		[DataMember]
		public Notification [] Notifications
		{
			get
			{
				if (_Notifications == null)
					return null;
				return _Notifications.ToArray(); 
			}
			set 
			{ 
				if( value == null )
					value = new Notification [0];
				_Notifications = new List<Notification>(value); 
			}
		}
		private List<Level> _Levels;
		[DataMember]
		public Level[] Levels
		{
			get 
			{
				if (_Levels == null)
					return null;
				return _Levels.ToArray(); 
			}
			set 
			{
				if (value == null)
					value = new Level[0];
				_Levels = new List<Level>(value); 
			}
		}
		
		private UserTransportEntity _Entity;

		public UserTransport() { }

		public UserTransport(UserTransportEntity e)
		{
			_Entity = e;
			_Address = e.Address;
		}

		public bool ResolveUser()
		{
			if (_Entity == null)
				return false;
			if (_Entity.User == null)
				return false;
			
			User = new User(_Entity.User);
			return true;
		}

		public bool ResolveTransport()
		{
			if (_Entity == null)
			{
				return false;
			}
			if (_Entity.Transport == null)
			{
				return false;
			}
			_Transport = new Transport(_Entity.Transport);
			return true;
		}

		public bool ResolveLevels()
		{
			if (_Entity == null)
				return false;
			if (_Entity.Levels == null)
			{
				return false;
			}
			
			_Levels = new List<Level>();
			foreach (LevelEntity e in _Entity.Levels)
			{
				Level level = new Level(e);
				_Levels.Add(level);
			}
			return true;
		}

		
	}
}
