using System;
using System.Linq;
using Insight.Database;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using Nexmo.Api;
using Kharban_AdminAPI.Helper;
using Kharban_AdminAPI.CommonClasses;
using Microsoft.AspNetCore.Hosting;

namespace Kharban_AdminAPI.Repository
{
    
    public class ProviderRepository
    {
       

        public ResponseList<List<ProviderModel>> GetProviders(RequestModel model)
        {

            string ReturnLink = string.Empty;

            string queryString = string.Empty;
            string orderbyString = string.Empty;
            string queryCount = string.Empty;
            int TotalRecords = 0;
            int recoardFrom = ((model.page - 1) * 10) + 1;
            int recoardTo = model.page * 10;
            ResponseList<List<ProviderModel>> returnModel = new ResponseList<List<ProviderModel>>();
            try
            {
                if (model != null)
                {
                    using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                    {
                        if (!string.IsNullOrEmpty(model.sortby))
                        {
                            if (model.sortby == "atoz")
                                orderbyString = " order by first_name ";
                            if (model.sortby == "ztoa")
                                orderbyString = " order by first_name desc ";
                            if (model.sortby == "newtoold")
                                orderbyString = " order by created desc";
                            if (model.sortby == "oldtonew")
                                orderbyString = " order by created";
                        }
                        else
                            orderbyString = " order by created desc";

                        queryCount = @"SELECT count(id) totalrecord FROM provider where is_deleted = 0 ";
                        queryString = @"select * from (SELECT ROW_NUMBER() OVER (" + orderbyString + @") row_num, id, user_id, first_name, last_name, profile_picture, service_category_id, iban_no, contact_no, country_code, otp, email, is_policy_accepted, is_mobile_verified, is_email_verified, admin_approve, is_online, is_notification_enable, auth_token, created_by, status, created, modified FROM provider where is_deleted = 0 ";


                        if (!string.IsNullOrEmpty(model.filterby) && !string.IsNullOrEmpty(model.keyword))
                        {
                            if (model.filterby == "name")
                            {
                                queryString += " and ((provider.first_name+' '+provider.last_name like @keyword) or first_name like @keyword or last_name like @keyword) ";
                                queryCount += " and ((provider.first_name+' '+provider.last_name like @keyword) or first_name like @keyword or last_name like @keyword) ";
                            }
                            if (model.filterby == "contact_number")
                            {
                                queryString += " and contact_no like @keyword ";
                                queryCount += " and contact_no like @keyword ";
                            }

                        }

                        if (!string.IsNullOrEmpty(model.keyword2))
                        {
                            queryString += " and status = @keyword2 ";
                            queryCount += " and status = @keyword2 ";
                        }

                        queryString += " ) t where row_num between " + recoardFrom + " and " + recoardTo;

                        returnModel.result = DB.QuerySql<ProviderModel>(queryString, new
                        {
                            keyword = "%" + model.keyword + "%",
                            keyword2 = model.keyword2
                        }).ToList();
                        TotalRecords = DB.QuerySql<int>(queryCount, new
                        {
                            keyword = "%" + model.keyword + "%",
                            keyword2 = model.keyword2
                        }).FirstOrDefault();

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
                    returnModel.msg = "Provider list";
                    returnModel.success = true;
                }
                else
                {
                    returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                    returnModel.msg = "Please submit proper data";
                    returnModel.success = false;
                }

            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                //LoggingRepository.SaveException(ex);
                returnModel.success = false;
            }
            return returnModel;
        }
        public ResponseList<List<ProviderModel>> GetProviderRequestList(RequestModel model)
        {
            // Not in use currently

            string ReturnLink = string.Empty;

            string queryString = string.Empty;
            string queryCount = string.Empty;
            int TotalRecords = 0;
            ResponseList<List<ProviderModel>> returnModel = new ResponseList<List<ProviderModel>>();
            try
            {
                if (model != null)
                {
                    using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                    {
                        queryCount = @"SELECT count(id) totalrecord FROM provider where is_deleted = 0 ";
                        queryString = @"SELECT top 10 id,user_id, first_name, last_name, profile_picture, iban_no, contact_no, country_code, otp, email, is_policy_accepted, is_mobile_verified, is_email_verified, admin_approve, is_online, is_notification_enable, auth_token, created_by, status, created, modified FROM provider where is_deleted = 0 ";


                        if (!string.IsNullOrEmpty(model.filterby) && !string.IsNullOrEmpty(model.keyword))
                        {
                            if (model.filterby == "name")
                            {
                                queryString += " and (first_name like @keyword or last_name like @keyword) ";
                                queryCount += " and (first_name like @keyword or last_name like @keyword) ";
                            }

                            if (model.filterby == "contact_number")
                            {
                                queryString += " and contact_no like @keyword ";
                                queryCount += " and contact_no like @keyword ";
                            }
                        }

                        if (!string.IsNullOrEmpty(model.sortby))
                        {
                            if (model.sortby == "atoz")
                                queryString += " order by first_name ";

                            if (model.sortby == "ztoa")
                                queryString += " order by first_name desc ";

                            if (model.sortby == "newtoold")
                                queryString += " order by created desc";

                            if (model.sortby == "oldtonew")
                                queryString += " order by created";
                        }
                        else
                            queryString += " order by created desc";

                        returnModel.result = null;
                        TotalRecords = 0;


                        returnModel.totalDocs = TotalRecords;
                        returnModel.limit = 10;
                        returnModel.totalPages = TotalRecords / 10 + (TotalRecords % 10) > 0 ? 1 : 0;

                        returnModel.page = 1;


                    }
                    returnModel.status = (int)EnumClass.ResponseState.Success;
                    returnModel.msg = "Provider Request List";
                    returnModel.success = true;
                }
                else
                {
                    returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                    returnModel.msg = "Please submit proper data";
                    returnModel.success = false;
                }

            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                //LoggingRepository.SaveException(ex);
                returnModel.success = false;
            }
            return returnModel;
        }
        public Response<ProviderModel> GetProvider(ProviderModel model)
        {
            string ReturnLink = string.Empty;

            Response<ProviderModel> returnModel = new Response<ProviderModel>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<ProviderModel>(@"SELECT provider.id, user_id, first_name, last_name, iban_no, profile_picture, document_image,  
                                            iban_image, contact_no, country_code, Otp, email, is_mobile_verified, is_email_verified, provider.created_by, provider.Status, 
                                            provider.created, provider.modified, service_category.name as service_category_name 
                                            FROM provider 
                                            left join service_category on service_category.id = service_category_id
                                            where provider.id = @id ", new { id = model.id }).FirstOrDefault();
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Provider data";
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
        public Response<ProviderModel> DeleteProvider(ProviderModel model)
        {
            string ReturnLink = string.Empty;

            Response<ProviderModel> returnModel = new Response<ProviderModel>();
            try
            {

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql(@"update provider set is_deleted = 1 where id = @id ", new { id = model.id });
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = Resource_Kharban.DeletedSuccessfully;
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
        public Response<ProviderModel> UpdateProviderStatus(ProviderModel model)
        {
            string ReturnLink = string.Empty;

            Response<ProviderModel> returnModel = new Response<ProviderModel>();
            try
            {

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql(@"update provider set status = @status where id = @id ", new { id = model.id, status = model.status });
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = Resource_Kharban.DeletedSuccessfully;
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
        public Response<ProviderModel> UpdateProvider(ProviderModel model,string imgPath,string imageURL)
        {
            string ReturnLink = string.Empty;
            
            Response<ProviderModel> returnModel = new Response<ProviderModel>();
            try
            {
                if (!string.IsNullOrEmpty(model.image) && !string.IsNullOrEmpty(model.image_extension))
                {
                    byte[] imageBytes = Convert.FromBase64String(model.image);
                    File.WriteAllBytes(imgPath, imageBytes);
                }

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql(@"update provider set first_name = @first_name, last_name = @last_name, country_code = @country_code, contact_no = @contact_no, iban_no = @iban_no, service_category_id = @service_category_id where id = @id ", new
                    {
                        id = model.id,
                        model.first_name,
                        model.last_name,
                        model.contact_no,
                        model.iban_no,
                        model.service_category_id,
                        model.country_code
                    });

                    if (!string.IsNullOrEmpty(model.image) && !string.IsNullOrEmpty(model.image_extension))
                    {
                        DB.ExecuteSql(@"update provider set  profile_picture = @profile_picture where id = @id ", new
                        {
                            id = model.id,
                            profile_picture = imageURL,
                        });
                    }
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = Resource_Kharban.UpdateSuccessfully;
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
        public Response<ProviderModel> ApproveRetailer(ProviderModel model)
        {
            string ReturnLink = string.Empty;
            var phone = string.Empty;
            Response<ProviderModel> returnModel = new Response<ProviderModel>();
            try
            {
                    
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql(@"update provider set admin_approve = @admin_approve where id = @id ", new
                    {
                        id = model.id,
                        admin_approve = model.admin_approve,
                    });

                    phone =  DB.QuerySql<string>(@"Select country_code + contact_no as phone, admin_approve from provider where id= @id", new
                    {
                        id = model.id

                    }).FirstOrDefault();
                }
                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = Resource_Kharban.UpdateSuccessfully;
                returnModel.success = true;

                if(!string.IsNullOrEmpty(phone))
                {

                    if (model.admin_approve == 1)
                    {
                        sendSMS("Kharban", $"{model.country_code}{model.contact_no}", "Your account is approved, You can login now");
                    }
                    else
                    {
                        sendSMS("Kharban", $"{model.country_code}{model.contact_no}", "Your account is rejected.");
                    }
                }
                string DeviceId = string.Empty;
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DeviceId = DB.QuerySql<string>("select device_id from device_data where user_id = @UserId and user_active = 1", new { UserId = model.id }).FirstOrDefault();
                    if (model.admin_approve == 1)
                    {
                        var notification = "Congratulations! Your account has been approved by the admin, you can now access the complete features of Kharban.";
                        SMSNotification.PushNotificationAsync(DeviceId, "Kharban", notification, "Home", 1);
                    }
                    else
                    {
                        var notification = "We are sorry, we can not approve your account at the moment, for any query please contact the admin.";
                        SMSNotification.PushNotificationAsync(DeviceId, "Kharban", notification, "Home", 1);
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

        public Response<string> UploadProviderImage(ProviderModel model, string imageURL, string imgPath)
        {
            Response<string> returnModel = new Response<string>();
            try
            {
                if (!string.IsNullOrEmpty(model.image) && !string.IsNullOrEmpty(model.image_extension))
                {
                    byte[] imageBytes = Convert.FromBase64String(model.image);
                    File.WriteAllBytes(imgPath, imageBytes);
                }
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    if (model.ImageType == "profile")
                    {
                        DB.ExecuteSql(@"update provider set  profile_picture = @profile_picture where id = @id ", new
                        {
                            id = model.id,
                            profile_picture = imageURL,
                        });
                    }
                    else if (model.ImageType == "iban")
                    {
                        DB.ExecuteSql(@"update provider set  iban_image = @iban_image where id = @id ", new
                        {
                            id = model.id,
                            iban_image = imageURL,
                        });
                    }
                    else if (model.ImageType == "document")
                    {
                        DB.ExecuteSql(@"update provider set  document_image = @document_image where id = @id ", new
                        {
                            id = model.id,
                            document_image = imageURL,
                        });
                    }


                    
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = Resource_Kharban.UpdateSuccessfully;
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

        public Response<string> UploadCustomerImage(CustomerModel model,string imageURL,string imgPath)
        {
            Response<string> returnModel = new Response<string>();
            try
            {
                if (!string.IsNullOrEmpty(model.image) && !string.IsNullOrEmpty(model.image_extension))
                {
                    byte[] imageBytes = Convert.FromBase64String(model.image);
                    File.WriteAllBytes(imgPath, imageBytes);
                }
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    if (model.ImageType == "profile")
                    {
                        DB.ExecuteSql(@"update customer set  profile_picture = @profile_picture where id = @id ", new
                        {
                            id = model.id,
                            profile_picture = imageURL,
                        });
                    }
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = Resource_Kharban.UpdateSuccessfully;
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

        private void sendSMS(string from,string to,string message)
        {
            var client = new Client(creds: new Nexmo.Api.Request.Credentials
            {
                ApiKey = "576e1832",
                ApiSecret = "CccV2HeQs9ugVz5B"
            });
            var results = client.SMS.Send(request: new SMS.SMSRequest
            {
                from = "Octal Test",
                to = "966505117115",
                text = "Hello from Octal Team"
            });
        }
    }
}