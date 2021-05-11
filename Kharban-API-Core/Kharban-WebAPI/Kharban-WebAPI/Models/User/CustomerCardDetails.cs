using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_WebAPI.Models
{
    public class CustomerCardDetails
    {

        public string id { get; set; }
        public string user_id { get; set; }
        public int expiry_month { get; set; }
        public string card_last_digit { get; set; }
        public int status { get; set; }
        public int expiry_year { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string card_ref_number { get; set; }
        
    }
}