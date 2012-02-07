using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CriticalResults.Properties;
using System.DirectoryServices;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;


namespace CriticalResults
{
	/// <summary>
	/// Directory Proxy allows performing a search against AD.
	/// </summary>
	public class DirectoryProxy
	{
        class SearchArgumentException : Exception
        {
            public SearchArgumentException() : base("Search arguments may not be empty or null.") { }
        }

        private TraceSource _Trace;

		private System.DirectoryServices.DirectoryEntry _Entry;
		private System.DirectoryServices.DirectorySearcher _Searcher;

		public enum ReturnType
		{
			Full,
			Min
		}

		public DirectoryProxy()
		{
			string traceName = this.GetType().ToString();
			Trace.WriteLine(string.Format("Configuring Trace Source {0}.  Please configure a TraceListener in your configuration's System.Diagnostics section.", traceName));
			_Trace = new TraceSource(traceName);

            _Entry = new System.DirectoryServices.DirectoryEntry(
                CriticalResults.Properties.Settings.Default.LDAPPath,
                CriticalResults.Properties.Settings.Default.LDAPUsername,
                CriticalResults.Properties.Settings.Default.LDAPPassword);
			_Searcher = new System.DirectoryServices.DirectorySearcher(_Entry);
		}

		public ActiveDirectoryUser[] Search(string filterString, ReturnType type)
		{
			_Searcher.Filter = filterString;
			SearchResultCollection results = _Searcher.FindAll();
			if (type == ReturnType.Min)
				return ParseSubset(results);
			else if (type == ReturnType.Full)
				return ParseFull(results);

			return null;
		}

		private ActiveDirectoryUser[] ParseSubset(SearchResultCollection results)
		{
			List<ActiveDirectoryUser> userList = new List<ActiveDirectoryUser>();

			foreach (SearchResult result in results)
			{
				ActiveDirectoryUser u = new ActiveDirectoryUser();
				userList.Add(u);

				DirectoryEntry innerEntry = new DirectoryEntry(result.Path,
                    CriticalResults.Properties.Settings.Default.LDAPUsername,
                    CriticalResults.Properties.Settings.Default.LDAPPassword);
				
				List<string> proxyAddresses = new List<string>();

				object[] proxyAddressesVal = (object[])innerEntry.Properties["proxyAddresses"].Value;
				if (proxyAddressesVal != null)
				{
					foreach (object obj in proxyAddressesVal)
					{
						//if value is an email address add it to the user object with name of 'mail_x'
						if (((string)obj).ToLower().Contains("smtp"))
						{
							proxyAddresses.Add(((string)obj).Replace("smtp:", "").Replace("SMTP:", ""));
						}
					}
				}
				u.proxyAddresses = proxyAddresses.ToArray();
				if(innerEntry.Properties["cn"].Value != null)
					u.cn = innerEntry.Properties["cn"].Value.ToString();
				if(innerEntry.Properties["givenName"].Value != null)
					u.givenName = innerEntry.Properties["givenName"].Value.ToString();
				if(innerEntry.Properties["sn"].Value != null)	
					u.sn = innerEntry.Properties["sn"].Value.ToString();
				if(innerEntry.Properties["mail"].Value != null)	
					u.mail = innerEntry.Properties["mail"].Value.ToString();
				if(innerEntry.Properties["displayName"].Value != null)
					u.displayName = innerEntry.Properties["displayName"].Value.ToString();
				if (string.IsNullOrEmpty(u.mail) && u.proxyAddresses.Length > 0)
				{
					u.mail = u.proxyAddresses[0];
				}
			}
			return userList.ToArray();
		}

		private ActiveDirectoryUser[] ParseFull(SearchResultCollection results)
		{
			List<ActiveDirectoryUser> userList = new List<ActiveDirectoryUser>();

			foreach (SearchResult result in results)
			{
				ActiveDirectoryUser u = new ActiveDirectoryUser();
				userList.Add(u);

				DirectoryEntry innerEntry = new DirectoryEntry(result.Path,
                    CriticalResults.Properties.Settings.Default.LDAPUsername,
                    CriticalResults.Properties.Settings.Default.LDAPPassword);
				int proxyAddressCount = 0;
				foreach (string name in innerEntry.Properties.PropertyNames)
				{
					//Added to retrieve proxy email address from active directory to facilitate partners search
					//Check if value is array of objects instead of single object
					if (innerEntry.Properties[name].Value.GetType() == typeof(object[]) && name == "proxyAddresses")
					{
						//Get array of values
						object[] value = (object[])innerEntry.Properties[name].Value;
						foreach (object obj in value)
						{
							//if value is an email address add it to the user object with name of 'mail_x'
							if (((string)obj).ToLower().Contains("smtp"))
							{
								proxyAddressCount++;
								string property_value = obj as string;
								string property_name = "mail_" + proxyAddressCount.ToString();
								u.AddProperty(property_name, property_value);
							}
						}
					}
					else
					{
						string value = innerEntry.Properties[name].Value.ToString();
						u.AddProperty(name, value);
					}
				}
				u.AddProperty("Proxy_Address_Count", proxyAddressCount.ToString());
			}
			return userList.ToArray();
		}

        public ActiveDirectoryUser[] SearchActiveDirectory(string lastname, string firstname)
        {
            if (string.IsNullOrEmpty(lastname))
            {
                throw new SearchArgumentException();
            }

            string searchString = "";
            if (firstname != "null")
            {
                searchString = string.Format("(&(sn={0}*)(givenName={1}*))", lastname, firstname);
            }
            else
            {
                searchString = string.Format("(&(sn={0}*))", lastname);
            }

            ActiveDirectoryUser[] users = Search(searchString, DirectoryProxy.ReturnType.Min);
            return users;
        }

        public ActiveDirectoryUser[] SearchActiveDirectory(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new SearchArgumentException();
            }

            string searchstring = string.Format("(&(cn={0}))", userName);
            ActiveDirectoryUser[] users = Search(searchstring, DirectoryProxy.ReturnType.Min);
            return users;

        }
    }
}