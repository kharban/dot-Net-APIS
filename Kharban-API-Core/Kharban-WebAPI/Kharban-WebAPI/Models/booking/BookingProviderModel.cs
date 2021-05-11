using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_WebAPI.Models
{
    public class BookingProviderModel
    {

        public string id { get; set; }
        public string provider_status { get; set; }
        public string booking_id { get; set; }
        public string provider_id { get; set; }
        //public int status { get; set; }
        //public int is_deleted { get; set; }
        //public DateTime created { get; set; }
        //public DateTime modified { get; set; }
    }
}