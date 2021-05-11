using System;
using System.Collections.Generic;
using System.Linq;
using Insight.Database;
using System.Data;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Data.SqlClient;
using Kharban_WebAPI.Models;
using Kharban_WebAPI;
using Kharban_WebAPI.Models.User;
using Kharban_WebAPI.Repository;
using Kharban_WebAPI.Common;
using System.IdentityModel.Tokens.Jwt;
using Nexmo.Api;
using Kharban_WebAPI.Helper;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using RestSharp;

namespace Kharban_WebAPI.Repository
{
    public class UserRepository
    {
        private void sendSMS(string from, string to, string message)
        {
            //pending


            var client = new Client(creds: new Nexmo.Api.Request.Credentials
            {
                ApiKey = "576e1832",
                ApiSecret = "CccV2HeQs9ugVz5B"
            });
            var results = client.SMS.Send(request: new SMS.SMSRequest
            {
                from = from,
                to = to,
                text = message
            });
        }


        public UserLoginModel GetOTP(UserLoginModel model)
        {
            UserLoginModel returnModel = new UserLoginModel();
            string queryString = string.Empty;
            string mobile = string.Empty;
            //Response<DashBoardChartModel> returnModel = new Response<DashBoardChartModel>();
            //returnModel.result = new DashBoardChartModel();
            try
            {
                int OTP = new Random().Next(1000, 9999);

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    string msg = "Please use the below One Time Password to access the application";
                    if (model.requesttype.ToLower() == "customer")
                    {
                        if (model.contact_no == "1231231230")
                        {
                            OTP = 1234;
                        }
                        queryString = @"select contact_no, id userid, country_code from customer where contact_no = @mobile";
                        returnModel = DB.QuerySql<UserLoginModel>(queryString, new { mobile = model.contact_no }).FirstOrDefault();
                        if (returnModel != null)
                        {
                            if (returnModel.country_code == model.country_code)
                            {
                                returnModel.contact_no = model.contact_no;
                                returnModel.country_code = model.country_code;
                                returnModel.usertype = "C";
                                returnModel.otp = OTP.ToString();
                                returnModel.isregistered = true;
                                returnModel.ismatchusertype = true;
                                returnModel.status = (int)EnumClass.ResponseState.Success;
                                returnModel.success = true;
                                DB.ExecuteSql("update customer set otp = @OTP where id = @id", new { OTP = OTP, id = returnModel.userid });
                                var smsTemplate = "Dear Customer </br> Please use the below One Time Password to access the application.</br> " + OTP + " </br> Thank You. </br>Team Kharban";
                                Utility.SendMsg(model.contact_no, smsTemplate);
                                sendSMS("Kharban", $"{model.country_code}{model.contact_no}", msg + "\n" + OTP.ToString() + " \nThank You \nTeam Kharban");
                                return returnModel;

                            }
                            else
                            {
                                returnModel = returnModel ?? new UserLoginModel();

                                returnModel.otp = OTP.ToString();
                                returnModel.isregistered = false;
                                returnModel.usertype = "C";
                                returnModel.ismatchusertype = false;
                                returnModel.message = "Country code does not match";
                                returnModel.status = (int)EnumClass.ResponseState.Failure;
                                returnModel.success = false;
                                return returnModel;
                            }
                        }
                        else
                        {
                            queryString = @"select contact_no, id userid from Provider where contact_no = @mobile";
                            returnModel = DB.QuerySql<UserLoginModel>(queryString, new { mobile = model.contact_no }).FirstOrDefault();
                            if (returnModel != null)
                            {
                                returnModel.contact_no = model.contact_no;
                                returnModel.isregistered = true;
                                returnModel.usertype = "P";
                                returnModel.ismatchusertype = false;
                                returnModel.otp = OTP.ToString();
                                returnModel.status = (int)EnumClass.ResponseState.Success;
                                returnModel.success = false;
                                returnModel.message = "You are registered as Provider!";
                                return returnModel;
                            }
                            else
                            {
                                DB.ExecuteSql("delete from unregistered_user_contact where contact_no = @mobile ", new { mobile = model.contact_no });

                                DB.ExecuteSql("insert into unregistered_user_contact values(@id,@mobile,@otp,@usertype) ",
                                    new
                                    {
                                        id = Guid.NewGuid().ToString(),
                                        mobile = model.contact_no,
                                        otp = OTP,
                                        usertype = (model.requesttype == "customer") ? 1 : (model.requesttype == "provider") ? 2 : 0
                                    });

                                returnModel = new UserLoginModel();

                                returnModel.otp = OTP.ToString();

                                returnModel.isregistered = false;
                                returnModel.ismatchusertype = false;
                                returnModel.message = "You are not registerd here.";
                                returnModel.status = (int)EnumClass.ResponseState.Success;
                                returnModel.success = false;
                                sendSMS("Kharban", $"{model.country_code}{model.contact_no}", msg + "\n" + OTP.ToString() + " \nThank You \nTeam Kharban");
                                return returnModel;
                            }
                        }
                    }
                    else if (model.requesttype.ToLower() == "provider")
                    {
                        if (model.contact_no == "1231231235")
                        {
                            OTP = 1234;
                        }

                        queryString = @"select contact_no, id userid, country_code,admin_approve approved_status from provider where contact_no = @mobile";
                        returnModel = DB.QuerySql<UserLoginModel>(queryString, new { mobile = model.contact_no }).FirstOrDefault();

                        if (returnModel != null)
                        {
                            if (returnModel.country_code == model.country_code)
                            {
                                returnModel.contact_no = model.contact_no;
                                returnModel.usertype = "P";
                                returnModel.otp = OTP.ToString();
                                returnModel.isregistered = true;
                                returnModel.ismatchusertype = true;
                                returnModel.status = (int)EnumClass.ResponseState.Success;
                                returnModel.success = true;
                                DB.ExecuteSql("update provider set otp = @OTP where id = @id", new { OTP = OTP, id = returnModel.userid });
                                sendSMS("Kharban", $"{model.country_code}{model.contact_no}", msg + "\n" + OTP.ToString() + " \nThank You \nTeam Kharban");
                                return returnModel;
                            }
                            else
                            {
                                returnModel.otp = OTP.ToString();
                                returnModel.isregistered = false;
                                returnModel.ismatchusertype = false;
                                returnModel.message = "Country code does not match";
                                returnModel.status = (int)EnumClass.ResponseState.Failure;
                                returnModel.success = false;
                                return returnModel;
                            }
                        }
                        else
                        {
                            queryString = @"select contact_no, id userid from customer where contact_no = @mobile";
                            returnModel = DB.QuerySql<UserLoginModel>(queryString, new { mobile = model.contact_no }).FirstOrDefault();
                            if (returnModel != null)
                            {
                                returnModel.contact_no = model.contact_no;
                                returnModel.isregistered = true;
                                returnModel.usertype = "C";
                                returnModel.ismatchusertype = false;
                                returnModel.otp = OTP.ToString();
                                returnModel.status = (int)EnumClass.ResponseState.Success;
                                returnModel.success = false;
                                returnModel.message = "You are registered as Customer!";
                                return returnModel;
                            }
                            else
                            {
                                DB.ExecuteSql("delete from unregistered_user_contact where contact_no = @mobile ", new { mobile = model.contact_no });

                                DB.ExecuteSql("insert into unregistered_user_contact values(@id,@mobile,@otp,@usertype) ",
                                    new
                                    {
                                        id = Guid.NewGuid().ToString(),
                                        mobile = model.contact_no,
                                        otp = OTP,
                                        usertype = (model.requesttype == "customer") ? 1 : (model.requesttype == "provider") ? 2 : 0
                                    });

                                returnModel = new UserLoginModel();

                                returnModel.otp = OTP.ToString();

                                returnModel.isregistered = false;
                                returnModel.ismatchusertype = false;
                                returnModel.message = "You are not registerd here.";
                                returnModel.status = (int)EnumClass.ResponseState.Success;
                                returnModel.success = false;
                                sendSMS("Kharban", $"{model.country_code}{model.contact_no}", msg + "\n" + OTP.ToString() + " \nThank You \nTeam Kharban");
                                return returnModel;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                returnModel.message = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                LoggingRepository.SaveException(ex);
                returnModel.success = false;
            }

            return returnModel;
        }


        public UserLoginModel AddCustomer(UserLoginModel model)
        {
            UserLoginModel returnModel = new UserLoginModel();
            string queryString = string.Empty;
            string mobile = string.Empty;
            string UserId = string.Empty;
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    string otp = DB.QuerySql<string>("select otp from unregistered_user_contact where contact_no = @mobile and user_type = 1 ", new { mobile = model.contact_no }).FirstOrDefault();
                    UserId = Guid.NewGuid().ToString();
                    string UserCode = GetUserCode("customer");
                    if (!string.IsNullOrEmpty(otp))
                    {
                        DB.ExecuteSql(@"insert into customer(id, user_id, first_name, last_name, contact_no, otp, email, is_email_verified, is_deleted, status, created, modified, country_code, is_tc_checked, latitude1, longitude1, address1, latitude2, longitude2, address2) 
                        values(@id, @user_id, @first_name, @last_name, @contact_no, @otp, @email, 1, 0, 1, @created, @modified, @country_code, @is_tc_checked, @latitude1, @longitude1, @address1, @latitude2, @longitude2, @address2)", new
                        {
                            id = UserId,
                            otp = model.otp,
                            model.first_name,
                            user_id = UserCode,
                            model.last_name,
                            model.is_tc_checked,
                            model.contact_no,
                            model.email,
                            model.country_code,
                            model.latitude1,
                            model.latitude2,
                            model.longitude1,
                            model.longitude2,
                            model.address1,
                            model.address2,
                            created = DateTime.Now,
                            modified = DateTime.Now
                        });


                        #region Upload Image

                        if (!string.IsNullOrEmpty(model.profile_picture) && !string.IsNullOrEmpty(model.profile_extension))
                        {
                            model.id = UserId;
                            model.ImageType = "profile";
                            model.image = model.profile_picture;
                            model.image_extension = model.profile_extension;
                            Get<Response<string>>("customerImagesUpload", model, Enumeration.WebMethod.POST, null);
                        }

                        #endregion

                        DB.ExecuteSql("delete from unregistered_user_contact where contact_no = @mobile ", new { mobile = returnModel.contact_no });

                        //update customer_code in configuration table
                        DB.ExecuteSql("update genrel_configuration set customer_user_code = @customer_user_code where id is not null ", new { customer_user_code = UserCode });

                        returnModel.id = UserId;
                        returnModel.first_name = model.first_name;
                        returnModel.contact_no = model.contact_no;
                        returnModel.status = (int)EnumClass.ResponseState.Success;
                        returnModel.success = true;
                        returnModel.message = "Registration Successfully";
                    }
                    else
                    {
                        returnModel.message = "Something went wrong";
                        returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                        returnModel.success = false;
                    }
                }
            }
            catch (Exception ex)
            {
                returnModel.message = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                LoggingRepository.SaveException(ex);
                returnModel.success = false;
            }

            return returnModel;
        }

        public UserLoginModel UpdateCustomer(UserLoginModel model, string UserId)
        {
            UserLoginModel returnModel = new UserLoginModel();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql(@"update customer set first_name = @first_name, last_name = @last_name, contact_no = @contact_no, modified = @modified, country_code = @country_code where id = @id", new
                    {
                        id = UserId,
                        model.first_name,
                        model.last_name,
                        model.contact_no,
                        model.country_code,
                        modified = DateTime.Now
                    });

                    if (!string.IsNullOrEmpty(model.profile_picture) && !string.IsNullOrEmpty(model.profile_extension))
                    {
                        model.id = UserId;
                        model.ImageType = "profile";
                        model.image = model.profile_picture;
                        model.image_extension = model.profile_extension;
                        Get<Response<string>>("customerImagesUpload", model, Enumeration.WebMethod.POST, null);
                    }

                    returnModel.status = (int)EnumClass.ResponseState.Success;
                    returnModel.success = true;
                    returnModel.message = "Updated Successfully";
                }
            }
            catch (Exception ex)
            {
                returnModel.message = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                LoggingRepository.SaveException(ex);
                returnModel.success = false;
            }

            return returnModel;
        }

        public UserLoginModel AddProvider(UserLoginModel model)
        {
            UserLoginModel returnModel = new UserLoginModel();
            string ImagePath = SiteKey.ImageURL;
            string queryString = string.Empty;
            string mobile = string.Empty;
            string imageURL = string.Empty;
            string imageName = string.Empty;
            string UserId = string.Empty;
            Response<string> returnImageUpload = new Response<string>();
            try
            {
                UserId = Guid.NewGuid().ToString();
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    string otp = DB.QuerySql<string>("select otp from unregistered_user_contact where contact_no = @mobile and user_type = 2 ", new { mobile = model.contact_no }).FirstOrDefault();

                    if (!string.IsNullOrEmpty(otp))
                    {
                        string UserCode = GetUserCode("provider");
                        DB.ExecuteSql(@"insert into provider(id, user_id, first_name, last_name, contact_no, service_category_id, otp, email, is_mobile_verified, is_email_verified, is_deleted, status, created, modified, country_code, iban_no, is_tc_checked, document_id, is_online, admin_approve) 
                        values(@id, @user_id, @first_name, @last_name, @contact_no, @service_category_id, @otp, @email, 1, 1, 0, 1, @created, @modified, @country_code, @iban_no, @is_tc_checked, @document_id, @is_online, @admin_approve)", new
                        {
                            id = UserId,
                            otp = model.otp,
                            user_id = UserCode,
                            model.first_name,
                            model.last_name,
                            model.service_category_id,
                            model.contact_no,
                            model.is_tc_checked,
                            model.email,
                            model.country_code,
                            model.iban_no,
                            model.document_id,
                            created = DateTime.Now,
                            modified = DateTime.Now,
                            is_online = 1,//temporary
                            admin_approve = 0//temporary
                        });


                        #region Upload Image

                        if (!string.IsNullOrEmpty(model.profile_picture) && !string.IsNullOrEmpty(model.profile_extension))
                        {
                            model.id = UserId;
                            model.ImageType = "profile";
                            model.image = model.profile_picture;
                            model.image_extension = model.profile_extension;
                            returnImageUpload = Get<Response<string>>("providerImagesUpload", model, Enumeration.WebMethod.POST, null);
                        }

                        if (!string.IsNullOrEmpty(model.document_image) && !string.IsNullOrEmpty(model.document_extension))
                        {
                            model.id = UserId;
                            model.ImageType = "document";
                            model.image = model.document_image;
                            model.image_extension = model.document_extension;
                            returnImageUpload = Get<Response<string>>("providerImagesUpload", model, Enumeration.WebMethod.POST, null);
                        }

                        if (!string.IsNullOrEmpty(model.iban_image) && !string.IsNullOrEmpty(model.iban_extension))
                        {
                            model.id = UserId;
                            model.ImageType = "iban";
                            model.image = model.iban_image;
                            model.image_extension = model.iban_extension;
                            returnImageUpload = Get<Response<string>>("providerImagesUpload", model, Enumeration.WebMethod.POST, null);
                        }

                        #endregion


                        DB.ExecuteSql("delete from unregistered_user_contact where contact_no = @mobile ", new { mobile = model.contact_no });

                        //update provider_code in configuration table
                        DB.ExecuteSql("update genrel_configuration set provider_user_code = @provider_user_code where id is not null ", new { provider_user_code = UserCode });

                        returnModel.id = UserId;
                        returnModel.first_name = model.first_name;
                        returnModel.contact_no = model.contact_no;
                        returnModel.status = (int)EnumClass.ResponseState.Success;
                        returnModel.success = true;
                        returnModel.message = "Registration Successfully";
                    }
                    else
                    {
                        returnModel.message = "Something went wrong";
                        returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                        returnModel.success = false;
                    }
                }
            }
            catch (Exception ex)
            {
                returnModel.message = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                LoggingRepository.SaveException(ex);
                returnModel.success = false;
            }

            return returnModel;
        }

        public Response<UserLoginModel> GetCustomer(UserLoginModel model, string UserId)
        {
            string queryString = string.Empty;
            Response<UserLoginModel> returnModel = new Response<UserLoginModel>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    if (model != null)
                    {
                        queryString = @"SELECT id, user_id, first_name, last_name, contact_no, otp, email, country_code, is_mobile_verified , is_email_verified ,is_deleted, created_by ,status, created, modified, address1, latitude1, longitude1, address2, latitude2, longitude2, profile_picture FROM customer where is_deleted = 0 and id = @UserId";

                        returnModel.result = DB.QuerySql<UserLoginModel>(queryString, new
                        {
                            UserId = string.IsNullOrEmpty(model.customer_id) ? UserId : model.customer_id
                        }).FirstOrDefault();
                    }

                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Customer Info";
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

        public Response<List<UserLoginModel>> GetProviders(UserLoginModel model)
        {
            string queryString = string.Empty;
            Response<List<UserLoginModel>> returnModel = new Response<List<UserLoginModel>>();
            try
            {
                if (model != null)
                {
                    using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                    {

                        Decimal distance;
                        distance = DB.QuerySql<decimal>(@"Select distance from setting", new { }).FirstOrDefault();
                        int radius = Convert.ToInt32(distance);

                        decimal radiusS = DB.QuerySql<decimal>("select distance from setting ").FirstOrDefault();
                        queryString = @"SELECT id, user_id, first_name, last_name,profile_picture, iban_no, latitude, longitude,contact_no, country_code, is_deleted, admin_approve, is_online, service_category_id, admin_approve as approved_status
  FROM  provider where  is_deleted = 0 and admin_approve = 1 and is_online = 1 and service_category_id = @service_category_id and((6371 * acos(cos(radians(latitude)) * cos(radians(@latitude))  * cos(radians(@longitude) - radians(longitude))  + sin(radians(latitude))   * sin(radians(@latitude))   )) < @radius)";

                        returnModel.result = DB.QuerySql<UserLoginModel>(queryString, new
                        {
                            service_category_id = model.service_category_id,
                            latitude = model.latitude,
                            longitude = model.longitude,
                            radius = radiusS

                        }).ToList();

                        string providerId = "'" + string.Join("','", returnModel.result.Select(x => x.provider_id)) + "'";

                        List<string> ProviderIds = DB.QuerySql<string>(@"select provider_id from booking where provider_id in (@providerId)
                                                and (booking_status = 3 or booking_status = 5 or booking_status = 6) ", new
                        {
                            providerId = providerId
                        }).ToList();

                        returnModel.result.ForEach(x => ProviderIds.Contains(x.provider_id));

                        for (int i = 0; i < returnModel.result.Count; i++)
                        {
                            //some code
                            if (ProviderIds.Contains(returnModel.result[i].provider_id))
                            {
                                returnModel.result.RemoveAt(i);
                            }
                        }



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
                LoggingRepository.SaveException(ex);
                returnModel.success = false;
            }
            return returnModel;
        }

        public Response<UserLoginModel> GetProviderInformation(UserLoginModel model, string UserId)
        {
            string queryString = string.Empty;
            Response<UserLoginModel> returnModel = new Response<UserLoginModel>();
            try
            {
                if (model != null)
                {
                    using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                    {
                        queryString = @"SELECT id,user_id, first_name, last_name, profile_picture, service_category_id,
                        iban_no, contact_no, country_code, otp, 
                        email, is_policy_accepted, is_mobile_verified, is_email_verified, admin_approve, 
                        is_online, is_notification_enable, auth_token, created_by, document_id, 
                        status, created, modified, address, latitude, longitude,
                        (Select sum(booking_amount) from booking where booking_status= 4 and provider_id = @ProviderId) as TotalEarning,
                        (select avg(provider_rating) from booking where provider_id = @ProviderId) as Rating
                        FROM provider where is_deleted = 0 and id = @ProviderId ";

                        returnModel.result = DB.QuerySql<UserLoginModel>(queryString, new
                        {
                            ProviderId = string.IsNullOrEmpty(model.provider_id) ? UserId : model.provider_id
                        }).FirstOrDefault();
                    }
                    returnModel.status = (int)EnumClass.ResponseState.Success;
                    returnModel.msg = "Provider Info";
                    returnModel.success = true;
                }
                else
                {
                    returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                    returnModel.msg = "Smoething went wrong";
                    returnModel.success = false;
                }

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

        public Response<ProviderModel> UpdateProviderInformation(ProviderModel model, string UserId)
        {
            string ReturnLink = string.Empty;
            string imageURL = string.Empty;
            Response<string> returnImageUpload = new Response<string>();
            Response<ProviderModel> returnModel = new Response<ProviderModel>();
            try
            {

                if (!string.IsNullOrEmpty(model.image) && !string.IsNullOrEmpty(model.image_extension))
                {

                    var parameter = new
                    {
                        id = UserId,
                        ImageType = "profile",
                        image = model.image,
                        image_extension = model.image_extension
                    };
                    returnImageUpload = Get<Response<string>>("providerImagesUpload", parameter, Enumeration.WebMethod.POST, null);
                }

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql(@"update provider set first_name = @first_name, last_name = @last_name, country_code = @country_code, contact_no = @contact_no, 
                                    iban_no = @iban_no, service_category_id = @service_category_id, latitude = @latitude, longitude = @longitude where id = @id ", new
                    {
                        id = UserId,
                        model.first_name,
                        model.last_name,
                        model.contact_no,
                        model.iban_no,
                        model.service_category_id,
                        model.latitude,
                        model.longitude,
                        model.country_code
                    });
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = Resource_Message.UpdatedSuccessfully;
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

        public Response<ProviderModel> UpdateProviderLocation(ProviderModel model, string UserId)
        {
            string ReturnLink = string.Empty;

            Response<ProviderModel> returnModel = new Response<ProviderModel>();
            try
            {

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql(@"update provider set latitude = @latitude, longitude = @longitude where id = @id ", new
                    {
                        id = UserId,
                        model.latitude,
                        model.longitude
                    });
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = Resource_Message.UpdatedSuccessfully;
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

        public Response<ProviderModel> UpdateProviderOnlineStatus(ProviderModel model, string UserId)
        {
            string ReturnLink = string.Empty;

            Response<ProviderModel> returnModel = new Response<ProviderModel>();
            try
            {

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql(@"update provider set is_online = @is_online where id = @id", new
                    {
                        id = UserId,
                        is_online = model.is_online
                    });
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = Resource_Message.UpdatedSuccessfully;
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

        public void SaveDeviceInfo(DeviceInfoModel model, string UserId)
        {
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    #region InsertDeviceInfo
                    if (model.device_id != null)
                    {

                        DB.ExecuteSql(@"DELETE from device_data  where user_id = @UserId ", new
                        {
                            UserId,

                        });
                        DB.ExecuteSql(@"INSERT INTO device_data
                           (id, user_id, device_id, device_type, user_active, device_version, created, modified)
                            VALUES (@id, @user_id, @device_id, @device_type, @user_active, @device_version, @created, @modified);", new
                        {
                            id = Guid.NewGuid().ToString(),
                            user_id = UserId,
                            model.device_id,
                            model.device_type,
                            user_active = 1,
                            model.device_version,
                            created = DateTime.UtcNow,
                            modified = DateTime.UtcNow,
                        });
                    }
                    else
                    {
                        DB.ExecuteSql(@"DELETE from device_data  where user_id = @UserId ", new
                        {
                            UserId,

                        });
                    }
                    #endregion
                }


            }
            catch (Exception ex)
            {
                LoggingRepository.SaveException(ex);
            }
        }

        private string GetUserCode(string UserType)
        {
            string AccountCode = "";

            using (SqlConnection Db = new SqlConnection(SiteKey.ConnectionString))
            {
                if (UserType == "customer")
                    AccountCode = Db.QuerySql<string>("select customer_user_code from genrel_configuration where id is not null ").FirstOrDefault();
                else if (UserType == "provider")
                    AccountCode = Db.QuerySql<string>("select provider_user_code from genrel_configuration where id is not null ").FirstOrDefault();
            }
            if (AccountCode != null)
            {
                try
                {
                    string code = AccountCode.Substring(0, 4);
                    string year = AccountCode.Substring(4, 4);
                    string count = AccountCode.Substring(8, 5);
                    if (DateTime.Now.Year.ToString() == year)
                    {
                        count = (int.Parse(count) + 1).ToString("00000");
                    }
                    else
                    {
                        year = DateTime.Now.Year.ToString();
                        count = "00001";
                    }
                    AccountCode = code + year + count;
                }
                catch (Exception ex)
                {
                    string msf = ex.Message;
                }

            }
            else
            {
                if (UserType == "Internal")
                    AccountCode = "CUST" + DateTime.Now.Year + "00001";
                else
                    AccountCode = "PROV" + DateTime.Now.Year + "00001";
            }
            return AccountCode;
        }

        public TModelResponse Get<TModelResponse>(string apiName, dynamic request1, Enumeration.WebMethod Method, string token, string apiUrl = null)
        {
            string restUrl = (apiUrl == null) ? SiteKey.ImageURL : apiUrl;

            object list = null;
            string URldata = restUrl + apiName;
            try
            {
                var content = APIBind.CasePOST(request1, URldata);
                var data = JsonConvert.DeserializeObject<TModelResponse>(content);
                if (data != null)
                    list = data;
                return (TModelResponse)list;
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                string responseData = string.Empty;
                if (ex.Status == WebExceptionStatus.ProtocolError && httpResponse != null)
                {
                    WebResponse response = ex.Response;
                    responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();
                }
                else
                {
                    responseData = JsonConvert.SerializeObject(new ResponseBaseModel() { Code = 500, Message = ex.Message, URLdata = apiName });
                }
                list = JsonConvert.DeserializeObject<TModelResponse>(responseData);

                return (TModelResponse)list;
            }
        }

        public Response<List<CountryModel>> GetCountry()
        {
            string ReturnLink = string.Empty;

            Response<List<CountryModel>> returnModel = new Response<List<CountryModel>>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<CountryModel>(@"select id, country_code, country, country_flag_icon from country order by (case country when 'Saudi Arabia' then 0 else 1 end), country Asc").ToList();
                }
                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Country List";
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

        public Response<UserLoginModel> MarkFavouriteLocation(UserLoginModel model, string UserId)
        {
            Response<UserLoginModel> returnModel = new Response<UserLoginModel>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    if (model.type == "Home")
                    {
                        DB.ExecuteSql(@"update customer set address1 =@address, longitude1 = @longitude, latitude1 = @latitude where id = @id", new
                        {
                            id = UserId,
                            address = model.address,
                            longitude = model.longitude,
                            latitude = model.latitude
                        });
                    }
                    else
                    {
                        DB.ExecuteSql(@"update customer set address2 =@address, longitude2 = @longitude, latitude2 = @latitude where id = @id", new
                        {
                            id = UserId,
                            address = model.address,
                            longitude = model.longitude,
                            latitude = model.latitude
                        });
                    }
                }
                returnModel.result = model;
                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Favourite location marked";
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

        public Response<CustomerCardDetails> InsertCustomerCard(CustomerCardDetails model, string UserId)
        {
            Response<CustomerCardDetails> returnModel = new Response<CustomerCardDetails>();
            try
            {

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql("insert into customer_payment_card values(@id,@user_id,@expiry_month,@card_last_digit,@status,@created,@modified,@expiry_year,@card_ref_number) ",

                                   new
                                   {
                                       id = Guid.NewGuid().ToString(),
                                       user_id = UserId,
                                       expiry_month = model.expiry_month,
                                       card_last_digit = model.card_last_digit,
                                       status = 1,
                                       created = DateTime.Now,
                                       modified = DateTime.Now,
                                       expiry_year = model.expiry_year,
                                       card_ref_number = model.card_ref_number
                                   });



                    returnModel.msg = "Card Added Successfully!.";
                    returnModel.status = (int)EnumClass.ResponseState.Success;
                    returnModel.success = false;
                    return returnModel;
                }
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.Failure;
                returnModel.success = false;
                return returnModel;
            }





        }

        public Response<List<CustomerCardDetails>> ViewCustomerCard(CustomerCardDetails model, string UserId)
        {
            Response<List<CustomerCardDetails>> returnModel = new Response<List<CustomerCardDetails>>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<CustomerCardDetails>("Select * from customer_payment_card where user_id = @user_id ", new
                    {
                        user_id = UserId
                    }).ToList();
                    returnModel.status = (int)EnumClass.ResponseState.Success;
                    returnModel.msg = "Card List";
                    returnModel.success = true;
                }

                return returnModel;
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                LoggingRepository.SaveException(ex);
                returnModel.success = false;
                return returnModel;
            }
        }
        public Response<CustomerCardDetails> DeleteCustomerCard(CustomerCardDetails model)
        {
            Response<CustomerCardDetails> returnModel = new Response<CustomerCardDetails>();
            try
            {

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql("delete from customer_payment_card where id = @id",
                                   new
                                   { id = model.id });



                    returnModel.msg = "Card Delete Successfully!.";
                    returnModel.status = (int)EnumClass.ResponseState.Success;
                    returnModel.success = false;
                    return returnModel;
                }
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.Failure;
                returnModel.success = false;
                return returnModel;
            }
        }
    }

}
