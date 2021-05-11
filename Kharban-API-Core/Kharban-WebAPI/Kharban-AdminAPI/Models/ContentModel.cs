using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_AdminAPI
{
    public class ContentModel
    {
        public string id { get; set; }
        public string content_type { get; set; }
        public string description { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
    }

}