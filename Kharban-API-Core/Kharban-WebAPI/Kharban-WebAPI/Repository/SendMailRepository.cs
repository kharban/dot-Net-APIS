//using Insight.Database;
//using MM.Global;
//using MySql.Data.MySqlClient;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.IO;
//using System.Linq;
//using System.Web;
//using DKR_API.Models;
//using DKR_API.Models.Email;
//using DKR_API.Common;
//using DKR_API.Models.Payment;
//using EmailValidation_Library;
//using iTextSharp.text;
//using iTextSharp.text.pdf;
//using iTextSharp.tool.xml;

//namespace DKR_API.Repository
//{
//    public static class SendMailRepository
//    {
//        public static string DefaultMailtemplate(string TemplatePath)
//        {
//            using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//            {
//                string DefaultTemplate = string.Empty;
//                //Get Site Data from general setting
//                GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name FROM generalsettings ").FirstOrDefault();
//                if (Model != null)
//                {
//                    DefaultTemplate = @"<table width='500' cellpadding='0' cellspacing='0' border='0' bgcolor='#E58B01' style='border:solid 10px #E58B01;'>
//                            <tr bgcolor='#FFFFFF'><td>&nbsp;</td></tr>
//                            <tr bgcolor='#FFFFFF' height='25'>
//                            <td style='padding-left:20px;'><img src ='" + Model.mail_url + "images/" + Model.sitelogo + "'  width='250' border='0' /></td></tr> ";
//                    //dynamic Code for body
//                    DefaultTemplate += TemplatePath;

//                    DefaultTemplate += @"<tr bgcolor='#FFFFFF'>;
//                        <td align = 'left' style = 'padding-left:20px; font-family:Arial; font-size:11px; line-height:18px; text-decoration:none; color:#000000; padding-left:20px;'> 
//                        Regards,<br>" + Model.site_admin + " <br />" +
//                                "<font><a href = '" + Model.mail_url + "/index.php' style = 'font-family:Arial; font-size:11px; font-weight:bold; " +
//                                "text-decoration:none; color:#0c568b;'> '" + Model.site_name + "' </a></font></td></tr><tr bgcolor = '#FFFFFF'><td> &nbsp;</td>" +
//                                "</tr><tr height = '40'><td align = 'right' style = 'font-family: Arial, Helvetica, sans-serif;font-size: 10px;background-color: #E58B01;" +
//                                "color: #000000;'>& copy; Copyright" + DateTime.Now.Year + " & nbsp; < a href = '" + Model.mail_url + "/index.php' style = 'font-family:Arial; font-size:11px; " +
//                                "font-weight:bold; text-decoration:none; color:#0c568b;' > " + Model.site_name + " </a></td></tr></table>";


//                }
//                return DefaultTemplate;
//            };


//        }

//        private static string ActivationResendEmail(string userName, string title, string message)
//        {
//            string body = string.Empty;
//            using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//            {
//                //using streamreader for reading my htmltemplate   
//                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/html/activity_email.html")))
//                {
//                    body = reader.ReadToEnd();
//                }
//                body = body.Replace("{mail_url}", userName); //replacing the required things  
//            };
//            return body;
//        }

//        public static BaseResponse ResendEmail(string email)
//        {
//            BaseResponse model = new BaseResponse();
//            try
//            {
//                using (EmailValidator objEmailValidator = new EmailValidator())
//                {
//                    if (objEmailValidator.ValidateEmail(email, ValidationMode.Syntax))
//                    {
//                        string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//                        string MailBody = string.Empty;
//                        using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/email_activation.html")))
//                        {
//                            MailBody = reader.ReadToEnd();
//                        }
//                        using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                        {
//                            GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name,site_team FROM generalsettings ").FirstOrDefault();
//                            MemberRegisterModel memberRegister = DB.QuerySql<MemberRegisterModel>(@"Select id,fname,lname,email,password,is_email_verified,activestatus from memberregister where email = ?email", new { email = email }).FirstOrDefault();

//                            MailBody = MailBody.Replace("{mail_url}", Model.mail_url);
//                            MailBody = MailBody.Replace("{site_logo}", Model.sitelogo);
//                            MailBody = MailBody.Replace("{fullname}", memberRegister.fname + " " + memberRegister.lname);
//                            MailBody = MailBody.Replace("{user_id}", memberRegister.id.Encrypt());
//                            MailBody = MailBody.Replace("{siteteam}", Model.site_team);
//                            MailBody = MailBody.Replace("{dateyear}", DateTime.Now.Year.ToString());
//                            MailBody = MailBody.Replace("{site_name}", Model.site_name);
//                            MailBody = MailBody.Replace("{dateyear}", DateTime.Now.Year.ToString());
//                            //Append Header and Footer
//                            // MailBody = DefaultMailtemplate(MailBody);

//                            MailSend mail = new MailSend(Website);
//                            bool IsDone = mail.SendMail(memberRegister.email, Resource_DKR.EmailVerificationMailSubject, MailBody);

//                            model.Status = true;
//                            model.Title = Resource_DKR.Done;
//                            model.Type = Resource_DKR.Success;
//                            model.StatusMessage = Resource_DKR.Hasbeensenttoyourregistermailaccount;
//                        }
//                    }
//                }
//                return model;
//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);

//                model.Status = false;
//                model.Title = Resource_DKR.Oops;
//                model.Type = Resource_DKR.Warning;
//                model.StatusMessage = Resource_DKR.SomethingWentWrong;
//                return model;
//            }
//        }

//        public static bool RequestDataEmail(string MemberId, string PartnerId, string RequestType)
//        {
//            try
//            {
//                string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//                string MailBody = string.Empty;
//                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/request_data.html")))
//                {
//                    MailBody = reader.ReadToEnd();
//                }
//                using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                {
//                    GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name,site_team,site_com FROM generalsettings ").FirstOrDefault();
//                    MemberRegisterModel memberUser = DB.QuerySql<MemberRegisterModel>(@"Select id,fname,lname,email,password,is_email_verified,activestatus from memberregister where id = ?MemberId", new { MemberId = MemberId }).FirstOrDefault();
//                    MemberRegisterModel memberPartner = DB.QuerySql<MemberRegisterModel>(@"Select id,fname,lname,profileid,email,password,is_email_verified,activestatus from memberregister where id = ?PartnerId", new { PartnerId = PartnerId }).FirstOrDefault();

//                    MailBody = MailBody.Replace("{mail_url}", Model.mail_url);
//                    MailBody = MailBody.Replace("{site_logo}", Model.sitelogo);
//                    MailBody = MailBody.Replace("{fullname}", memberUser.fname + " " + memberUser.lname);
//                    MailBody = MailBody.Replace("{user_id}", memberUser.id.ToString());
//                    MailBody = MailBody.Replace("{siteteam}", Model.site_team);
//                    MailBody = MailBody.Replace("{dateyear}", DateTime.Now.Year.ToString());
//                    MailBody = MailBody.Replace("{site_name}", Model.site_name);
//                    MailBody = MailBody.Replace("{partnername}", memberPartner.fname + " " + memberPartner.lname);
//                    MailBody = MailBody.Replace("{partnerprofileid}", memberPartner.profileid);
//                    MailBody = MailBody.Replace("{type}", RequestType);
//                    MailBody = MailBody.Replace("{link}", Model.site_com + "view-profile/" + MemberId.Encrypt());
//                    //Append Header and Footer
//                    // MailBody = DefaultMailtemplate(MailBody);

//                    MailSend mail = new MailSend(Website);
//                    bool IsDone = mail.SendMail(memberUser.email, "Request for " + RequestType, MailBody);
//                    return IsDone;
//                }
//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);
//                return false;
//            }
//        }

//        public static bool SharePartnerProfileEmail(string MemberId, string PartnerId, DateTime ValidTill, string Info, string ShareProfileId, string Email)
//        {
//            try
//            {
//                string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//                string MailBody = string.Empty;
//                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/share_profile.html")))
//                {
//                    MailBody = reader.ReadToEnd();
//                }
//                using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                {
//                    GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name,site_team,site_com FROM generalsettings ").FirstOrDefault();
//                    MemberRegisterModel memberUser = DB.QuerySql<MemberRegisterModel>(@"Select id,display_name,email,password,is_email_verified,activestatus from memberregister where id = ?MemberId", new { MemberId = MemberId }).FirstOrDefault();
//                    MemberRegisterModel memberPartner = DB.QuerySql<MemberRegisterModel>(@"Select id,fname,lname,profileid,email,password,is_email_verified,activestatus from memberregister where id = ?PartnerId", new { PartnerId = PartnerId }).FirstOrDefault();

//                    MailBody = MailBody.Replace("{mail_url}", Model.mail_url);
//                    MailBody = MailBody.Replace("{site_logo}", Model.sitelogo);
//                    MailBody = MailBody.Replace("{fullname}", memberUser.display_name);
//                    MailBody = MailBody.Replace("{email}", Email);
//                    MailBody = MailBody.Replace("{end}", ValidTill.ToString("dd-MM-yyyy"));
//                    MailBody = MailBody.Replace("{info}", Info);
//                    MailBody = MailBody.Replace("{url}", Model.site_com + "/share-profile-view?uref=" + ShareProfileId.Encrypt());
//                    MailBody = MailBody.Replace("{dateyear}", DateTime.Now.Year.ToString());
//                    MailBody = MailBody.Replace("{site_name}", Model.site_name);
//                    MailBody = MailBody.Replace("{siteteam}", Model.site_team);
//                    MailSend mail = new MailSend(Website);
//                    bool IsDone = mail.SendMail(Email, memberUser.display_name + " has shared profile details with you.", MailBody);
//                    return IsDone;
//                }
//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);
//                return false;
//            }
//        }

//        public static bool AccountActivationEmail(int MemberId, int PartnerId, DateTime ValidTill, string Info, string ShareProfileId, string Email)
//        {
//            try
//            {
//                string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//                string MailBody = string.Empty;
//                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/AccountActivation.html")))
//                {
//                    MailBody = reader.ReadToEnd();
//                }
//                using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                {
//                    GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name,site_team,site_com FROM generalsettings ").FirstOrDefault();
//                    MemberRegisterModel memberUser = DB.QuerySql<MemberRegisterModel>(@"Select id,fname,lname,email,password,is_email_verified,activestatus from memberregister where id = ?MemberId", new { MemberId = MemberId }).FirstOrDefault();

//                    MailBody = MailBody.Replace("{mail_url}", Model.mail_url);
//                    MailBody = MailBody.Replace("{site_logo}", Model.sitelogo);
//                    MailBody = MailBody.Replace("{fullname}", memberUser.fname + " " + memberUser.lname);
//                    MailBody = MailBody.Replace("{url}", Model.site_com + "share-profile-view?id=" + ShareProfileId.Encrypt());
//                    MailBody = MailBody.Replace("{dateyear}", DateTime.Now.Year.ToString());
//                    MailBody = MailBody.Replace("{site_name}", Model.site_name);
//                    MailBody = MailBody.Replace("{siteteam}", Model.site_team);
//                    MailSend mail = new MailSend(Website);
//                    bool IsDone = mail.SendMail(Email, Resource_DKR.YourEmailAddressVerified, MailBody);
//                    return IsDone;
//                }
//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);
//                return false;
//            }
//        }

//        public static UserLoginResponse GovtDocsSkipEmail(string MemberId)
//        {
//            UserLoginResponse userLoginResponse = new UserLoginResponse();
//            try
//            {
//                if (MemberId == null)
//                {
//                    userLoginResponse.Type = Resource_DKR.Error;
//                    userLoginResponse.Title = Resource_DKR.Oops;
//                    userLoginResponse.Status = false;
//                    userLoginResponse.StatusMessage = Resource_DKR.SorryInvalidRequest;
//                }
//                else
//                {
//                    string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//                    string MailBody = string.Empty;
//                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/mail_on_skip.html")))
//                    {
//                        MailBody = reader.ReadToEnd();
//                    }
//                    using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                    {
//                        GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name,site_team,site_com FROM generalsettings ").FirstOrDefault();
//                        MemberRegisterModel memberUser = DB.QuerySql<MemberRegisterModel>(@"Select id,fname,lname,email,password,is_email_verified,activestatus from memberregister where id = ?MemberId", new { MemberId = MemberId.Decrypt().ValidateGuid() }).FirstOrDefault();
//                        MailBody = MailBody.Replace("{mail_url}", Model.mail_url);
//                        MailBody = MailBody.Replace("{site_logo}", Model.sitelogo);
//                        MailBody = MailBody.Replace("{dateyear}", DateTime.Now.Year.ToString());
//                        MailBody = MailBody.Replace("{site_name}", Model.site_name);
//                        MailBody = MailBody.Replace("{siteteam}", Model.site_team);
//                        MailBody = MailBody.Replace("{fullname}", memberUser.fname + " " + memberUser.lname);
//                        MailSend mail = new MailSend(Website);
//                        bool IsDone = mail.SendMail(memberUser.email, "Registration Completed.", MailBody);
//                        userLoginResponse.Type = Resource_DKR.Success;
//                        userLoginResponse.Title = Resource_DKR.DocumentSkip;
//                        userLoginResponse.Status = true;
//                        userLoginResponse.StatusMessage = Resource_DKR.GovtDocsSkipEmail;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                userLoginResponse.Type = Resource_DKR.Error;
//                userLoginResponse.Title = Resource_DKR.Oops;
//                userLoginResponse.Status = false;
//                userLoginResponse.StatusMessage = Resource_DKR.SomethingWentWrong;
//                LoggingRepository.SaveException(ex);
//            }
//            return userLoginResponse;
//        }

//        public static void ActivityEmail(string memberId, string message, string partnerName)
//        {
//            UserLoginResponse userLoginResponse = new UserLoginResponse();
//            try
//            {

//                if (memberId == null)
//                {
//                    userLoginResponse.Type = Resource_DKR.Error;
//                    userLoginResponse.Title = Resource_DKR.Oops;
//                    userLoginResponse.Status = false;
//                    userLoginResponse.StatusMessage = Resource_DKR.SorryInvalidRequest;
//                }
//                else
//                {
//                    string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//                    string MailBody = string.Empty;
//                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/activity_email.html")))
//                    {
//                        MailBody = reader.ReadToEnd();
//                    }
//                    using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                    {
//                        GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name,site_team,site_com FROM generalsettings ").FirstOrDefault();
//                        MemberRegisterModel memberUser = DB.QuerySql<MemberRegisterModel>(@"Select id,fname,lname,email,password,is_email_verified,activestatus from memberregister where id = ?MemberId", new { MemberId = memberId }).FirstOrDefault();
//                        using (EmailValidator objEmailValidator = new EmailValidator())
//                        {
//                            if (objEmailValidator.ValidateEmail(memberUser.email, ValidationMode.Syntax))
//                            {
//                                MailBody = MailBody.Replace("{mail_url}", Model.mail_url);
//                                MailBody = MailBody.Replace("{site_logo}", Model.sitelogo);
//                                MailBody = MailBody.Replace("{dateyear}", DateTime.Now.Year.ToString());
//                                MailBody = MailBody.Replace("{site_name}", Model.site_name);
//                                MailBody = MailBody.Replace("{siteteam}", Model.site_team);
//                                MailBody = MailBody.Replace("{fullname}", memberUser.fname + " " + memberUser.lname);
//                                MailBody = MailBody.Replace("{message}", message);
//                                MailBody = MailBody.Replace("{link}", Model.site_com + "view-profile/" + memberId.Encrypt());
//                                MailBody = MailBody.Replace("{partnername}", partnerName);
//                                MailSend mail = new MailSend(Website);
//                                bool IsDone = mail.SendMail(memberUser.email, message, MailBody);
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);
//            }
//        }

//        public static void ContactEnquiryEmail(ContactUsModel model)
//        {
//            UserLoginResponse userLoginResponse = new UserLoginResponse();
//            try
//            {
//                using (EmailValidator objEmailValidator = new EmailValidator())
//                {
//                    if (objEmailValidator.ValidateEmail("", ValidationMode.Syntax))
//                    {
//                        string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//                        string MailBody = string.Empty;
//                        using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/contact_enquiry.html")))
//                        {
//                            MailBody = reader.ReadToEnd();
//                        }
//                        using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                        {
//                            GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name,site_team,site_com FROM generalsettings ").FirstOrDefault();

//                            MailBody = MailBody.Replace("{mail_url}", Model.mail_url);
//                            MailBody = MailBody.Replace("{sitelogo}", Model.sitelogo);
//                            MailBody = MailBody.Replace("{dateyear}", DateTime.Now.Year.ToString());
//                            MailBody = MailBody.Replace("{sitename}", Model.site_name);
//                            MailBody = MailBody.Replace("{siteteam}", Model.site_team);
//                            MailBody = MailBody.Replace("{name}", model.name);
//                            MailBody = MailBody.Replace("{message}", model.message);
//                            MailBody = MailBody.Replace("{subject}", model.subject);
//                            MailBody = MailBody.Replace("{email}", model.email);
//                            MailBody = MailBody.Replace("{Year}", DateTime.Now.Year.ToString());

//                            MailSend mail = new MailSend(Website);
//                            bool IsDone = mail.SendMail("hiringshophimanshu@gmail.com", Resource_DKR.EnquiryDetailsFromDilKeRishteContactForm, MailBody);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);
//            }
//        }

//        public static void EnquiryAcknowledgementEmail(ContactUsModel model)
//        {
//            UserLoginResponse userLoginResponse = new UserLoginResponse();
//            try
//            {
//                using (EmailValidator objEmailValidator = new EmailValidator())
//                {
//                    if (objEmailValidator.ValidateEmail(model.email, ValidationMode.Syntax))
//                    {
//                        string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//                        string MailBody = string.Empty;
//                        using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/enquiry_acknowledgement.html")))
//                        {
//                            MailBody = reader.ReadToEnd();
//                        }
//                        using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                        {
//                            GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name,site_team,site_com FROM generalsettings ").FirstOrDefault();

//                            MailBody = MailBody.Replace("{mail_url}", Model.mail_url);
//                            MailBody = MailBody.Replace("{sitelogo}", Model.sitelogo);
//                            MailBody = MailBody.Replace("{dateyear}", DateTime.Now.Year.ToString());
//                            MailBody = MailBody.Replace("{sitename}", Model.site_name);
//                            MailBody = MailBody.Replace("{siteteam}", Model.site_team);
//                            MailBody = MailBody.Replace("{name}", model.name);
//                            MailBody = MailBody.Replace("{message}", model.message);
//                            MailBody = MailBody.Replace("{subject}", model.subject);
//                            MailBody = MailBody.Replace("{email}", model.email);
//                            MailBody = MailBody.Replace("{Year}", DateTime.Now.Year.ToString());

//                            MailSend mail = new MailSend(Website);
//                            bool IsDone = mail.SendMail(model.email, Resource_DKR.EnquiryAcknowledgementFromDilKeRishte, MailBody);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);
//            }
//        }

//        //public static void ApplyPostAdminEmail(CareerApplicantModel model, string title)
//        //{
//        //    UserLoginResponse userLoginResponse = new UserLoginResponse();
//        //    try
//        //    {

//        //        string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//        //        string MailBody = string.Empty;
//        //        using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/apply_career.html")))
//        //        {
//        //            MailBody = reader.ReadToEnd();
//        //        }
//        //        using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//        //        {
//        //            GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name,site_team,site_com FROM generalsettings ").FirstOrDefault();

//        //            MailBody = MailBody.Replace("{mail_url}", Model.mail_url);
//        //            MailBody = MailBody.Replace("{sitelogo}", Model.sitelogo);
//        //            MailBody = MailBody.Replace("{dateyear}", DateTime.Now.Year.ToString());
//        //            MailBody = MailBody.Replace("{sitename}", Model.site_name);
//        //            MailBody = MailBody.Replace("{siteteam}", Model.site_team);
//        //            MailBody = MailBody.Replace("{name}", model.name);
//        //            MailBody = MailBody.Replace("{email}", model.email);
//        //            MailBody = MailBody.Replace("{post}", title);
//        //            MailBody = MailBody.Replace("{Year}", DateTime.Now.Year.ToString());

//        //            MailSend mail = new MailSend(Website);
//        //            //bool IsDone = mail.SendMail("hiringshophimanshu@gmail.com", "Applied for " + title, MailBody);
//        //            bool IsDone = mail.SendMailAttachment("hiringshophimanshu@gmail.com", "Applied for " + title, MailBody, model.resume);
//        //        }

//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        LoggingRepository.SaveException(ex);
//        //    }
//        //}

//        public static void ApplyPostUserEmail(CareerApplicantModel model)
//        {
//            UserLoginResponse userLoginResponse = new UserLoginResponse();
//            try
//            {

//                string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//                string MailBody = string.Empty;
//                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/apply_career_user.html")))
//                {
//                    MailBody = reader.ReadToEnd();
//                }
//                using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                {
//                    GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name,site_team,site_com FROM generalsettings ").FirstOrDefault();

//                    MailBody = MailBody.Replace("{mail_url}", Model.mail_url);
//                    MailBody = MailBody.Replace("{sitelogo}", Model.sitelogo);
//                    MailBody = MailBody.Replace("{dateyear}", DateTime.Now.Year.ToString());
//                    MailBody = MailBody.Replace("{sitename}", Model.site_name);
//                    MailBody = MailBody.Replace("{siteteam}", Model.site_team);
//                    MailBody = MailBody.Replace("{name}", model.name);
//                    MailBody = MailBody.Replace("{Year}", DateTime.Now.Year.ToString());

//                    MailSend mail = new MailSend(Website);
//                    string filename = model.resume.Substring(model.resume.LastIndexOf("/"));
//                    bool IsDone = mail.SendMail(model.email, Resource_DKR.ThankYoufromDilKeRishte, MailBody, Utility.GetFileStreamFromS3(model.resume), filename);
//                }

//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);
//            }
//        }

//        public static void ApplyBusinessAdminEmail(BusinessOpportunityModel model)
//        {
//            try
//            {
//                string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//                string MailBody = string.Empty;
//                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/admin_apply_business_opportunity.html")))
//                {
//                    MailBody = reader.ReadToEnd();
//                }
//                using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                {
//                    GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name,site_team,site_com FROM generalsettings ").FirstOrDefault();

//                    MailBody = MailBody.Replace("{mail_url}", Model.mail_url);
//                    MailBody = MailBody.Replace("{sitelogo}", Model.sitelogo);
//                    MailBody = MailBody.Replace("{dateyear}", DateTime.Now.Year.ToString());
//                    MailBody = MailBody.Replace("{sitename}", Model.site_name);
//                    MailBody = MailBody.Replace("{siteteam}", Model.site_team);
//                    MailBody = MailBody.Replace("{associate_name}", model.associate_name);
//                    MailBody = MailBody.Replace("{name_of_applicant}", model.name_of_applicant);
//                    MailBody = MailBody.Replace("{name_of_owner}", model.name_of_owner);
//                    MailBody = MailBody.Replace("{current_business}", model.current_business);
//                    MailBody = MailBody.Replace("{current_business_setup}", model.current_business_setup);
//                    MailBody = MailBody.Replace("{address}", model.address);
//                    MailBody = MailBody.Replace("{phone}", model.phone);
//                    MailBody = MailBody.Replace("{pincode}", model.pincode);
//                    MailBody = MailBody.Replace("{address}", model.address);
//                    MailBody = MailBody.Replace("{Year}", DateTime.Now.Year.ToString());

//                    MailSend mail = new MailSend(Website);
//                    string[] adminIds = Model.site_admin.Split(',');
//                    foreach (var item in adminIds)
//                    {
//                        bool IsDone = mail.SendMail(item, Resource_DKR.AppliedForBusinessOpportunity + " " + model.associate_name, MailBody);
//                    }

//                }

//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);
//            }
//        }

//        public static void AdvertiseAdminEmail(AdvertiseModel model)
//        {
//            try
//            {

//                string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//                string MailBody = string.Empty;
//                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/admin_advertise_enquiry.html")))
//                {
//                    MailBody = reader.ReadToEnd();
//                }
//                using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                {
//                    GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name,site_team,site_com FROM generalsettings ").FirstOrDefault();

//                    MailBody = MailBody.Replace("{mail_url}", Model.mail_url);
//                    MailBody = MailBody.Replace("{sitelogo}", Model.sitelogo);
//                    MailBody = MailBody.Replace("{dateyear}", DateTime.Now.Year.ToString());
//                    MailBody = MailBody.Replace("{sitename}", Model.site_name);
//                    MailBody = MailBody.Replace("{siteteam}", Model.site_team);
//                    MailBody = MailBody.Replace("{name}", model.name);
//                    MailBody = MailBody.Replace("{email}", model.email);
//                    MailBody = MailBody.Replace("{phone}", model.phone);
//                    MailBody = MailBody.Replace("{description}", model.description);
//                    MailBody = MailBody.Replace("{Year}", DateTime.Now.Year.ToString());

//                    MailSend mail = new MailSend(Website);
//                    string[] adminIds = Model.site_admin.Split(',');
//                    foreach (var item in adminIds)
//                    {
//                        bool IsDone = mail.SendMail(item, Resource_DKR.EnquiryDetailsFromDilKeRishteAdvertiseWithUs, MailBody);
//                    }

//                }

//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);
//            }
//        }

//        public static bool NoActivityReminder(string id)
//        {
//            try
//            {
//                string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//                string MailBody = string.Empty;
//                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/AccountActivation.html")))
//                {
//                    MailBody = reader.ReadToEnd();
//                }
//                using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                {
//                    GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name,site_team,site_com FROM dkrlive.generalsettings ").FirstOrDefault();
//                    MemberRegisterModel memberUser = DB.QuerySql<MemberRegisterModel>(@"Select id,fname,lname,email,password,is_email_verified,activestatus from memberregister where id = ?MemberId", new { MemberId = id }).FirstOrDefault();

//                    MailBody = MailBody.Replace("{mail_url}", Model.mail_url);
//                    MailBody = MailBody.Replace("{site_logo}", Model.sitelogo);
//                    MailBody = MailBody.Replace("{fullname}", memberUser.fname + " " + memberUser.lname);
//                    MailBody = MailBody.Replace("{dateyear}", DateTime.Now.Year.ToString());
//                    MailBody = MailBody.Replace("{site_name}", Model.site_name);
//                    MailBody = MailBody.Replace("{siteteam}", Model.site_team);
//                    MailSend mail = new MailSend(Website);
//                    bool IsDone = mail.SendMail(memberUser.email, "Not active last 30 days.", MailBody);
//                    return IsDone;
//                }
//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);
//                return false;
//            }
//        }

//        public static BaseResponse ChangePassword(string name, string email, string memberid)
//        {
//            BaseResponse model = new BaseResponse();
//            try
//            {
//                string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//                string MailBody = string.Empty;
//                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/change_password.html")))
//                {
//                    MailBody = reader.ReadToEnd();
//                }
//                using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                {
//                    GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name,site_team,site_com FROM generalsettings ").FirstOrDefault();

//                    MailBody = MailBody.Replace("{name}", name);
//                    MailBody = MailBody.Replace("{mail_url}", Model.mail_url);
//                    MailBody = MailBody.Replace("{sitelogo}", Model.sitelogo);
//                    MailBody = MailBody.Replace("{siteteam}", Model.site_team);
//                    MailBody = MailBody.Replace("{Year}", DateTime.Now.Year.ToString());
//                    MailBody = MailBody.Replace("{sitename}", Model.site_name);
//                    //MailBody = MailBody.Replace("{link}", Model.site_com + "view-profile/" + memberid.Encrypt());

//                    //Append Header and Footer
//                    // MailBody = DefaultMailtemplate(MailBody);

//                    MailSend mail = new MailSend(Website);
//                    bool IsDone = mail.SendMail(email, Resource_DKR.ChangePasswordMailSubject, MailBody);

//                    model.Status = true;
//                    model.Title = Resource_DKR.Done;
//                    model.Type = Resource_DKR.Success;
//                    model.StatusMessage = Resource_DKR.Hasbeensenttoyourregistermailaccount;
//                    return model;
//                }
//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);

//                model.Status = false;
//                model.Title = Resource_DKR.Oops;
//                model.Type = Resource_DKR.Warning;
//                model.StatusMessage = Resource_DKR.SomethingWentWrong;
//                return model;
//            }
//        }

//        public static BaseResponse EmailActivation(string id)
//        {
//            BaseResponse model = new BaseResponse();
//            try
//            {
//                using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                {
//                    var perameter = new
//                    {
//                        id = id.Decrypt().ValidateGuid(),
//                    };

//                    int activate = DB.QuerySql<int>("select is_email_verified from memberregister where id = ?id", perameter).FirstOrDefault();
//                    if (activate == 0)
//                    {
//                        model.Status = false;
//                        model.Title = Resource_DKR.Oops;
//                        model.Type = Resource_DKR.Info;
//                        model.StatusMessage = Resource_DKR.EmailAddressAlreadyVerified;
//                        return model;
//                    }
//                    DB.ExecuteSql(@"Update memberregister set is_email_verified = 0 where id = ?id", perameter);
//                    model.Status = true;
//                    model.Title = Resource_DKR.Congratulations;
//                    model.Type = Resource_DKR.Success;
//                    model.StatusMessage = Resource_DKR.ThankYouForVerifyYourEmailAddress;
//                    return model;
//                }
//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);

//                model.Status = false;
//                model.Title = Resource_DKR.Oops;
//                model.Type = Resource_DKR.Warning;
//                model.StatusMessage = Resource_DKR.SomethingWentWrong;
//                return model;
//            }
//        }

//        public static void UserReferenceEmail(string Email, string FullName, string Price, string Code, string Apklink, string Site, string siteteam, string Url, string username)
//        {
//            UserLoginResponse userLoginResponse = new UserLoginResponse();
//            try
//            {

//                string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//                string MailBody = string.Empty;
//                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/UserReference.html")))
//                {
//                    MailBody = reader.ReadToEnd();
//                }
//                using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                {

//                    MailBody = MailBody.Replace("{fullname}", FullName);
//                    MailBody = MailBody.Replace("{price}", Price);
//                    MailBody = MailBody.Replace("{code}", Code);
//                    MailBody = MailBody.Replace("{android_applink}", Apklink);
//                    MailBody = MailBody.Replace("{site_name}", Site);
//                    MailBody = MailBody.Replace("{Year}", DateTime.Now.Year.ToString());
//                    MailBody = MailBody.Replace("{mail_url}", Url);
//                    MailBody = MailBody.Replace("{username}", username);
//                    MailBody = MailBody.Replace("{siteteam}", siteteam);

//                    MailSend mail = new MailSend(Website);
//                    bool IsDone = mail.SendMail(Email, Resource_DKR.SurpriseGiftFromYourFriend + " " + FullName, MailBody);
//                }

//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);
//            }
//        }

//        public static void SurveyEmail(string email, string name)
//        {
//            UserLoginResponse userLoginResponse = new UserLoginResponse();
//            try
//            {

//                string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//                string MailBody = string.Empty;
//                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/survey.html")))
//                {
//                    MailBody = reader.ReadToEnd();
//                }
//                using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                {
//                    GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name,site_team,site_com FROM generalsettings ").FirstOrDefault();

//                    MailBody = MailBody.Replace("{mail_url}", Model.mail_url);
//                    MailBody = MailBody.Replace("{sitelogo}", Model.sitelogo);
//                    MailBody = MailBody.Replace("{dateyear}", DateTime.Now.Year.ToString());
//                    MailBody = MailBody.Replace("{sitename}", Model.site_name);
//                    MailBody = MailBody.Replace("{siteteam}", Model.site_team);
//                    MailBody = MailBody.Replace("{name}", name);
//                    MailBody = MailBody.Replace("{date}", "DATE");
//                    MailBody = MailBody.Replace("{email}", email);
//                    MailBody = MailBody.Replace("{Year}", DateTime.Now.Year.ToString());

//                    MailSend mail = new MailSend(Website);
//                    bool IsDone = mail.SendMail(email, "Survey", MailBody);
//                }

//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);
//            }
//        }



//        public static void BuyCustomMemberShipEmail(string email, string name, string plan, string addonstr, DateTime enddate)
//        {
//            UserLoginResponse userLoginResponse = new UserLoginResponse();
//            try
//            {

//                string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//                string MailBody = string.Empty;
//                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/buycustommembershipemail.html")))
//                {
//                    MailBody = reader.ReadToEnd();
//                }
//                using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                {
//                    GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name,site_team,site_com FROM generalsettings ").FirstOrDefault();

//                    MailBody = MailBody.Replace("{mail_url}", Model.mail_url);
//                    MailBody = MailBody.Replace("{sitelogo}", Model.sitelogo);
//                    MailBody = MailBody.Replace("{dateyear}", DateTime.Now.Year.ToString());
//                    MailBody = MailBody.Replace("{sitename}", Model.site_name);
//                    MailBody = MailBody.Replace("{siteteam}", Model.site_team);
//                    MailBody = MailBody.Replace("{fullname}", name);
//                    MailBody = MailBody.Replace("{email}", email);
//                    MailBody = MailBody.Replace("{membershipplan}", plan);
//                    MailBody = MailBody.Replace("{AddonList}", addonstr);
//                    MailBody = MailBody.Replace("{enddate}", enddate.ToString("dd MMMM yyyy"));

//                    MailBody = MailBody.Replace("{Year}", DateTime.Now.Year.ToString());

//                    MailSend mail = new MailSend(Website);
//                    bool IsDone = mail.SendMail(email, "Congratulations! You are now entitled to " + plan + " membership", MailBody, PaymentRepository.generateInvoice());
//                }

//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);
//            }
//        }

//        public static void BuyMemberShipEmail(string email, string name, string plan, string duration)
//        {
//            UserLoginResponse userLoginResponse = new UserLoginResponse();
//            try
//            {

//                string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//                string MailBody = string.Empty;
//                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/buymembershipemail.html")))
//                {
//                    MailBody = reader.ReadToEnd();
//                }
//                using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                {
//                    GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name,site_team,site_com FROM generalsettings ").FirstOrDefault();

//                    MailBody = MailBody.Replace("{mail_url}", Model.mail_url);
//                    MailBody = MailBody.Replace("{sitelogo}", Model.sitelogo);
//                    MailBody = MailBody.Replace("{dateyear}", DateTime.Now.Year.ToString());
//                    MailBody = MailBody.Replace("{sitename}", Model.site_name);
//                    MailBody = MailBody.Replace("{siteteam}", Model.site_team);
//                    MailBody = MailBody.Replace("{fullname}", name);
//                    MailBody = MailBody.Replace("{email}", email);
//                    MailBody = MailBody.Replace("{membershipplan}", plan);
//                    MailBody = MailBody.Replace("{planduration}", duration);


//                    MailBody = MailBody.Replace("{Year}", DateTime.Now.Year.ToString());

//                    MailSend mail = new MailSend(Website);
//                    bool IsDone = mail.SendMail(email, "Congratulations! You are now entitled to " + plan + " membership", MailBody);
//                }

//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);
//            }
//        }

//        public static void FeedbackEmailToUser(string name, string email)
//        {
//            UserLoginResponse userLoginResponse = new UserLoginResponse();
//            try
//            {
//                using (EmailValidator objEmailValidator = new EmailValidator())
//                {
//                    if (objEmailValidator.ValidateEmail(email, ValidationMode.Syntax))
//                    {

//                        string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//                        string MailBody = string.Empty;
//                        using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/feedback_mail_user.html")))
//                        {
//                            MailBody = reader.ReadToEnd();
//                        }
//                        using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                        {
//                            GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name,site_team,site_com FROM generalsettings ").FirstOrDefault();

//                            MailBody = MailBody.Replace("{mail_url}", Model.mail_url);
//                            MailBody = MailBody.Replace("{sitelogo}", Model.sitelogo);
//                            MailBody = MailBody.Replace("{dateyear}", DateTime.Now.Year.ToString());
//                            MailBody = MailBody.Replace("{sitename}", Model.site_name);
//                            MailBody = MailBody.Replace("{siteteam}", Model.site_team);
//                            MailBody = MailBody.Replace("{fullname}", name);
//                            MailBody = MailBody.Replace("{Year}", DateTime.Now.Year.ToString());

//                            MailSend mail = new MailSend(Website);
//                            bool IsDone = mail.SendMail(email, "FeedBack", MailBody);
//                        }

//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);
//            }
//        }

//        public static void FeedBackEmailToAdmin(string name, string email, string feedback, string subject)
//        {
//            UserLoginResponse userLoginResponse = new UserLoginResponse();
//            try
//            {
//                string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//                string MailBody = string.Empty;
//                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/feedback_mail_admin.html")))
//                {
//                    MailBody = reader.ReadToEnd();
//                }
//                using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                {
//                    GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name,site_team,site_com FROM generalsettings ").FirstOrDefault();

//                    MailBody = MailBody.Replace("{mail_url}", Model.mail_url);
//                    MailBody = MailBody.Replace("{sitelogo}", Model.sitelogo);
//                    MailBody = MailBody.Replace("{dateyear}", DateTime.Now.Year.ToString());
//                    MailBody = MailBody.Replace("{sitename}", Model.site_name);
//                    MailBody = MailBody.Replace("{siteteam}", Model.site_team);
//                    MailBody = MailBody.Replace("{fullname}", name);
//                    MailBody = MailBody.Replace("{email}", email);
//                    MailBody = MailBody.Replace("{feedback}", feedback);
//                    MailBody = MailBody.Replace("{Year}", DateTime.Now.Year.ToString());

//                    MailSend mail = new MailSend(Website);

//                    string[] adminIds = Model.site_admin.Split(',');
//                    foreach (var item in adminIds)
//                    {
//                        bool IsDone = mail.SendMail(item, "Feedback", MailBody);
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);
//            }
//        }

//        public static void TrustScoreEmailToUser(string name, string email, string trust)
//        {
//            UserLoginResponse userLoginResponse = new UserLoginResponse();
//            try
//            {

//                string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//                string MailBody = string.Empty;
//                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/activation_missing.html")))
//                {
//                    MailBody = reader.ReadToEnd();
//                }
//                using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                {
//                    GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name,site_team,site_com FROM generalsettings ").FirstOrDefault();

//                    MailBody = MailBody.Replace("{mail_url}", Model.mail_url);
//                    MailBody = MailBody.Replace("{sitelogo}", Model.sitelogo);
//                    MailBody = MailBody.Replace("{dateyear}", DateTime.Now.Year.ToString());
//                    MailBody = MailBody.Replace("{sitename}", Model.site_name);
//                    MailBody = MailBody.Replace("{siteteam}", Model.site_team);
//                    MailBody = MailBody.Replace("{fullname}", name);
//                    MailBody = MailBody.Replace("{trust}", trust);
//                    MailBody = MailBody.Replace("{Year}", DateTime.Now.Year.ToString());

//                    MailSend mail = new MailSend(Website);
//                    bool IsDone = mail.SendMail(email, "FeedBack", MailBody);

//                }

//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);
//            }
//        }

//        public static void PaymentEmail(Payment_Request model)
//        {
//            UserLoginResponse userLoginResponse = new UserLoginResponse();
//            try
//            {

//                string Website = System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();
//                string MailBody = string.Empty;
//                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/HtmlTemplate/enquiry_acknowledgement.html")))
//                {
//                    MailBody = reader.ReadToEnd();
//                }
//                using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                {
//                    GeneralSettingsModel Model = DB.QuerySql<GeneralSettingsModel>(@"SELECT mail_url,sitelogo,site_admin,site_name,site_team,site_com FROM generalsettings ").FirstOrDefault();

//                    MailBody = MailBody.Replace("{mail_url}", Model.mail_url);
//                    MailBody = MailBody.Replace("{sitelogo}", Model.sitelogo);
//                    MailBody = MailBody.Replace("{dateyear}", DateTime.Now.Year.ToString());
//                    MailBody = MailBody.Replace("{sitename}", Model.site_name);
//                    MailBody = MailBody.Replace("{siteteam}", Model.site_team);
//                    //MailBody = MailBody.Replace("{name}", model.name);
//                    //MailBody = MailBody.Replace("{message}", model.message);
//                    //MailBody = MailBody.Replace("{subject}", model.subject);
//                    //MailBody = MailBody.Replace("{email}", model.email);
//                    MailBody = MailBody.Replace("{Year}", DateTime.Now.Year.ToString());

//                    MailSend mail = new MailSend(Website);
//                    bool IsDone = mail.SendMail("model.email", Resource_DKR.EnquiryAcknowledgementFromDilKeRishte, MailBody);
//                }

//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);
//            }
//        }


//    }
//}