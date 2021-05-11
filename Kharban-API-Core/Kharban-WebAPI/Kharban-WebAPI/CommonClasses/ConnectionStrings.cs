using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kharban_WebAPI.CommonClasses
{
    public class DbConnectionStrings
    {

        private static IConfiguration Configuration;
        public DbConnectionStrings(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        public static string DefaultConnectionString()
        {
            return Configuration.GetConnectionString("DevelopmentConnectionString");
        }
       // public string DefaultConnectionString { get; set; }
    }
}
