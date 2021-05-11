using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_WebAPI.Models
{
    public class CategoryModel
    {
        public string id { get; set; }
        public string value { get; set; }
        public string label { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public string image_extension { get; set; }
        public bool status { get; set; }
        public bool is_deleted { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; } 
        public string arabic_name { get; set; }
    }

    public class CategoryRequestModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public string status { get; set; }
        public string is_deleted { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public int page { get; set; }
        public string filterby { get; set; }
        public string keyword { get; set; }
        public string sortby { get; set; }



    }
}