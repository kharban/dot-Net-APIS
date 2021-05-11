using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Insight.Database;
using System.Data.SqlClient;
using Kharban_WebAPI.CommonClasses;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Kharban_WebAPI.Helper;

namespace Kharban_WebAPI.Repository
{
    public static class LoggingRepository
    {

        public static void SaveException(Exception model)
        {
            try
            {
                var perameter = new
                {
                    message = model.Message,
                    type = model.GetType().ToString(),
                    source = model.StackTrace == null ? "" : model.StackTrace.ToString(),
                    url = "",// _httpContextAccessor.HttpContext.Request.Url.AbsoluteUri,
                    created = DateTime.Now,
                    createdby = 5,
                };
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    #region Insert
                    DB.ExecuteSql(@"INSERT INTO exception_logging(
                    message, type, source, url, created, createdby) 
                    VALUES (@message,@type,@source,@url,@created,@createdby);", perameter);
                    #endregion

                };
            }
            catch (Exception ex)
            {
            }
        }
    }
}