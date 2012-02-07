using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CriticalResults
{
    // you should derive from that class to extend functionality
    public class DefaultExtension
    {
        public virtual string GetUserLink()
        {
            return "";
        }

        public virtual void ExtendUserFromExternalDirectory(User user, Transport[] transports, Level[] levels, string email, string[] proxyAddresses)
        {
        }
    }
}
