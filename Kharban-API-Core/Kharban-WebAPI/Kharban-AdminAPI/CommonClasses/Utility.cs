using Kharban_AdminAPI.Helper;
using Kharban_AdminAPI.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Security.Cryptography;
using System.Text;

namespace Kharban_AdminAPI.Common
{
    public static class Utility
    {
        public enum DurationType
        {
            Unlimited = 0,
            Day = 1,
            Week = 2,
            Month = 3,
            Year = 4
        }
        public static void SendMsg(string mobile, string message)
        {
            string username = "u35429";
            string password = "9711631084";
            string sender_id = "DKRSMS";
            string msg_token = "BSTqRa";
            string restUrl = "http://manage.sarvsms.com/api/send_transactional_sms.php?username=" + WebUtility.UrlEncode(username) + "&msg_token=" + WebUtility.UrlEncode(msg_token) + "&sender_id=" + WebUtility.UrlEncode(sender_id) + "&message=" + WebUtility.UrlEncode(message) + "&mobile=" + WebUtility.UrlEncode(mobile);
            string URldata;
            string QueryData = null;
            DateTime RequestDate = DateTime.Now;
            int index = restUrl.IndexOf("?");
            if (index > 0)
            {
                QueryData = restUrl.Substring(index + 1);
            }
            URldata = restUrl;
            try
            {
                using (WebClient Client = new WebClient())
                {
                    var http = (HttpWebRequest)WebRequest.Create(URldata);
                    http.Accept = "application/json";
                    http.ContentType = "application/x-www-form-urlencoded"; //"application/json";
                    http.Method = "GET";

                    UTF8Encoding encoding = new UTF8Encoding();
                    RequestDate = DateTime.Now;
                    var response = http.GetResponse();
                    var stream = response.GetResponseStream();
                    var sr = new StreamReader(stream);
                    var content = sr.ReadToEnd();
                    if (!content.Contains("SUCCESS"))
                    {
                        Exception ex = new Exception();
                        LoggingRepository.SaveException(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingRepository.SaveException(ex);
            }
        }

        public static dynamic POSTRequest(string apiName, dynamic request, Enumeration.WebMethod Method, string token)
        {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                             | SecurityProtocolType.Tls11
                                             | SecurityProtocolType.Tls12;
            dynamic dataItem;
            string URldata;
            string restUrl;
            string QueryData = null;
            DateTime RequestDate = DateTime.Now;


            int index = apiName.IndexOf("?");
            if (index > 0)
            {
                QueryData = apiName.Substring(index + 1);

            }

            URldata = apiName;
            try
            {
                using (WebClient Client = new WebClient())
                {
                    if (Method == Enumeration.WebMethod.PUT ||
                        Method == Enumeration.WebMethod.GET && token != string.Empty || Method == Enumeration.WebMethod.POST)
                    {
                        Client.Headers.Add("Authorization", "Bearer " + token);
                        // http.Headers.Add("Authorization", "Bearer " + token);
                    }
                    Client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    var output = Client.UploadString(URldata, request);
                    var d = JsonConvert.DeserializeObject<dynamic>(output);
                    dataItem = d;

                }
                return dataItem;
            }
            catch (WebException ex)
            {
                //ExceptionList.SendErrorToText(ex);
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                string responseData = string.Empty;
                if (ex.Status == WebExceptionStatus.ProtocolError && httpResponse != null)
                {
                    WebResponse response = ex.Response;
                    dataItem = new StreamReader(response.GetResponseStream()).ReadToEnd();
                }
                else
                {
                    dataItem = JsonConvert.SerializeObject(new { code = 500, message = ex.Message, URLdata = apiName });
                }
                dataItem = JsonConvert.DeserializeObject<dynamic>(responseData);

                //SessionKey.Error = JsonConvert.DeserializeObject<ResponseBaseModel>(errData);
                return dataItem;
            }
        }

        public static dynamic SendRequest(string apiName, dynamic request, Enumeration.WebMethod Method, string token)
        {
            dynamic dataItem;

            string restUrl;
            string URldata;

            DateTime RequestDate = DateTime.Now;

            URldata = apiName;

            try
            {
                using (WebClient Client = new WebClient())
                {
                    var http = (HttpWebRequest)WebRequest.Create(URldata);
                    http.Accept = "application/json";
                    http.ContentType = "application/x-www-form-urlencoded"; //"application/json";



                    if (Method == Enumeration.WebMethod.PUT || Method == Enumeration.WebMethod.GET && token != string.Empty || Method == Enumeration.WebMethod.POST)
                    {
                        Client.Headers.Add("Authorization", "Bearer " + token);
                        http.Headers.Add("Authorization", "Bearer " + token);
                    }

                    if (Method == Enumeration.WebMethod.POST || Method == Enumeration.WebMethod.PUT)
                    {
                        string jsonString = string.Empty;
                        // PUT HERE YOUR JSON PARSED CONTENT >>;
                        jsonString = JsonConvert.SerializeObject(request);
                        //  ASCIIEncoding encoding = new ASCIIEncoding();
                        UTF8Encoding encoding = new UTF8Encoding();
                        Byte[] bytes = encoding.GetBytes(jsonString);
                        Stream newStream = http.GetRequestStream();
                        newStream.Write(bytes, 0, bytes.Length);
                        newStream.Close();
                    }
                    RequestDate = DateTime.Now;
                    var response = http.GetResponse();
                    var stream = response.GetResponseStream();
                    var sr = new StreamReader(stream);
                    var content = sr.ReadToEnd();
                    var data = JsonConvert.DeserializeObject<dynamic>(content);
                    dataItem = data;
                }
                return dataItem;
            }
            catch (WebException ex)
            {
                //ExceptionList.SendErrorToText(ex);
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                string responseData = string.Empty;
                if (ex.Status == WebExceptionStatus.ProtocolError && httpResponse != null)
                {
                    WebResponse response = ex.Response;
                    responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();
                }
                else
                {
                    //responseData = JsonConvert.SerializeObject(new ResponseBaseModel() { code = 500, message = ex.Message, URLdata = apiName });
                }
                dataItem = JsonConvert.DeserializeObject<dynamic>(responseData);
                return dataItem;
            }
        }

        public static bool sendMyFileToS3(System.IO.Stream localFilePath, string subDirectoryInBucket, string fileNameInS3)
        {
            string accessKey = SiteKey.AWSSecretKey; 
            string secretKey = SiteKey.AWSSecretKey; 
            string bucketName = SiteKey.S3BucketName;
            return true; //indicate that the file was sent  
        }
        public static NameValueCollection NameValueConvert(FormDataCollection formDataCollection)
        {
            //Validate.IsNotNull("formDataCollection", formDataCollection);

            IEnumerator<KeyValuePair<string, string>> pairs = formDataCollection.GetEnumerator();

            NameValueCollection collection = new NameValueCollection();

            while (pairs.MoveNext())
            {
                KeyValuePair<string, string> pair = pairs.Current;

                collection.Add(pair.Key, pair.Value);
            }

            return collection;
        }

        public static StreamReader ConvertTextToStream(string content)
        {

            // convert string to stream
            byte[] byteArray = Encoding.ASCII.GetBytes(content);
            MemoryStream stream = new MemoryStream(byteArray);

            // convert stream to string
            return new StreamReader(stream);


        }

        public static bool WriteFile(string filePath, string content)
        {
            bool isWritten = false;
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.WriteAllText(filePath, content);
                isWritten = true;
            }
            catch (Exception ex)
            {
                //LoggingRepository.SaveException(ex);
            }
            return isWritten;
        }

        public static string CreateProfileId(int totalCount)
        {
            int len = (totalCount + 1).ToString().Length;
            string ProfileId = string.Empty;
            string Prefix = "VS";
            if (len == 1)
            {
                ProfileId = Prefix + "0000" + (totalCount + 1);
            }
            else if (len == 2)
            {
                ProfileId = Prefix + "000" + (totalCount + 1);
            }
            else if (len == 3)
            {
                ProfileId = Prefix + "00" + (totalCount + 1);
            }
            else if (len == 4)
            {
                ProfileId = Prefix + "0" + (totalCount + 1);
            }
            else if (len == 5)
            {
                ProfileId = Prefix + (totalCount + 1);
            }
            else
            {
                ProfileId = Prefix + (totalCount + 1);
            }
            return ProfileId;
        }

        public static List<KeyValuePair<string, int>> GetEnumList<T>()
        {
            var list = new List<KeyValuePair<string, int>>();
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                list.Add(new KeyValuePair<string, int>(e.ToString(), (int)e));
            }
            return list;
        }

        public static void sendHoroscopeRequest(string hostURI)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(hostURI);
            request.Method = "GET";
            String test = String.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                test = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
            }
        }

        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        public static string ValidateGuid(this string input)
        {
            Guid guid;
            if (!string.IsNullOrEmpty(input))
            {
                if (!Guid.TryParse(input, out guid))
                {
                    // not a validstring representation of a guid
                    throw new Exception("Invalid GUID");
                }
            }
            return input;
        }

        public static string splitAndDecrypt(this string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                string[] inputValue = input.Split(',');
                string inputData = string.Empty;
                foreach (var item in inputValue)
                {
                    inputData += item.Decrypt().ValidateGuid() + ",";
                }
                return inputData.TrimEnd(',');
            }
            return input;
        }

        public static string splitAndEncrypt(this string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                string[] inputValue = input.Split(',');
                string inputData = string.Empty;
                foreach (var item in inputValue)
                {
                    inputData += item.Encrypt() + ",";
                }
                return inputData.TrimEnd(',');
            }
            return input;
        }

        public static string isNullorNot(this string input)
        {
            return input == "" ? null : input;

        }

        public static string FirstLetterCapitalization(this string input)
        {
            if (!string.IsNullOrEmpty(input))
                input = input.Substring(0, 1).ToUpper() + input.Substring(1);
            return input;
        }

        //public static string generateOtp()
        //{
        //    Random generator = new Random();
        //    return randomNo = generator.Next(0, 999999).ToString("D6");
        //}

        //public static bool chkUserConditions(MemberIdModel model)
        //{
        //    int isTrue = 0;
        //    using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
        //    {
        //        isTrue = DB.ExecuteScalarSql<dynamic>("select count(id) from member where AND NOT EXISTS (select * from block_list where (?MemberId = block_list.userid OR ?MemberId = block_list.profileid) AND block_list.status = 1)", model);
        //        isTrue = DB.ExecuteScalarSql<dynamic>("select count(id) from ignore_list where user_id = ?MemberId and partner_id = ?PartnerId ", model);
        //        isTrue = DB.ExecuteScalarSql<dynamic>("select count(id) from report_abuse where userid = ?MemberId and profile_id = ?PartnerId ", model);
        //        isTrue = DB.ExecuteScalarSql<dynamic>("select count(id) from shortlist where shortlist_userid = ?MemberId and shortlist_partnerid = ?PartnerId and shortlist_status = 0 ", model);
        //        isTrue = DB.ExecuteScalarSql<dynamic>("select count(id) from invitation where user_id = ?MemberId and profile_id = ?PartnerId and status = 1 and show_status = 1  ", model);
        //    };


        //    return true;
        //}
    }

    public class Enumeration
    {
        public enum WebMethod
        {
            POST = 1,
            PUT = 2,
            GET = 3
        }

    }
}
