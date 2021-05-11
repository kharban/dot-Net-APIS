using System;
using System.Linq;
using Insight.Database;
using System.Data.SqlClient;
using System.Collections.Generic;
using Kharban_AdminAPI.Helper;

namespace Kharban_AdminAPI.Repository
{
    public class FaqRepository
    {
        public Response<List<FaqModel>> GetFaqList(FaqModel model)
        {
            Response<List<FaqModel>> returnModel = new Response<List<FaqModel>>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<FaqModel>("SELECT id, question, answer, question_arabic, answer_arabic, status, is_deleted, created, modified, faq_for from faq where is_deleted = 0 and faq_for = @FaqFor ", new { FaqFor = model.faq_for}).ToList();
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "FAQ List";
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

        public Response<FaqModel> GetFaq(FaqModel model)
        {
            string ReturnLink = string.Empty;

            Response<FaqModel> returnModel = new Response<FaqModel>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<FaqModel>(@"SELECT id, question, answer, question_arabic, answer_arabic, status, is_deleted, created, modified, faq_for from faq where id = @id ", new { id = model.id }).FirstOrDefault();
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "";
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                LoggingRepository.SaveException(ex);
            }
            return returnModel;
        }

        public Response<FaqModel> DeleteFaq(FaqModel model)
        {
            string ReturnLink = string.Empty;

            Response<FaqModel> returnModel = new Response<FaqModel>();
            try
            {

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql(@"update faq set is_deleted = 1 where id = @id ", new { id = model.id });
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = Resource_Kharban.DeletedSuccessfully;
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                LoggingRepository.SaveException(ex);
            }
            return returnModel;
        }

        public Response<FaqModel> UpdateStatusFaq(FaqModel model)
        {
            string ReturnLink = string.Empty;

            Response<FaqModel> returnModel = new Response<FaqModel>();
            try
            {

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql(@"update faq set status = @status where id = @id ", new { id = model.id, status = model.status });
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

        public Response<FaqModel> UpdateFaq(FaqModel model)
        {
            string ReturnLink = string.Empty;

            Response<FaqModel> returnModel = new Response<FaqModel>();
            try
            {

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql(@"update faq set question = @question, answer = @answer, question_arabic = @question_arabic, answer_arabic = @answer_arabic where id = @id ", new
                    {
                        id = model.id,
                        model.question,
                        model.question_arabic,
                        model.answer,
                        model.answer_arabic,
                    });
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

        public Response<FaqModel> AddFaq(FaqModel model)
        {
            Response<FaqModel> returnModel = new Response<FaqModel>();
            try
            {

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql(@"insert into faq values(@id,@question,@question_arabic,@answer,@answer_arabic,@status,@is_deleted,@created,@modified,@faq_for) ", new
                    {
                        id = Guid.NewGuid().ToString(),
                        model.question,
                        model.question_arabic,
                        model.answer,
                        model.answer_arabic,
                        model.status,
                        model.faq_for,
                        is_deleted = 0,
                        modified = DateTime.Now,
                        created = DateTime.Now,
                    });
                }

                returnModel.success = true;
                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = Resource_Kharban.InsertSuccessfully;
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                LoggingRepository.SaveException(ex);
            }
            return returnModel;
        }

    }
}