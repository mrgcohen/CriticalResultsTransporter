using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace CriticalResults
{
	[ServiceContract]
	public interface ISystemMessageSender
	{
		[OperationContract]
		bool SendSystemMessage_Json(string address, string value);
		[OperationContract]
		bool SendSystemMessage(string address, string value);
	}
}
