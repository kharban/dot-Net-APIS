using System;
using System.Collections.Generic;
using System.Text;

namespace Kharban_AdminAPI
{
    public class RequestModel
    {
        public string id { get; set; }
        public int page { get; set; }
        public string filterby { get; set; }
        public string filterby2 { get; set; }
        public string keyword { get; set; }
        public string keyword2 { get; set; }
        public string sortby { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }

    }



}
