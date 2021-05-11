using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_AdminAPI
{
    public class SettingModel
    {
        public string id { get; set; }
        public string primary_email { get; set; }
        public string primary_contact { get; set; }
        public decimal provider_cancellation_fee { get; set; }
        public string facebook_link { get; set; }
        public string twitter_link { get; set; }
        public string google_plus_link { get; set; }
        public string instagram_link { get; set; }
        public int nearby_radius { get; set; }
        public string logo { get; set; }
        public decimal platform_fee { get; set; }
        public bool platform_fee_enable { get; set; }
        public decimal tax { get; set; }
        public decimal distance { get; set; }
        public int status { get; set; }
        public string type { get; set; }
        public string image { get; set; }
        public string image_extension { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
    }
}