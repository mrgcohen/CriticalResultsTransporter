using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CriticalResults
{
	// NOTE: If you change the interface name "ITransportService" here, you must also update the reference to "ITransportService" in App.config.
	[ServiceContract]
	public interface ITransportService
	{
		[OperationContract]
		bool RequestNotification(string notificationUid, string address, string value);
	}
}
