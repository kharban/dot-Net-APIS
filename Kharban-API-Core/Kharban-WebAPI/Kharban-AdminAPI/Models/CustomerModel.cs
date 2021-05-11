using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_AdminAPI
{
    public class CustomerModel
    {
        public string id { get; set; }
        public string user_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string contact_no { get; set; }
        public string country_code { get; set; }
        public string otp { get; set; }
        public string email { get; set; }
        public bool is_mobile_verified { get; set; }
        public bool is_email_verified { get; set; }
        public bool is_deleted { get; set; }
        public string auth_token { get; set; }
        public string created_by { get; set; }
        public string image { get; set; }
        public string image_extension { get; set; }
        public string ImageType { get; set; }
        public int status { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }

    }

    public class CustomerRequestModel
    {
        public string id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string contact_no { get; set; }
        public string country_code { get; set; }
        public string otp { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public bool is_mobile_verified { get; set; }
        public bool is_email_verified { get; set; }
        public bool is_deleted { get; set; }
        public string auth_token { get; set; }
        public string created_by { get; set; }
        public int status { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        
        public int page { get; set; }
        public string filterby { get; set; }
        public string keyword { get; set; }
        public string sortby { get; set; }



    }
}