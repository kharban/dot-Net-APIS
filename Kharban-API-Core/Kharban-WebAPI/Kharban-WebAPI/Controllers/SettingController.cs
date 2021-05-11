using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using Kharban_WebAPI;
using Kharban_WebAPI.Models;
using Kharban_WebAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Kharban_WebAPI.Controllers
{

    [Route("api/v1")]
    public class SettingController : BaseController
    {
        private IConfiguration Configuration;
        public SettingController(IConfiguration _configuration)
        {
            this.Configuration = _configuration;
        }

        [HttpPost, Route(APIURL.GET_CONTENT)]
        public ActionResult GetContent([FromBody]ContentModel model)
        {
            Response<ContentModel> returnModel = new SettingRepository().GetContent(model);
            return Ok(returnModel);
        }

        [Authorize]
        [HttpPost, Route(APIURL.GET_FAQ_LIST)]
        public ActionResult GetFaqList(FaqModel model)
        {
            if (model == null)
                model = new FaqModel();
            Response<List<FaqModel>> returnModel = new SettingRepository().GetFaqList(model);
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_FAQ)]
        public ActionResult GetFaq(FaqModel model)
        {
            if (model == null)
                model = new FaqModel();
            Response<FaqModel> returnModel = new SettingRepository().GetFaq(model);
            return Ok(returnModel);
        }

        [Authorize]
        [HttpPost, Route(APIURL.SEND_SUPPORT_EMAIL)]
        public ActionResult SendSupportEmail(EmailModel model)
        {
            string connStrings = Configuration.GetConnectionString("DevelopmentConnectionString");
            Response<string> returnModel = new Response<string>();
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("noreply@octalsoftware.com");

                message.To.Add(new MailAddress("himanshu@mailinator.com"));

                message.Subject = model.subject;
                //message.IsBodyHtml = true; //to make message body as html  
                message.Body = model.description;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("noreply@octalsoftware.com", "Manage@123");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception ex)
            {

            }

            return Ok(returnModel);
        }
        
        [Authorize]
        [HttpPost, Route(APIURL.GET_NOTIFICATION)]
        public ActionResult GetNotification()
        {
            
            Response<List<NotificationModel>> returnModel = new SettingRepository().GetNotifications(GetUserId());
            return Ok(returnModel);
        }

        [Authorize]
        [HttpPost, Route(APIURL.DELETE_NOTIFICATION)]
        public ActionResult DeleteNotification(NotificationModel model)
        {
            if (model == null)
                model = new NotificationModel();
            Response<int> returnModel = new SettingRepository().DeleteNotification(model);
            return Ok(returnModel);
        }
    }
}