using System;
using System.Linq;
using Insight.Database;
using System.Data.SqlClient;
using System.Collections.Generic;
using Kharban_WebAPI.Models;
using Kharban_WebAPI.Helper;

namespace Kharban_WebAPI.Repository
{
    public class SettingRepository
    {
      
        public Response<ContentModel> GetContent(ContentModel model)
        {
            Response<ContentModel> returnModel = new Response<ContentModel>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<ContentModel>(@"SELECT id, description, created, modified FROM content where content_type = @ContentType ", new { ContentType = model.content_type }).FirstOrDefault();
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Content";
                returnModel.success = true;
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                LoggingRepository.SaveException(ex);
            }

            return returnModel;
        }

        public Response<List<FaqModel>> GetFaqList(FaqModel model)
        {
            Response<List<FaqModel>> returnModel = new Response<List<FaqModel>>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<FaqModel>("SELECT id, question, answer, question_arabic, answer_arabic, status, is_deleted, created, modified, faq_for from faq where is_deleted = 0 and faq_for = @FaqFor ", new { FaqFor = model.faq_for }).ToList();
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

        public Response<List<NotificationModel>> GetNotifications(string UserId)
        {
            Response<List<NotificationModel>> returnModel = new Response<List<NotificationModel>>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<NotificationModel>(@"SELECT id, user_id, notification, notification_type, 
                                        for_admin, status, is_deleted, created, modified FROM notification where is_deleted = 0 and user_id = @UserId order by created desc", new
                    {
                        UserId = UserId
                    }).ToList();

                }
                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Notification List";
                returnModel.success = true;
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                LoggingRepository.SaveException(ex);
            }
            return returnModel;
        }

        public Response<int> DeleteNotification(NotificationModel model)
        {
            string ReturnLink = string.Empty;

            string queryString = string.Empty;
            Response<int> returnModel = new Response<int>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    if (model != null)
                    {
                        DB.ExecuteSql(@"delete FROM notification where id = @Id ", new { Id = model.id });
                    }

                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Notification Deleted Successfully";
                returnModel.success = true;
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                LoggingRepository.SaveException(ex);
            }
            return returnModel;
        }

        public async System.Threading.Tasks.Task SaveNotification(NotificationModel model)
        {
            try
            {
                string guid = Guid.NewGuid().ToString();
                var perameter = new
                {
                    id = guid,
                    receiver_id = model.receiver_id,
                    user_id = model.receiver_id,
                    sender_id = model.sender_id,
                    model.notification,
                    notification_type = 0,
                    for_admin = 0,
                    created = DateTime.UtcNow,
                    modified = DateTime.UtcNow,
                    status = 1,
                    is_deleted = 0,
                };
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                   await DB.ExecuteSqlAsync(@"insert into notification(id, user_id, sender_id, receiver_id, notification, notification_type, for_admin, status, is_deleted, created, modified) 
                                    Values(@id, @user_id, @sender_id, @receiver_id, @notification, @notification_type, @for_admin, @status, @is_deleted, @created, @modified)", perameter);
                }
            }
            catch (Exception ex)
            {
                LoggingRepository.SaveException(ex);
            }
        }

    }


}