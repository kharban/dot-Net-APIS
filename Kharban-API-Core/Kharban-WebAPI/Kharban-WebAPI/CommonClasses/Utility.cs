using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using Insight.Database;
using System.Security.Cryptography;
using Kharban_WebAPI.Repository;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon;
using Kharban_WebAPI.Helper;

namespace Kharban_WebAPI.Common
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
        public enum VideoType
        {
            MEDIA_1080, MEDIA_720, MEDIA_480, MEDIA_360
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
            catch (WebException ex)
            {
                //ExceptionList.SendErrorToText(ex, connString);
            }
        }
        public static bool sendMyFileToS3(System.IO.Stream localFilePath, string subDirectoryInBucket, string fileNameInS3)
        {
            IAmazonS3 client = new AmazonS3Client(RegionEndpoint.APSouth1);
            TransferUtility utility = new TransferUtility(client);
            TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();
            string bucketName = SiteKey.S3BucketName;

            if (subDirectoryInBucket == "" || subDirectoryInBucket == null)
            {
                request.BucketName = bucketName; //no subdirectory just bucket name  
            }
            else
            {   // subdirectory and bucket name  
                request.BucketName = bucketName + @"/" + subDirectoryInBucket;
            }
            request.Key = fileNameInS3; //file name up in S3  
            request.InputStream = localFilePath;
            request.CannedACL = S3CannedACL.PublicRead;
            utility.Upload(request); //commensing the transfer  

            return true; //indicate that the file was sent  
        }

        //public static NameValueCollection NameValueConvert(FormDataCollection formDataCollection)
        //{
        //    //Validate.IsNotNull("formDataCollection", formDataCollection);

        //    IEnumerator<KeyValuePair<string, string>> pairs = formDataCollection.GetEnumerator();

        //    NameValueCollection collection = new NameValueCollection();

        //    while (pairs.MoveNext())
        //    {
        //        KeyValuePair<string, string> pair = pairs.Current;

        //        collection.Add(pair.Key, pair.Value);
        //    }

        //    return collection;
        //}

        public static string RefineBase64(string value)
        {
            value = value.Replace(@"\", "");
            int c = value.Length % 4;
            if ((c) != 0)
                value = value.PadRight((value.Length + (4 - c)), '=');
            return value;
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
                LoggingRepository.SaveException(ex);
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

        //public static void TranscodeVideo(String fileName = "GSNHWCLTLV_1433186308.mp4", string folderPath = "GSNHWCLTLV/")
        //{
        //    string accessKey = System.Configuration.ConfigurationManager.AppSettings["AWSAccessKey"].ToString();
        //    string secretKey = System.Configuration.ConfigurationManager.AppSettings["AWSSecretKey"].ToString();
        //    string bucketName = System.Configuration.ConfigurationManager.AppSettings["S3BucketName"].ToString();
        //    string arn_Role = System.Configuration.ConfigurationManager.AppSettings["ARN_Role"].ToString();
        //    //string regionEndPoint = System.Configuration.ConfigurationManager.AppSettings["S3RegionEndPoint"].ToString();


        //    //filename = filename.Replace("+", "");
        //    //string file_Path = ;
        //    //if (string.IsNullOrEmpty(Path.GetExtension(filePath)))
        //    //{
        //    //    filePath += ".mp4";
        //    //}

        //    //using (var client = new Amazon.S3.AmazonS3Client(accessKey, secretKey, S3Config))
        //    //{
        //    //    PutObjectRequest request = new PutObjectRequest { BucketName = bucketName, CannedACL = S3CannedACL.PublicRead, Key = filePath, InputStream = fs };
        //    //    client.PutObject(request);
        //    //}
        //    string outputPath_1080 = folderPath + "cvt/1080/" + fileName;
        //    string outputPath_720 = folderPath + "cvt/720/" + fileName;
        //    string outputPath_480 = folderPath + "cvt/480/" + fileName;
        //    string outputPath_360 = folderPath + "cvt/360/" + fileName;

        //    //AmazonS3Config S3Config = new AmazonS3Config();

        //    var etsClient = new AmazonElasticTranscoderClient(accessKey, secretKey, RegionEndpoint.APSouth1);

        //    var notifications = new Notifications()
        //    {
        //        //Completed = "arn:aws:sns:us-west-2:277579135337:Transcode",
        //        //Error = "arn:aws:sns:us-west-2:277579135337:Transcode",
        //        //Progressing = "arn:aws:sns:us-west-2:277579135337:Transcode",
        //        //Warning = "arn:aws:sns:us-west-2:277579135337:Transcode"
        //    };

        //    var pipeline = new Pipeline();


        //    etsClient.CreateJob(new CreateJobRequest()
        //    {
        //        PipelineId = "1543421132365-44xkcf",
        //        Input = new JobInput()
        //        {
        //            AspectRatio = "auto",
        //            Container = "mp4", //H.264
        //            FrameRate = "auto",
        //            Interlaced = "auto",
        //            Resolution = "auto",
        //            Key = folderPath + fileName,
        //        },
        //        Output = new CreateJobOutput()
        //        {
        //            ThumbnailPattern = "",
        //            Rotate = "0",
        //            PresetId = "1351620000001-000001", //Generic-1080 px
        //            Key = outputPath_1080,
        //        }
        //    });
        //    etsClient.CreateJob(new CreateJobRequest()
        //    {
        //        PipelineId = "1543421132365-44xkcf",
        //        Input = new JobInput()
        //        {
        //            AspectRatio = "auto",
        //            Container = "mp4", //H.264
        //            FrameRate = "auto",
        //            Interlaced = "auto",
        //            Resolution = "auto",
        //            Key = folderPath + fileName,
        //        },
        //        Output = new CreateJobOutput()
        //        {
        //            ThumbnailPattern = "",
        //            Rotate = "0",
        //            PresetId = "1351620000001-000010", //Generic-720 px
        //            Key = outputPath_720,
        //        }
        //    });
        //    etsClient.CreateJob(new CreateJobRequest()
        //    {
        //        PipelineId = "1543421132365-44xkcf",
        //        Input = new JobInput()
        //        {
        //            AspectRatio = "auto",
        //            Container = "mp4", //H.264
        //            FrameRate = "auto",
        //            Interlaced = "auto",
        //            Resolution = "auto",
        //            Key = folderPath + fileName,
        //        },
        //        Output = new CreateJobOutput()
        //        {
        //            ThumbnailPattern = "",
        //            Rotate = "0",
        //            PresetId = "1351620000001-000020", //Generic-480 px
        //            Key = outputPath_480,
        //        }
        //    });
        //    etsClient.CreateJob(new CreateJobRequest()
        //    {
        //        PipelineId = "1543421132365-44xkcf",
        //        Input = new JobInput()
        //        {
        //            AspectRatio = "auto",
        //            Container = "mp4", //H.264
        //            FrameRate = "auto",
        //            Interlaced = "auto",
        //            Resolution = "auto",
        //            Key = folderPath + fileName,
        //        },
        //        Output = new CreateJobOutput()
        //        {
        //            ThumbnailPattern = "",
        //            Rotate = "0",
        //            PresetId = "1351620000001-000061", //Generic-360 px
        //            Key = outputPath_360,
        //        }
        //    });
        //}

        //public static bool TranscodeVideo(VideoType type, string currentFilePath, string convertedFilePath)
        //{
        //    string accessKey = System.Configuration.ConfigurationManager.AppSettings["AWSAccessKey"].ToString();
        //    string secretKey = System.Configuration.ConfigurationManager.AppSettings["AWSSecretKey"].ToString();
        //    string bucketName = System.Configuration.ConfigurationManager.AppSettings["S3BucketName"].ToString();
        //    string arn_Role = System.Configuration.ConfigurationManager.AppSettings["ARN_Role"].ToString();
            //string regionEndPoint = System.Configuration.ConfigurationManager.AppSettings["S3RegionEndPoint"].ToString();


            //filename = filename.Replace("+", "");
            //string file_Path = ;
            //if (string.IsNullOrEmpty(Path.GetExtension(filePath)))
            //{
            //    filePath += ".mp4";
            //}

            //using (var client = new Amazon.S3.AmazonS3Client(accessKey, secretKey, S3Config))
            //{
            //    PutObjectRequest request = new PutObjectRequest { BucketName = bucketName, CannedACL = S3CannedACL.PublicRead, Key = filePath, InputStream = fs };
            //    client.PutObject(request);
            //}
        //    var etsClient = new AmazonElasticTranscoderClient(accessKey, secretKey, RegionEndpoint.APSouth1);
        //    var pipeline = new Pipeline();

        //    if (type == VideoType.MEDIA_1080)
        //    {
        //        string outputPath_1080 = convertedFilePath;

        //        etsClient.CreateJob(new CreateJobRequest()
        //        {
        //            PipelineId = "1543421132365-44xkcf",
        //            Input = new JobInput()
        //            {
        //                AspectRatio = "auto",
        //                Container = "mp4", //H.264
        //                FrameRate = "auto",
        //                Interlaced = "auto",
        //                Resolution = "auto",
        //                Key = currentFilePath,
        //            },
        //            Output = new CreateJobOutput()
        //            {
        //                ThumbnailPattern = "",
        //                Rotate = "0",
        //                PresetId = "1351620000001-000001", //Generic-1080 px
        //                Key = outputPath_1080,
        //            }
        //        });
        //    }
        //    else if (type == VideoType.MEDIA_720)
        //    {
        //        string outputPath_720 = convertedFilePath;
        //        etsClient.CreateJob(new CreateJobRequest()
        //        {
        //            PipelineId = "1543421132365-44xkcf",
        //            Input = new JobInput()
        //            {
        //                AspectRatio = "auto",
        //                Container = "mp4", //H.264
        //                FrameRate = "auto",
        //                Interlaced = "auto",
        //                Resolution = "auto",
        //                Key = currentFilePath,
        //            },
        //            Output = new CreateJobOutput()
        //            {
        //                ThumbnailPattern = "",
        //                Rotate = "0",
        //                PresetId = "1351620000001-000010", //Generic-720 px
        //                Key = outputPath_720,
        //            }
        //        });
        //    }
        //    else if (type == VideoType.MEDIA_480)
        //    {
        //        string outputPath_480 = convertedFilePath;
        //        etsClient.CreateJob(new CreateJobRequest()
        //        {
        //            PipelineId = "1543421132365-44xkcf",
        //            Input = new JobInput()
        //            {
        //                AspectRatio = "auto",
        //                Container = "mp4", //H.264
        //                FrameRate = "auto",
        //                Interlaced = "auto",
        //                Resolution = "auto",
        //                Key = currentFilePath,
        //            },
        //            Output = new CreateJobOutput()
        //            {
        //                ThumbnailPattern = "",
        //                Rotate = "0",
        //                PresetId = "1351620000001-000020", //Generic-480 px
        //                Key = outputPath_480,
        //            }
        //        });
        //    }
        //    else if (type == VideoType.MEDIA_360)
        //    {
        //        string outputPath_360 = convertedFilePath;
        //        etsClient.CreateJob(new CreateJobRequest()
        //        {
        //            PipelineId = "1543421132365-44xkcf",
        //            Input = new JobInput()
        //            {
        //                AspectRatio = "auto",
        //                Container = "mp4", //H.264
        //                FrameRate = "auto",
        //                Interlaced = "auto",
        //                Resolution = "auto",
        //                Key = currentFilePath,
        //            },
        //            Output = new CreateJobOutput()
        //            {
        //                ThumbnailPattern = "",
        //                Rotate = "0",
        //                PresetId = "1351620000001-000061", //Generic-360 px
        //                Key = outputPath_360,
        //            }
        //        });
        //    }

        //    return true; //deleteFileFromS3(currentFilePath);
        //    //AmazonS3Config S3Config = new AmazonS3Config();

        //    //var notifications = new Notifications()
        //    //{
        //    //    //Completed = "arn:aws:sns:us-west-2:277579135337:Transcode",
        //    //    //Error = "arn:aws:sns:us-west-2:277579135337:Transcode",
        //    //    //Progressing = "arn:aws:sns:us-west-2:277579135337:Transcode",
        //    //    //Warning = "arn:aws:sns:us-west-2:277579135337:Transcode"
        //    //};
        //}

        //public static ResponseModel CheckGovtIdentity(string MemberId)
        //{
        //    List<GovtIdentityModel> govtIdentityModel = new List<GovtIdentityModel>();
        //    ResponseModel responseModel = new ResponseModel();
        //    try
        //    {
        //        using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
        //        {
        //            var memberGovtIdApproveCount = DB.ExecuteScalarSql<dynamic>(@"SELECT count(id) FROM member_govt_identity where userid = ?MemberId and approved = 1", new { MemberId });
        //            //MemberGovtIdentityModel memberGovtIdApproveCount = DB.QuerySql<MemberGovtIdentityModel>(@"SELECT id FROM member_govt_identity where userid = ?MemberId and approved = 1", new { MemberId }).FirstOrDefault();
        //            if (memberGovtIdApproveCount > 0)
        //            {
        //                responseModel.Status = true;
        //            }
        //            else
        //            {
        //                int memberGovtIdCount = 0;
        //                MemberIdModel memberIdModel = new MemberIdModel();
        //                var membergovtIdentityCount = DB.ExecuteScalarSql<dynamic>(@"SELECT count(id) FROM member_govt_identity where userid = ?MemberId and approved = 3", new { MemberId });
        //                if (membergovtIdentityCount == 5)
        //                {
        //                    responseModel.StatusMessage = Resource_DKR.PleaseUploadAtLeastOneIdentity;
        //                    responseModel.StatusKey = 3;
        //                    memberIdModel.MemberId = MemberId;
        //                }
        //                else
        //                {
        //                    membergovtIdentityCount = DB.ExecuteScalarSql<dynamic>(@"SELECT count(id) FROM member_govt_identity where userid = ?MemberId and approved = 0", new { MemberId });
        //                    if (membergovtIdentityCount > 0)
        //                    {
        //                        responseModel.StatusMessage = Resource_DKR.YourVerificationIdentityStillUnderApproval;
        //                        responseModel.StatusKey = 0;
        //                        memberIdModel.MemberId = MemberId;
        //                    }
        //                    else
        //                    {
        //                        responseModel.StatusMessage = Resource_DKR.YourVerificationIdentityHasBeenRejected;
        //                        responseModel.StatusKey = 2;
        //                        memberIdModel.MemberId = MemberId;
        //                    }
        //                }
        //                responseModel.Status = false;
        //                responseModel.Type = Resource_DKR.Warning;
        //                responseModel.Title = Resource_DKR.Alert;
        //                responseModel.MemberData = memberIdModel;
        //            }
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        responseModel.Status = false;
        //        responseModel.StatusMessage = Resource_DKR.SomethingWentWrong;
        //        LoggingRepository.SaveException(ex);
        //    }
        //    return responseModel;
        //}

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

        //public static string splitAndDecrypt(this string input)
        //{
        //    if (!string.IsNullOrEmpty(input))
        //    {
        //        string[] inputValue = input.Split(',');
        //        string inputData = string.Empty;
        //        foreach (var item in inputValue)
        //        {
        //            inputData += item.Decrypt().ValidateGuid() + ",";
        //        }
        //        return inputData.TrimEnd(',');
        //    }
        //    return input;
        //}

        //public static string splitAndEncrypt(this string input)
        //{
        //    if (!string.IsNullOrEmpty(input))
        //    {
        //        string[] inputValue = input.Split(',');
        //        string inputData = string.Empty;
        //        foreach (var item in inputValue)
        //        {
        //            inputData += item.Encrypt() + ",";
        //        }
        //        return inputData.TrimEnd(',');
        //    }
        //    return input;
        //}

        //public static string isNullorNot(this string input)
        //{
        //    return input == "" ? null : input;

        //}

        //public static string generateOtp()
        //{
        //    Random generator = new Random();
        //    return generator.Next(0, 999999).ToString("D6");
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

        public static string AlphaNumericRandomCode(int len)
        {
            string alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string small_alphabets = "abcdefghijklmnopqrstuvwxyz";
            string numbers = "1234567890";

            string characters = string.Empty;

            characters = alphabets + small_alphabets + numbers;
            string otp = string.Empty;
            for (int i = 0; i < len; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (otp.IndexOf(character) != -1);
                otp += character;
            }

            return otp;
        }

        //public static Stream GetFileStreamFromS3(string path)
        //{
        //    Stream streamResponse = null;
        //    IAmazonS3 client = new AmazonS3Client(RegionEndpoint.APSouth1);
        //    try
        //    {
        //        GetObjectRequest request = new GetObjectRequest
        //        {
        //            BucketName = System.Configuration.ConfigurationManager.AppSettings["S3BucketName"].ToString(),
        //            Key = path
        //        };
        //        GetObjectResponse response = client.GetObject(request);
        //        streamResponse = response.ResponseStream;
        //    }
        //    catch
        //    {

        //    }
        //    return streamResponse;
        //}

        //public static bool deleteFileFromS3(string filePath)
        //{
        //    IAmazonS3 client = new AmazonS3Client(RegionEndpoint.APSouth1);
        //    TransferUtility utility = new TransferUtility(client);
        //    TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();
        //    string bucketName = System.Configuration.ConfigurationManager.AppSettings["S3BucketName"].ToString();


        //    var deleteObjectRequest = new DeleteObjectRequest
        //    {
        //        BucketName = bucketName,
        //        Key = filePath
        //    };

        //    client.DeleteObjectAsync(deleteObjectRequest);

        //    return true; //indicate that the file was sent  
        //}

        //public static void DownloadObject(string keyName)
        //{
        //    string[] keySplit = keyName.Split('/');
        //    string fileName = keySplit[keySplit.Length - 1];
        //    string dest = Path.Combine(HttpRuntime.CodegenDir, fileName);
        //    using (IAmazonS3 client = new AmazonS3Client(RegionEndpoint.APSouth1))
        //    {
        //        GetObjectRequest request = new GetObjectRequest
        //        {
        //            BucketName = System.Configuration.ConfigurationManager.AppSettings["S3BucketName"].ToString(),
        //            Key = keyName
        //        };

        //        using (GetObjectResponse response = client.GetObject(request))
        //        {
        //            response.WriteResponseStreamToFile(dest, false);
        //        }

        //        HttpContext.Current.Response.Clear();
        //        HttpContext.Current.Response.AppendHeader("content-disposition", "attachment; filename=" + fileName);
        //        HttpContext.Current.Response.ContentType = "application/octet-stream";
        //        HttpContext.Current.Response.TransmitFile(dest);
        //        HttpContext.Current.Response.Flush();
        //        HttpContext.Current.Response.End();

        //        // Clean up temporary file.
        //        System.IO.File.Delete(dest);
        //    }
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
