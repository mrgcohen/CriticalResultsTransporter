using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;

namespace CriticalResults
{
    class WcfHelper
    {
    
        internal static string GetIPv4FromWCF()
        {
            return GetIPv4FromWCF(System.ServiceModel.OperationContext.Current);
        }

        internal static string GetIPv4FromWCF(System.ServiceModel.OperationContext operationContext)
        {
            if (operationContext != null)
            {
                System.ServiceModel.Channels.RemoteEndpointMessageProperty endpoint = operationContext.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                string hostName = endpoint.ToString();
                string ipAddress = endpoint.Address;

                //HACK
                int ipLen = 15;
                if (ipAddress.Length < ipLen)
                    ipLen = ipAddress.Length;

                return ipAddress.Substring(0, ipLen);
            }
            else
            {
                return "";
            }
        }
    }
}
