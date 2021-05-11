using System;
using System.IO;
using System.Net;
using System.Text;
using Kharban_AdminAPI.Helper;
using Kharban_AdminAPI.Models;
using Nancy.Json;

namespace Kharban_AdminAPI.CommonClasses
{
    public class SMSNotification
    {
        public enum Templates
        {
            SEND_OTP,
            CALL_FOR_VIDEO_CALL,
            CALL_FOR_CHAT,
            REQUESTER_SHARED_PROFILE,
            CHANGE_PASSWORD,
            INVITION_TO_CONNECT,
            REFFERED_TO_USE_APPLICATION,
            REFFERED_AGAIN_TO_USE_APPLICATION,
            PAYMENT_CONFIRMATION,
            SUCCESSFUL_REGISTER,
            WELCOME_MESSAGE,
            WELCOME_FOR_BUSINESS_PURPOSE,
            APPOINTMENT_FOR_VIDEO_SHOOT,
            REMINDER_FOR_MEMBERSHIP_RENEWAL,
            PROVIDER_APPROVAL,
        }
        public static void Send(Templates smsType, string mobile, string otp, string fromUser, string toUser, string date, string promotionalCode)
        {
            string message = string.Empty;
            switch (smsType)
            {
                /*1*/
                case Templates.SEND_OTP:
                    message = "Your OTP for mobile no. verification is " + otp + "." + Environment.NewLine + "Regards" + Environment.NewLine + "Team Dil ke Rishte";
                    break;

                /*2*/
                case Templates.CALL_FOR_VIDEO_CALL:
                    message = "Hi " + fromUser + Environment.NewLine + "Come online.I want to have video call with you." + Environment.NewLine + "Message From XXXX(XXXX)." + Environment.NewLine + "Regards" + Environment.NewLine + "Team Dil ke Rishte";
                    break;

                /*3*/
                case Templates.CALL_FOR_CHAT:
                    message = "Hi " + fromUser + Environment.NewLine + "Come online for chat." + Environment.NewLine + "Message From XXXX(XXXX)." + Environment.NewLine + "Regards" + Environment.NewLine + "Team Dil ke Rishte";
                    break;

                /*4*/
                case Templates.REQUESTER_SHARED_PROFILE:
                    message = "Hi" + Environment.NewLine + "XXXX has shared profile details link with you till XXXX. You may check user's XXXX details by clicking on the link XXXX." + Environment.NewLine + Environment.NewLine + "Regards" + Environment.NewLine + "Dil ke Rishte";
                    break;
                /*5*/
                case Templates.CHANGE_PASSWORD:
                    message = "Hi " + fromUser + "," + Environment.NewLine + "Your Password has been changed." + Environment.NewLine + "Regards" + Environment.NewLine + "Team Dil ke Rishte";
                    break;
                /*6*/
                case Templates.INVITION_TO_CONNECT:
                    message = "Hi" + Environment.NewLine + fromUser + "has sent you the invitation to connect on Dil ke Rishte application." + Environment.NewLine + "Regards" + Environment.NewLine + "Team Dil ke Rishte";
                    break;
                /*7*/
                case Templates.REFFERED_TO_USE_APPLICATION:
                    message = "Dear User" + Environment.NewLine + "You have been referred by XXXX to use DKR application. Avail membership plans & video packages by applying code XXXX." + Environment.NewLine + "Regards" + Environment.NewLine + "Team Dil ke Rishte";
                    break;
                /*8*/
                case Templates.REFFERED_AGAIN_TO_USE_APPLICATION:
                    message = "Hi XXXX," + Environment.NewLine + "XXXX has also referred you to DKR application." + Environment.NewLine + "Regards," + Environment.NewLine + "Team Dil ke Rishte";
                    break;
                /*9*/
                case Templates.PAYMENT_CONFIRMATION:
                    message = "Congrats for hiring the best professional services. Thanks We have received a payment for the same Accounts deptt" + Environment.NewLine + "Team Dil ke Rishte";
                    break;
                /*10*/
                case Templates.SUCCESSFUL_REGISTER:
                    message = "Happy to see you on board with the world's most innovative concept ,Promising excellent career with appropriate atmosphere for professional and personal growth." + Environment.NewLine + "Regards" + Environment.NewLine + "Team Dil ke Rishte";
                    break;
                /*11*/
                case Templates.WELCOME_MESSAGE:
                    message = "Happy to see you on a network of people , who are the best on earth . watch them visually and choose your most ideal one." + Environment.NewLine + "Regards" + Environment.NewLine + "Team Dil ke Rishte";
                    break;
                /*12*/
                case Templates.WELCOME_FOR_BUSINESS_PURPOSE:
                    message = "Hi, XXXX," + Environment.NewLine + "Happy to take you on board. Welcome to the world's most systematic business process for a endless profitable journey. " + Environment.NewLine + "Regards" + Environment.NewLine + "Team Dil ke Rishte";
                    break;
                /*13*/
                case Templates.APPOINTMENT_FOR_VIDEO_SHOOT:
                    message = "Congrats, your appointment for shooting has been fixed, please check your mail for details and reach 30 Min. before the set time. Please come prepared with script, costume & makeup, as needed by you. Thanks " + Environment.NewLine + "Customer Care deptt " + Environment.NewLine + "Team Dil ke Rishte";
                    break;
                /*14*/
                case Templates.REMINDER_FOR_MEMBERSHIP_RENEWAL:
                    message = "Hi XXXX" + Environment.NewLine + "Did you forget to renew your membership? Your Account is near to expire .Be Available for any new people to explore." + Environment.NewLine + "Regards " + Environment.NewLine + "Accounts deptt. " + Environment.NewLine + "Team Dil ke Rishte";
                    break;
                case Templates.PROVIDER_APPROVAL:
                    message = "Your Account is approved please login";
                    break;


                    // message = "Congrats for honouring your balance payment, which completes the process of hiring our services from your side. Now its our job." + Environment.NewLine + "Regards " + Environment.NewLine + "Accounts deptt. " + Environment.NewLine + "Team Dil ke Rishte";
            }
        }
       
        public static async System.Threading.Tasks.Task<BaseResponse> PushNotificationAsync(string deviceid, string title, string message, string code = null, int Navi_Type = 0)
        {


            BaseResponse model = new BaseResponse();
            try
            {


                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                //serverKey - Key from Firebase cloud messaging server  
                tRequest.Headers.Add(string.Format("Authorization: key={0}", SiteKey.PublicKey));
                //Sender Id - From firebase project setting  
                tRequest.Headers.Add(string.Format("Sender: id={0}", SiteKey.PrivateKey));
                tRequest.ContentType = "application/json";
                var payload = new
                {
                    to = deviceid,
                    priority = "high",
                    content_available = true,

                    notification = new
                    {
                        body = message,
                        title = title,
                        sound = "",
                        badge = Navi_Type
                    },
                    data = new
                    {
                        NotificationType = code,
                        body = message,
                        title = title,
                        sound = "",
                        badge = Navi_Type
                    },
                };
                
                var serializer = new JavaScriptSerializer();
                Byte[] byteArray = Encoding.UTF8.GetBytes(serializer.Serialize(payload));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream =await tRequest.GetRequestStreamAsync())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse =await tRequest.GetResponseAsync())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer =await tReader.ReadToEndAsync();
                                    model.StatusMessage = sResponseFromServer;
                                }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //LoggingRepository.SaveException(ex);
            }
            return model;
        }
   
    }

}