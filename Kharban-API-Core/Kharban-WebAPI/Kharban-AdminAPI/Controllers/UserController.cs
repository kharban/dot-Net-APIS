using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Kharban_AdminAPI.Models;
using Kharban_AdminAPI.Authentication;
using Microsoft.AspNetCore.Authorization;
using System;
using Kharban_AdminAPI.Repository;
using System.Net.Mail;
using System.Net;
using System.Collections.Generic;

namespace Kharban_AdminAPI.Controllers
{
    [Route("api/v1")]
    public class UserController : BaseController
    {
        private IConfiguration Configuration;
        private IUserService userService;
        public UserController(IConfiguration _configuration, IUserService _userService)
        {
            this.Configuration = _configuration;
            this.userService = _userService;
        }
        [HttpPost]
        [Route("/authorization")]
        public IActionResult Authorization([FromBody]AuthenticateRequest model)
        {
            var authorizationDetail = userService.Authorization(model);
            if (authorizationDetail == null)
                return BadRequest(new { message = "Error" });
            return Ok(authorizationDetail);
        }

        [Authorize]
        [HttpPost, Route(APIURL.GET_LOGIN_USER_INFO)]
        public IActionResult GetUserLoginInfo()
        {
            UserLoginResponse returnModel = new UserLoginResponse();

            returnModel.email = Email();
            returnModel.first_name = FirstName();
            returnModel.last_name = LastName();
            returnModel.username = UserName();
            returnModel.country_code = CountryCode();
            returnModel.mobile = Mobile();

            Response<UserLoginResponse> mdl = new Response<UserLoginResponse>();
            mdl.result = returnModel;
            mdl.status = 1;
            return Ok(mdl);
        }

        [HttpPost, Route(APIURL.ADMIN_FORGOT_PASSWORD)]
        public IActionResult forgetpasswordbyaddmin([FromBody]UserLoginResponse model)
        {

            if (model == null)
                model = new UserLoginResponse();

            Response<string> returnModel = new Response<string>();
            try
            {

                int OTP = new Random().Next(100000, 999999);

                returnModel = new UserRepository().SaveOTPForChangePwdByAdmin(OTP, model.email);

                if (returnModel.success)
                {

                    MailMessage message = new MailMessage();
                    SmtpClient smtp = new SmtpClient();
                    message.From = new MailAddress("noreply@octalsoftware.com");
                    message.To.Add(new MailAddress(model.email));
                    message.Subject = "Test";
                    //message.IsBodyHtml = true; //to make message body as html  
                    message.Body = "your otp is " + OTP;
                    smtp.Port = 587;
                    smtp.Host = "smtp.gmail.com"; //for gmail host  
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("noreply@octalsoftware.com", "Manage@123");
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {

            }

            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.ADMIN_CHANGE_PASSWORD)]
        public IActionResult changepasswordofaddmin([FromBody]UserLoginResponse model)
        {
            if (model == null)
                model = new UserLoginResponse();

            Response<int> returnModel = new UserRepository().AdminChangePasswordByOTP(model);

            return Ok(returnModel);
        }

        [Authorize]
        [HttpPost, Route(APIURL.GET_COUNTRIES)]
        public IActionResult GetCountries([FromBody]RequestModel model)
        {
            Response<List<CountryModel>> returnModel = new UserRepository().GetCountry(model);
            return Ok(returnModel);
        }

        [Authorize]
        [HttpPost, Route(APIURL.GET_NOTIFICATIONS)]
        public IActionResult GetNotifications([FromBody]NotificationModel model)
        {
            ResponseList<List<NotificationModel>> returnModel = new UserRepository().GetNotifications(model);
            return Ok(returnModel);
        }

        [Authorize]
        [HttpPost, Route(APIURL.SEND_PUSH_NOTIFICATIONS)]
        public async System.Threading.Tasks.Task<IActionResult> SendPushNotification([FromBody]PushNotificationModel model)
        {
            Response<PushNotificationModel> returnModel =await new UserRepository().SendPushNotifications(model);
            return Ok(returnModel);
        }

        [Authorize]
        [HttpPost, Route(APIURL.GET_CONTENT)]
        public IActionResult GetContent([FromBody]ContentModel model)
        {
            if (model == null)
                model = new ContentModel();
            Response<ContentModel> returnModel = new UserRepository().GetContent(model);

            return Ok(returnModel);
        }

        [Authorize]
        [HttpPost, Route(APIURL.UPDATE_CONTENT)]
        public IActionResult UpdateContent([FromBody]ContentModel model)
        {
            if (model == null)
                model = new ContentModel();

            Response<int> returnModel = new UserRepository().UpdateContent(model);

            return Ok(returnModel);
        }
    }
}