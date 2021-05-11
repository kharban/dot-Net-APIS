using Insight.Database;
using Kharban_WebAPI.Helper;
using Kharban_WebAPI.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace Kharban_WebAPI.Repository
{
    public class Accounts
    {
        //Varifying user credentials
        public UserLoginResponse getValidate(string mobile, string otp)
        {
            UserLoginResponse resModel = new UserLoginResponse();
            try
            {   // Check email id empty
                if (string.IsNullOrEmpty(mobile))
                {

                    resModel.Status = false;
                    resModel.Title = Resource_Message.Alert;
                    resModel.Type = Resource_Message.Warning;
                    resModel.StatusMessage = Resource_Message.PleaseEnterYourEmailAddress;
                }
                // Check password empty
                else if (string.IsNullOrEmpty(otp))
                {
                    resModel.Status = false;
                    resModel.Title = Resource_Message.Alert;
                    resModel.Type = Resource_Message.Warning;
                    resModel.StatusMessage = Resource_Message.PleaseTypeYourPassword;
                }
                else
                {
                    //string encryptPassword = Common.Utility.MD5Hash(password);
                    using (SqlConnection con = new SqlConnection(SiteKey.ConnectionString))
                    {
                        //Get All data from MemberRegister
                        resModel = con.QuerySql<UserLoginResponse>(@"select id, concat(first_name,' ', last_name) fullname, email from customer where contact_no = @mobile and otp = @otp", new { mobile = mobile, otp = otp }).FirstOrDefault();

                        if (resModel == null)
                            resModel = con.QuerySql<UserLoginResponse>(@"select id, concat(first_name,' ', last_name) fullname, email from provider where contact_no = @mobile and otp = @otp", new { mobile = mobile, otp = otp }).FirstOrDefault();

                        if (resModel == null)
                        {
                            resModel = new UserLoginResponse();
                            resModel.Status = false;
                            resModel.Title = Resource_Message.Alert;
                            resModel.Type = Resource_Message.Warning;
                            resModel.StatusMessage = Resource_Message.TheEmailAddressPasswordEnteredInvalid;
                        }
                        else
                        {
                            
                            resModel.Status = true;
                            resModel.Title = "";
                            resModel.Type = Resource_Message.Success;
                            resModel.StatusMessage = Resource_Message.ThankYouForLogin;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingRepository.SaveException(ex);
            }
            return resModel;
        }
    }
}