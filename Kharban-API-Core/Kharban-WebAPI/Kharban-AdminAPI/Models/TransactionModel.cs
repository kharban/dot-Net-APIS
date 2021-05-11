using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_AdminAPI
{
    public class TransactionModel
    {
        public string id { get; set; }
        public string provider_name { get; set; }
        public string iban_no { get; set; }
        public decimal pending_amount { get; set; }
        public int booking_count { get; set; }
    }

}