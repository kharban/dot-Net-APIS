using System;
using System.Linq;
using Insight.Database;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;
using Kharban_AdminAPI.Helper;

namespace Kharban_AdminAPI.Repository
{
    public class CustomerRepository
    {
        public static string GenerateShortCode()
        {
            int size = 6;
            string shortCode = string.Empty;
            StringBuilder result = new StringBuilder(size);

            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
                    byte[] data = new byte[size];
                    using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
                    {
                        crypto.GetBytes(data);
                    }
                    foreach (byte b in data)
                    {
                        result.Append(chars[b % (chars.Length)]);
                    }

                    shortCode = DB.QuerySql<string>("select code from shortcode where code = BINARY ?code", new { code = result.ToString() }).FirstOrDefault();

                    if (!string.IsNullOrEmpty(shortCode))
                    {
                        GenerateShortCode();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingRepository.SaveException(ex);
            }
            return result.ToString();
        }

        public ResponseList<List<CustomerModel>> GetCustomers(RequestModel model)
        {
            string ReturnLink = string.Empty;

            string queryString = string.Empty;
            string orderbyString = string.Empty;
            string queryCount = string.Empty;
            int TotalRecords = 0;
            int recoardFrom = ((model.page - 1) * 10) + 1;
            int recoardTo = model.page * 10;
            ResponseList<List<CustomerModel>> returnModel = new ResponseList<List<CustomerModel>>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    if (model != null)
                    {
                        if (!string.IsNullOrEmpty(model.sortby))
                        {
                            if (model.sortby == "atoz")
                                orderbyString = " order by first_name ";

                            if (model.sortby == "ztoa")
                                orderbyString = " order by first_name desc ";

                            if (model.sortby == "newtoold")
                                orderbyString = " order by created desc";

                            if (model.sortby == "oldtonew")
                                orderbyString = " order by created";
                        }
                        else
                            orderbyString = " order by created desc";

                        queryCount = @"SELECT count(id) totalrecord FROM customer where is_deleted = 0 ";
                        queryString = @"select * from (SELECT ROW_NUMBER() OVER (" + orderbyString + @") row_num, id, user_id, first_name, last_name, contact_no, otp, email, 
                                        country_code, is_mobile_verified , is_email_verified ,is_deleted, created_by ,status, created, modified FROM customer 
                                        where is_deleted = 0 ";

                        if (model.filterby == "name")
                        {
                            queryString += " and ((customer.first_name+' '+customer.last_name like @keyword) or first_name like @keyword or last_name like @keyword) ";
                            queryCount += " and ((customer.first_name+' '+customer.last_name like @keyword) or first_name like @keyword or last_name like @keyword) ";
                        }

                        if (model.filterby == "contact_number")
                        {
                            queryString += " and contact_no like @keyword ";
                            queryCount += " and contact_no like @keyword ";
                        }

                        queryString += " ) t where row_num between " + recoardFrom + " and " + recoardTo;

                        returnModel.result = DB.QuerySql<CustomerModel>(queryString, new
                        {
                            keyword = "%" + model.keyword + "%",
                        }).ToList();


                        TotalRecords = DB.QuerySql<int>(queryCount, new
                        {
                            keyword = "%" + model.keyword + "%",
                        }).FirstOrDefault();
                    }

                    returnModel.totalDocs = TotalRecords;
                    returnModel.limit = 10;
                    returnModel.totalPages = (TotalRecords / 10) + ((TotalRecords % 10) > 0 ? 1 : 0);
                    returnModel.hasNextPage = model.page < returnModel.totalPages;
                    returnModel.hasPrevPage = returnModel.totalPages > 1 && model.page > 1;
                    returnModel.page = model.page;
                    returnModel.nextPage = model.page + 1;
                    returnModel.pagingCounter = recoardFrom;
                    returnModel.prevPage = model.page - 1;
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Users list";
                returnModel.success = true;
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                LoggingRepository.SaveException(ex);
                returnModel.success = false;
            }
            return returnModel;
        }

        public Response<CustomerModel> DeleteCustomers(CustomerRequestModel model)
        {
            string ReturnLink = string.Empty;

            Response<CustomerModel> returnModel = new Response<CustomerModel>();
            try
            {

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql(@"update customer set is_deleted = 1 where id = @id ", new { id = model.id });
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = Resource_Kharban.DeletedSuccessfully;
                returnModel.success = true;
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                returnModel.success = false;
                LoggingRepository.SaveException(ex);
            }
            return returnModel;
        }

        public Response<CustomerModel> UpdateCustomerStatus(CustomerRequestModel model)
        {
            string ReturnLink = string.Empty;

            Response<CustomerModel> returnModel = new Response<CustomerModel>();
            try
            {

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql(@"update customer set status = @status where id = @id ", new { id = model.id, status = model.status });
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = Resource_Kharban.UpdateSuccessfully;
                returnModel.success = true;
            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                returnModel.success = false;
                LoggingRepository.SaveException(ex);
            }
            return returnModel;
        }

        public Response<CustomerModel> UpdateCustomer(CustomerRequestModel model)
        {
            string ReturnLink = string.Empty;

            Response<CustomerModel> returnModel = new Response<CustomerModel>();
            try
            {
                if (model != null)
                {
                    using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                    {
                        DB.ExecuteSql(@"update customer set first_name = @first_name, last_name = @last_name, country_code = @country_code, contact_no = @contact_no where id = @id ", new
                        {
                            id = model.id,
                            model.first_name,
                            model.last_name,
                            model.contact_no,
                            model.country_code
                        });
                    }

                    returnModel.status = (int)EnumClass.ResponseState.Success;
                    returnModel.msg = Resource_Kharban.UpdateSuccessfully;
                    returnModel.success = true;

                }
                else
                {
                    returnModel.msg = "please submit proper data";
                    returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                    returnModel.success = false;
                }

            }
            catch (Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                returnModel.success = false;
                LoggingRepository.SaveException(ex);
            }
            return returnModel;
        }

    }
}