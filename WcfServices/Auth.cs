using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CriticalResults
{
    public class Auth
    {
        public static bool Authenticate(string authKey, string authValue, string userName, string userIP, TraceSource _Trace, out string outUserName, out string tokenString, out string message)
        {
            outUserName="";
            tokenString = "";
            CriticalResults.CriticalResultsEntityManager manager = new CriticalResultsEntityManager();
            bool hasPassword = false;
            string queryString = string.Format("it.Type='AuthExt' AND it.Key='{0}' AND it.Value='{1}'", authKey, authValue);
            // if userName is specified then we go from ANCR and authValue is password, overwise authValue is username of already authenticated user
            if (userName != null)
            {
                hasPassword = true;
                UserEntity user = manager.GetUser(userName);
                if (user == null)
                {
                    message = "Invalid username or password.";
                    return false;
                }
                queryString = string.Format("it.Type='AuthExt' AND it.Key='{0}' AND it.Value='{1}' AND it.User.Id={2}", authKey, authValue, user.Id);
            }

            
            CriticalResults.UserEntryEntity[] entries = manager.QueryUserEntryEntities(queryString, null, null);
            if (entries.Count() == 1)
            {
                UserEntity user=entries.First().User;
                if (!hasPassword)
                {
                    _Trace.TraceEvent(TraceEventType.Information, 0, "Lookup for username \"{0}\" mapped to \"{1}\".", authValue, user.UserName);
                }
                if (user.Enabled == true)
                {
                    outUserName = user.UserName;
                    CriticalResults.TokenEntity[] currentTokens = manager.GetTokensForUser(user.UserName);
                    foreach (CriticalResults.TokenEntity t in currentTokens)
                    {
                        if (t.Ipv4 == userIP)
                        {
                            t.UpdatedTime = DateTime.Now;
                            manager.SaveChanges();
                            tokenString = t.Token.ToString();
                            message = "";
                            return true;
                        }
                    }
                    CriticalResults.TokenEntity token = manager.CreateToken(user, userIP);
                    tokenString = token.Token.ToString();
                    message = "";
                    return true;
                }
                else
                {
                    _Trace.TraceEvent(TraceEventType.Warning, 0, "Found user \"{0}\" mapped to \"{1}\".  ANCR Account disabled.", authValue, user.UserName);
                    message = "Your account is disabled. Please contact your System Administrator.";
                    return false;
                }
            }
            else
            {
                if (entries.Count() > 1)
                {
                    _Trace.TraceEvent(TraceEventType.Warning, 0, "Found multiple users \"{0}\" (Count: {1})", authValue, entries.Count());
                    message = "Multiple ANCR users found for your username, until this is resolved you may not login with your credentials.  Please contact your System Administrator.";
                }
                else
                {
                    if (hasPassword)
                    {
                        _Trace.TraceEvent(TraceEventType.Warning, 0, "Invalid username or password \"{0}\"", userName);
                        message = "Invalid username or password.";
                    }
                    else
                    {
                        _Trace.TraceEvent(TraceEventType.Warning, 0, "No ANCR account found for  \"{0}\"", authValue);
                        message = "No ANCR user is found for your credentials. Please contact your System Administrator.";
                    }
                }
                return false;
            }
        }

        //public void Auth2()
        //{
        //    string queryString = string.Format("it.Type='AuthExt' AND it.Key='{0}' AND it.Value='{1}'", AuthExtName, WindowsUser);
        //    CriticalResults.CriticalResultsEntityManager manager = new CriticalResultsEntityManager();
        //    CriticalResults.UserEntryEntity[] entries = manager.QueryUserEntryEntities(queryString, null, null);
        //    if (entries.Count() == 1)
        //    {
        //        if (entries.First().User.Enabled == true)
        //        {
        //            UserName = entries.First().User.UserName;
        //            _Trace.TraceEvent(TraceEventType.Information, 0, "Lookup for user \"{0}\" mapped to \"{1}\".", WindowsUser, UserName);

        //            CriticalResults.TokenEntity[] currentTokens = manager.GetTokensForUser(UserName);
        //            foreach (CriticalResults.TokenEntity t in currentTokens)
        //            {
        //                if (t.Ipv4 == Request.UserHostAddress)
        //                {
        //                    TokenGuid = t.Token.ToString();
        //                    t.UpdatedTime = DateTime.Now;
        //                    manager.SaveChanges();
        //                }
        //            }
        //            if (TokenGuid == "")
        //            {

        //                CriticalResults.TokenEntity token = manager.CreateToken(entries.First().User, Request.UserHostAddress);
        //                TokenGuid = token.Token.ToString();
        //            }

        //        }
        //        else
        //        {
        //            _Trace.TraceEvent(TraceEventType.Warning, 0, "Found user \"{0}\" mapped to \"{1}\".  ANCR Account disabled.", WindowsUser, entries.First().User.UserName);
        //        }
        //    }
        //    else if (entries.Count() == 0)
        //    {
        //        PageMessage = "No ANCR accounts found for this Windows User.  Until this is resolved you may not login with your Windows User Account.  Please contact your System Administrator.";
        //        _Trace.TraceEvent(TraceEventType.Warning, 0, "Failed lookup for user \"{0}\".  {1} User Entries Returned.", WindowsUser, entries.Count());
        //    }
        //    else
        //    {
        //        PageMessage = "Multiple ANCR accounts resolved to this Windows User.  Until this is resolved you may not login with your Windows User Account.  Please contact your System Administrator.";
        //        _Trace.TraceEvent(TraceEventType.Warning, 0, "Failed lookup for user \"{0}\".  {1} User Entries Returned.", WindowsUser, entries.Count());
        //    }
        //}
        //public void Auth1()
        //{

        //    string queryString = string.Format("it.Type='AuthExt' AND it.Key='{0}' AND it.Value='{1}'", AuthExtName, CentricityUserName);
        //    CriticalResults.CriticalResultsEntityManager manager = new CriticalResults.CriticalResultsEntityManager();
        //    CriticalResults.UserEntryEntity[] entries = manager.QueryUserEntryEntities(queryString, null, null);
        //    if (entries.Count() == 1)
        //    {
        //        CriticalResults.UserEntity user = entries.First().User;
        //        UserName = user.UserName;
        //        if (user.Enabled == true)
        //        {
        //            CriticalResults.TokenEntity[] currentTokens = manager.GetTokensForUser(user.UserName);
        //            foreach (CriticalResults.TokenEntity t in currentTokens)
        //            {
        //                if (t.Ipv4 == Request.UserHostAddress)
        //                {
        //                    Token = t.Token.ToString();
        //                    t.UpdatedTime = DateTime.Now;
        //                    manager.SaveChanges();
        //                }
        //            }
        //            if (Token == "")
        //            {
        //                CriticalResults.TokenEntity token = manager.CreateToken(user, Request.UserHostAddress);
        //                Token = token.Token.ToString();
        //            }
        //        }
        //        else
        //        {
        //            _Trace.TraceEvent(TraceEventType.Warning, 0, "Found user \"{0}\" mapped to \"{1}\".  ANCR Account disabled.", CentricityUserName, entries.First().User.UserName);
        //        }

        //    }
        //    else
        //    {
        //        _Trace.TraceEvent(TraceEventType.Warning, 0, "Failed lookup for user \"{0}\".  {1} User Entries Returned.", CentricityUserName, entries.Count());
        //    }

        //}
        //public void Auth3()
        //{
        //    CriticalResultsEntityManager manager = new CriticalResultsEntityManager();
        //    UserEntity user = manager.GetUser(userName);
        //    if (user == null)
        //    {
        //        AuditEvent("GetToken:Failure", string.Format("{0}:User does not exist", userName));
        //        return null;
        //    }
        //    if (!user.Enabled)
        //    {
        //        AuditEvent("GetToken:Failure", string.Format("{0}:User is disabled", userName));
        //        return null;
        //    }

        //    foreach (UserEntryEntity entry in user.UserEntries)
        //    {
        //        if (entry.Key == "ANCR" && entry.Type == "AuthExt" && String.Compare(entry.Value, passwordHash, true) == 0)
        //        {
        //            Guid token;
        //            TokenEntity[] currentTokens = manager.GetTokensForUser(userName);
        //            string ipAddress = WcfHelper.GetIPv4FromWCF();
        //            if (currentTokens.Length > 0)
        //            {
        //                foreach (TokenEntity t in currentTokens)
        //                {
        //                    if (t.Ipv4 == ipAddress)
        //                    {
        //                        AuditEvent("GetToken:Successful", string.Format("{0}:{1}:Return Existing Token", userName, t.Token));
        //                        t.UpdatedTime = DateTime.Now;
        //                        manager.SaveChanges();
        //                        return (t.Token);
        //                    }
        //                }
        //            }
        //            token = manager.CreateToken(user, WcfHelper.GetIPv4FromWCF()).Token;
        //            AuditEvent("GetToken:Successful", string.Format("{0}:{1}:Return New Token", userName, token));
        //            return token;
        //        }
        //    }

        //    AuditEvent("GetToken:Failure", string.Format("{0}:Authentication Failed", userName));
        //    return null;


        //}
    }
}
