using System;
using System.Linq;
using Insight.Database;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using Kharban_AdminAPI.Helper;
using Microsoft.AspNetCore.Http;
using Kharban_AdminAPI.Common;

namespace Kharban_AdminAPI.Repository
{
    public class CategoryRepository
    {
        public ResponseList<List<CategoryModel>> GetCategories(RequestModel model)
        {
            string ReturnLink = string.Empty;


            string queryString = string.Empty;
            string orderbyString = string.Empty;
            string queryCount = string.Empty;
            int TotalRecords = 0;
            int recoardFrom = ((model.page - 1) * 10) + 1;
            int recoardTo = model.page * 10;

            ResponseList<List<CategoryModel>> returnModel = new ResponseList<List<CategoryModel>>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {

                    if (!string.IsNullOrEmpty(model.sortby))
                    {
                        if (model.sortby == "atoz")
                            orderbyString = " order by name ";

                        else if (model.sortby == "ztoa")
                            orderbyString = " order by name desc ";

                        else if (model.sortby == "newtoold")
                            orderbyString = " order by created desc";

                        else if (model.sortby == "oldtonew")
                            orderbyString = " order by created";
                    }
                    else
                        orderbyString = " order by created desc";


                    queryCount = @"SELECT count(id) totalrecord FROM service_category  where is_deleted = 0 ";
                    queryString = @"select * from (SELECT ROW_NUMBER() OVER (" + orderbyString + @") row_num, id, name, image, status FROM service_category 
                                    where is_deleted = 0 ";

                    if (model.filterby == "name")
                    {
                        queryString += " and name like @keyword ";
                        queryCount += " and name like @keyword ";
                    }
                    if (!string.IsNullOrEmpty(model.keyword2))
                    {
                        queryString += " and status = @keyword2 ";
                        queryCount += " and status = @keyword2 ";
                    }

                    queryString += " ) t where row_num between " + recoardFrom + " and " + recoardTo;


                    returnModel.result = DB.QuerySql<CategoryModel>(queryString, new { keyword = "%" + model.keyword + "%", keyword2 = model.keyword2 }).ToList();
                    TotalRecords = DB.QuerySql<int>(queryCount, model).FirstOrDefault();


                    returnModel.totalDocs = TotalRecords;
                    returnModel.limit = 10;
                    returnModel.totalPages = (TotalRecords / 10) + ((TotalRecords % 10) > 0 ? 1 : 0);
                    returnModel.hasNextPage = model.page < returnModel.totalPages;
                    returnModel.hasPrevPage = returnModel.totalPages > 1 && model.page > 1;
                    returnModel.page = model.page;
                    returnModel.nextPage = model.page + 1;
                    returnModel.pagingCounter = recoardFrom;
                    returnModel.prevPage = model.page - 1;
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
        public Response<List<CategoryModel>> GetCategoryList()
        {
            Response<List<CategoryModel>> returnModel = new Response<List<CategoryModel>>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<CategoryModel>("SELECT id, name, image, status FROM service_category where is_deleted = 0 and status = 1 ").ToList();
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
        public Response<CategoryModel> GetCategory(CategoryModel model)
        {
            string ReturnLink = string.Empty;

            Response<CategoryModel> returnModel = new Response<CategoryModel>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<CategoryModel>(@"SELECT id, name, image, status FROM service_category where id = @id ", new { id = model.id }).FirstOrDefault();
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "";
                //LoggingRepository.SaveException(ex);
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                LoggingRepository.SaveException(ex);
            }
            return returnModel;
        }
        public Response<CategoryModel> DeleteCategory(CategoryModel model)
        {
            string ReturnLink = string.Empty;

            Response<CategoryModel> returnModel = new Response<CategoryModel>();
            try
            {

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql(@"update service_category set is_deleted = 1 where id = @id ", new { id = model.id });
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = Resource_Kharban.DeletedSuccessfully;
                //LoggingRepository.SaveException(ex);
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                LoggingRepository.SaveException(ex);
            }
            return returnModel;
        }
        public Response<CategoryModel> UpdateStatusCategory(CategoryModel model)
        {
            string ReturnLink = string.Empty;

            Response<CategoryModel> returnModel = new Response<CategoryModel>();
            try
            {

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql(@"update service_category set status = @status where id = @id ", new { id = model.id, status = model.status });
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = Resource_Kharban.UpdateSuccessfully;
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                LoggingRepository.SaveException(ex);
            }
            return returnModel;
        }
        public Response<CategoryModel> UpdateCategory(CategoryModel model,string imgPath, string imageURL)
        {
            string ReturnLink = string.Empty;
            Response<CategoryModel> returnModel = new Response<CategoryModel>();
            try
            {
                if (!string.IsNullOrEmpty(model.image) && !string.IsNullOrEmpty(model.image_extension))
                {
                    byte[] imageBytes = Convert.FromBase64String(model.image);
                    File.WriteAllBytes(imgPath, imageBytes);
                }

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql(@"update service_category set name = @name, status = @status, arabic_name = @arabic_name where id = @id ", new
                    {
                        id = model.id,
                        name = model.name.FirstLetterCapitalization(),
                        model.status,
                        arabic_name = model.arabic_name
                    });

                    if (!string.IsNullOrEmpty(model.image) && !string.IsNullOrEmpty(model.image_extension))
                    {
                        DB.ExecuteSql(@"update service_category set image = @image where id = @id ", new
                        {
                            id = model.id,
                            image = imageURL
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                LoggingRepository.SaveException(ex);
            }
            return returnModel;
        }
        public Response<CategoryModel> AddCategory(CategoryModel model, string imgPath, string imageURL)
        {
            string ReturnLink = string.Empty;
            Response<CategoryModel> returnModel = new Response<CategoryModel>();

            try
            {
                if (!string.IsNullOrEmpty(model.image) && !string.IsNullOrEmpty(model.image_extension))
                {
                    byte[] imageBytes = Convert.FromBase64String(model.image);
                    File.WriteAllBytes(imgPath, imageBytes);
                }
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql(@"insert into service_category values(@id,@name,@image,@status,@is_deleted,@created,@modified,@arabic_name) ", new
                    {
                        id = Guid.NewGuid().ToString(),
                        image = imageURL,
                        name = model.name.FirstLetterCapitalization(),
                        model.status,
                        is_deleted = 0,
                        modified = DateTime.Now,
                        created = DateTime.Now,
                        arabic_name = model.arabic_name
                    });
                }

                returnModel.success = true;
                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = Resource_Kharban.InsertSuccessfully;
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.success = false;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                LoggingRepository.SaveException(ex);
            }
            return returnModel;
        }

    }
}