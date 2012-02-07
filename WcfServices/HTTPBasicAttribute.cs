using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Security;
using System.ServiceModel.Web;
using System.Runtime.Serialization;

namespace CriticalResults
{
    [DataContract]
    public class SecurityExceptionObject
    {
        public SecurityExceptionObject(string e)
        {
            exception = e;
        }
        [DataMember]
        string exception {get; set;}
    }
    class HTTPBasicChecker : IOperationInvoker
    {
        IOperationInvoker parent;

        bool RefreshToken { get; set; }
        bool Json { get; set; }
        HTTPCheckRoles Roles { get; set; }

        public HTTPBasicChecker(IOperationInvoker parent, bool refreshToken, bool json, HTTPCheckRoles Roles)
        {
            this.Json = json;
            this.RefreshToken = refreshToken;
            this.parent = parent;
            this.Roles = Roles;
        }

        #region IOperationInvoker Members

        public object[] AllocateInputs()
        {
            return parent.AllocateInputs();
        }

        public bool Authenticate()
        {
            HTTPBasicAuthenticationHeader header = HTTPBasicAuthenticationHeader.GetFromWCF();

            string ipAddress = WcfHelper.GetIPv4FromWCF();

            HTTPCheckRoles role;
            if (!CheckTokenUtil.CheckToken(header.UserName, header.Password, ipAddress, header.Method, RefreshToken, out role))
            {
                return false;
            }
            if (Roles != 0 && (Roles & role) == 0)
            {
                return false;
            }
            return true;
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            if (!Authenticate())
            {
                if (Json)
                {
                    outputs = new object[] {};
                    return new SecurityExceptionObject("SecurityException");
                }
                else
                    throw new SecurityException();
            }
            return parent.Invoke(instance, inputs, out outputs);
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            return parent.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            return parent.InvokeEnd(instance, out outputs, result);
        }

        public bool IsSynchronous
        {
            get { return parent.IsSynchronous; }
        }

        #endregion
    }

    public class HTTPBasicCheckAttribute : Attribute, IOperationBehavior
    {
        public HTTPBasicCheckAttribute()
        {
            RefreshToken = true;
            Roles = 0;
        }
        public bool RefreshToken {get; set;}
        public HTTPCheckRoles Roles { get; set; }

        #region IOperationBehavior Members

        public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.ClientOperation clientOperation)
        {
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.DispatchOperation dispatchOperation)
        {
            bool json = false;
            foreach (IOperationBehavior b in operationDescription.Behaviors)
            {
                if (b is WebInvokeAttribute)
                {
                    if (((WebInvokeAttribute)b).ResponseFormat == WebMessageFormat.Json)
                    {
                        json = true;
                    }
                }
            }
            dispatchOperation.Invoker = new HTTPBasicChecker(dispatchOperation.Invoker, RefreshToken, json, Roles);
        }

        public void Validate(OperationDescription operationDescription)
        {
        }

        #endregion

        internal static void AdditionalCheck(string userName, HTTPCheckRoles rolesAllowed)
        {
            HTTPBasicAuthenticationHeader header = HTTPBasicAuthenticationHeader.GetFromWCF();
            if (string.Compare(header.UserName, userName, true) != 0)
            {
                UserEntity e = new CriticalResultsEntityManager().GetUser(userName);
                User u = new User(e);
                u.ResolveRoles();
                bool ok = false;
                foreach (Role r in u.Roles)
                {
                    object robj = Enum.Parse(typeof(HTTPCheckRoles), r.Name, true);
                    if (robj != null)
                    {
                        if (((HTTPCheckRoles)robj | rolesAllowed)!=0)
                        {
                            ok=true;
                            break;
                        }
                    }
                }
                if (!ok)
                {
                    throw new SecurityException();
                }
            }
        }
    }

    [Flags]
    public enum HTTPCheckRoles
    {
        sender = 0x01,
        receiver = 0x02,
        sysAdmin = 0x04,
        clinAdmin = 0x08
    }
}
