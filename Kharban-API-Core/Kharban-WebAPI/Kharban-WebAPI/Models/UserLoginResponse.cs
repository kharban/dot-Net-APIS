using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_WebAPI.Models
{
    public class UserLoginResponse
    {
        public bool Status { get; set; }

        public string StatusMessage { get; set; }

        public int AccountActivate { get; set; }

        public string member_id { get; set; }

        public string HoroScopeUrl { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string fullname { get; set; }

        public string Email { get; set; }

        public string IsDatingMember { get; set; }

        public string IsFirstLogin { get; set; }

        public string IsPlanAdded { get; set; }

        public string IsAccountVerified { get; set; }

        public string IsEmailVerified { get; set; }

        public string IsDocumentVerified { get; set; }

        public string Message { get; set; }

        public string IPAddress { get; set; }

        public string ProfileId { get; set; }
        //public string DeviceId { get; set; }

        //public string DeviceOs { get; set; }

        //public string DeviceType { get; set; }
    }
}