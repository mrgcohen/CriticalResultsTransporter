using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using CriticalResults.Properties;
using System.IO;
using System.ServiceModel.Activation;
using System.Diagnostics;

namespace CriticalResults
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class DirectoryService : IDirectoryService
	{
        private TraceSource _Tracer;		
		private DirectoryProxy _Service;
		
		public DirectoryService()
		{
			string traceName = this.GetType().FullName;
			Trace.WriteLine(string.Format("Registering Trace Source: {0}.  Please add a Trace Listener in the System.Diagnostics setting of your configuration", traceName));
			_Tracer = new TraceSource(traceName);
			
			_Service = new DirectoryProxy();
		}

		class SearchArgumentException : Exception
		{ 
			public SearchArgumentException() : base("Search arguments may not be empty or null."){}
		}
		
		/// <summary>
		/// Wildcards are appended to search arguments.
		/// </summary>
		/// <param name="lastName">required, partial match string - a multi-char (*) wildcard will be appended</param>
		/// <param name="firstName">required, partial match string - a multi-char (*) wildcard will be appended</param>
		[WebGet( UriTemplate="User/Name/{lastName}/{firstName}", ResponseFormat=WebMessageFormat.Json)]
        [HTTPBasicCheck]
		public ActiveDirectoryUser [] LookupUser(string lastName, string firstName)
		{
			if (string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(firstName))
			{
				throw new SearchArgumentException();
			}
			
			if (firstName.ToLower() == "user" && lastName.ToLower() == "test")
			{
				return GetTestUsers().ToArray();
			}
			string searchString = "";
			if (firstName != "null")
			{
				searchString = string.Format("(&(sn={0}*)(givenName={1}*))", lastName, firstName);
			}
			else
			{
				searchString = string.Format("(&(sn={0}*))", lastName);
			}
			ActiveDirectoryUser [] users = _Service.Search(searchString, DirectoryProxy.ReturnType.Full);
			return users;
		}
		[WebGet(UriTemplate = "User/Mail/{emailAddress}", ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck]
        public ActiveDirectoryUser[] GetUserByEmail(string emailAddress)
		{
			if (emailAddress.ToLower() == "test.user@example.com")
			{
				return GetTestUsers();
			}

			string searchString = string.Format("mail={0}", emailAddress);
			ActiveDirectoryUser[] users = _Service.Search(searchString, DirectoryProxy.ReturnType.Full);
			return users;
		}

        [WebGet(UriTemplate = "User/CN/{cn}", ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck]
        public ActiveDirectoryUser[] GetUserByCanonicalName(string cn)
		{
			string searchString = string.Format("cn={0}", cn);
            ActiveDirectoryUser[] users = _Service.Search(searchString, DirectoryProxy.ReturnType.Min);
			return users;
		}

        public ActiveDirectoryUser[] GetUserByCanonicalNameInternal(string cn)
        {
            string searchString = string.Format("cn={0}", cn);
            ActiveDirectoryUser[] users = _Service.Search(searchString, DirectoryProxy.ReturnType.Full);
            return users;
        }

        [WebInvoke(Method = "POST", UriTemplate = "Person/Creatable/{name}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck]
        public ActiveDirectoryUser[] GetCreatableUsers(string name)
        {
            string[] names = name.Split(new char[] { ',' });

            ActiveDirectoryUser[] adUsers = null;
            if (name.Length > 1)
                adUsers = _Service.SearchActiveDirectory(names[0], names[1]);
            else if (name.Length == 1)
                adUsers = _Service.SearchActiveDirectory(names[0], "");

            ResultsService manager = new ResultsService();

            Setting[] settings = manager.GetSettings_NonWeb("System");
            string defaultAccountDomain = "";
            foreach (Setting setting in settings)
            {
                if (setting.EntryKey == "DefaultAccountDomain")
                    defaultAccountDomain = setting.Value;
            }
            foreach (ActiveDirectoryUser adUser in adUsers)
            {
                User ue = manager.GetUserByAuthExt("Windows", defaultAccountDomain + "\\" + adUser.cn);
                if (ue != null)
                {
                    adUser.ANCRAccountUserName = ue.UserName;
                    adUser.Enabled = ue.Enabled;
                }
                if (string.IsNullOrEmpty(adUser.ANCRAccountUserName))
                {
                    if (adUser.mail != "")
                    {
                        User uebm = manager.GetUser_NonWeb(adUser.mail);
                        if (uebm != null)
                            adUser.ANCRAccountUserName = uebm.UserName;
                    }
                }
                if (string.IsNullOrEmpty(adUser.ANCRAccountUserName))
                {
                    foreach (string mail in adUser.proxyAddresses)
                    {
                        User uebm = manager.GetUser_NonWeb(mail);
                        if (uebm != null)
                        {
                            adUser.ANCRAccountUserName = uebm.UserName;
                            break;
                        }
                    }
                }
            }

            return adUsers;
        }


        [WebInvoke(Method = "POST", UriTemplate = "User", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [HTTPBasicCheck]
        public User CreateUser(ActiveDirectoryUser adUser)
        {
            ActiveDirectoryUser[] adUsers = _Service.SearchActiveDirectory(adUser.cn);
            if (adUsers.Length > 1)
            {
                throw new UserCreationException("Multiple user accounts found in Active Directory for " + adUser.cn + ".  Cannot continue.");
            }
            else if (adUsers.Length == 0)
            {
                throw new UserCreationException("No user account found in Active Directory for " + adUser.cn + ".  Cannot continue.");
            }

            ResultsService manager = new ResultsService();
            Level[] levels = manager.GetLevels();
            Transport[] transports = manager.GetTransports();
            Transport smtp = null;
            foreach (Transport transport in transports)
            {
                if (transport.Name == Properties.Settings.Default.SmtpTransportName)
                    smtp = transport;
            }

            User user = new User();
            user.UserName = adUser.mail;
            user.MiddleName = "";

            user.LastName = adUser.sn ?? "";
            user.FirstName = adUser.givenName ?? "";

            user.Enabled = true;

            List<UserTransport> userTransports = new List<UserTransport>();
            if (smtp != null)
            {
                UserTransport userMail = new UserTransport();
                userMail.Address = adUser.mail ?? "";
                userMail.Levels = levels;
                userMail.Transport = smtp;
                userTransports.Add(userMail);
            }

            user.Transports = userTransports.ToArray();
            Extension.DefaultExtension.ExtendUserFromExternalDirectory(user, transports, levels, adUser.mail, adUser.proxyAddresses);

            user = manager.CreateANCRUser(user);

            manager.AssignUserToRole(user.UserName, "Receiver");
            //manager.AccountConfirmation(user.UserName);
            //manager.CreatePassword(user.UserName);

            Setting[] settings = manager.GetSettings_NonWeb("System");
            string defaultAccountDomain = "";
            foreach (Setting setting in settings)
            {
                if (setting.EntryKey == "DefaultAccountDomain")
                    defaultAccountDomain = setting.Value;
            }

            manager.UserEntry(user.UserName, "AuthExt", "Windows", defaultAccountDomain + "\\" + adUser.cn, "");
            return user;

        }

		public static ActiveDirectoryUser [] GetTestUsers()
		{
			List<ActiveDirectoryUser> testUsers = new List<ActiveDirectoryUser>();
			ActiveDirectoryUser u = new ActiveDirectoryUser();
			u.AddProperty("cn", "tu001");
			u.AddProperty("mail", "test.user@example.com");
			u.AddProperty("sn", "Test");
			u.AddProperty("givenName", "User,MD");
			testUsers.Add(u);
			return testUsers.ToArray();
		}

	}

    public class UserCreationException : Exception
    {
        public string Details { get; set; }
        public UserCreationException() { }
        public UserCreationException(string details) { Details = details; }
    }
}
