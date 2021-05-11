using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_AdminAPI.Models
{
    public class BaseResponse
    {
        public bool Status { get; set; }

        public string StatusMessage { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public string Parameter { get; set; }
    }
}