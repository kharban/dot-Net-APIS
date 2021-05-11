using System;
using System.Linq;
using Insight.Database;
using System.Data.SqlClient;
using System.Collections.Generic;
using Kharban_AdminAPI.Helper;

namespace Kharban_AdminAPI.Repository
{
    public class DashboardRepository
    {
        public Response<DashBoardChartModel> GetTotalEarnChart(DashBoardRequestModel model)
        {
            string queryString = string.Empty;
            Response<DashBoardChartModel> returnModel = new Response<DashBoardChartModel>();
            returnModel.result = new DashBoardChartModel();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    queryString = @"select id,amount,transaction_date
                                    from booking_platform_fee
                                    where is_deleted = 0 ";

                    if (model != null && !string.IsNullOrEmpty(model.filterby))
                    {
                        if (model.filterby == "today")
                        {
                            queryString += " and cast(transaction_date as Date) = cast(getdate() as Date) ";
                        }
                        if (model.filterby == "all")
                        {
                            queryString += " and transaction_date > DATEADD(year,-1,GETDATE()) ";
                        }
                    }

                    List<PaymentsModel> resultList = DB.QuerySql<PaymentsModel>(queryString).ToList();
                    returnModel.result.total = resultList.Sum(x => x.amount);

                    int hour = DateTime.Now.Hour + 1;
                    DateTime month = DateTime.Now;

                    List<string> Label = new List<string>();
                    List<decimal> recordlist = new List<decimal>();

                    if (model.filterby == "today")
                    {
                        for (int i = 1; i <= hour; i++)
                        {
                            if (i <= 12)
                                Label.Add(i + " AM");
                            else
                                Label.Add((i - 12) + " PM");
                            recordlist.Add(resultList.Where(x => x.transaction_date.Hour < i && x.transaction_date.Hour >= i - 1).Sum(x => x.amount));
                        }
                    }
                    else if (model.filterby == "all")
                    {
                        for (DateTime i = DateTime.Now.AddMonths(-11); i <= month; i = i.AddMonths(1))
                        {
                            MonthNameList myValueAsEnum = (MonthNameList)i.Month;
                            Label.Add(myValueAsEnum.ToString());
                            recordlist.Add(resultList.Where(x => x.transaction_date.Month == i.Month).Sum(x => x.amount));
                        }
                    }
                    returnModel.result.label = Label;
                    returnModel.result.data = recordlist;
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Total Earns";
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

        public Response<DashBoardChartModel> GetCustomerChart(DashBoardRequestModel model)
        {
            string queryString = string.Empty;
            Response<DashBoardChartModel> returnModel = new Response<DashBoardChartModel>();
            returnModel.result = new DashBoardChartModel();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    queryString = @"select id,created
                                    from customer
                                    where is_deleted = 0 ";

                    if (model != null && !string.IsNullOrEmpty(model.filterby))
                    {
                        if (model.filterby == "today")
                        {
                            queryString += " and cast(created as Date) = cast(getdate() as Date) ";
                        }
                        if (model.filterby == "all")
                        {
                            queryString += " and created > DATEADD(year,-1,GETDATE()) ";
                        }
                    }

                    List<CustomerModel> resultList = DB.QuerySql<CustomerModel>(queryString).ToList();
                    returnModel.result.total = resultList.Count;

                    int hour = DateTime.Now.Hour + 1;
                    DateTime month = DateTime.Now;
                    List<string> Label = new List<string>();
                    List<decimal> recordlist = new List<decimal>();
                    if (model.filterby == "today")
                    {
                        for (int i = 1; i <= hour; i++)
                        {
                            if (i <= 12)
                                Label.Add(i + " AM");
                            else
                                Label.Add((i - 12) + " PM");

                            recordlist.Add(resultList.Where(x => x.created.Hour < i && x.created.Hour >= i - 1).Count());
                        }
                    }
                    else if (model.filterby == "all")
                    {
                        for (DateTime i = DateTime.Now.AddMonths(-11); i <= month; i = i.AddMonths(1))
                        {
                            MonthNameList myValueAsEnum = (MonthNameList)i.Month;
                            Label.Add(myValueAsEnum.ToString());
                            recordlist.Add(resultList.Where(x => x.created.Month == i.Month).Count());
                        }
                    }

                    returnModel.result.label = Label;
                    returnModel.result.data = recordlist;
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Total Customers";
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

        public Response<DashBoardChartModel> GetProviderChart(DashBoardRequestModel model)
        {
            string queryString = string.Empty;
            Response<DashBoardChartModel> returnModel = new Response<DashBoardChartModel>();
            returnModel.result = new DashBoardChartModel();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    queryString = @"select id,created
                                    from provider
                                    where is_deleted = 0 ";

                    if (model != null && !string.IsNullOrEmpty(model.filterby))
                    {
                        if (model.filterby == "today")
                        {
                            queryString += " and cast(created as Date) = cast(getdate() as Date) ";
                        }
                        if (model.filterby == "all")
                        {
                            queryString += " and created > DATEADD(year,-1,GETDATE()) ";
                        }
                        if (!string.IsNullOrEmpty(model.service_id))
                        {
                            queryString += " and service_category_id = @service_id ";
                        }
                    }

                    List<ProviderModel> resultList = DB.QuerySql<ProviderModel>(queryString, model).ToList();
                    returnModel.result.total = resultList.Count;


                    int hour = DateTime.Now.Hour + 1;
                    DateTime month = DateTime.Now;
                    List<string> Label = new List<string>();
                    List<decimal> recordlist = new List<decimal>();
                    if (model.filterby == "today")
                    {
                        for (int i = 1; i <= hour; i++)
                        {
                            if (i <= 12)
                                Label.Add(i + " AM");
                            else
                                Label.Add((i - 12) + " PM");
                            recordlist.Add(resultList.Where(x => x.created.Hour < i && x.created.Hour >= i - 1).Count());
                        }
                    }
                    else if (model.filterby == "all")
                    {
                        for (DateTime i = DateTime.Now.AddMonths(-11); i <= month; i = i.AddMonths(1))
                        {
                            MonthNameList myValueAsEnum = (MonthNameList)i.Month;
                            Label.Add(myValueAsEnum.ToString());
                            recordlist.Add(resultList.Where(x => x.created.Month == i.Month).Count());
                        }
                    }

                    returnModel.result.label = Label;
                    returnModel.result.data = recordlist;
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Total Providers";
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

        public Response<DashBoardChartModel> GetGrowthChart(DashBoardRequestModel model)
        {
            string queryString = string.Empty;
            Response<DashBoardChartModel> returnModel = new Response<DashBoardChartModel>();
            returnModel.result = new DashBoardChartModel();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {

                    queryString = @"select id,created
                                    from booking
                                    where is_deleted = 0 and status = 1 ";


                    if (model != null && !string.IsNullOrEmpty(model.filterby))
                    {
                        if (model.filterby == "today")
                        {
                            queryString += " and cast(created as Date) = cast(getdate() as Date) ";
                        }
                        else if (model.filterby == "week")
                        {
                            queryString += " and DateDiff(wk,created,getdate()) = 0 ";
                        }
                        else if (model.filterby == "month")
                        {
                            queryString += " and DateDiff(mm,created,getdate()) = 0 ";
                        }
                        else if (model.filterby == "year")
                        {
                            queryString += " and DateDiff(yy,created,getdate()) = 0 ";
                        }
                    }

                    List<BookingModel> resultList = DB.QuerySql<BookingModel>(queryString).ToList();
                    returnModel.result.total = resultList.Count;


                    int hour = DateTime.Now.Hour + 1;
                    DateTime month = DateTime.Now;
                    List<string> Label = new List<string>();
                    List<decimal> recordlist = new List<decimal>();
                    if (model.filterby == "today")
                    {
                        for (int i = 1; i <= hour; i++)
                        {
                            if (i <= 12)
                                Label.Add(i + " AM");
                            else
                                Label.Add((i - 12) + " PM");
                            recordlist.Add(resultList.Where(x => x.created.Hour < i && x.created.Hour >= i - 1).Count());
                        }
                    }
                    else if (model.filterby == "week")
                    {
                        for (int i = 0; i <= 6; i++)
                        {
                            DayOfWeek myValueAsEnum = (DayOfWeek)i;
                            Label.Add(myValueAsEnum.ToString());
                            recordlist.Add(resultList.Where(x => x.created.DayOfWeek == myValueAsEnum).Count());
                        }
                    }
                    else if (model.filterby == "month")
                    {
                        for (int i = 1; i <= 31; i++)
                        {
                            Label.Add(i.ToString());
                            recordlist.Add(resultList.Where(x => x.created.Day == i).Count());
                        }
                    }
                    else if (model.filterby == "year")
                    {
                        for (int i = 1; i <= 12; i++)
                        {
                            MonthNameList myValueAsEnum = (MonthNameList)i;
                            Label.Add(myValueAsEnum.ToString());
                            recordlist.Add(resultList.Where(x => x.created.Month == i).Count());
                        }
                    }

                    returnModel.result.label = Label;
                    returnModel.result.data = recordlist;
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Total Earns";
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

        public Response<DashBoardChartModel> GetPieChart(DashBoardRequestModel model)
        {
            string queryString = string.Empty;
            Response<DashBoardChartModel> returnModel = new Response<DashBoardChartModel>();
            returnModel.result = new DashBoardChartModel();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {

                    queryString = @"select service_category.name service_name, count(booking.id) total_booking from service_category 
                                    left join booking on service_id = service_category.id
                                    where service_category.is_deleted = 0
                                    group by booking.id,service_category.name";

                    List<BookingModel> resultList = DB.QuerySql<BookingModel>(queryString).ToList();
                    returnModel.result.total = resultList.Sum(x => x.total_booking);


                    int hour = DateTime.Now.Hour + 1;
                    DateTime month = DateTime.Now;
                    List<string> Label = new List<string>();
                    List<decimal> recordlist = new List<decimal>();

                    foreach (var item in resultList)
                    {
                        Label.Add(item.service_name);
                        recordlist.Add(item.total_booking);
                    }

                    returnModel.result.label = Label;
                    returnModel.result.data = recordlist;
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Service Bookings";
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

        public Response<DashBoardChartModel> GetAvgRateChart(DashBoardRequestModel model)
        {
            string queryString = string.Empty;
            Response<DashBoardChartModel> returnModel = new Response<DashBoardChartModel>();
            returnModel.result = new DashBoardChartModel();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {

                    queryString = @"select ISNULL(AVG(provider_rating), 0) avrag from booking where service_id = @service_id and is_deleted = 0 ";

                    returnModel.result.total = DB.QuerySql<int>(queryString, model).FirstOrDefault();
                }

                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "Total average";
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


    }
}