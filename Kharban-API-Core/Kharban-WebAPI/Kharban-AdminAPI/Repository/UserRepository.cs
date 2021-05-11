using System;
using System.Linq;
using Insight.Database;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics;
using Kharban_AdminAPI.Helper;
using Kharban_AdminAPI.Models;
using Kharban_AdminAPI.CommonClasses;

namespace Kharban_AdminAPI.Repository
{
    public class UserRepository
    {
        public static string GenerateTokenForChangepassword()
        {
            int size = 32;
            //string shortCode = string.Empty;
            StringBuilder result = new StringBuilder(size);

            try
            {
                char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
                byte[] data = new byte[size];
                using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
                {
                    crypto.GetBytes(data);
                }
                foreach (byte b in data)
                {
                    result.Append(chars[b % (chars.Length)]);
                }
            }
            catch (Exception ex)
            {
                LoggingRepository.SaveException(ex);
            }
            return result.ToString();
        }

        public Response<List<CountryModel>> GetCountry(RequestModel model)
        {
            string ReturnLink = string.Empty;

            Response<List<CountryModel>> returnModel = new Response<List<CountryModel>>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<CountryModel>(@"SELECT id, country_code, country, country_flag_icon FROM country ").ToList();
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Country List";
                returnModel.success = true;
                //LoggingRepository.SaveException(ex);
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                //LoggingRepository.SaveException(ex);
            }
            return returnModel;
        }

        public ResponseList<List<NotificationModel>> GetNotifications(NotificationModel model)
        {
            string ReturnLink = string.Empty;

            string queryString = string.Empty;
            string orderbyString = string.Empty;
            string queryCount = string.Empty;
            int TotalRecords = 0;
            int recoardFrom = ((model.page - 1) * 10) + 1;
            int recoardTo = model.page * 10;
            ResponseList<List<NotificationModel>> returnModel = new ResponseList<List<NotificationModel>>();
            try
            {

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    if (model != null)
                    {
                        if (!string.IsNullOrEmpty(model.sortby))
                        {
                            if (model.sortby == "newtoold")
                                orderbyString = " order by created desc";

                            if (model.sortby == "oldtonew")
                                orderbyString = " order by created";
                        }
                        else
                            orderbyString = " order by created desc";

                        queryCount = @"SELECT count(id) totalrecord FROM notification where is_deleted = 0 ";
                        queryString = @"select * from (SELECT ROW_NUMBER() OVER (" + orderbyString + @") row_num, id, user_id, notification, notification_type, 
                                        for_admin, status, is_deleted, created, modified FROM notification where is_deleted = 0 ";

                        if (!string.IsNullOrEmpty(model.filterby) && !string.IsNullOrEmpty(model.keyword))
                        {
                            if (model.filterby == "type")
                            {
                                queryString += " and notification_type = @keyword ";
                                queryCount += " and notification_type = @keyword ";
                            }

                        }

                        queryString += " ) t where row_num between " + recoardFrom + " and " + recoardTo;

                        returnModel.result = DB.QuerySql<NotificationModel>(queryString, new
                        {
                            keyword = model.keyword,
                        }).ToList();


                        TotalRecords = DB.QuerySql<int>(queryCount, new
                        {
                            keyword = model.keyword,
                        }).FirstOrDefault();
                    }

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
                returnModel.msg = "Notification List";
                returnModel.success = true;

                returnModel.totalDocs = returnModel.result.Count;
                returnModel.limit = 10;
                returnModel.totalPages = returnModel.totalDocs / 10 + (returnModel.totalDocs % 10) > 0 ? 1 : 0;

                returnModel.page = 1;


                //LoggingRepository.SaveException(ex);
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                //LoggingRepository.SaveException(ex);
            }
            return returnModel;
        }

        public async System.Threading.Tasks.Task<Response<PushNotificationModel>> SendPushNotifications(PushNotificationModel model)
        {

            int TotalRecords = 0;
            Response<PushNotificationModel> returnModel = new Response<PushNotificationModel>();

            if (string.IsNullOrEmpty(model.customer_id) && string.IsNullOrEmpty(model.provider_id) && !model.receiver_all)
            {
                returnModel.msg = "Please select the receiver";
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                returnModel.success = false;
                return returnModel;
            }

            if (string.IsNullOrEmpty(model.text))
            {
                returnModel.msg = "Please enter the message";
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                returnModel.success = false;
                return returnModel;
            }

            using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
            {
                List<string> Receivers = new List<string>();
                List<string> Receivers1 = new List<string>();
                string DeviceId = string.Empty;




                if (model.receiver_all)
                {
                    //send notification to customer
                    Receivers = DB.QuerySql<string>(@"SELECT id FROM customer where is_deleted = 0 ").ToList();

                    if (Receivers != null)
                    {

                        foreach (var item in Receivers)
                        {
                            
                            DeviceId = DB.QuerySql<string>("select device_id from device_data where user_id = @UserId and user_active = 1", new { UserId = item }).FirstOrDefault();
                            if (DeviceId != null)
                            {
                                await SMSNotification.PushNotificationAsync(DeviceId, "Kharban", model.subject + Environment.NewLine + model.text, "Home", 1);
                            }
                        }
                    }


                    //send notification to provider
                    Receivers1 = DB.QuerySql<string>(@"SELECT id FROM provider where is_deleted = 0 and is_online = 1 ").ToList();

                    if (Receivers1 != null)
                    {
                        foreach (var item in Receivers1)
                        {
                            DeviceId = DB.QuerySql<string>("select device_id from device_data where user_id = @UserId and user_active = 1", new { UserId = item }).FirstOrDefault();
                            if (DeviceId != null)
                            {
                                await SMSNotification.PushNotificationAsync(DeviceId, "Kharban", model.subject + Environment.NewLine + model.text, "Home", 1);
                            }
                        }
                    }
                }
                if (model.customer_id != string.Empty)
                {
                    DeviceId = DB.QuerySql<string>("select device_id from device_data where user_id = @UserId and user_active = 1", new { UserId = model.customer_id }).FirstOrDefault();
                    if (DeviceId != null)
                    {
                       await SMSNotification.PushNotificationAsync(DeviceId, "Kharban", model.subject + Environment.NewLine + model.text, "Home", 1);
                    }
                }

                if (model.provider_id != string.Empty)
                {
                    DeviceId = DB.QuerySql<string>("select device_id from device_data where user_id = @UserId and user_active = 1", new { UserId = model.provider_id }).FirstOrDefault();
                    if (DeviceId != null)
                    {
                       await SMSNotification.PushNotificationAsync(DeviceId, "Kharban", model.subject + Environment.NewLine + model.text, "Home", 1);
                    }
                }
            }


            returnModel.status = (int)EnumClass.ResponseState.Success;
            returnModel.success = true;

            return returnModel;
        }

        public Response<string> SaveOTPForChangePwdByAdmin(int otp, string Email)
        {

            Response<string> returnModel = new Response<string>();
            try
            {
                string token = GenerateTokenForChangepassword();
                string isExistEmail = "";
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    isExistEmail = DB.QuerySql<string>("select id from admin_user where email = @email ", new { email = Email }).FirstOrDefault();

                    if (string.IsNullOrEmpty(isExistEmail))
                    {
                        returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                        returnModel.msg = "Email is not available in our database. ";
                        returnModel.success = false;
                    }
                    else
                    {
                        DB.ExecuteSql(@"update admin_user set otp = @OTP, otp_token = @Token where email = @email ", new { OTP = otp, email = Email, Token = token });
                        returnModel.status = (int)EnumClass.ResponseState.Success;
                        returnModel.msg = "OTP has been sent to your mail id. ";
                        returnModel.success = true;
                        returnModel.result = token;
                    }
                }

            }
            catch (Exception ex)
            {
                //LoggingRepository.SaveException(ex);
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                returnModel.msg = ex.Message;
                returnModel.success = false;
            }
            return returnModel;
        }

        public Response<int> AdminChangePasswordByOTP(UserLoginResponse model)
        {

            Response<int> returnModel = new Response<int>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    string UserId = DB.QuerySql<string>("select id from admin_user where otp_token = @Token ", new { Token = model.forgotToken }).FirstOrDefault();

                    if (string.IsNullOrEmpty(UserId))
                    {
                        returnModel.msg = "Something went wrong, please try again";
                        returnModel.success = false;
                        returnModel.status = (int)EnumClass.ResponseState.TokenExpired;
                    }
                    else
                    {
                        UserId = DB.QuerySql<string>("select id from admin_user where otp = @otp and otp_token = @Token ", new { otp = model.otp, Token = model.forgotToken }).FirstOrDefault();

                        if (string.IsNullOrEmpty(UserId))
                        {
                            returnModel.msg = "Please enter valid OTP";
                            returnModel.success = false;
                            returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                        }
                        else
                        {
                            DB.ExecuteSql(@"update admin_user set password = @password, otp_token = '' where id = @id ", new { id = UserId, password = model.password });

                            returnModel.status = (int)EnumClass.ResponseState.Success;
                            returnModel.msg = "Password has been changed successfully. ";
                            returnModel.success = true;
                        }
                    }



                }
            }
            catch (Exception ex)
            {
                //LoggingRepository.SaveException(ex);
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                returnModel.msg = ex.Message;
                returnModel.success = false;
            }
            return returnModel;
        }

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
            }

            return returnModel;
        }

        public Response<int> UpdateContent(ContentModel model)
        {
            Response<int> returnModel = new Response<int>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql(@"update content set description = @Description where id = @Id ", new { Id = model.id, Description = model.description });
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = Resource_Kharban.UpdateSuccessfully;
                returnModel.success = true;
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
            }

            return returnModel;
        }
    }
}