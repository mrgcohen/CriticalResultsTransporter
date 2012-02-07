using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CriticalResults
{
	[ServiceContract]
    [ServiceKnownType(typeof(SecurityExceptionObject))]
    public interface IDirectoryService
	{
		[OperationContract]
		ActiveDirectoryUser [] LookupUser(string lastName, string firstName);
		[OperationContract]
		ActiveDirectoryUser[] GetUserByEmail(string emailAddress);
        [OperationContract]
        ActiveDirectoryUser[] GetUserByCanonicalName(string cn);
        [OperationContract]
        ActiveDirectoryUser[] GetCreatableUsers(string name);
        [OperationContract]
        User CreateUser(ActiveDirectoryUser adUser);
    }
}
