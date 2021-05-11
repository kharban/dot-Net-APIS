using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kharban_AdminAPI.Models
{
    public class UserLoginResponse
    {
        public string id { get; set; }
        public string user_id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string mobile { get; set; }
        public string otp { get; set; }
        public string forgotToken { get; set; }
        public string address { get; set; }
        public string country_code { get; set; }
        public string email { get; set; }
        public bool Status { get; set; }

        public string StatusMessage { get; set; }

        public DateTime RequestTime { get; set; }
        public string IpAddress { get; set; }
        public string Location { get; set; }
    }
}
