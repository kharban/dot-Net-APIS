using System;
using System.Linq;
using Insight.Database;
using System.Data.SqlClient;
using System.Collections.Generic;
using Kharban_AdminAPI.Helper;

namespace Kharban_AdminAPI.Repository
{
    public class PaymentRepository
    {
        public ResponseList<PaymentModel> GetPayments(RequestModel model)
        {

            string queryString = string.Empty;
            string orderbyString = string.Empty;
            string queryCount = string.Empty;
            int TotalRecords = 0;
            int recoardFrom = ((model.page - 1) * 10) + 1;
            int recoardTo = model.page * 10;

            ResponseList<PaymentModel> returnModel = new ResponseList<PaymentModel>();
            returnModel.result = new PaymentModel();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    if (!string.IsNullOrEmpty(model.sortby))
                    {
                        //if (model.sortby == "atoz")
                        //    queryString += " order by name ";

                        //if (model.sortby == "ztoa")
                        //    queryString += " order by name desc ";

                        if (model.sortby == "newtoold")
                            orderbyString = " order by transaction_date desc";

                        if (model.sortby == "oldtonew")
                            orderbyString = " order by transaction_date";
                    }
                    else
                        orderbyString = " order by transaction_date desc";

                    queryCount = @"select count(booking_platform_fee.id) totalrecord from booking_platform_fee
                                    left join booking on booking.id = booking_id
                                    left join customer on customer.id = customer_id
                                    left join provider on provider.id = provider_id
                                    where booking_platform_fee.is_deleted = 0 ";

                    queryString = @"select * from (SELECT ROW_NUMBER() OVER (" + orderbyString + @") row_num, booking_platform_fee.id,booking.booking_code,concat(customer.first_name,' ',customer.last_name) customer_name,
                                    concat(provider.first_name,' ',provider.last_name) provider_name, transaction_id, transaction_date, iban_no, booking_platform_fee.amount 
                                    from booking_platform_fee
                                    left join booking on booking.id = booking_id
                                    left join customer on customer.id = customer_id
                                    left join provider on provider.id = provider_id
                                    where booking_platform_fee.is_deleted = 0 ";

                    if (model.filterby == "provider_name")
                    {
                        queryString += " and ((provider.first_name+' '+provider.last_name like @keyword) or provider.first_name like @keyword or provider.last_name like @keyword) ";
                        queryCount += " and ((provider.first_name+' '+provider.last_name like @keyword) or provider.first_name like @keyword or provider.last_name like @keyword) ";
                    }
                    if (model.filterby == "customer_name")
                    {
                        queryString += " and ((customer.first_name+' '+customer.last_name like @keyword) or customer.first_name like @keyword or customer.last_name like @keyword) ";
                        queryCount += " and ((customer.first_name+' '+customer.last_name like @keyword) or customer.first_name like @keyword or customer.last_name like @keyword) ";
                    }
                    if (model.filterby == "transaction_id")
                    {
                        queryString += " and booking_platform_fee.transaction_id like @keyword ";
                        queryCount += " and booking_platform_fee.transaction_id like @keyword ";
                    }

                    if (model.startdate != DateTime.MinValue)
                    {
                        queryString += " and CONVERT(date, booking_platform_fee.transaction_date) >= CONVERT(date, @StartDate) ";
                        queryCount += " and CONVERT(date, booking_platform_fee.transaction_date) >= CONVERT(date, @StartDate) ";
                    }

                    if (model.enddate != DateTime.MinValue)
                    {
                        queryString += " and CONVERT(date, booking_platform_fee.transaction_date) <= CONVERT(date, @EndDate) ";
                        queryCount += " and CONVERT(date, booking_platform_fee.transaction_date) <= CONVERT(date, @EndDate) ";
                    }

                    queryString += " ) t where row_num between " + recoardFrom + " and " + recoardTo;


                    returnModel.result.PaymentList = DB.QuerySql<PaymentsModel>(queryString, new { keyword = "%" + model.keyword + "%", EndDate = model.enddate, StartDate = model.startdate }).ToList();

                    returnModel.result.TotalEarning = DB.QuerySql<decimal>(@"select sum(booking_platform_fee.amount) totalearning from booking_platform_fee
                                    left join booking on booking.id = booking_id
                                    left join customer on customer.id = customer_id
                                    left join provider on provider.id = provider_id
                                    where booking_platform_fee.is_deleted = 0 ").FirstOrDefault();

                    TotalRecords = DB.QuerySql<int>(queryCount, new { keyword = "%" + model.keyword + "%", EndDate = model.enddate, StartDate = model.startdate }).FirstOrDefault();


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
                returnModel.msg = "Payment List";
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

        public Response<List<PaymentsModel>> GetPaymentExport(RequestModel model)
        {

            string queryString = string.Empty;
            string queryCount = string.Empty;
            int TotalRecords = 0;
            Response<List<PaymentsModel>> returnModel = new Response<List<PaymentsModel>>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {

                    queryString = @"select booking_platform_fee.id,booking.booking_code,concat(customer.first_name,' ',customer.last_name) customer_name,
                                    concat(provider.first_name,' ',provider.last_name) provider_name, transaction_id, transaction_date, iban_no, booking_platform_fee.amount 
                                    from booking_platform_fee
                                    left join booking on booking.id = booking_id
                                    left join customer on customer.id = customer_id
                                    left join provider on provider.id = provider_id
                                    where booking_platform_fee.is_deleted = 0 ";


                    if (model != null && !string.IsNullOrEmpty(model.filterby) && !string.IsNullOrEmpty(model.keyword))
                    {
                        if (model.filterby == "provider_name")
                        {
                            queryString += " and (provider.first_name like @keyword or provider.last_name like @keyword) ";

                        }
                        if (model.filterby == "customer_name")
                        {
                            queryString += " and (customer.first_name like @keyword or customer.last_name like @keyword) ";

                        }
                        if (model.filterby == "transaction_id")
                        {
                            queryString += " and booking_platform_fee.transaction_id like @keyword ";

                        }
                    }



                    if (model != null && !string.IsNullOrEmpty(model.sortby))
                    {
                        if (model.sortby == "newtoold")
                            queryString += " order by transaction_date desc";
                        if (model.sortby == "oldtonew")
                            queryString += " order by transaction_date";
                    }
                    else
                        queryString += " order by transaction_date desc";

                    returnModel.result = DB.QuerySql<PaymentsModel>(queryString, new { keyword = "%" + model.keyword + "%" }).ToList();
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Payment List Export";
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

        public Response<int> ProviderPaymentSettle(RequestModel model)
        {
            Response<int> returnModel = new Response<int>();
            List<TransactionModel> AllTransactions = new List<TransactionModel>();

            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DateTime LastSettledDate = DB.QuerySql<DateTime>(@"select top 1 created from payment_settled_history order by created desc").FirstOrDefault();

                    string QueryString = @"select provider.id, concat(provider.first_name,' ',provider.last_name) provider_name, 
                                            count(booking_earning.id) booking_count, sum(booking_earning.amount) pending_amount from booking_earning
                                            left join booking on booking_earning.booking_id = booking.id 
                                            left join provider on booking.provider_id = provider.id
                                            where booking_earning.is_deleted = 0 ";

                    if (LastSettledDate != DateTime.MinValue)
                    {
                        QueryString += " and booking_earning.created > @LastSettledDate ";
                    }

                    QueryString += " group by provider.first_name,provider.last_name,provider.id ";

                    AllTransactions = DB.QuerySql<TransactionModel>(QueryString, new { LastSettledDate }).ToList();

                    if (AllTransactions.Count > 0)
                    {
                        foreach (var item in AllTransactions)
                        {
                            DB.ExecuteSql(@"insert into payment_settled_history(id, provider_id, booking_count, booking_ids, settled_amount, created)
                                        values(@id, @provider_id, @booking_count, @booking_ids, @settled_amount, @created)", new
                            {
                                id = Guid.NewGuid().ToString(),
                                provider_id = item.id,
                                booking_count = item.booking_count,
                                settled_amount = item.pending_amount,
                                booking_ids = "",
                                created = DateTime.UtcNow
                            });
                        }
                        returnModel.status = (int)EnumClass.ResponseState.Success;
                        returnModel.msg = "All pending payments are settled successfully";
                        returnModel.success = true;
                    }
                    else
                    {
                        returnModel.status = (int)EnumClass.ResponseState.Success;
                        returnModel.msg = "There are no pending payments are available for settlement ";
                        returnModel.success = true;
                    }
                }
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
    }
}