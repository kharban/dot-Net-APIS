using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_AdminAPI
{
    public class PaymentModel
    {
        public decimal TotalEarning { get; set; }
        public List<PaymentsModel> PaymentList { get; set; }
    }


    public class PaymentsModel
    {
        public string id { get; set; }
        public string transaction_id { get; set; }
        public string customer_name { get; set; }
        public string provider_name { get; set; }
        public decimal amount { get; set; }
        public string iban_no { get; set; }
        public DateTime transaction_date { get; set; }
    }
}