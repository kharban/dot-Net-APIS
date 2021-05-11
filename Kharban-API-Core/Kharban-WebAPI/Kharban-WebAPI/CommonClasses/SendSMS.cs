using Kharban_WebAPI.Models;
using Kharban_WebAPI.Repository;
using Insight.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using Kharban_WebAPI.Helper;
using Nancy.Json;

namespace Kharban_WebAPI.Common
{
    public static class SendSMS
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
            ADD_MEMBERSHIP_PLAN,
            REFFERAL_AMOUNT,
            ADD_ADDONS,
            CALLING,
        }

        public static void Send(Templates smsType, string mobile, string otp, string fromUser, string toUser, string date, string promotionalCode, string profileid = "", string planname = "", string amount = "", string PromoCode = "", string MemberId = null, string PartnerId = null, string deviceToken = null,string connStrings=null)
        {
            string message = string.Empty;

            switch (smsType)
            {
                /*1*/
                case Templates.SEND_OTP:
                    message = "Your OTP for mobile verification on DilKeRishte is " + otp + ".";
                    Utility.SendMsg(mobile, message);
                    break;

                /*2*/
                case Templates.CALL_FOR_VIDEO_CALL:
                    message = "Hi " + toUser + Environment.NewLine + "Come online.I want to have video call with you." + Environment.NewLine + "Message From XXXX(XXXX)." + Environment.NewLine + "Regards" + Environment.NewLine + "Team Dil ke Rishte";
                   Utility.SendMsg(mobile, message);
                    break;

                /*3*/
                case Templates.CALL_FOR_CHAT:
                    message = "Hi " + toUser + Environment.NewLine + "Come online for chat." + Environment.NewLine + "Message From XXXX(XXXX)." + Environment.NewLine + "Regards" + Environment.NewLine + "Team Dil ke Rishte";
                    Utility.SendMsg(mobile, message);
                    break;

                /*4*/
                case Templates.REQUESTER_SHARED_PROFILE:
                    message = "Hi" + Environment.NewLine + "XXXX has shared profile details link with you till XXXX. You may check user's XXXX details by clicking on the link XXXX." + Environment.NewLine + Environment.NewLine + "Regards" + Environment.NewLine + "Dil ke Rishte";
                    Utility.SendMsg(mobile, message);
                    break;
                /*5*/
                case Templates.CHANGE_PASSWORD:
                    message = "Hi " + toUser + "," + Environment.NewLine + "Your Password has been changed." + Environment.NewLine + "Regards" + Environment.NewLine + "Team Dil ke Rishte";
                    Utility.SendMsg(mobile, message);
                    break;
                /*6*/
                case Templates.INVITION_TO_CONNECT:
                    message = "Hi" + Environment.NewLine + toUser + " has sent you the invitation to connect on Dil ke Rishte application." + Environment.NewLine + "Regards" + Environment.NewLine + "Team Dil ke Rishte";
                    //Utility.SendMsg(mobile, message);
                    if (!string.IsNullOrEmpty(deviceToken))
                    {
                        //PushNotificationToProviderAsync(deviceToken, "DilKeRishte", message, "101");
                    }
                    break;
                /*7*/
                case Templates.REFFERED_TO_USE_APPLICATION:
                    message = "Dear User" + Environment.NewLine + "You have been referred by XXXX to use DKR application. Avail membership plans & video packages by applying code XXXX." + Environment.NewLine + "Regards" + Environment.NewLine + "Team Dil ke Rishte";
                    Utility.SendMsg(mobile, message);
                    break;
                /*8*/
                case Templates.REFFERED_AGAIN_TO_USE_APPLICATION:
                    message = "Hi XXXX," + Environment.NewLine + "XXXX has also referred you to DKR application." + Environment.NewLine + "Regards," + Environment.NewLine + "Team Dil ke Rishte";
                    Utility.SendMsg(mobile, message);
                    break;
                /*9*/
                case Templates.PAYMENT_CONFIRMATION:
                    message = "Congrats for hiring the best professional services. Thanks We have received a payment for the same Accounts deptt" + Environment.NewLine + "Team Dil ke Rishte";
                    Utility.SendMsg(mobile, message);
                    break;
                /*10*/
                case Templates.SUCCESSFUL_REGISTER:
                    message = "Hi " + toUser + ", Thanks for registering with DilKeRishte. Verification email has been sent to your email account." + Environment.NewLine + "Regards," + Environment.NewLine + "Team DilKeRishte";
                    //message = "Hi " + toUser + ", Thank you for registering with DilKeRishte. Your Matrimony ID is " + profileid + ". Wishing you luck in your partner search." + Environment.NewLine + "Regards" + Environment.NewLine + "Team DilKeRishte.";
                    Utility.SendMsg(mobile, message);
                    break;
                /*11*/
                case Templates.WELCOME_MESSAGE:
                    message = "Happy to see you on a network of people , who are the best on earth . watch them visually and choose your most ideal one." + Environment.NewLine + "Regards" + Environment.NewLine + "Team Dil ke Rishte";
                    Utility.SendMsg(mobile, message);
                    break;
                /*12*/
                case Templates.WELCOME_FOR_BUSINESS_PURPOSE:
                    message = "Hi, XXXX," + Environment.NewLine + "Happy to take you on board. Welcome to the world's most systematic business process for a endless profitable journey. " + Environment.NewLine + "Regards" + Environment.NewLine + "Team Dil ke Rishte";
                    Utility.SendMsg(mobile, message);
                    break;
                /*13*/
                case Templates.APPOINTMENT_FOR_VIDEO_SHOOT:
                    message = "Congrats, your appointment for shooting has been fixed, please check your mail for details and reach 30 Min. before the set time. Please come prepared with script, costume & makeup, as needed by you. Thanks " + Environment.NewLine + "Customer Care deptt " + Environment.NewLine + "Team Dil ke Rishte";
                    Utility.SendMsg(mobile, message);
                    break;
                /*14*/
                case Templates.REMINDER_FOR_MEMBERSHIP_RENEWAL:
                    message = "Hi XXXX" + Environment.NewLine + "Did you forget to renew your membership? Your Account is near to expire .Be Available for any new people to explore." + Environment.NewLine + "Regards " + Environment.NewLine + "Accounts deptt. " + Environment.NewLine + "Team Dil ke Rishte";
                    Utility.SendMsg(mobile, message);
                    break;
                /*15*/
                case Templates.ADD_MEMBERSHIP_PLAN:
                    message = "Thanks for selecting " + planname + " membership for your profile " + profileid + " on dilkerishte.com.";
                    //message = planname + " membership for your profile " + profileid + " on dilkerishte.com has been activated.";
                    Utility.SendMsg(mobile, message);
                    break;
                /*16*/
                case Templates.REFFERAL_AMOUNT:
                    message = "Hi Friend, " + toUser + " has credited Rs. " + amount + " to your DilKeRishte account. Use the code " + PromoCode + " to redeem the amount on dilkerishte.com";
                    //message = planname + " membership for your profile " + profileid + " on dilkerishte.com has been activated.";
                    Utility.SendMsg(mobile, message);
                    break;
                /*17*/
                case Templates.ADD_ADDONS:
                    message = "Thanks for buying Addons for your profile " + profileid + " on dilkerishte.com.";
                    //message = planname + " membership for your profile " + profileid + " on dilkerishte.com has been activated.";
                    Utility.SendMsg(mobile, message);
                    break;
                // message = "Congrats for honouring your balance payment, which completes the process of hiring our services from your side. Now its our job." + Environment.NewLine + "Regards " + Environment.NewLine + "Accounts deptt. " + Environment.NewLine + "Team Dil ke Rishte";
                /*18*/
                case Templates.CALLING:
                    message = "Hello " + toUser + "," + Environment.NewLine + "Please come online on DilKeRishte, I want to have " + planname + " with you." + Environment.NewLine + Environment.NewLine + "Thanks," + Environment.NewLine + fromUser;
                    Utility.SendMsg(mobile, message);
                    break;
            }


            SendSMS.Save(PartnerId, MemberId, mobile, message, connStrings);

        }

        public static void Save(string sender_id, string reciver_id, string reciver_mobile, string msg,string connStrings)
        {
            try
            {
                string guid = Guid.NewGuid().ToString();
                var perameter = new
                {
                    id = guid,
                    reciver_id = reciver_id,
                    sender_id = sender_id,
                    reciver_mobile = reciver_mobile,
                    msg = msg,
                    created = DateTime.Now,
                };
                using (SqlConnection DB = new SqlConnection(connStrings))
                {
                    DB.ExecuteSql(@"insert into sent_sms(id, reciver_id, sender_id, reciver_mobile, msg, created) Values(?id, ?reciver_id, ?sender_id, ?reciver_mobile, ?msg, ?created)", perameter);
                }
            }
            catch (Exception ex)
            {
                LoggingRepository.SaveException(ex);  
            }
        }

        public static async System.Threading.Tasks.Task<BaseResponse> PushNotificationToProviderAsync(string deviceid, string title, string message, string code = null)
        {


            BaseResponse model = new BaseResponse();
            try
            {


                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                //serverKey - Key from Firebase cloud messaging server  
                tRequest.Headers.Add(string.Format("Authorization: key={0}", SiteKey.PublicKey)); 
                //Sender Id - From firebase project setting  
                tRequest.Headers.Add(string.Format("Sender: id={0}",SiteKey.PrivateKey));
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
                        badge = 1
                    },
                    data = new
                    {
                        NotificationType = code,
                        body = message,
                        title = title,
                        sound = "",
                        badge = 1
                    },
                };
                var serializer = new JavaScriptSerializer();
                Byte[] byteArray = Encoding.UTF8.GetBytes(serializer.Serialize(payload));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer = tReader.ReadToEnd();
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

        public static async System.Threading.Tasks.Task<BaseResponse> PushNotificationToCustomerAsync(string deviceid, string title, string message, string code = null)
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
                        badge = 1
                    },
                    data = new
                    {
                        NotificationType = code,
                        body = message,
                        title = title,
                        sound = "",
                        badge = 1
                    },
                };
                var serializer = new JavaScriptSerializer();
                Byte[] byteArray = Encoding.UTF8.GetBytes(serializer.Serialize(payload));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer = tReader.ReadToEnd();
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

        public static async System.Threading.Tasks.Task<BaseResponse> PushNotificationAsync(string deviceid, string title, string message, string code = null, int Navi_Type = 0, string bookingId = null, int reject_count = 0)
        {


            BaseResponse model = new BaseResponse();
            try
            {


                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                //serverKey - Key from Firebase cloud messaging server  
                tRequest.Headers.Add(string.Format("Authorization: key={0}", SiteKey.PrivateKey));
                //Sender Id - From firebase project setting  
                tRequest.Headers.Add(string.Format("Sender: id={0}", SiteKey.PublicKey));
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
                        badge = Navi_Type,
                        booking_id = bookingId,
                        receipt_reject_count = reject_count
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
                LoggingRepository.SaveException(ex);
            }
            return model;
        }
    }
}