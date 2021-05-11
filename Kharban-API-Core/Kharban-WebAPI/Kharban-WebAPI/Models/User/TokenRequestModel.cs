using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_WebAPI.Models.User
{
    public class TokenRequestModel
    {
        public string TokenKey { get; set; }

        public string TokenValue { get; set; }
    }

}