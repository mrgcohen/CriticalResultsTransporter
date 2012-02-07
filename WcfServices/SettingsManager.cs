using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CriticalResults
{
    class SettingsManager
    {
        public const string SETTINGS_SYSTEM = "System";
        public const string SETTINGS_TIMEOUT = "Session_Timeout";
        private static Dictionary<String, Setting> _SystemSettings;

        public static Setting GetSystemSetting(string key)
        {
            if (_SystemSettings == null)
            {
                CriticalResultsTransporterEntities context = new CriticalResultsTransporterEntities();
                var query = from settings in context.SettingEntitySet
                            where settings.Owner == SETTINGS_SYSTEM
                            select settings;
                _SystemSettings = query.ToDictionary<SettingEntity, String, Setting>(x => x.EntryKey, x => new Setting(x));
            }
            return _SystemSettings[key];
        }

        internal static Setting UpdateSetting(string owner, string uuid, string value)
        {
            if (owner == SETTINGS_SYSTEM)
                _SystemSettings = null;
            return new Setting(new CriticalResultsEntityManager().UpdateSetting(null, new Guid(uuid), null, value, null));
        }
    }
}
