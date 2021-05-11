using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_WebAPI.Models.User
{
    public class DeviceInfoModel
    {
        public string id { get; set; }
        public string user_id { get; set; }
        public string device_id { get; set; }
        public string device_type { get; set; }
        public string user_active { get; set; }
        public string device_version { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
    }
}