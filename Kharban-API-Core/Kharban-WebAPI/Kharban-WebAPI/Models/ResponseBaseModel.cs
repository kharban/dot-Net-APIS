using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_WebAPI
{

    public class ResponseBaseModel : SuperClass
    {

        public int Code { get; set; }

        public string URLdata { get; set; }

        public string Message { get; set; }

        public string Title { get; set; }

        public dynamic Model { get; set; }

        public bool Status { get; set; }

        ~ResponseBaseModel()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}