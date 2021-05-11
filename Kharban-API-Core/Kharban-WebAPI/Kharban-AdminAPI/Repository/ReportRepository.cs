using System;
using System.Linq;
using Insight.Database;
using System.Data.SqlClient;
using System.Collections.Generic;
using Kharban_AdminAPI.Helper;

namespace Kharban_AdminAPI.Repository
{
    public class ReportRepository
    {
        public Response<DashBoardModel> GetReportData(RequestModel model)
        {

            Response<DashBoardModel> returnModel = new Response<DashBoardModel>();
            DashBoardModel result = new DashBoardModel();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {

                    result.customer_count = DB.QuerySql<int>(@"select count(id) from customer where is_deleted = 0").FirstOrDefault();
                    result.booking_count = DB.QuerySql<int>(@"select count(id) from booking where is_deleted = 0").FirstOrDefault();

                    DateTime LastSettledDate = DB.QuerySql<DateTime>(@"select top 1 created from payment_settled_history order by created desc").FirstOrDefault();

                    string QueryString = @"select isnull(sum(amount),0) transaction_amount from booking_earning ";

                    if (LastSettledDate != DateTime.MinValue)
                        QueryString += " where created > @LastSettledDate ";

                    result.transaction_amount = DB.QuerySql<decimal>(QueryString, new { LastSettledDate = LastSettledDate }).FirstOrDefault();

                    returnModel.result = result;
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Data Count";
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

        public Response<List<CustomerModel>> GetCustomerList(RequestModel model)
        {

            Response<List<CustomerModel>> returnModel = new Response<List<CustomerModel>>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<CustomerModel>(@"select id, user_id, first_name, last_name, contact_no, otp, email, country_code, is_mobile_verified , is_email_verified ,is_deleted, created_by ,status, created, modified FROM customer where is_deleted = 0 ").ToList();
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Customer List";
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

        public Response<List<ProviderModel>> GetProviderList(RequestModel model)
        {
            Response<List<ProviderModel>> returnModel = new Response<List<ProviderModel>>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<ProviderModel>(@"SELECT id, user_id, first_name, last_name, profile_picture, iban_no, contact_no, country_code, otp, email, is_policy_accepted, is_mobile_verified, is_email_verified, admin_approve, is_online, is_notification_enable, auth_token, created_by, status, created, modified FROM provider where is_deleted = 0 ").ToList();
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Provider List";
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

        public Response<List<BookingModel>> GetBookingList(RequestModel model)
        {

            Response<List<BookingModel>> returnModel = new Response<List<BookingModel>>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<BookingModel>(@"select id, booking_code, customer_id, provider_id, service_id, coupon_id, address_title, landmark, longitude, description, booking_status, provider_cancellation_fee, status, is_deleted, created, modified, payment_type FROM booking where is_deleted = 0 ").ToList();
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Booking List";
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

        public Response<List<TransactionModel>> GetTransictionList(RequestModel model)
        {
            Response<List<TransactionModel>> returnModel = new Response<List<TransactionModel>>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DateTime LastSettledDate = DB.QuerySql<DateTime>(@"select top 1 created from payment_settled_history order by created desc").FirstOrDefault();

                    string QueryString = @"select provider.id, concat(provider.first_name,' ',provider.last_name) provider_name, provider.iban_no,
                                            count(booking_earning.id) booking_count, sum(booking_earning.amount) pending_amount from booking_earning
                                            left join booking on booking_earning.booking_id = booking.id 
                                            left join provider on booking.provider_id = provider.id
                                            where booking_earning.is_deleted = 0 ";

                    if (LastSettledDate != DateTime.MinValue)
                    {
                        QueryString += " and booking_earning.created > @LastSettledDate ";
                    }

                    QueryString += " group by provider.first_name, provider.last_name, provider.id, provider.iban_no ";

                    returnModel.result = DB.QuerySql<TransactionModel>(QueryString, new { LastSettledDate }).ToList();

                }


                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Transiction List";
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