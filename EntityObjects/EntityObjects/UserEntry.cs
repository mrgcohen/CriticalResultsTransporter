using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CriticalResults
{
	[DataContract]
	public class UserEntry
	{
		private int _Id;

		private string _Type;
		[DataMember]
		public string Type
		{
			get { return _Type; }
			set { _Type = value; }
		}
		private string _Key;
		[DataMember]
		public string Key
		{
			get { return _Key; }
			set { _Key = value; }
		}
		private string _Value;
		[DataMember]
		public string Value
		{
			get { return _Value; }
			set { _Value = value; }
		}

        private bool _RestrictedAccess;
		[DataMember]
        public bool RestrictedAccess
		{
            get { return _RestrictedAccess; }
            set { _RestrictedAccess = value; }
		}
		private string _XmlValue;
		[DataMember]
		public string XmlValue
		{
			get { return _XmlValue; }
			set { _XmlValue = value; }
		}
		private User _User;
		[DataMember]
		public User User
		{
			get
			{
				if (_User != null)
					return _User;
				return null;
			}
			set { _User = value; }
		}

		private UserEntryEntity _Entity;

		public UserEntry()
		{

		}
		public UserEntry(UserEntryEntity e)
		{
			_Entity = e;
			_Type = e.Type;
			_Key = e.Key;
			_Value = e.Value;
			_XmlValue = e.XmlValue;
            _RestrictedAccess = e.RestrictedAccess;
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
	}
}
