using System;
using System.Linq;
using Insight.Database;
using System.Data.SqlClient;
using System.IO;
using Kharban_AdminAPI.Helper;

namespace Kharban_AdminAPI.Repository
{
    public class SettingRepository
    {
        public Response<SettingModel> GetSetting(RequestModel model)
        {

            Response<SettingModel> returnModel = new Response<SettingModel>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<SettingModel>(@"select id, primary_email, primary_contact, provider_cancellation_fee, platform_fee_enable, facebook_link, twitter_link, google_plus_link, instagram_link, nearby_radius, logo, platform_fee, tax, distance, status, created, modified from setting ").FirstOrDefault();
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Setting ";
                returnModel.success = true;
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                LoggingRepository.SaveException(ex);
            }
            return returnModel;
        }

        public Response<int> UpdateSetting(SettingModel model,string imgPath, string imageURL)
        {
            Response<int> returnModel = new Response<int>();
            try
            {

                if (!string.IsNullOrEmpty(model.image) && !string.IsNullOrEmpty(model.image_extension))
                {
                    byte[] imageBytes = Convert.FromBase64String(model.image);
                    File.WriteAllBytes(imgPath, imageBytes);
                }



                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    if (model.type == "platform_fee")
                        DB.ExecuteSql(@"update setting set platform_fee = @platform_fee , platform_fee_enable = @platform_fee_enable where id = @id ", new { id = model.id, platform_fee = model.platform_fee, platform_fee_enable = model.platform_fee_enable });
                    else if (model.type == "tax")
                        DB.ExecuteSql(@"update setting set tax = @tax where id = @id ", new { id = model.id, tax = model.tax });
                    else if (model.type == "distance")
                        DB.ExecuteSql(@"update setting set distance = @distance where id = @id ", new { id = model.id, distance = model.distance });
                    else if (model.type == "logo")
                        DB.ExecuteSql(@"update setting set logo = @logo where id = @id ", new { id = model.id, logo = imageURL });
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = Resource_Kharban.UpdateSuccessfully;
                returnModel.success = true;
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                LoggingRepository.SaveException(ex);
            }
            return returnModel;
        }
    }


}