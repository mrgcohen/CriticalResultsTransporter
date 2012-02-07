using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace CriticalResults
{
	public class CriticalResultsEntityManager
	{
		CriticalResultsTransporterEntities _ObjectContext;

		public CriticalResultsTransporterEntities ObjectContext
		{
			get { return _ObjectContext; }
		}

		public const string MANUAL_TRANSPORT_NAME = "Manual";	//TODO: 2009-07-18, JR: this doesn't belong here.  Maybe it belongs in the Transport, but this is primarily a data layer...

		public CriticalResultsEntityManager()
		{
			_ObjectContext = new CriticalResultsTransporterEntities();
		}
		public void SaveChanges()
		{
			_ObjectContext.SaveChanges(); 
		}

		/// <summary>
		/// Modified: 2009-07-31, John Morgan
		///		- Added blank password object to UserEntryEntities on CreateUserEntity
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public UserEntity CreateUserEntity(UserEntity user)
		{
			var checkUser = GetUser(user.UserName);
			if (checkUser != null)
			{
				checkUser.FirstName = user.FirstName;
				checkUser.MiddleName = user.MiddleName;
				checkUser.LastName = user.LastName;
				if (!String.IsNullOrEmpty(user.Title))
				{
					checkUser.Title = user.Title;
				}
				if (!String.IsNullOrEmpty(user.Credentials))
				{
					checkUser.Credentials = user.Credentials;
				}
				_ObjectContext.SaveChanges();
			}
			else
			{
				_ObjectContext.AddToUserEntitySet(user);
				_ObjectContext.SaveChanges();
			}
			return GetUser(user.UserName);
		}

		public UserEntity GetUser(string userName)
		{
			var query = from users in _ObjectContext.UserEntitySet.Include("Roles").Include("UserTransports").Include("UserTransports.Levels").Include("UserTransports.Transport").Include("ProxiedUsers").Include("ProxiedUsers.User").Include("UserEntries")
						where users.UserName == userName
						select users;
			if (query.Count() == 1)
			{
				return query.First();
			}
			return null;
		}

		/// <summary>
		/// Modified: 2009-10-26, John Morgan
		/// Was not setting IsHomoSapien
		/// </summary>
		/// <param name="changedUser"></param>
		/// <returns></returns>
		public UserEntity UpdateUserEntity(UserEntity changedUser)
		{
			UserEntity persistentUser = GetUser(changedUser.UserName);

			persistentUser.FirstName = changedUser.FirstName;
			persistentUser.LastName = changedUser.LastName;
			persistentUser.MiddleName = changedUser.MiddleName;
			persistentUser.Title = changedUser.Title;
			persistentUser.IsHomoSapien = changedUser.IsHomoSapien;
			persistentUser.Credentials = changedUser.Credentials;
            persistentUser.Address = changedUser.Address;
            persistentUser.CellPhone = changedUser.CellPhone;
            persistentUser.OfficePhone = changedUser.OfficePhone;
            persistentUser.HomePhone = changedUser.HomePhone;
            persistentUser.State = changedUser.State;
            persistentUser.Zip = changedUser.Zip;
            persistentUser.Country = changedUser.Country;
            persistentUser.City = changedUser.City;
            persistentUser.CellPhoneProvider = changedUser.CellPhoneProvider;
            persistentUser.Pager = changedUser.Pager;
            persistentUser.PagerId = changedUser.PagerId;
            persistentUser.PagerType = changedUser.PagerType;
            persistentUser.NPI = changedUser.NPI;

			persistentUser.Enabled = changedUser.Enabled;

			_ObjectContext.SaveChanges();

			return persistentUser;
		}

		public UserEntity UpdateUserRoles(string userName, string[] roleNames)
		{
			UserEntity ue = GetUser(userName);
			List<RoleEntity> reList = new List<RoleEntity>();
			reList.AddRange(ue.Roles);
			foreach (RoleEntity re in reList)
			{
				ue.Roles.Remove(re);
			}
			foreach (string role in roleNames)
			{
				RoleEntity re = GetRole(role);
				ue.Roles.Add(re);
			}
			_ObjectContext.SaveChanges();
			return ue;
		}

		public void UpdateUserCredentials(UserEntity changedUser, string passwordHash, string pinHash)
		{
			throw new System.NotImplementedException();
			
			//UserEntity persistentUser = GetUser(changedUser.UserName);

			//persistentUser.PasswordHash = changedUser.PasswordHash;
			//persistentUser.PinHash = changedUser.PinHash;

			//_ObjectContext.SaveChanges();
		}

		public UserEntity[] GetUsersForProxy(string userName)
		{
			var query = from users in _ObjectContext.UserEntitySet.Include("ProxiedUsers").Include("ProxiedUsers.User")
					where users.UserName == userName
					select users;
			UserEntity u = query.First();
			List<UserEntity> proxiedUsers = new List<UserEntity>();
			foreach (ProxyEntity p in u.ProxiedUsers)
			{
				proxiedUsers.Add(p.User);
			}

			return proxiedUsers.ToArray();
		}
		public ProxyEntity CreateProxy(string masterUserName, string proxyUserName, string relationshipDescription)
		{
			UserEntity master = GetUser(masterUserName);
			UserEntity proxy = GetUser(proxyUserName);
			ProxyEntity p = new ProxyEntity();
			
			p.ProxyUser = proxy;
			p.User = master;
			p.Uuid = new Guid();
			p.RelationshipDescription = relationshipDescription;
			
			_ObjectContext.SaveChanges();
			return p;
		}
		public ProxyEntity GetProxyEntity(string masterUserName, string proxyUserName)
		{
			var query = from users in _ObjectContext.UserEntitySet
						from proxies in users.ProxyUsers
						where users.UserName == masterUserName
						where proxies.ProxyUser.UserName == proxyUserName
						select proxies;
			if (query.Count() == 0)
				return query.First();
			return null;
		}
		public void DeleteProxy(string masterUserName, string proxyUserName)
		{
			ProxyEntity proxy = GetProxyEntity(masterUserName, proxyUserName);
			_ObjectContext.DeleteObject(proxy);
			_ObjectContext.SaveChanges();
		}

		public UserEntity[] GetAllUsers()
		{
			var query = from users in _ObjectContext.UserEntitySet.Include("Roles")
						select users;
			return query.ToArray();
		}

		public RoleEntity[] GetUserRoles(string userName)
		{
			var query = from roles in _ObjectContext.RoleEntitySet
						from users in roles.Users
						where users.UserName == userName
						select roles;
			return query.ToArray();
		}
		public RoleEntity GetUserRole(string userName, string roleName)
		{
			var query = from role in _ObjectContext.RoleEntitySet
						from users in role.Users
						where role.Name == roleName
						where users.UserName == userName
						select role;
			return query.First();
		}
		public RoleEntity[] GetAllRoles()
		{
			var query = from roles in _ObjectContext.RoleEntitySet
						select roles;
			return query.ToArray();
		}
		public RoleEntity GetRole(string roleName)
		{
			var query = from role in _ObjectContext.RoleEntitySet
						where role.Name == roleName
						select role;
			return query.First();
		}
		public void AddUserToRole(string userName, string roleName)
		{
			UserEntity user = GetUser(userName);
			RoleEntity role = GetRole(roleName);
			user.Roles.Add(role);
			_ObjectContext.SaveChanges();
		}
		public void RemoveUserFromRole(string userName, string roleName)
		{
			UserEntity user = GetUser(userName);
			RoleEntity role = GetRole(roleName);
			user.Roles.Remove(role);
			_ObjectContext.SaveChanges();
		}

		public TransportEntity CreateTransport(string name, string transportUri, string friendlyName)
		{
			TransportEntity transport = new TransportEntity();
			transport.Name = name;
			transport.TransportURI = transportUri;
			transport.FriendlyAddressName = friendlyName;
			_ObjectContext.AddToTransportEntitySet(transport);
			_ObjectContext.SaveChanges();
			return transport;
		}

		public bool DeleteTransport(string name, string transportUri)
		{
			TransportEntity[] transports = (from p in _ObjectContext.TransportEntitySet.Include("TransportLevels").Include("TransportLevels.Level").Include("UserTransports")
											where p.Name == name && p.TransportURI == transportUri
											select p).ToArray();
			foreach (TransportEntity transport in transports)
			{
				if (transport.TransportLevels.Count() > 0 || transport.UserTransports.Count() > 0)
				{
					return false;
				}
				else
				{
					_ObjectContext.DeleteObject(transport);
				}
			}
			_ObjectContext.SaveChanges();
			return true;
		}

		public TransportEntity UpdateTransport(string origName, string origUri, string newName, string newUri, string friendlyName)
		{
			TransportEntity transport = (from transports in _ObjectContext.TransportEntitySet
										 where transports.Name == origName && transports.TransportURI == origUri
										 select transports).FirstOrDefault();
			transport.Name = newName;
			transport.TransportURI = newUri;
			transport.FriendlyAddressName = friendlyName;
			_ObjectContext.SaveChanges();
			return transport;
		}

		public TransportEntity GetTransport(string name)
		{
			var query = from transports in _ObjectContext.TransportEntitySet.Include("TransportLevels").Include("TransportLevels.Level")
						where transports.Name == name
						select transports;
			return query.First();
		}

		public TransportEntity[] GetAllTransports()
		{
			var query = from transports in _ObjectContext.TransportEntitySet.Include("TransportLevels").Include("TransportLevels.Level")
						select transports;
			return query.ToArray();
		}

		public UserTransportEntity CreateUserTransport(string userName, string transportName, string address)
		{
			UserEntity user = GetUser(userName);
			TransportEntity transport = GetTransport(transportName);

			return CreateUserTransport(user, transport, address);
		}

		public UserTransportEntity CreateUserTransport(UserEntity user, TransportEntity transport, string address)
		{
			UserTransportEntity userTransport = new UserTransportEntity();
			userTransport.User = user;
			userTransport.Transport = transport;
			userTransport.Address = address;

			_ObjectContext.SaveChanges();

			return userTransport;
		}

		public UserTransportEntity GetUserTransport(string userName, string transportName)
		{
			var query = from userTransports in _ObjectContext.UserTransportEntitySet.Include("Levels").Include("Transport")
						where userTransports.User.UserName == userName
						where userTransports.Transport.Name == transportName
						select userTransports;
			return query.First();
		}

		/// <summary>
		/// Modified: 2009-08-07, John Morgan
		/// Altered to return null of no return from query.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="transportName"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public UserTransportEntity GetUserTransport(string userName, string transportName, string address)
		{
			var query = from userTransports in _ObjectContext.UserTransportEntitySet.Include("Levels").Include("Transport")
						where userTransports.User.UserName == userName && userTransports.Transport.Name == transportName
						select userTransports;
			if (query.Count() > 0)
				return query.First();
			else
				return null;
	
		}

		/// <summary>
		/// Modified: 2009-08-07, John Morgan
		/// Altered to skip changes if no UserTransport is found
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="transportName"></param>
		/// <param name="originalAddress"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public UserTransportEntity UpdateUserTransport(string userName, string transportName, string originalAddress, string address)
		{
			UserTransportEntity userTransport = GetUserTransport(userName, transportName, originalAddress);
			if (userTransport != null)
			{
				userTransport.Address = address;
				_ObjectContext.SaveChanges();
				return userTransport;
			}
			return null;
		}

		public UserTransportEntity[] GetUserTransports(string userName)
		{
			var query = from userTransports in _ObjectContext.UserTransportEntitySet.Include("Transport").Include("Levels")
						where userTransports.User.UserName == userName
						select userTransports;
			return query.ToArray();
		}

        /// <summary>
        /// Modified: 2009-10-01 - John Morgan: removed add of manual transport
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="shortDescription"></param>
        /// <param name="colorValue"></param>
        /// <param name="escalationTimeout"></param>
        /// <param name="dueTimeout"></param>
        /// <param name="directContactRequired"></param>
        /// <returns></returns>
		public LevelEntity CreateLevel(string name, string description, string shortDescription, string colorValue, DateTime escalationTimeout, DateTime dueTimeout, bool directContactRequired)
		{
			LevelEntity level = new LevelEntity();
			level.Name = name;
			level.Uuid = Guid.NewGuid();
			level.Description = description;
			level.ColorValue = colorValue;
			level.EscalationTimeout = escalationTimeout;
			level.DueTimeout = dueTimeout;
			level.ShortDescription = shortDescription;
			level.DirectContactRequired = directContactRequired;

			_ObjectContext.AddToLevelEntitySet(level);
			_ObjectContext.SaveChanges();

			return level;
		}

		public LevelEntity UpdateLevel(Guid uuid, string name, string description, string shortDescription, string colorValue, DateTime escalationTimeout, DateTime dueTimeout, bool directContactRequired)
		{
			LevelEntity level = GetLevel(uuid);
			
			level.Name = name;
			level.Description = description;
			level.ShortDescription = shortDescription;
			level.ColorValue = colorValue;
			level.EscalationTimeout = escalationTimeout;
			level.DueTimeout = dueTimeout;
			level.DirectContactRequired = directContactRequired;

			_ObjectContext.SaveChanges();
			return level;
		}

		public LevelEntity GetLevel(string name)
		{
			var query = from levels in _ObjectContext.LevelEntitySet.Include("TransportLevels").Include("TransportLevels.Transport")
						where levels.Name == name
						select levels;
			if (query.Count() == 1)
				return query.First();
			return null;
		}

		public LevelEntity GetLevel(Guid Uuid)
		{
			var query = from levels in _ObjectContext.LevelEntitySet.Include("TransportLevels").Include("TransportLevels.Transport")
						where levels.Uuid == Uuid
						select levels;
			if (query.Count() == 1)
				return query.First();
			return null;
		}

		public LevelEntity[] GetLevels()
		{
			var query = from levels in _ObjectContext.LevelEntitySet.Include("TransportLevels").Include("TransportLevels.Transport").OrderBy("it.EscalationTimeout")
						select levels;
            return query.ToArray();
		}

		public TransportEntity[] GetTransportsForLevel(Guid levelUuid)
		{
			var query = from levels in _ObjectContext.LevelEntitySet.Include("TransportLevels").Include("TransportLevels.Transport")
						where levels.Uuid == levelUuid
						select levels;
			List<TransportEntity> list = new List<TransportEntity>();
			foreach (TransportLevelEntity tl in query.First().TransportLevels.ToArray())
			{
				list.Add(tl.Transport);
			}
			return list.ToArray();
		}

		public bool CreateTransportLevelEntity(Guid levelUuid, string transportName, bool mandatory)
		{
			var levelQuery = from levels in _ObjectContext.LevelEntitySet.Include("TransportLevels").Include("TransportLevels.Transport")
							 where levels.Uuid == levelUuid
							 select levels;
			var transportQuery = from transports in _ObjectContext.TransportEntitySet
								 where transports.Name == transportName
								 select transports;

			TransportLevelEntity tl = new TransportLevelEntity();
			tl.Level = levelQuery.First();
			tl.Transport = transportQuery.First();
			tl.Mandatory = mandatory;
							
			_ObjectContext.SaveChanges();
			
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="levelUuid"></param>
		/// <param name="transportName"></param>
		/// <returns></returns>
		/// <remarks>
		/// Improvements: Query for TransportLevel instead of looping
		/// </remarks>
		public bool RemoveTransportFromLevel(Guid levelUuid, string transportName)
		{
			var query = from levels in _ObjectContext.LevelEntitySet.Include("TransportLevels").Include("TransportLevels.Transport")
							 where levels.Uuid == levelUuid
							 select levels;
			LevelEntity level = query.First();
			
			
			TransportLevelEntity deleteMe = null;
			foreach (TransportLevelEntity e in level.TransportLevels)
			{
				if (e.Transport.Name == transportName)
				{
					deleteMe = e;
					break;
				}
			}
			_ObjectContext.DeleteObject(deleteMe);
			_ObjectContext.SaveChanges();
			return true;
			
		}


		public ResultEntity CreateResultEntity(string message, DateTime creationTime, string receiverUserName, string senderUserName, string levelName)
		{
			ResultEntity result = new ResultEntity();
			result.Message = message;
			result.CreationTime = creationTime;
			result.Uuid = Guid.NewGuid();
			result.Sender = GetUser(senderUserName);
			result.Receiver = GetUser(receiverUserName);
			result.Level = GetLevel(levelName);

			return CreateResultEntity(result);
		}

		public ResultEntity CreateResultEntity(ResultEntity result)
		{
			//_ObjectContext.AddToResultEntitySet(result);  shouldn't be necessary since we have added the linkages
			_ObjectContext.SaveChanges();

			return result;
		}

		public ResultListEntity[] QueryResultList(string queryString, int? pageSize, int? pageNumber)
		{
			int skipRecords = 0;
			int limitRecords = int.MaxValue;
			if (pageSize != null)
				limitRecords = pageSize.Value;
			if (pageNumber != null && pageSize != null)
				skipRecords = (pageNumber.Value - 1) * pageSize.Value;

			if (string.IsNullOrEmpty(queryString))
				queryString = "1 == 1";

			ObjectQuery<ResultListEntity> query = _ObjectContext.ResultListEntitySet.Where(queryString);
			query.MergeOption = MergeOption.OverwriteChanges;
			return query.OrderBy("it.AcknowledgmentTime, it.EscalationTime, it.CreationTime").Skip(skipRecords).Take(limitRecords).ToArray();
		}

		public ResultEntity GetResultEntity(Guid uuid)
		{
			var query = from results in _ObjectContext.ResultEntitySet.Include("Sender").Include("Receiver").Include("Level").Include("Level.TransportLevels").Include("Level.TransportLevels.Transport").Include("ResultContexts").Include("SenderProxy")
						where results.Uuid == uuid
						select results;

			if (query.Count() == 1)
			{
				ResultEntity result = query.First();
				_ObjectContext.Refresh(RefreshMode.StoreWins, result);
				return result;
			}
			return null;
		}

		public ResultEntity[] QueryResultEntity(string queryString, int? pageSize, int? pageNumber)
		{
			int skipRecords = 0;
			int limitRecords = int.MaxValue;
			if (pageSize != null)
				limitRecords = pageSize.Value;
			if (pageNumber != null && pageSize != null)
				skipRecords = (pageNumber.Value - 1) * pageSize.Value;

			if (string.IsNullOrEmpty(queryString))
				queryString = "1 == 1";

			ObjectQuery<ResultEntity> query = _ObjectContext.ResultEntitySet.Where(queryString);
			query.MergeOption = MergeOption.PreserveChanges;
			return query.OrderBy("it.Id").Skip(skipRecords).Take(limitRecords).ToArray();
		}

		public ResultEntity UpdateResultReceiver(Guid uuid, string userName)
		{
			ResultEntity result = GetResultEntity(uuid);
			UserEntity user = GetUser(userName);
			result.Receiver = user;
			_ObjectContext.SaveChanges();
			return result;
		}

		/// <summary>
		/// Modified: 2009-10-2, John Morgan
		/// Return only first 100 records.
		/// </summary>
		/// <param name="queryString"></param>
		/// <param name="pageSize"></param>
		/// <param name="pageNumber"></param>
		/// <returns></returns>
		public UserEntity[] QueryUserEntity(string queryString, int? pageSize, int? pageNumber)
		{
			int skipRecords = 0;
			int limitRecords = 100;
			if (pageSize != null)
				limitRecords = pageSize.Value;
			if (pageNumber != null && pageSize != null)
				skipRecords = (pageNumber.Value - 1) * pageSize.Value;

			if (string.IsNullOrEmpty(queryString))
				queryString = "1 == 1";

			ObjectQuery<UserEntity> query = _ObjectContext.UserEntitySet.Where(queryString);
			query.MergeOption = MergeOption.PreserveChanges;
			return query.OrderBy("it.Id").Skip(skipRecords).Take(limitRecords).ToArray();
		}

		public ContextTypeEntity CreateContextType(string namespace_uri, string name, string description, string xmlSchema, string jsonTemplate)
		{
			ContextTypeEntity c = new ContextTypeEntity();
			c.Uuid = Guid.NewGuid();
			c.NamespaceUri = namespace_uri;
			c.Name = name;
			c.Description = description;
			c.XmlSchema = xmlSchema;
			c.JsonTemplate = jsonTemplate;

			_ObjectContext.AddToContextTypeEntitySet(c);
			_ObjectContext.SaveChanges();
			return c;
		}

		public ResultContextEntity CreateResultContextEntity(ResultEntity resultEntity, ContextTypeEntity contextTypeEntity, string jsonValue, string xmlValue, string patientKey, string examKey)
		{
			ResultContextEntity r = new ResultContextEntity();
			r.JsonValue = jsonValue;
			r.XmlValue = xmlValue;
			r.Result = resultEntity;
			r.ContextType = contextTypeEntity;
			r.PatientKey = patientKey;
			r.ExamKey = examKey;

			_ObjectContext.AddToResultContextEntitySet(r);
			_ObjectContext.SaveChanges();

			return r;
		}

		public ContextTypeEntity[] GetAllContextTypes()
		{
			var query = from contextTypes in _ObjectContext.ContextTypeEntitySet
						select contextTypes;
			return query.ToArray();
		}
		public ContextTypeEntity GetContextType(string name)
		{
			var query = from contextTypes in _ObjectContext.ContextTypeEntitySet
						where contextTypes.Name == name
						select contextTypes;
			if (query.Count() == 1)
			{
				return query.First();
			}
			return null;
		}

		public AcknowledgmentEntity GetResultAcknowledgment(Guid resultGuid)
		{
			var query = from ack in _ObjectContext.AcknowledgmentEntitySet.Include("User")
						where ack.Result.Uuid == resultGuid
						select ack;
			if (query.Count() == 1)
				return query.First();
			return null;
		}

		public NotificationEntity[] GetResultNotifications(Guid resultGuid)
		{
			var query = from note in _ObjectContext.NotificationEntitySet.Include("UserTransport").Include("UserTransport.Transport")
						where note.Result.Uuid == resultGuid
						select note;
			return query.ToArray();
		}

		/// <summary>
		/// Modified: 
		///		2009-07-27, Jeremy Richardson
		///			-Added check to assign a uuid when the uuid is null.
		/// </summary>
		/// <param name="notificationEntity"></param>
		/// <returns></returns>
		public NotificationEntity CreateResultNotification(NotificationEntity notificationEntity)
		{
			if (notificationEntity.Uuid == Guid.Empty)
			{
				notificationEntity.Uuid = Guid.NewGuid();
			}
			_ObjectContext.AddToNotificationEntitySet(notificationEntity);
			_ObjectContext.SaveChanges();
			
			return notificationEntity;
		}

		public NotificationEntity CreateResultNotification(string userName, string transportName, Guid resultGuid, string notes, string state)
		{
			NotificationEntity entity = new NotificationEntity();

			entity.Uuid = Guid.NewGuid();
			entity.UserTransport = GetUserTransport(userName, transportName);
			entity.CreationTime = DateTime.Now;
			entity.Notes = notes;
			entity.Result = GetResultEntity(resultGuid);
			entity.State = state;

			_ObjectContext.SaveChanges();
			return entity;
		}


		public AcknowledgmentEntity CreateAcknowledgment(Guid resultGuid, string userName, string notes)
		{
			AcknowledgmentEntity e = new AcknowledgmentEntity();
			ResultEntity r = GetResultEntity(resultGuid);
			UserEntity u = GetUser(userName);
			e.Result = r;
			e.User = u;
			e.Notes = notes;
			e.CreationTime = DateTime.Now;

            r.AcknowledgmentTime = e.CreationTime;

			_ObjectContext.SaveChanges();

			return e;
		}

		/// <summary>
		/// modified: 2009-11-23, Jeremy R
		///  -Added creation of new Guid
		/// </summary>
		/// <param name="user"></param>
		/// <param name="ipv4"></param>
		/// <returns></returns>
		public TokenEntity CreateToken(UserEntity user, string ipv4)
		{
			TokenEntity e = new TokenEntity();
			e.Token = Guid.NewGuid();
			e.User = user;
			e.Ipv4 = ipv4;
			e.CreatedTime = DateTime.Now;
			e.UpdatedTime = DateTime.Now;
			_ObjectContext.AddToTokenEntitySet(e);
			_ObjectContext.SaveChanges();
			return e;
		}

		public TokenEntity [] GetTokensForUser(string userName)
		{
			//_ObjectContext.Refresh(RefreshMode.StoreWins, _ObjectContext.TokenEntitySet);
            var query = from tokens in _ObjectContext.TokenEntitySet.Include("User").Include("User.Roles")
						where tokens.User.UserName == userName
						select tokens;
			return query.ToArray();
		}

		/// <summary>
		/// Created: 2009-10-07, John Morgan
		/// Added Create and Update function for UpdateUser client functionality
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="type"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="xmlValue"></param>
		/// <param name="restricted"></param>
		/// <returns></returns>
		public UserEntryEntity CreateUpdateUserEntryEntity(string userName, string type, string key, string value, string xmlValue, bool restricted)
		{
			UserEntity user = GetUser(userName);
			if (!user.UserEntries.IsLoaded)
			{
				user.UserEntries.Load();
			}
			UserEntryEntity[] userEntries = (from entries in user.UserEntries
											 where entries.Type == type && entries.Key == key
											 select entries).ToArray();
			if (userEntries.Count() == 0)
			{
				UserEntryEntity newEntry = new UserEntryEntity();
				newEntry.Key = key;
				newEntry.Type = type;
				newEntry.Value = value;
				newEntry.XmlValue = xmlValue;
				newEntry.User = user;
				newEntry.RestrictedAccess = restricted;
				_ObjectContext.SaveChanges();
				return newEntry;
			}
			else
			{
				userEntries[0].Value = value;
				userEntries[0].XmlValue = value;
				_ObjectContext.SaveChanges();
				return userEntries[0];
			}
		}

		public UserEntryEntity CreateUserEntryEntity(string userName, string type, string key, string value, string xmlValue, bool restricted)
		{
			UserEntity user = GetUser(userName);

			if (!user.UserEntries.IsLoaded)
			{
				user.UserEntries.Load();
			}

			bool addEntry = true;
			UserEntryEntity e = null;
			foreach (UserEntryEntity ue in user.UserEntries)
			{
				if (ue.Key == key && ue.Type == type)
				{
					addEntry = false;
					e = ue;
				}
			}

			if (addEntry)
			{
				e = new UserEntryEntity();
				e.Type = type;
				e.Key = key;
				e.Value = value;
				e.XmlValue = xmlValue;
				e.User = user;
				e.RestrictedAccess = restricted;

				_ObjectContext.SaveChanges();
			}
			return e;
		}

		public bool DeleteUserEntryEntity(string userName, string type, string key)
		{
			UserEntity user = GetUser(userName);
			UserEntryEntity toDelete = null;
			foreach (UserEntryEntity e in user.UserEntries)
			{
				if (e.Type == type && e.Key == key)
				{
					toDelete = e;
				}
			}
			if (toDelete != null)
			{
				_ObjectContext.DeleteObject(toDelete);
				_ObjectContext.SaveChanges();
				return true;
			}
			return false;
		}

		public UserEntryEntity[] QueryUserEntryEntities(string queryString, int? pageSize, int? pageNumber)
		{
			int skipRecords = 0;
			int limitRecords = int.MaxValue;
			if (pageSize != null)
				limitRecords = pageSize.Value;
			if (pageNumber != null && pageSize != null)
				skipRecords = (pageNumber.Value - 1) * pageSize.Value;

			if (string.IsNullOrEmpty(queryString))
				queryString = "1 == 1";

			ObjectQuery<UserEntryEntity> query = _ObjectContext.UserEntryEntitySet.Where(queryString).Include("User");
			return query.OrderBy("it.Id").Skip(skipRecords).Take(limitRecords).ToArray();
		}

		/// <summary>
		/// Modified: 2009-08-12, John Morgan: modified to create UserEntryEntity if it does not exist.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="type"></param>
		/// <param name="entryKey"></param>
		/// <param name="newValue"></param>
		/// <returns></returns>
		public bool UpdateUserEntryEntity(string userName, string type, string entryKey, string newValue, bool restricted)
		{
			var entities = from entries in _ObjectContext.UserEntryEntitySet.Include("User")
						   where entries.User.UserName == userName && entries.Type == type && entries.Key == entryKey
						   select entries;

			if (entities.Count() == 0)
			{
				UserEntryEntity uee = CreateUserEntryEntity(userName, type, entryKey, newValue, null, restricted);
				if (uee == null)
					return false;
				else
					return true;
			}
			else
			{
				entities.First().Value = newValue;
				_ObjectContext.SaveChanges();
				return true;
			}

		}

		//Modified: 2009-29-09 John Morgan
		//      Changed method to remove levels prior to adding.
		public UserTransportEntity UpdateLevelsToUserTransport(string userName, string transportName, string address, string[] levelNames)
		{
			UserTransportEntity userTransport =  GetUserTransport(userName, transportName, address);
			if (userTransport != null)
			{
				List<LevelEntity> cLevels = new List<LevelEntity>();
				cLevels.AddRange(userTransport.Levels);
				foreach (LevelEntity l in cLevels)
				{
					userTransport.Levels.Remove(l);
				}
				_ObjectContext.SaveChanges();
			}
			List<LevelEntity> levels = new List<LevelEntity>();
			levels.AddRange(userTransport.Levels);
			foreach (string levelName in levelNames)
			{
					LevelEntity level = GetLevel(levelName);
					userTransport.Levels.Add(level);
			}
			_ObjectContext.SaveChanges();
			return userTransport;
		}

		/// <summary>
		///
		/// Created: 2009-07-18, Jeremy Richardson
		///		Primary use is by the Notify Agent to find outstanding notifications.
		/// Modified: 
		/// </summary>
		/// <param name="queryString"></param>
		/// <param name="pageSize"></param>
		/// <param name="pageNumber"></param>
		/// <returns></returns>
		public NotificationEntity[] QueryNotificationEntity(string queryString, int? pageSize, int? pageNumber)
		{
			int skipRecords = 0;
			int limitRecords = int.MaxValue;
			if (pageSize != null)
				limitRecords = pageSize.Value;
			if (pageNumber != null && pageSize != null)
				skipRecords = (pageNumber.Value - 1) * pageSize.Value;

			if (string.IsNullOrEmpty(queryString))
				queryString = "1 == 1";

			ObjectQuery<NotificationEntity> query = _ObjectContext.NotificationEntitySet.Where(queryString).Include("Result").Include("Result.Sender").Include("Result.Receiver").Include("UserTransport").Include("UserTransport.User").Include("UserTransport.Transport");
			return query.OrderBy("it.CreationTime").Skip(skipRecords).Take(limitRecords).ToArray();
		}

		/// <summary>
		/// Created: 2009-07-18, Jeremy Richardson
		/// Modified: 
		/// </summary>
		/// <param name="notificationUuid"></param>
		/// <returns></returns>
		public NotificationEntity GetNotificationEntity(Guid notificationUuid)
		{
			var query = from notifications in _ObjectContext.NotificationEntitySet
						where notifications.Uuid == notificationUuid
						select notifications;
			if (query.Count() == 1)
				return query.First();
			return null;
		}

		/// <summary>
		///	Created: 2009-07-18, Jeremy Richardson
		/// Modified: 
		/// </summary>
		/// <param name="notificationUid"></param>
		/// <param name="state"></param>
		public NotificationEntity UpdateNotificationEntity(Guid notificationUuid, string state, string notes)
		{
			NotificationEntity notification = GetNotificationEntity(notificationUuid);

			if( !string.IsNullOrEmpty(state))
			{
				notification.State = state;
			}
			if( !string.IsNullOrEmpty(notes))
			{
				notification.Notes = notes;
			}
			_ObjectContext.SaveChanges();
			return notification;
		}

        /// <summary>
        /// Created: 2009-10-01, John Morgan
        /// Modified:
        /// </summary>
        /// <returns></returns>
        public SettingEntity[] GetAllSettings()
        {
            var query = from settings in _ObjectContext.SettingEntitySet
                        select settings;

            return query.ToArray();
        }

		/// <summary>
		/// Created: 2009-07-29, John Morgan
		/// Modified:
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="entryKey"></param>
		/// <returns></returns>
		public SettingEntity[] GetSettings(string owner)
		{
			var query = from settings in _ObjectContext.SettingEntitySet
						where settings.Owner == owner
						select settings;
			
			return query.ToArray();
		}

		/// <summary>
		/// Created: 2009-07-29, John Morgan
		/// Modified:
		/// </summary>
		/// <param name="uuid"></param>
		/// <returns></returns>
		public SettingEntity GetSetting(Guid uuid)
		{
			var query = from settings in _ObjectContext.SettingEntitySet
						where settings.Uuid == uuid
						select settings;
			if (query.Count() > 0)
			{
				return query.First(); 
			}
			return null;
		}

		/// <summary>
		/// Created: 2009-07-29, John Morgan
		/// Modified:
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="entryKey"></param>
		/// <param name="value"></param>
		/// <param name="xmlValue"></param>
		/// <returns></returns>
		public SettingEntity CreateSetting(string owner, string entryKey, string value, string xmlValue)
		{
			SettingEntity e = new SettingEntity();
			e.Uuid = Guid.NewGuid();
			e.Owner = owner;
			e.EntryKey = entryKey;
			e.Value = value;
			e.XmlValue = xmlValue;

			_ObjectContext.SaveChanges();

			return GetSetting(e.Uuid);
		}

		/// <summary>
		/// Created: 2009-10-01, John Morgan
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="uuid"></param>
		/// <param name="entryKey"></param>
		/// <param name="value"></param>
		/// <param name="xmlValue"></param>
		/// <returns></returns>
		public SettingEntity UpdateSetting(string owner, Guid uuid, string entryKey, string value, string xmlValue)
		{
			SettingEntity e = GetSetting(uuid);
			if (e != null)
			{
				if(!String.IsNullOrEmpty(owner))
					e.Owner = owner;
				if(!String.IsNullOrEmpty(entryKey))
					e.EntryKey = entryKey;
				if(!String.IsNullOrEmpty(value))
					e.Value = value;
				if(!String.IsNullOrEmpty(xmlValue))
					e.XmlValue = xmlValue;

				_ObjectContext.SaveChanges();
			}
			return e;
		}

        public RatingEntity CreateRating(Guid uuid, string username, int rating, string comments)
        {
            ResultEntity result = GetResultEntity(uuid);
            if (result == null)
                return null;
            UserEntity user = GetUser(username);
            if (user == null)
                return null;
            RatingEntity entity = new RatingEntity();
            entity.comments = comments;
            entity.value = rating;
            entity.User = user;
            entity.Result = result;
            _ObjectContext.SaveChanges();
            return entity;
        }

		public UserEntity GetUserByAuthExt(string key, string value)
		{
			UserEntryEntity authExt = (from u in _ObjectContext.UserEntryEntitySet.Include("User")
										  where u.Type == "AuthExt" && u.Key == key && u.Value == value
										  select u).FirstOrDefault();
			if (authExt == null)
				return null;
			else
				return authExt.User;
		}        
	}
}
