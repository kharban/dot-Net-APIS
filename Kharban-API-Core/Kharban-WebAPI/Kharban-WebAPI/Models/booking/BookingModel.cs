using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_WebAPI.Models
{
    public class BookingModel
    {
        public BookingModel()
        {

        }
        public string id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string contact_no { get; set; }
        public string country_code { get; set; }
        public string booking_code { get; set; }
        public string booking_id { get; set; }
        public string customer_id { get; set; }
        public string provider_id { get; set; }
        public string service_id { get; set; }
        public string service_category_id { get; set; }
        public string transaction_id { get; set; }
        public string customer_name { get; set; }
        public string CustomerContactNo { get; set; }
        public string provider_name { get; set; }
        public string ProviderContactNo { get; set; }
        public string ProviderProfilePicture { get; set; }
        public string profile_picture { get; set; }
        public decimal distance { get; set; }
        public string service_name { get; set; }
        public string service_image { get; set; }
        public string coupon_id { get; set; }
        public string address_title { get; set; }
        public string landmark { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string booking_longitude { get; set; }
        public string booking_latitude { get; set; }
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
        public int provider_status { get; set; }
        public decimal provider_rating { get; set; }
        public string provider_rating_description { get; set; }
        public string provider_receipt_description { get; set; }
        public string booking_receipt_id { get; set; }
        public decimal receipt_amount { get; set; }

        public string strDate { get; set; }
        public string customer_receipt_description { get; set; }



        public string endDate { get; set; }
        public int receipt_reject_count { get; set; }
        public string trip_snapshot { get; set; }
        public string receipt_status { get; set; }
        public DateTime AcceptDate { get; set; }
        public List<BookingReceiptModel> booking_receipts { get; set; }
    }


    public class BookingReceiptModel {

        public string id { get; set; }
        public string booking_id   { get; set; }
        public int booking_status  { get; set; }
        public int status   { get; set; }
        public int is_deleted  { get; set; }
        public DateTime created  { get; set; }
        public string provider_receipt_description { get; set; }
        public string customer_receipt_description { get; set; }
        public int receipt_status { get; set; }
        public decimal receipt_amount { get; set; }
        public string trip_snapshot { get; set; }
        public string file_extension { get; set; }


    }

    public class Searchfilter
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class LocationModel
    {
       // public int id { get; set; }
        //public string customer_id { get; set; }
        public string address { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        //public int is_deleted { get; set; }
        //public DateTime created { get; set; }

    }

    public class AdminMailModel
    {
        public string CustomerName { get; set; }
        public string CustomerContactNo { get; set; }
        public string ProviderName { get; set; }
        public string ProviderContactNo { get; set; }
        public string BookingCode { get; set; }
        public string BookingStatus { get; set; }

    }

    
}