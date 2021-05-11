using Kharban_WebAPI.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace Kharban_WebAPI.Controllers
{
    [Route("api/v1")]
    public class testController : ControllerBase
    {
        [HttpPost, Route("getsql")]
        public ActionResult getsql()
        {

            string plainStr = "06969b0d-0523-11e9-8656-00ff3751ad98";
            int keySize = 256;
            string completeEncodedKey = AESEncryption.GenerateKey(keySize);
            string encryptedText = plainStr.Encrypt();
            string decrypted = encryptedText.Decrypt().ValidateGuid();
            return Ok();
        }

        [HttpGet, Route("sendsmstest")]
        public ActionResult ztest()
        {
            SendSMS.Send(SendSMS.Templates.SUCCESSFUL_REGISTER, "8058758011", "", "", "himanshu", "", "");
            //message = message.Replace("OTP", "123456");
            //Utility.SendMsg(mobile, message);
            return Ok();
        }
        [HttpGet, Route("checkredis")]
        public ActionResult CheckRedis()
        {
            try
            {
                RedisConnection.Connection.StringSet("TestDate", DateTime.Now.AddDays(10).ToString(), TimeSpan.FromMinutes(5));
                return Ok(RedisConnection.Connection.StringGet("TestDate"));
            }
            catch (Exception ex)
            {
                return Ok(ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet, Route("pushnotification")]
        public ActionResult testpushnotification()
        {
            return Ok(SendSMS.PushNotificationAsync("csXWg5iodU2cq5iB88uVFv:APA91bFCtyIH-iU7HkOrsqXDhIRekmY2EFeiHCZWvNZQ6BufYDQWPPcpcRkoVapoTUwcK8vHHqfv6iVVJ3t-491eSEOyNispBaUmzLKegLVJW7tOKl_l61JUzmLsaTLf__pOtV5IUtPR", "Kharban", "hello", "OrderTracking",2));
        }

        [HttpGet, Route("checkstreamreturn")]
        public ActionResult checkstreamreturn()
        {
            string imag = "";
            imag = imag.Replace(@"\", "");
            int c = imag.Length % 4;
            if ((c) != 0)
                imag = imag.PadRight((imag.Length + (4 - c)), '=');
            var bytes = Convert.FromBase64String(imag);

            Utility.sendMyFileToS3(new MemoryStream(bytes), "horoscope", "fdgsdf.jpg");
            return Ok();
        }
        //public byte[] Base64DecodeString(string inputStr)
        //{
        //    byte[] decodedByteArray =
        //      Convert.FromBase64CharArray(inputStr.ToCharArray(),
        //                                    0, inputStr.Length);
        //    return (decodedByteArray);
        //}
    }
}
