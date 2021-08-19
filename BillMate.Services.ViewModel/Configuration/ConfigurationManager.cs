using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BillMate.Services.ViewModel.Configuration
{
    public class ConfigurationManager
    {
        internal static ApplicationDataContainer LocalSettings { get; set; } = ApplicationData.Current.LocalSettings;

        public static object GetLocalSetting(string settingName)
        {
            if (CheckLocalSetting(settingName))
            {
                return LocalSettings.Values[settingName];
            }
            else
            {
                return null;
            }
        }
        public static void SetLocalSetting(string settingName, string value)
        {
            LocalSettings.Values[settingName] = value;
        }
        /// <summary>
        /// Checks the local configuration store for an instance of that setting. 
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns>True if the setting exists, otherwise False.</returns>
        public static bool CheckLocalSetting(string settingName)
        {
            return LocalSettings.Values.ContainsKey(settingName);
        }
    }
}
