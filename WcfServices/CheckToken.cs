using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace CriticalResults
{
    public class CheckTokenUtil
    {
        public static bool CheckToken(string userName, string tokenValue, string ipAddress, string method, bool refreshToken, out HTTPCheckRoles role)
        {
            role = 0;
            ExpireAllTokensForUser(userName);
            CriticalResultsEntityManager manager = new CriticalResultsEntityManager();
            TokenEntity[] tokens = manager.GetTokensForUser(userName);
            foreach (TokenEntity token in tokens)
            {
                if (token.Token == new Guid(tokenValue) && token.Ipv4 == ipAddress)
                {
                    if (refreshToken)
                        token.UpdatedTime = DateTime.Now;
                    manager.SaveChanges();
                    foreach (RoleEntity re in token.User.Roles)
                    {
                        object r = Enum.Parse(typeof(HTTPCheckRoles), re.Name, true);
                        if (r != null)
                        {
                            role |= (HTTPCheckRoles)r;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        private static void ExpireAllTokensForUser(string userName)
        {
            CriticalResultsTransporterEntities context = new CriticalResultsTransporterEntities();
            DateTime deadTokenTime = DateTime.Now - GetTokenLifespan();
            var query = from tokens in context.TokenEntitySet.Include("User")
                        where tokens.User.UserName == userName
                        where tokens.UpdatedTime < deadTokenTime
                        select tokens;
            TokenEntity[] tokenArray = query.ToArray();
            foreach (TokenEntity token in tokenArray)
            {
                context.DeleteObject(token);
            }
            try
            {
                context.SaveChanges();
            }
            catch { }
        }

        private static TimeSpan TOKEN_LIFESPAN = new TimeSpan(0, 60, 0);
        private static TimeSpan GetTokenLifespan()
        {
            try
            {
                return new TimeSpan(0, Int32.Parse(SettingsManager.GetSystemSetting(SettingsManager.SETTINGS_TIMEOUT).Value, CultureInfo.InvariantCulture.NumberFormat), 0);
            }
            catch (Exception)
            {
                return TOKEN_LIFESPAN;
            }
        }
    }
}
