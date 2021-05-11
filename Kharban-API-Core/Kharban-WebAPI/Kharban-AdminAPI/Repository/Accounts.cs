using Insight.Database;
using Kharban_AdminAPI.Helper;
using Kharban_AdminAPI.Models;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace Kharban_AdminAPI.Repository
{
    public class Accounts
    {
        //Varifying user credentials
        public UserLoginResponse getValidate(string username, string password)
        {
            UserLoginResponse resModel = new UserLoginResponse();
            try
            {   // Check email id empty
                if (string.IsNullOrEmpty(username))
                {
                    resModel.Status = false;
                    resModel.StatusMessage = Resource_Kharban.PleaseEnterYourEmailAddress;
                }
                // Check password empty
                else if (string.IsNullOrEmpty(password))
                {
                    resModel.Status = false;
                    resModel.StatusMessage = Resource_Kharban.PleaseTypeYourPassword;
                }
                else
                {
                   
                    using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                    {
                        UserLoginResponse model = DB.QuerySql<UserLoginResponse>(@"SELECT username , email Email, first_name , last_name , mobile, country_code,password  FROM admin_user where email = @email and password = @password", new { email = username, password = password }).FirstOrDefault();

                        if (model == null)
                        {
                            resModel.Status = false;
                            resModel.StatusMessage = Resource_Kharban.TheEmailAddressPasswordEnteredInvalid;
                        }
                        else
                        {
                            resModel.Status = true;
                            resModel.username = model.username;
                            resModel.email = model.email;
                            resModel.password = model.password;
                            resModel.first_name = model.first_name;
                            resModel.last_name = model.last_name;
                            resModel.mobile = model.mobile;
                            resModel.country_code = model.country_code;
                            resModel.StatusMessage = Resource_Kharban.ThankYouForLogin;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //LoggingRepository.SaveException(ex);
            }
            return resModel;
        }
    }
}
