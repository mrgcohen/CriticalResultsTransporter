using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

[assembly: ContractNamespace("http://partners.org/brigham/criticalresults/", ClrNamespace="CriticalResults")]
namespace CriticalResults
{
	// NOTE: If you change the interface name "IResultsService" here, you must also update the reference to "IResultsService" in App.config.
	[ServiceContract]
    [ServiceKnownType(typeof(SecurityExceptionObject))]
	public interface IResultsService
	{
		[OperationContract]
		string Echo(string message);

		[OperationContract]
		string DoubleEcho(string message, string secondMessage);

		[OperationContract]
		string EchoPost(string message);
				
		/// <summary>
		/// 
		/// </summary>
		/// <param name="uuid"></param>
		[OperationContract]
		Result GetResult(string uuid);
		[OperationContract]
		Result GetResult_Json(string uuid);

		[OperationContract]
		void AuditEvent(string eventName, string eventDescription);

		/// <summary>
		/// Should return
		///		ResultEntity w/
		///			Level
		///			ResultContext
		///			Acknowledgment
		///			Sender
		///			Receiver
		///			Notification		
		/// </summary>
		/// <param name="queryString"></param>
		/// <param name="pageSize"></param>
		/// <param name="pageNumber"></param>
		[OperationContract]
		Result [] QueryResults(string queryString, int? pageSize, int? pageNumber);
		[OperationContract]
		Result[] QueryResults_Json(string queryString, int? pageSize, int? pageNumber);

		/// <summary>
		/// Should expect
		///		ResultEntity w/
		///			Level
		///			ResultContext
		///			Sender
		///			Receiver
		///			Notification (for manual notification)
		/// </summary>
		/// <param name="queryString"></param>
		/// <param name="pageSize"></param>
		/// <param name="pageNumber"></param>
		[OperationContract]
		Result CreateResult(Result result);
		[OperationContract]
		Result CreateResult_Json(Result result);

		[OperationContract]
		Result UpdateResultReceiver(string uuid, string userName);
		[OperationContract] 
		Result UpdateResultReceiver_Json(string uuid, string userName);

		[OperationContract]
		ResultViewEntry[] QueryResultList(string queryString, int? pageSize, int? pageNumber);
		[OperationContract]
		ResultViewEntry[] QueryResultList_Json(string queryString, int? pageSize, int? pageNumber);

		[OperationContract]
		bool DeleteUserEntry(string userName, string type, string key);
		[OperationContract]
		bool DeleteUserEntry_Json(string userName, string type, string key);
		
		[OperationContract]
		Level[] GetAllLevels();
		[OperationContract]
		Level[] GetAllLevels_Json();
        [OperationContract]
        Level CreateLevel(Level level);
        [OperationContract]
        Level CreateLevel_Json(Level level);
        [OperationContract]
        Level UpdateLevel(Level level);
        [OperationContract]
        Level UpdateLevel_Json(Level level);

		[OperationContract]
		User CreateUser(User user);
		[OperationContract]
		User CreateUser_Json(User user);
		[OperationContract]
		User UpdateUser(User user);
		[OperationContract]
		User UpdateUser_Json(User user);
		[OperationContract]
		bool UpdateUserRoles(string userName, string[] roles);
		[OperationContract]
		bool UpdateUserRoles_Json(string userName, string[] roles);
		[OperationContract]
		bool AddUserToRole(string userName, string roleName);
		[OperationContract]
		bool AddUserToRole_Json(string userName, string roleName);
		[OperationContract]
		User [] GetAllUsers();
		[OperationContract]
		User [] GetAllUsers_Json();
		[OperationContract]
		User GetUser(string userName);
		[OperationContract]
		User GetUser_Json(string userName);
		[OperationContract]
		User[] QueryUsers(string queryString, int? pageSize, int? pageNumber);
		[OperationContract]
		//User[] QueryUsers_Json(string queryString, int? pageSize, int? pageNumber);
		User[] QueryUsers_Json(string queryString, int? pageSize, int? pageNumber);

		[OperationContract]
		ContextType [] GetAllContextTypes();
		[OperationContract]
		ContextType [] GetAllContextTypes_Json();

		[OperationContract]
		Acknowledgment GetResultAcknowledgment(string resultUuid);
		[OperationContract]
		Acknowledgment GetResultAcknowledgment_Json(string resultUuid);
		
		[OperationContract]
		Acknowledgment CreateResultAcknowledgment(string resultUuid, string userName, string notes);
		[OperationContract]
		Acknowledgment CreateResultAcknowledgment_Json(string resultUuid, string userName, string notes);

		[OperationContract]
		Notification[] GetResultNotifications(string resultUuid);
		[OperationContract]
		Notification[] GetResultNotifications_Json(string resultUuid);
		
		[OperationContract]
		Notification[] QueryNotifications(string queryString, int? pageSize, int? pageNumber);
		[OperationContract]
		Notification[] GetPendingNotifications();
		[OperationContract]
		Notification[] GetNotifications(string notificationStateName, string startDateRangeString, string endDateRangeString);
        //[OperationContract]
        //Notification[] GetLastDaysNotifications();
		[OperationContract]
		Notification UpdateNotification(string notificationUuid, string state, string notes);
		
		[OperationContract]
		Notification CreateResultNotification(Notification notification, string selectedTransport);
		[OperationContract]
		Notification CreateResultNotification_Json(Notification notification, string selectedTransport);

        [OperationContract]
        Transport[] GetAllTransports();
        [OperationContract]
        Transport[] GetAllTransports_Json();
        [OperationContract]
		Transport CreateTransport(string name, string transportUri, string friendlyName);
        [OperationContract]
		Transport CreateTransport_Json(string name, string transportUri, string friendlyName);
        [OperationContract]
        bool DeleteTransport(string name, string transportUri);
        [OperationContract]
        bool DeleteTransport_Json(string name, string transportUri);
        [OperationContract]
		Transport UpdateTransport(string origName, string origUri, string newName, string newUri, string friendlyName);
        [OperationContract]
		Transport UpdateTransport_Json(string origName, string origUri, string newName, string newUri, string friendlyName);
        [OperationContract]
        Transport[] GetTransportsForLevel(string levelUuid);
        [OperationContract]
        Transport[] GetTransportsForLevel_Json(string levelUuid);
        [OperationContract]
		bool CreateTransportLevel(string levelUuid, string transportName, string mandatoryString);
        [OperationContract]
		bool CreateTransportLevel_Json(string levelUuid, string transportName, string mandatoryString);
        [OperationContract]
        bool RemoveTransportFromLevel(string levelUuid, string transportName);
        [OperationContract]
        bool RemoveTransportFromLevel_Json(string levelUuid, string transportName);

		[OperationContract]
		Guid? GetToken(string userName, string passwordHash);
		[OperationContract]
		Guid? GetToken_Json(string userName, string passwordHash);

		[OperationContract]
		User CreateUserEntry(string userName, string type, string key, string value, string xmlValue);
		[OperationContract]
		User CreateUserEntry_Json(string userName, string type, string key, string value, string xmlValue);

		[OperationContract]
		UserEntry CreateUpdateUserEntry(string userName, string type, string key, string value, string xmlValue);
		[OperationContract]
		UserEntry CreateUpdateUserEntry_Json(string userName, string type, string key, string value, string xmlValue);

		[OperationContract]
		UserEntry[] QueryUserEntries(string queryString, int? pageSize, int? pageNumber );
		[OperationContract]
		UserEntry[] QueryUserEntries_Json(string queryString, int? pageSize, int? pageNumber);

		[OperationContract]
		Role[] GetAllRoles();
		[OperationContract]
		Role[] GetAllRoles_Json();

		[OperationContract]
		UserTransport[] GetUserTransports(string userName);
		[OperationContract]
		UserTransport[] GetUserTransports_Json(string userName);

		[OperationContract]
		UserTransport CreateUserTransport(string userName, string transportName, string address);
		[OperationContract]
		UserTransport CreateUserTransport_Json(string userName, string transportName, string address);
		[OperationContract]
		UserTransport UpdateUserTransport(string userName, string transportName, string originalAddress, string address);
		[OperationContract]
		UserTransport UpdateUserTransport_Json(string userName, string transportName, string originalAddress, string address);
		[OperationContract]
		UserTransport AddLevelsToUserTransport(string userName, string transportName, string address, string[] levelNames);
		[OperationContract]
		UserTransport AddLevelsToUserTransport_Json(string userName, string transportName, string address, string[] levelNames);

		[OperationContract]
		ProxyEntity CreateProxy(string masterUserName, string proxyUserName, string relationshipDescription);
		[OperationContract]
		ProxyEntity CreateProxy_Json(string masterUserName, string proxyUserName, string relationshipDescription);
		[OperationContract]
		void DeleteProxy(string masterUserName, string proxyUserName);
		[OperationContract]
		void DeleteProxy_Json(string masterUserName, string proxyUserName);
		[OperationContract]
		User[] GetUsersForProxy(string proxyUserName);
		[OperationContract]
		User [] GetUsersForProxy_Json(string proxyUserName);

		/// <summary>
		/// Created: 2009-07-29, John Morgan
		/// Modified:
		/// </summary>
        [OperationContract]
        Setting[] GetAllSettings();
        [OperationContract]
        Setting[] GetAllSettings_Json();
		[OperationContract]
		Setting[] GetSettings(string owner);
		[OperationContract]
		Setting[] GetSettings_Json(string owner);

		/// <summary>
		/// Created: 2009-10-01, John Morgan
		/// </summary>
		/// <param name="uuid"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		[OperationContract]
		Setting UpdateSetting(string owner, string uuid, string value);
		[OperationContract]
		Setting UpdateSetting_Json(string owner, string uuid, string value);

		/// <summary>
		/// Created: 2009-08-31, John Morgan
		/// Modified:
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="oldPassword"></param>
		/// <param name="newPassword"></param>
		/// <returns></returns>
		[OperationContract]
		bool SetPassword(string userName, string currentPassword, string newPassword);
		[OperationContract]
		bool SetPassword_Json(string userName, string currentPassword, string newPassword);

		[OperationContract]
		bool GeneratePassword(string userName);
		[OperationContract]
		bool GeneratePassword_Json(string userName);

        [OperationContract]
        bool LeaveFeedback(string resultUuid, string userName, int rating, string comments);
        [OperationContract]
        bool LeaveFeedback_Json(string resultUuid, string userName, int rating, string comments);

		[OperationContract]
		bool SendAccountConfirmation(string username);
		[OperationContract]
		bool SendAccountConfirmation_Json(string username);
		[OperationContract]
		User CheckAuthExtUsage(string AuthExtKey, string AuthExtValue);
    }
}
