using System;
using System.Linq;
using Insight.Database;
using System.Data.SqlClient;
using System.Collections.Generic;
using Kharban_AdminAPI.Helper;

namespace Kharban_AdminAPI.Repository
{
    public class BookingRepository
    {
        public ResponseList<List<BookingModel>> GetBookings(RequestModel model)
        {
            string ReturnLink = string.Empty;

            string queryString = string.Empty;
            string queryCount = string.Empty;
            int TotalRecords = 0;
            string orderbyString = string.Empty;
            int recoardFrom = ((model.page - 1) * 10) + 1;
            int recoardTo = model.page * 10;

            ResponseList<List<BookingModel>> returnModel = new ResponseList<List<BookingModel>>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    if (!string.IsNullOrEmpty(model.sortby))
                    {
                        if (model.sortby == "newtoold")
                            orderbyString = " order by booking.created desc";
                        if (model.sortby == "oldtonew")
                            orderbyString = " order by booking.created";
                    }
                    else
                        orderbyString = " order by booking.created desc";



                    queryCount = @"SELECT count(booking.id) totalrecord FROM booking
                                    left join customer on customer.id = customer_id
                                    left join provider on provider.id = provider_id
                                    left join service_category on service_category.id = service_id
                                    where booking.is_deleted = 0 ";

                    queryString = @"select * from (SELECT ROW_NUMBER() OVER (" + orderbyString + @") row_num, booking.id, LTRIM(RTRIM(booking.booking_code)) as booking_code, coupon_id, booking_amount, address_title, 
                                    landmark, booking.longitude, booking.latitude, booking.description, booking.booking_status, provider_cancellation_fee, 
                                    booking.status, booking.created, booking.payment_type,
                                    service_category.name service_name, LTRIM(RTRIM((CONCAT(provider.first_name,' ',provider.last_name)))) provider_name,
                                    CONCAT(customer.first_name,' ',customer.last_name) customer_name,
                                    provider_rating, provider_rating_description, distance
                                    from booking
                                    left join customer on customer.id = customer_id
                                    left join provider on provider.id = provider_id
                                    left join service_category on service_category.id = service_id
                                    where booking.is_deleted = 0 ";


                    if (model != null)
                    {
                        if (model.filterby == "booking_code")
                        {
                            queryString += " and booking.booking_code like @keyword ";
                            queryCount += " and booking.booking_code like @keyword ";
                        }
                        else if (model.filterby == "provider_name")
                        {
                            queryString += " and ((provider.first_name+' '+provider.last_name like @keyword) or provider.first_name like @keyword or provider.last_name like @keyword) ";
                            queryCount += " and ((provider.first_name+' '+provider.last_name like @keyword) or provider.first_name like @keyword or provider.last_name like @keyword) ";
                        }
                        else if (model.filterby == "customer_name")
                        {
                            queryString += " and ((customer.first_name+' '+customer.last_name like @keyword) or customer.first_name like @keyword or customer.last_name like @keyword) ";
                            queryCount += " and ((customer.first_name+' '+customer.last_name like @keyword) or customer.first_name like @keyword or customer.last_name like @keyword) ";
                        }
                        else if (model.filterby == "service_name")
                        {
                            queryString += " and service_category.name like @keyword ";
                            queryCount += " and service_category.name like @keyword ";
                        }

                        if (model.filterby2 == "provider_id")
                        {
                            queryString += " and booking.provider_id = @keyword2 ";
                            queryCount += " and booking.provider_id = @keyword2 ";
                        }
                    }

                    queryString += " ) t where row_num between " + recoardFrom + " and " + recoardTo;

                    returnModel.result = DB.QuerySql<BookingModel>(queryString, new { keyword = "%" + model.keyword + "%", keyword2 = model.keyword2 }).ToList();
                    TotalRecords = DB.QuerySql<int>(queryCount, new { keyword = "%" + model.keyword + "%", keyword2 = model.keyword2 }).FirstOrDefault();

                    if (returnModel.result != null)
                    {
                        foreach (var item in returnModel.result)
                        {
                            item.booking_receipts = DB.QuerySql<BookingReceiptModel>("select receipt_status, receipt_amount, customer_receipt_description, provider_receipt_description from booking_receipt where is_deleted = 0 and booking_id = @BookingId ", new { BookingId = item.id }).ToList();
                        }
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
                returnModel.msg = "Booking List";
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


    }
}