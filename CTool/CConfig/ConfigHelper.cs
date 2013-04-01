using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTool.CConfig
{
    class ConfigHelper
    {
        public static string GetAppSetting(string appKey)
        {
            if (string.IsNullOrEmpty(appKey))
                throw new ArgumentNullException("appKey");

            return System.Configuration.ConfigurationManager.AppSettings[appKey];
        }
        public static string GetConnection(string conKey)
        {
            if (string.IsNullOrEmpty(conKey))
                throw new ArgumentNullException("conKey");

            return System.Configuration.ConfigurationManager.ConnectionStrings[conKey].ConnectionString;
        }
    }
}
