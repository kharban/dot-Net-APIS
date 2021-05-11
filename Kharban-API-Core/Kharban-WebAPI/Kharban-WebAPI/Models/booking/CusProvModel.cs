using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_WebAPI.Models
{
    public class CusProvModel
    {
        public string longitude { get; set; }

        public string latitude { get; set; }

        public string image { get; set; }
        public ProviderModel[] Providers { get; set; }

    }
}