using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CriticalResults
{
	[DataContract]
	public class User
	{
		private string _UserName;
		[DataMember]
		public string UserName
		{
			get { return _UserName; }
			set { _UserName = value; }
		}
		private string _FirstName;
		[DataMember]
		public string FirstName
		{
			get { return _FirstName; }
			set { _FirstName = value; }
		}
		private string _LastName;
		[DataMember]
		public string LastName
		{
			get { return _LastName; }
			set { _LastName = value; }
		}
		private string _MiddleName;
		[DataMember]
		public string MiddleName
		{
			get { return _MiddleName; }
			set { _MiddleName = value; }
		}
		private string _Title;
		[DataMember]
		public string Title
		{
			get { return _Title; }
			set { _Title = value; }
		}

		private string _PinHash;
		public string PinHash
		{
			get { return _PinHash; }
			set { _PinHash = value; }
		}

		private string _PasswordHash;
		public string PasswordHash
		{
			get { return _PasswordHash; }
			set { _PasswordHash = value; }
		}

        private bool _HasPassword;
        [DataMember]
        public bool HasPassword
        {
            get { return _HasPassword; }
            set { _HasPassword = value; }
        }


		private bool _IsSystemAccount;
		[DataMember]
		public bool IsSystemAccount
		{
			get { return _IsSystemAccount; }
			set { _IsSystemAccount = value; }
		}

		private bool _Enabled;
		[DataMember]
		public bool Enabled
		{
			get { return _Enabled; }
			set { _Enabled = value; }
		}

		private List<Result> _SentResults = null;
		private List<Result> _ReceivedResults = null;
		private List<Role> _Roles;
		[DataMember]
		public Role [] Roles
		{
			get 
			{
				if (_Roles != null)
					return _Roles.ToArray();
				return null;
			}
			set 
			{
				if (value != null)
					_Roles = new List<Role>(value);
			}
		}

		private string _Credentials;
		[DataMember]
		public string Credentials
		{
			get { return _Credentials; }
			set { _Credentials = value; }
		}

        private string _Address;
        [DataMember]
        public string Address
        {
            get { return _Address; }
            set { _Address = value; }
        }

        private string _CellPhone;
        [DataMember]
        public string CellPhone
        {
            get { return _CellPhone; }
            set { _CellPhone = value; }
        }

        private string _OfficePhone;
        [DataMember]
        public string OfficePhone
        {
            get { return _OfficePhone; }
            set { _OfficePhone = value; }
        }

        private string _HomePhone;
        [DataMember]
        public string HomePhone
        {
            get { return _HomePhone; }
            set { _HomePhone = value; }
        }

        private string _CellPhoneProvider;
        [DataMember]
        public string CellPhoneProvider
        {
            get { return _CellPhoneProvider; }
            set { _CellPhoneProvider = value; }
        }

        private string _City;
        [DataMember]
        public string City
        {
            get { return _City; }
            set { _City = value; }
        }

        private string _State;
        [DataMember]
        public string State
        {
            get { return _State; }
            set { _State = value; }
        }

        private string _Zip;
        [DataMember]
        public string Zip
        {
            get { return _Zip; }
            set { _Zip = value; }
        }

        private string _Country;
        [DataMember]
        public string Country
        {
            get { return _Country; }
            set { _Country = value; }
        }

        private string _NPI;
        [DataMember]
        public string NPI
        {
            get { return _NPI; }
            set { _NPI = value; }
        }

        private string _Pager;
        [DataMember]
        public string Pager
        {
            get { return _Pager; }
            set { _Pager = value; }
        }

        private string _PagerId;
        [DataMember]
        public string PagerId
        {
            get { return _PagerId; }
            set { _PagerId = value; }
        }

        private string _PagerType;
        [DataMember]
        public string PagerType
        {
            get { return _PagerType; }
            set { _PagerType = value; }
        }

		private List<Acknowledgment> _Acknowledgments = null;
		private List<Rating> _Ratings = null;
		private List<UserTransport> _UserTransports = null;
		[DataMember]
		public UserTransport[] Transports
		{
			get
			{
				if (_UserTransports == null)
					return null;
				return _UserTransports.ToArray();;
			}
			set 
			{
				if (value != null)
					_UserTransports = new List<UserTransport>(value);
			}
		}
		private List<UserResultTag> _Tags = null;
		[DataMember]
		public UserResultTag [] Tags
		{
			get 
			{
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

		private List<User> _ProxyUsers = null;
		[DataMember]
		public User [] ProxyUsers
		{
			get
			{
				if (_ProxyUsers != null)
					return _ProxyUsers.ToArray();
				return null;
			}
			set
			{
				if (value != null)
					_ProxyUsers = new List<User>(value);
			}
		}
		private List<User> _ProxiedUsers = null;
		[DataMember]
		public User [] ProxiedUsers
		{
			get
			{
				if (_ProxiedUsers != null)
					return _ProxiedUsers.ToArray();
				return null;
			}
			set
			{
				if (value != null)
					_ProxiedUsers = new List<User>(value);
			}
		}
		private List<UserEntry> _UserEntries = null;
		[DataMember]
		public UserEntry[] UserEntries
		{
			get
			{
				if (_UserEntries != null)
					return _UserEntries.ToArray();
				return null;
			}
			set
			{
				if (value != null)
					_UserEntries = new List<UserEntry>(value);
			}
		}

		private UserEntity _Entity;

		public User(){}

		public User(UserEntity e)
		{
			_Entity = e;
			_UserName = e.UserName;
			_FirstName = e.FirstName;
			_LastName = e.LastName;
			_MiddleName = e.MiddleName;
			_Title = e.Title;
			_IsSystemAccount = !e.IsHomoSapien;
            _Credentials = e.Credentials;
            _Address = e.Address ?? "";
            _CellPhone = e.CellPhone ?? "";
            _OfficePhone = e.OfficePhone ?? "";
            _HomePhone = e.HomePhone ?? "";
            _City = e.City ?? "";
            _State = e.State ?? "";
            _Country = e.Country ?? "";
            _Zip = e.Zip ?? "";
            _CellPhoneProvider = e.CellPhoneProvider ?? "";
            _Pager = e.Pager ?? "";
            _PagerId = e.PagerId ?? "";
            _PagerType = e.PagerType ?? "";
            _NPI = e.NPI ?? "";
			_Enabled = e.Enabled;
			
		}

		public bool ResolveEntries()
		{
			if (_Entity == null)
				return false;
			if (_Entity.UserEntries == null)
				return false;

			_UserEntries = new List<UserEntry>();
			foreach (UserEntryEntity e in _Entity.UserEntries)
			{
                if (e.Key == "ANCR")
                    _HasPassword = true;
				//if (e.RestrictedAccess == false)
				{
					UserEntry u = new UserEntry(e);
					_UserEntries.Add(u);
				}
			}
			return true;
		}

		public bool ResolveTransports()
		{
			if (_Entity == null)
				return false;
			if (_Entity.UserTransports == null)
				return false;

			_UserTransports = new List<UserTransport>();
			foreach (UserTransportEntity e in _Entity.UserTransports)
			{
				UserTransport t = new UserTransport(e);
				t.ResolveTransport();
				_UserTransports.Add(t);
			}
			return true;

		}
		public bool ResolveRoles()
		{
			if (_Entity == null)
				return false;
			if (_Entity.Roles == null)
				return false;

			_Roles = new List<Role>();
			foreach (RoleEntity entity in _Entity.Roles)
			{
				Role r = new Role(entity);
				_Roles.Add(r);
			}
			return true;
		}
		public bool ResolveProxied()
		{
			if (_Entity == null)
				return false;
			if (_Entity.ProxiedUsers == null)
				return false;

			_ProxiedUsers = new List<User>();
			foreach (ProxyEntity entity in _Entity.ProxiedUsers)
			{
				if (entity.User == null)
					return false;

				User u = new User(entity.User);
				_ProxiedUsers.Add(u);
			}
			return true;
		}

		public bool ResolveProxies()
		{
			if (_Entity == null)
				return false;
			
			if (_Entity.ProxyUsers == null)
				return false;

			_ProxyUsers = new List<User>();
			foreach (ProxyEntity entity in _Entity.ProxyUsers)
			{
				if (entity.ProxyUser == null)
					return false;

				User u = new User(entity.ProxyUser);
				_ProxyUsers.Add(u);
			}
			return true;

		}

		public UserEntity ToEntity()
		{
			UserEntity entity = new UserEntity();
			entity.Credentials = Credentials;
            entity.Address = Address;
            entity.CellPhone = CellPhone;
            entity.OfficePhone = OfficePhone;
            entity.HomePhone = HomePhone;
            entity.CellPhoneProvider = CellPhoneProvider;
            entity.City = City;
            entity.Country = Country;
            entity.State = State;
            entity.Zip = Zip;
            entity.Pager = Pager;
            entity.PagerId = PagerId;
            entity.PagerType = PagerType;
            entity.NPI = NPI;
			entity.FirstName = FirstName;
			entity.IsHomoSapien = !IsSystemAccount;
			entity.LastName = LastName;
			entity.MiddleName = MiddleName;
			entity.Title = Title;
			entity.UserName = UserName;
			entity.Enabled = Enabled;
			return entity;
		}
		
	}
}
