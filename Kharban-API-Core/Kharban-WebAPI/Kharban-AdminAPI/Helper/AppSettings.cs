namespace Kharban_AdminAPI.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }
    }

    public class PushNotificationsOptions
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
}