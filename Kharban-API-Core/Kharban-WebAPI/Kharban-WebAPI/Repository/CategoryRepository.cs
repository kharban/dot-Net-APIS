using System;
using System.Collections.Generic;
using System.Linq;
using Insight.Database;
using System.Data.SqlClient;
using Kharban_WebAPI.Models;
using Kharban_WebAPI.Helper;

namespace Kharban_WebAPI.Repository
{
    public class CategoryRepository
    {
        public Response<List<CategoryModel>> GetCategoryList(RequestModel model)
        {
            Response<List<CategoryModel>> returnModel = new Response<List<CategoryModel>>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<CategoryModel>("SELECT id, name, arabic_name, id value, name label, image, status FROM service_category where is_deleted = 0 and status = 1 ").ToList();
                }
                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Service Category List";
                returnModel.success = true;
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                LoggingRepository.SaveException(ex);
                returnModel.success = false;
            }
            return returnModel;
        }

    }
}
