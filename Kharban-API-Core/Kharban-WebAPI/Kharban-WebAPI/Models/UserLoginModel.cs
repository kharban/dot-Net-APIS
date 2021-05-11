using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_WebAPI.Models
{
    public class UserLoginModel
    {
        public UserLoginModel()
        {

        }
        public string id { get; set; }
        public string userid { get; set; }
        public string provider_id { get; set; }
        public string customer_id { get; set; }
        public string booking_id { get; set; }
        public string provider_name { get; set; }
        public string customer_name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string contact_no { get; set; }
        public string email { get; set; }
        public string country_code { get; set; }
        public string requesttype { get; set; }
        public string otp { get; set; }
        public bool isregistered { get; set; }
        public bool ismatchusertype { get; set; }
        public int is_tc_checked { get; set; }
        public string message { get; set; }
        public int TotalEarning { get; set; }
        public string Rating { get; set; }
        public bool success { get; set; }
        public int status { get; set; }
        public string iban_no { get; set; }
        public string address { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string latitude { get; set; }
        public string latitude1 { get; set; }
        public string latitude2 { get; set; }
        public string longitude { get; set; }
        public string longitude1 { get; set; }
        public string longitude2 { get; set; }
        public string service_category_id { get; set; }
        public int is_online { get; set; }
        public string distance { get; set; }
        public string profile_picture { get; set; }
        public string profile_extension{ get; set; }
        public string document_id { get; set; }
        public string document_image { get; set; }
        public string document_extension { get; set; }
        public string iban_image { get; set; }
        public string iban_extension { get; set; }
        public string ImageType { get; set; }
        public string image { get; set; }
        public string image_extension { get; set; }
        public string booking_code { get; set; }
        public int Radius { get; set; }

        public int approved_status { get; set; }

        public int platformFee { get; set; }
        public string type { get; set; }

        public string usertype { get; set; }
    }

    public class CountryModel
    {
        public string country_code { get; set; }
        public string country { get; set; }
        public string country_flag_icon { get; set; }
        public string id { get; set; }
    }
}