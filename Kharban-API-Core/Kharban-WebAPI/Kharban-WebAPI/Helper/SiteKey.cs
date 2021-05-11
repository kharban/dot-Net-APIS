using Kharban_WebAPI.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kharban_WebAPI.Helper
{
    public class SiteKey
    {
        #region App-Setting Reading start
        private static IConfigurationSection _configureAppSetting;
        public static void ConfigureAppSettings(IConfigurationSection configureAppSetting)
        {
            _configureAppSetting = configureAppSetting;
        }
        public static string SiteUrl => _configureAppSetting["SiteUrl"];
        public static string DKR_MAIL => _configureAppSetting["DKR_MAIL"];
        public static string S3BucketName => _configureAppSetting["S3BucketName"];
        public static string IsLive => _configureAppSetting["IsLive"];
        public static string ImageURL => _configureAppSetting["ImageURL"];
        #endregion App-Setting Reading End

        #region Push-Notification Reading start
        private static IConfigurationSection _configurationPushNotification;
        public static void ConfigurePushNotification(IConfigurationSection configurationPushNotification)
        {
            _configurationPushNotification = configurationPushNotification;
        }
        public static string PublicKey => _configurationPushNotification["PublicKey"];

        public static string PrivateKey => _configurationPushNotification["PrivateKey"];
        #endregion Push-Notification Reading End

        #region ConnectionString Reading Start
        private static IConfigurationSection _configureConnectionString;
        public static void ConfigureConnectionString(IConfigurationSection configureConnectionString)
        {
            _configureConnectionString = configureConnectionString;
        }
        public static string ConnectionString => _configureConnectionString["DevelopmentConnectionString"];
        #endregion ConnectionString Reading End
    }
}
