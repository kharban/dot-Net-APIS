using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_AdminAPI
{
    public class DashBoardModel
    {
        public int customer_count { get; set; }
        public int booking_count { get; set; }
        public decimal transaction_amount { get; set; }
    }

    public class DashBoardChartModel
    {
        public List<string> label { get; set; }
        public List<decimal> data { get; set; }
        public decimal total { get; set; }
    }

    public class DashBoardRequestModel
    {
        public string id { get; set; }
        public int page { get; set; }
        public string filterby { get; set; }
        public string service_id { get; set; }
        public string keyword { get; set; }
        public string sortby { get; set; }
    }

}