using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_WebAPI.Models
{
    public class ProviderModel
    {
        //public string id { get; set; }
        //public string user_id { get; set; }
        public string service_category_id { get; set; }
        //public string service_category_name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string iban_no { get; set; }
        //public decimal total_earns { get; set; }
        //public decimal rating { get; set; }
        //public string profile_picture { get; set; }
        //public string document_image { get; set; }
        //public string iban_image { get; set; }
        public string country_code { get; set; }
        public string contact_no { get; set; }
        //public string otp { get; set; }
        //public string email { get; set; }
        //public bool is_policy_accepted { get; set; }
        //public bool is_mobile_verified { get; set; }
        //public bool is_email_verified { get; set; }
        //public bool is_deleted { get; set; }
        //public int admin_approve { get; set; }
        public int is_online { get; set; }
        //public bool is_notification_enable { get; set; }
        //public string auth_token { get; set; }
        //public string created_by { get; set; }
        //public int status { get; set; }
        //public DateTime created { get; set; }
        //public DateTime modified { get; set; }
        public string image { get; set; }
        public string image_extension { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
    }

    public class ProviderRequestModel
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string profile_picture { get; set; }
        public string contact_no { get; set; }
        public string otp { get; set; }
        public string email { get; set; }
        public string is_policy_accepted { get; set; }
        public string is_mobile_verified { get; set; }
        public string is_enail_verified { get; set; }
        public string admin_approve { get; set; }
        public string is_online { get; set; }
        public string is_notification_enable { get; set; }
        public string auth_token { get; set; }
        public string created_by { get; set; }
        public string status { get; set; }
        public string created { get; set; }
        public string modified { get; set; }


        public int page { get; set; }
        public string filterby { get; set; }
        public string keyword { get; set; }
        public string sortby { get; set; }



    }
}