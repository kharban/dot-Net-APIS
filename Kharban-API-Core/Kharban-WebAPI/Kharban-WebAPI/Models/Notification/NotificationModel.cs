using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_WebAPI.Models
{
    public class NotificationModel
    {
        public string id { get; set; }
        public string user_id { get; set; }
        public string sender_id { get; set; }
        public string receiver_id { get; set; }
        public string notification { get; set; }
        public int notification_type { get; set; }
        public int for_admin { get; set; }
        public int status { get; set; }
        public int is_deleted { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public int page { get; set; }
        public string filterby { get; set; }
        public string keyword { get; set; }
        public string sortby { get; set; }
    }

    public class PushNotificationModel
    {
        public string id { get; set; }
        public string receiver_type { get; set; }
        public bool receiver_all { get; set; }
        public string customer_id { get; set; }
        public string provider_id { get; set; }
        public string subject { get; set; }
        public string text { get; set; }

    }

}