using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_WebAPI.Models.User
{
    public class OtpModel
    {
        public string otp { get; set; }

        public string RequestType { get; set; }

        public string IpAddress { get; set; }

        public string mobile { get; set; }

        public string CountryCode { get; set; }

        public string MemberId { get; set; }
    }
}