using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_AdminAPI
{
    public class BookingModel
    {
        public string id { get; set; }
        public string booking_code { get; set; }
        public string booking_id { get; set; }
        public string customer_id { get; set; }
        public string provider_id { get; set; }
        public string service_id { get; set; }
        public string customer_name { get; set; }
        public string provider_name { get; set; }
        public decimal distance { get; set; }
        public string service_name { get; set; }
        public string coupon_id { get; set; }
        public string address_title { get; set; }
        public string landmark { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string description { get; set; }
        public int booking_status { get; set; }
        public decimal provider_cancellation_fee { get; set; }
        public decimal booking_amount { get; set; }
        public int status { get; set; }
        public bool is_deleted { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public int payment_type { get; set; }
        public int total_booking { get; set; }
        public decimal provider_rating { get; set; }
        public string provider_rating_description { get; set; }
        public List<BookingReceiptModel> booking_receipts { get; set; }
    }


    public class BookingReceiptModel {

        public string provider_receipt_description { get; set; }
        public string customer_receipt_description { get; set; }
        public int receipt_status { get; set; }
        public decimal receipt_amount { get; set; }
    }
}