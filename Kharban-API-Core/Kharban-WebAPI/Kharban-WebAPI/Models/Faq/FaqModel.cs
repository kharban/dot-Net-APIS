using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_WebAPI.Models
{
    public class FaqModel
    {
        public string id  { get; set; }
        public string question { get; set; }
        public string question_arabic { get; set; }
        public string answer { get; set; }
        public string answer_arabic { get; set; }
        public int status { get; set; }
        public bool is_deleted { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public int faq_for { get; set; }
    }

}