using System;
using System.Linq;
using Insight.Database;
using System.Data.SqlClient;
using System.Collections.Generic;
using Kharban_WebAPI.Models;
using Kharban_WebAPI.Common;
using System.IO;
using Kharban_WebAPI.Helper;

namespace Kharban_WebAPI.Repository
{
    public class BookingRepository
    {
        
        public ResponseList<List<BookingModel>> GetBookings(RequestModel model,string UserId)
        {
            string ReturnLink = string.Empty;

            string queryString = string.Empty;
            string queryCount = string.Empty;
            int TotalRecords = 0;
            ResponseList<List<BookingModel>> returnModel = new ResponseList<List<BookingModel>>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    queryCount = @"SELECT count(booking.id) totalrecord FROM booking
                                    left join customer on customer.id = customer_id
                                    left join provider on provider.id = provider_id
                                    left join service_category on service_category.id = service_id
                                    where booking.is_deleted = 0 ";

                    queryString = @"select booking.id, booking.booking_code, coupon_id, booking_amount, address_title, 
                                    landmark, booking.longitude, booking.latitude, booking.description, booking.booking_status, provider_cancellation_fee, 
                                    booking.status, booking.created, booking.payment_type,
                                    service_category.name service_name, CONCAT(provider.first_name,' ',provider.last_name) provider_name,
                                    CONCAT(customer.first_name,' ',customer.last_name) customer_name,
                                    provider_rating, provider_rating_description, distance
                                    from booking
                                    left join customer on customer.id = customer_id
                                    left join provider on provider.id = provider_id
                                    left join service_category on service_category.id = service_id
                                    where booking.is_deleted = 0 ";
                    queryString += " and (provider_id = @UserId or customer_id = @UserId)";

                    if (model != null && !string.IsNullOrEmpty(model.filterby) && !string.IsNullOrEmpty(model.keyword))
                    {
                        if (model.filterby == "booking_code")
                        {
                            queryString += " and booking.booking_code like @keyword ";
                            queryCount += " and booking.booking_code like @keyword ";
                        }
                        else if (model.filterby == "provider_name")
                        {
                            queryString += " and provider.first_name like @keyword or provider.last_name like @keyword ";
                            queryCount += " and provider.first_name like @keyword or provider.last_name like @keyword ";
                        }
                        else if (model.filterby == "customer_name")
                        {
                            queryString += " and customer.first_name like @keyword or customer.last_name like @keyword ";
                            queryCount += " and customer.first_name like @keyword or customer.last_name like @keyword ";
                        }
                        else if (model.filterby == "service_name")
                        {
                            queryString += " and service_category.name like @keyword ";
                            queryCount += " and service_category.name like @keyword ";
                        }

                        if (model.filterby2 == "provider_id")
                        {
                            queryString += " and provider.id = @keyword2 ";
                            queryCount += " and provider.id = @keyword2 ";
                        }
                    }



                    if (model != null && !string.IsNullOrEmpty(model.sortby))
                    {
                        if (model.sortby == "newtoold")
                            queryString += " order by booking.created desc";
                        if (model.sortby == "oldtonew")
                            queryString += " order by booking.created";
                    }
                    else
                        queryString += " order by booking.created desc";
                    
                    returnModel.result = DB.QuerySql<BookingModel>(queryString, new { keyword = "%" + model.keyword + "%", keyword2 = model.keyword2, UserId = UserId  }).ToList();
                    TotalRecords = DB.QuerySql<int>(queryCount, new { keyword = "%" + model.keyword + "%", keyword2 = model.keyword2 }).FirstOrDefault();

                    if (returnModel.result != null)
                    {
                        foreach (var item in returnModel.result)
                        {
                            item.booking_receipts = DB.QuerySql<BookingReceiptModel>("select receipt_status, receipt_amount, provider_receipt_description, customer_receipt_description from booking_receipt where is_deleted = 0 and booking_id = @BookingId ", new { BookingId = item.id }).ToList();
                        }
                    }



                    returnModel.totalDocs = TotalRecords;
                    returnModel.limit = 10;
                    returnModel.totalPages = TotalRecords / 10 + (TotalRecords % 10) > 0 ? 1 : 0;
                    returnModel.page = 1;
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
                returnModel.success = false;
            }
            return returnModel;
        }

        public async System.Threading.Tasks.Task<Response<string>>SaveBookingRequest(BookingModel model, string UserId)
        {

            string queryString = string.Empty;

            Response<string> returnModel = new Response<string>();

            List<UserLoginModel> Providers = new List<UserLoginModel>();
            try
            {          
                if (model != null)
                {
                    using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                    {
                        
                        decimal radiusS =DB.QuerySql<decimal>("select distance from setting ").FirstOrDefault();
                        queryString = @"SELECT id provider_id FROM provider where is_deleted = 0 and service_category_id = @service_category_id and is_online = 1 and latitude is not null and longitude is not null and admin_approve = 1 and ((6371 * acos(cos(radians(latitude)) * cos(radians(@latitude))  * cos(radians(@longitude) - radians(longitude))  + sin(radians(latitude))   * sin(radians(@latitude))   )) < @radius)";

                       
                        Providers = DB.QuerySql<UserLoginModel>(queryString, new { service_category_id = model.service_category_id, latitude = model.latitude, longitude = model.longitude, radius = radiusS }).ToList();


                        // create booking
                        string BookingCode = GetBookingCode();
                        var BookingParameter = new
                        {
                            id = Guid.NewGuid().ToString(),
                            booking_code = BookingCode,
                            customer_id = UserId,         
                            service_id = model.service_category_id,
                            booking_status = 0,
                            status = 1,
                            is_deleted = 0,
                            latitude = model.latitude,
                            longitude = model.longitude,
                            created = DateTime.Now,
                            modified = DateTime.Now,
                            AcceptDate = DateTime.Now,
                        };

                        DB.ExecuteSql(@"insert into booking(id, booking_code, customer_id, service_id, booking_status, status, is_deleted, latitude, longitude, created, modified,AcceptDate)
                               values(@id, @booking_code, @customer_id, @service_id, @booking_status, @status, @is_deleted, @latitude, @longitude, @created, @modified,@AcceptDate) ", BookingParameter);

                        //update booking_code in configuration table
                        DB.ExecuteSql("update genrel_configuration set booking_code = @booking_code where id is not null ", new { booking_code = BookingCode });


                        //save payment for booking temporary
                        decimal PlatFormFee = DB.QuerySql<decimal>("Select platform_fee from setting").FirstOrDefault();

                        DB.ExecuteSql(@"insert into booking_platform_fee(id, booking_id, amount, status, is_deleted, created, modified, transaction_date, transaction_id, transaction_status) 
                                    values(@id, @booking_id, @amount, @status, @is_deleted, @created, @modified, @transaction_date, @transaction_id, @transaction_status) ", new
                        {
                            id = Guid.NewGuid().ToString(),
                            booking_id = BookingParameter.id,
                            status = 1,
                            is_deleted = 0,
                            created = DateTime.Now,
                            modified = DateTime.Now,
                            amount = PlatFormFee,
                            transaction_date = DateTime.Now,
                            transaction_id = model.transaction_id,
                            transaction_status = 0
                        });

                        //return booking id as result
                        returnModel.result = BookingParameter.id;

                        //submit booking for provider

                        foreach (var item in Providers)
                        {
                            DB.ExecuteSql(@"insert into booking_provider(id, provider_id, booking_id, provider_status, status, is_deleted, created, modified) 
                                    values(@id, @provider_id, @booking_id, @provider_status, @status, @is_deleted, @created, @modified) ", new
                            {
                                id = Guid.NewGuid().ToString(),
                                provider_id = item.provider_id,
                                booking_id = BookingParameter.id,
                                provider_status = 0,
                                status = 1,
                                is_deleted = 0,
                                created = DateTime.Now,
                                modified = DateTime.Now
                            });
                            string DeviceId = DB.QuerySql<string>("select device_id from device_data where user_id = @UserId and user_active = 1", new { UserId = item.provider_id }).FirstOrDefault();

                            string providername = DB.QuerySql<string>("select first_name + ISNULL(last_name, ' ') from provider where id = @id", new { id = item.provider_id }).FirstOrDefault();

                            string Customername = DB.QuerySql<string>("select customer.first_name + ' ' + customer.last_name from booking inner join customer on  booking.customer_id = customer.id where booking.booking_code ='" + BookingCode + "'").FirstOrDefault();

                            if (DeviceId != null){
                                //send notification to provider
                                await SendSMS.PushNotificationAsync(DeviceId, "Kharban", "Dear '" + providername + "', You have a new request by '" + Customername + "'.", APPLICATION_PAGE_NAME.HOME, 1);

                                await new SettingRepository().SaveNotification(new NotificationModel()
                                {
                                    receiver_id = item.provider_id,
                                    sender_id = UserId,
                                    notification = "Booking Request to Provider",
                                });
                            }
                        }
                    }
                    returnModel.status = (int)EnumClass.ResponseState.Success;
                    returnModel.msg = "Request submited successfully";
                    returnModel.success = true;
                }
                else
                {
                    returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                    returnModel.msg = "Please submit proper data";
                    returnModel.success = false;
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
        public Response<int> CustomerCancelJob(BookingModel model, string UserId)
        {
            //string queryString = string.Empty;
            Response<int> returnModel = new Response<int>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql("update booking set booking_status = 2, modified = @modified where id = @BookingId and customer_id = @UserId", new
                    {
                        BookingId = model.booking_id,
                        UserId = UserId,
                        modified = DateTime.Now
                    });
                    string ProviderId = DB.QuerySql<string>("select provider_id from booking where id = @Id", new { Id = model.booking_id }).FirstOrDefault();

                    string DeviceId = DB.QuerySql<string>("select device_id from device_data where user_id = @UserId and user_active = 1", new { UserId = ProviderId }).FirstOrDefault();

                    //send notification to provider
                    SendSMS.PushNotificationAsync(DeviceId, "Kharban", "This booking has been cancelled by the customer.", APPLICATION_PAGE_NAME.HOME, 1);
                    new SettingRepository().SaveNotification(new NotificationModel()
                    {
                        receiver_id = ProviderId,
                        sender_id = UserId,
                        notification = "This booking has been cancelled by the customer.",
                    });
                }
                returnModel.msg = "Booking cancel successfully";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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

        public Response<List<UserLoginModel>> GetConnectedProviders(BookingModel model, string UserId)
        {

            string queryString = string.Empty;
            Response<List<UserLoginModel>> returnModel = new Response<List<UserLoginModel>>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<UserLoginModel>(@"select concat(provider.first_name,' ',provider.last_name) provider_name from booking_provider
                                        left join provider on booking_provider.provider_id = provider.id
                                        where provider.is_online = 1
                                        and booking_id = @BookingId ", new
                    {
                        BookingId = model.booking_id,
                    }).ToList();
                }
                returnModel.msg = "Connected Providers";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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
        public Response<UserLoginModel> GetAcceptedProvider(BookingModel model, string UserId)
        {

            string queryString = string.Empty;
            Response<UserLoginModel> returnModel = new Response<UserLoginModel>();

            try
            {
                //provider_status

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<UserLoginModel>(@"select concat(provider.first_name,' ',provider.last_name) provider_name from booking_provider
                                        left join provider on booking_provider.provider_id = provider.id
                                        where provider.is_online = 1 and provider_status = 1
                                        and booking_id = @BookingId ", new
                    {
                        BookingId = model.booking_id,
                    }).FirstOrDefault();
                }
                returnModel.msg = "Accepted Provider";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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
        public Response<BookingModel> GetPendingRatingBooking(string UserId)
        {

            string queryString = string.Empty;
            Response<BookingModel> returnModel = new Response<BookingModel>();

            try
            {
                //provider_status

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<BookingModel>(@"select top 1 b.id,p.profile_picture,b.provider_rating, b.provider_rating_description from booking b join provider p on b.provider_id = p.id where b.customer_id = @UserId and b.provider_rating is null and b.booking_status = 4  order by b.created desc ", new
                    {
                        UserId = UserId,
                    }).FirstOrDefault();
                }
                returnModel.msg = "pending rating";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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
        public Response<int> SubmitRating(BookingModel model, string UserId)
        {

            string queryString = string.Empty;
            Response<int> returnModel = new Response<int>();

            try
            {
                //provider_status

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql("update booking set provider_rating = @provider_rating, provider_rating_description = @provider_rating_description where id = @id ", new
                    {
                        id = model.booking_id,
                        model.provider_rating,
                        model.provider_rating_description
                    });
                }
                returnModel.msg = "Rating submitted successfully";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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
        public Response<int> ReceiptAcceptDecline(BookingModel model, string UserId)
        {
            string queryString = string.Empty;
            Response<int> returnModel = new Response<int>();
            try
            {
                //provider_status
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    string ProviderId = DB.QuerySql<string>("select provider_id from booking where id = @Id", new { Id = model.booking_id }).FirstOrDefault();

                    string DeviceId = DB.QuerySql<string>("select device_id from device_data where user_id = @UserId and user_active = 1", new { UserId = ProviderId }).FirstOrDefault();


                    DB.ExecuteSql("update booking_receipt set receipt_status = @booking_status where id = @id ", new
                    {
                        id = model.booking_receipt_id,
                        booking_status = model.status,//1=accept,2=reject
                    });

                    if (model.status == 1)
                    {
                        DB.ExecuteSql("update booking set booking_amount = @booking_amount, booking_status = @booking_status where id = @id ", new
                        {
                            id = model.booking_id,
                            booking_status = 4,
                            model.booking_amount,
                        });

                        DB.ExecuteSql(@"insert into booking_earning(id, booking_id, amount, payment_type, transaction_id, transaction_data, transaction_status, status, is_deleted, created, modified)
                                values(@id, @booking_id, @amount, @payment_type, @transaction_id, @transaction_data, @transaction_status, @status, @is_deleted, @created, @modified)", new
                        {
                            id = Guid.NewGuid().ToString(),
                            booking_id = model.booking_id,
                            amount = model.booking_amount,
                            payment_type = model.payment_type,
                            transaction_id = model.transaction_id,
                            transaction_data = "",
                            transaction_status = 1,
                            status = 1,
                            is_deleted = 0,
                            created = DateTime.Now,
                            modified = DateTime.Now
                        });

                        // Create CustomerName
                        string customername = DB.QuerySql<string>("select customer.first_name + ' ' + customer.last_name from booking inner join customer on booking.customer_id = customer.id where booking.id = @id", new { id = model.booking_id }).FirstOrDefault();

                        //send notification to provider
                        SendSMS.PushNotificationAsync(DeviceId, "Kharban", "your receipt has been accepted by '" + customername + "', for '" + model.booking_id + "'.", APPLICATION_PAGE_NAME.RECEIPT_ACCEPT, 1);

                        new SettingRepository().SaveNotification(new NotificationModel()
                        {
                            receiver_id = ProviderId,
                            sender_id = UserId,
                            notification = "Your receipt Accepted by customer",
                        });

                    }
                    else if (model.status == 2)
                    {
                        int receiptRejectCount = DB.QuerySql<int>("select receipt_reject_count from booking_receipt where booking_id = @id", new
                        {
                            id = model.booking_id
                        }).FirstOrDefault();
                        if (receiptRejectCount <= 2)
                        {
                            DB.ExecuteSql("update booking_receipt set receipt_reject_count = @count where booking_id = @id", new
                            {
                                count = receiptRejectCount + 1,
                                id = model.booking_id
                            });

                            DB.ExecuteSql("update booking_receipt set customer_receipt_description = @description where booking_id = @id ", new
                            {
                                id = model.booking_id,
                                model.description
                            });

                            DB.ExecuteSql("update booking set booking_status = @booking_status where id = @id ", new
                            {
                                id = model.booking_id,
                                booking_status = 9,  // 9 is for reciept declined by customer
                            });

                            // Create CustomerName
                            string customername = DB.QuerySql<string>("select customer.first_name + ' ' + customer.last_name from booking inner join customer on booking.customer_id = customer.id where booking.id = @id", new { id = model.booking_id }).FirstOrDefault();

                            //send notification to provider
                            SendSMS.PushNotificationAsync(DeviceId, "Kharban", "Receipt has rejected by '" + customername + "' for '" + model.booking_id + "'.", APPLICATION_PAGE_NAME.RECEIPT_REJECT, 1, model.booking_id, receiptRejectCount + 1);
                            new SettingRepository().SaveNotification(new NotificationModel()
                            {
                                receiver_id = ProviderId,
                                sender_id = UserId,
                                notification = "Your receipt Decline by customer",
                            });
                        }
                    }

                }
                returnModel.msg = "Status Updated successfully";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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
        public Response<List<BookingModel>> GetJobRequest(string UserId)
        {
            string queryString = string.Empty;
            Response<List<BookingModel>> returnModel = new Response<List<BookingModel>>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<BookingModel>(@"select booking_provider.id, booking_provider.booking_id, 
                                  service_category.name service_name,booking_provider.created,booking.booking_status, 
                                  service_category.image service_image, booking.latitude, booking.longitude
                                  from booking_provider
                                  left join booking on booking_id = booking.id
                                  left join service_category on service_id = service_category.id
                                  where booking_provider.is_deleted = 0 and booking_provider.status = 1 and booking_provider.provider_id = @ProviderId and booking.booking_status = 0 and booking_provider.provider_status = 0 ", new
                    {
                        ProviderId = UserId,
                    }).ToList();

                }
                returnModel.msg = "Job Request";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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
        public Response<BookingModel> GetJobDetail(BookingModel model, string UserId)
        {

            string queryString = string.Empty;
            Response<BookingModel> returnModel = new Response<BookingModel>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<BookingModel>(@"select booking_provider.booking_id, service_category.name service_name 
                                  from booking_provider
                                  left join booking on booking_id = booking.id
                                  left join service_category on service_id = service_category.id
                                  where booking_provider.is_deleted = 0 and booking_provider.status = 1 and booking_provider.id = @Id ", new
                    {
                        Id = model.id,
                    }).FirstOrDefault();
                }
                returnModel.msg = "Job Request";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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
        public Response<UserLoginModel> JobAcceptDecline(BookingProviderModel model, string UserId)
        {
            string queryString = string.Empty;
            Response<UserLoginModel> returnModel = new Response<UserLoginModel>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql("update booking_provider set provider_status = @provider_status where id = @id ", new
                    {
                        id = model.id,
                        model.provider_status
                    });



                    if (model.provider_status == "1")
                    {

                        DB.ExecuteSql("update booking set provider_id = @UserId ,AcceptDate=@AcceptDate where id = @booking_id ", new
                        {
                            booking_id = model.booking_id,
                            UserId = UserId,
                            AcceptDate = DateTime.Now
                        });

                        returnModel.result = DB.QuerySql<UserLoginModel>(@"select customer.id customer_id, customer.first_name, customer.last_name, customer.contact_no, customer.profile_picture, booking.booking_code, booking.id booking_id,customer.country_code from booking_provider 
                                              left join booking on booking.id = booking_provider.booking_id
                                              left join customer on customer.id = booking.customer_id
                                              where booking_provider.provider_id = @ProviderId and booking_id = @BookingId and provider_status  = 1 ", new {
                            ProviderId = UserId, BookingId = model.booking_id 
                        }).FirstOrDefault();

                        DB.ExecuteSql("update booking set provider_id = @UserId, booking_status = @booking_status where id = @id ", new
                        {
                            id = model.booking_id,
                            booking_status = 3,
                            UserId = UserId
                        });

                        //set previous pending booking to cancel
                        DB.ExecuteSql("update booking set booking_status = 1 where provider_id = @UserId and booking_status = 0 ", new
                        {
                           UserId = UserId
                        });

                        DB.ExecuteSql("update booking_provider set provider_status = 4 where provider_id = @UserId and id != @currentId and provider_status = 0 ", new
                        {
                           UserId = UserId,
                            currentId = model.id
                        });


                        string DeviceId = DB.QuerySql<string>("select device_id from device_data where user_id = @UserId and user_active = 1", new { UserId = returnModel.result.customer_id }).FirstOrDefault();
                        //string Notification = "Dear " + returnModel.result.first_name + ", Your Job Accepted";
                        //string Notification = "عزيزي العميل ، تم قبول طلبك";
                        string Notification = " عزيزي '" + returnModel.result.customer_name + "', تم قبول طلبك من قبل '" + returnModel.result.provider_name + "'.";

                        SendSMS.PushNotificationAsync(DeviceId, "Kharban", Notification, APPLICATION_PAGE_NAME.ORDER_TRACKING, 1);

                        List<string> ProviderId = new List<string>();
                        ProviderId = DB.QuerySql<string>(@"select provider_id from booking_provider where booking_id=@Id and provider_status=0", new
                        {
                            Id = model.booking_id,
                            provider_Id = model.id
                        }).ToList();

                        foreach (var item in ProviderId)
                        {
                            string DeviceIds = DB.QuerySql<string>("select device_id from device_data where user_id = @UserId and user_active = 1", new { UserId = item }).FirstOrDefault();
                            var Notifications = "A booking was just accepted by another provider, so it is no more available for you.";
                            //send notification to provider
                            SendSMS.PushNotificationAsync(DeviceIds, "Kharban", Notifications, APPLICATION_PAGE_NAME.HOME, 1);
                        }



                    }
                    else if (model.provider_status == "2")
                    {
                        string Notification = string.Empty;
                        DateTime bookingAcceptDate = DB.QuerySql<DateTime>("Select AcceptDate from booking where  id = @booking_id ", new
                        {
                            booking_id = model.booking_id,
                            UserId = UserId
                        }).FirstOrDefault();

                        DateTime currentDateTime = DateTime.Now;

                        string max_cancellation_Time = DB.QuerySql<string>("Select max_cancellation_time From setting").FirstOrDefault();

                        decimal provider_cancellation_fee = DB.QuerySql<decimal>("Select provider_cancellation_fee From setting").FirstOrDefault();

                        TimeSpan diffResult = currentDateTime.Subtract(bookingAcceptDate);
                        if (diffResult.TotalMinutes > Convert.ToInt32(max_cancellation_Time))
                        {
                            /////////added booking_status= 10 to mark the booking fully cancelled 
                            DB.ExecuteSql("update booking set provider_id = @UserId , booking_status = 10 , provider_cancellation_fee = @providerCancellationFee where id = @booking_id ", new
                            {
                                booking_id = model.booking_id,
                                providerCancellationFee = provider_cancellation_fee,
                                UserId = UserId
                            });
                            DB.ExecuteSql("update booking_provider set provider_status = 4 where booking_id = @booking_id and id = @currentId ", new
                            {
                                booking_id = model.booking_id,
                                currentId = model.id

                            });

                            DB.ExecuteSql("update booking_provider set is_deleted = 1 where booking_id = @booking_id and id <> @currentId ", new
                            {
                                booking_id = model.booking_id,
                                currentId = model.id

                            });
                            Notification = "لأسف ، اضطر المزود المعين إلى إلغاء طلبك ، وسنقوم بمعالجة استرداد رسوم الخدمة لهذا الطلب. يمكنك إنشاء طلب جديد..";

                            returnModel.result = DB.QuerySql<UserLoginModel>(@"select customer.id customer_id, customer.first_name, customer.last_name, customer.contact_no, customer.profile_picture, booking.booking_code, booking.id booking_id,booking_provider.provider_id from booking_provider 
                                              left join booking on booking.id = booking_provider.booking_id
                                              left join customer on customer.id = booking.customer_id
                                              where booking_provider.booking_id = @BookingId  ", new { BookingId = model.booking_id }).FirstOrDefault();

                            string Provider_DeviceId = DB.QuerySql<string>("select device_id from device_data where user_id = @UserId and user_active = 1", new { UserId = model.id }).FirstOrDefault();

                            var BookingCode = DB.QuerySql<string>("Select booking_code from booking where  id = @booking_id ", new
                            {
                                booking_id = model.booking_id
                            }).FirstOrDefault();

                            string provider_Notification = "You have cancelled a request outside the allowed timeframe therefore we have charged you a penalty of " + provider_cancellation_fee + " SAR for the booking " + BookingCode + "";

                            SendSMS.PushNotificationAsync(Provider_DeviceId, "Kharban", provider_Notification, APPLICATION_PAGE_NAME.HOME, 1);
                        }
                        else
                        {
                            DB.ExecuteSql("update booking set provider_id = '' , booking_status = 0  where id = @booking_id ", new
                            {
                                booking_id = model.booking_id

                            });

                            DB.ExecuteSql("update booking_provider set provider_status = 0 where booking_id = @booking_id and id <> @currentId ", new
                            {
                                booking_id = model.booking_id,
                                currentId = model.id

                            });

                            DB.ExecuteSql("update provider set is_online = 1 , is_deleted = 0 where user_id = @provider_id ", new
                            {
                                provider_id = model.provider_id

                            });
                            Notification = "للأسف ، اضطر المزود المعين إلى إلغاء طلبك ، يرجى البقاء معنا وسنقوم بتعيين مزود جديد قريبًا";

                            ////notification available for all provider when one proider cancelled
                            List<string> ProviderId = new List<string>();
                            ProviderId = DB.QuerySql<string>(@"select provider_id from booking_provider where booking_id=@Id and provider_status <> 2", new
                            {
                                Id = model.booking_id,
                            }).ToList();

                            foreach (var item in ProviderId)
                            {
                                string DeviceIds = DB.QuerySql<string>("select device_id from device_data where user_id = @UserId and user_active = 1", new { UserId = item }).FirstOrDefault();
                                var Notifications = "A booking was just cancelled by a provider, it is now available for you to accept.";
                                //send notification to provider
                                SendSMS.PushNotificationAsync(DeviceIds, "Kharban", Notifications, APPLICATION_PAGE_NAME.HOME, 1);
                            }
                        }



                        returnModel.result = DB.QuerySql<UserLoginModel>(@"select customer.id customer_id, customer.first_name, customer.last_name, customer.contact_no, customer.profile_picture, booking.booking_code, booking.id booking_id from booking_provider 
                                              left join booking on booking.id = booking_provider.booking_id
                                              left join customer on customer.id = booking.customer_id
                                              where booking_provider.booking_id = @BookingId  ", new { BookingId = model.booking_id }).FirstOrDefault();

                        string DeviceId = DB.QuerySql<string>("select device_id from device_data where user_id = @UserId and user_active = 1", new { UserId = returnModel.result.customer_id }).FirstOrDefault();
                        //string Notification = "Dear " + returnModel.result.first_name + ", Your Job Canceled";
                        //string Notification = "عزيزي العميل، للأسف تم الغاء طلبك من مزود الخدمه";

                        SendSMS.PushNotificationAsync(DeviceId, "Kharban", Notification, APPLICATION_PAGE_NAME.HOME, 1);

                    }

                    else if (model.provider_status == "3")
                    {
                        DB.ExecuteSql("update booking set provider_id = '' , booking_status = 0  where id = @booking_id ", new
                        {
                            booking_id = model.booking_id

                        });
                        DB.ExecuteSql("update booking_provider set provider_status = 2 where booking_id = @booking_id and id = @currentId ", new
                        {
                            booking_id = model.booking_id,
                            currentId = model.id

                        });

                        DB.ExecuteSql("update provider set is_online = 1 , is_deleted = 0 where user_id = @provider_id ", new
                        {
                            provider_id = model.provider_id

                        });
                        var Notification = "للأسف ، اضطر المزود المعين إلى إلغاء طلبك ، يرجى البقاء معنا وسنقوم بتعيين مزود جديد قريبًا";

                        ////notification available for all provider when one proider cancelled
                        List<string> ProviderId = new List<string>();
                        ProviderId = DB.QuerySql<string>(@"select provider_id from booking_provider where booking_id=@Id and  provider_status <> 2", new
                        {
                            Id = model.booking_id,
                        }).ToList();

                        foreach (var item in ProviderId)
                        {
                            string DeviceIds = DB.QuerySql<string>("select device_id from device_data where user_id = @UserId and user_active = 1", new { UserId = item }).FirstOrDefault();
                            var Notifications = "A booking was just cancelled by a provider, it is now available for you to accept.";
                            //send notification to provider
                            SendSMS.PushNotificationAsync(DeviceIds, "Kharban", Notifications, APPLICATION_PAGE_NAME.HOME, 1);
                        }
                        returnModel.result = DB.QuerySql<UserLoginModel>(@"select customer.id customer_id, customer.first_name, customer.last_name, customer.contact_no, customer.profile_picture, booking.booking_code, booking.id booking_id from booking_provider 
                                              left join booking on booking.id = booking_provider.booking_id
                                              left join customer on customer.id = booking.customer_id
                                              where booking_provider.booking_id = @BookingId  ", new { BookingId = model.booking_id }).FirstOrDefault();

                        string DeviceId = DB.QuerySql<string>("select device_id from device_data where user_id = @UserId and user_active = 1", new { UserId = returnModel.result.customer_id }).FirstOrDefault();
                        //string Notification = "Dear " + returnModel.result.first_name + ", Your Job Canceled";
                        //string Notification = "عزيزي العميل، للأسف تم الغاء طلبك من مزود الخدمه";

                        //SendSMS.PushNotificationAsync(DeviceId, "Kharban", Notification, APPLICATION_PAGE_NAME.HOME, 1);

                        dynamic data = DB.QuerySql<dynamic>(@"select provider_id from booking_provider where booking_id=@Id and provider_status=2", new
                        {
                            Id = model.booking_id,
                        }).ToList();

                        var count = DB.QuerySql<dynamic>(@"select provider_id from booking_provider where booking_id=@Id", new
                        {
                            Id = model.booking_id,
                        }).Count();

                        if (count == data.Count)
                        {
                            if (data != null)
                            {
                                foreach (var item in data)
                                {
                                    DB.ExecuteSql("update booking_provider set provider_status = 4 where id = @Id ", new { Id = item.provider_id });

                                    DB.ExecuteSql("update booking set booking_status = 10 where id = @Id ", new { Id = model.booking_id });

                                }
                                string DeviceId1 = DB.QuerySql<string>("select device_id from device_data where user_id = @UserId and user_active = 1", new { UserId = returnModel.result.customer_id }).FirstOrDefault();
                                //var Notification1 = "Unfortunately, currently there are no proviers to accept your request, we will process a refund for the service fee. Kindly try again after some time.";

                                var Notification1 = "لسوء الحظ ، لا يوجد حاليًا محاضرون لقبول طلبك ، وسنقوم بمعالجة استرداد رسوم الخدمة. يرجى المحاولة مرة أخرى بعد بعض الوقت.";
                                SendSMS.PushNotificationAsync(DeviceId1, "Kharban", Notification1, APPLICATION_PAGE_NAME.HOME, 2);
                            }
                        }
                    }

                }
                returnModel.msg = "Status Updated successfully";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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
        
        public Response<int> GenerateReceipt(BookingReceiptModel model, string UserId)
        {
            string queryString = string.Empty;

            Response<int> returnModel = new Response<int>();

            try
            {
                string imageName = DateTime.Now.ToString();
                string imgPath = string.Empty;

                if (!string.IsNullOrEmpty(model.trip_snapshot))
                {

                    string path = "";//httpContextAccessor.HttpContext.se.Server.MapPath("~/images/trip_snapshot");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path); //Create directory if it doesn't exist
                    }
                    imageName = imageName + model.file_extension;
                    //set the image path
                    imgPath = Path.Combine(path, imageName);
                    byte[] imageBytes = Convert.FromBase64String(model.trip_snapshot);
                    File.WriteAllBytes(imgPath, imageBytes);

                }

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    // change status of job as receipt generate
                    DB.ExecuteSql("update booking set booking_status = @booking_status where id = @Id", new
                    {
                        Id = model.booking_id,
                        booking_status = 5
                    });
                    int count = DB.QuerySql<int>("Select receipt_reject_count from booking_receipt where booking_id = @id", new
                    {
                        @id = model.booking_id
                    }).FirstOrDefault();

                    if (count == 0)
                    {

                        DB.ExecuteSql(@"insert into booking_receipt(id, booking_id, receipt_status, receipt_amount, booking_status, status, is_deleted, created, provider_receipt_description,trip_snapshot) 
                                    values(@id, @booking_id, @receipt_status, @receipt_amount, @booking_status, @status, @is_deleted, @created, @provider_receipt_description, @trip_snapshot) ", new
                        {
                            id = Guid.NewGuid().ToString(),
                            model.booking_id,
                            model.receipt_status,
                            receipt_amount = model.receipt_amount,
                            model.booking_status,
                            status = 1,
                            is_deleted = 0,
                            created = DateTime.Now,
                            model.provider_receipt_description,
                            trip_snapshot = imgPath
                        });
                    }
                    else
                    {
                        DB.ExecuteSql("Update booking_receipt set receipt_amount = @receipt_amount, provider_receipt_description = @provider_receipt_description, created = @created", new
                        {
                            receipt_amount = model.receipt_amount,
                            provider_receipt_description = model.provider_receipt_description,
                            created = DateTime.Now
                        });
                    }
                    // send notification to customer
                    string CustomerId = DB.QuerySql<string>("select customer_id from booking where id = @Id", new { Id = model.booking_id }).FirstOrDefault();

                    string DeviceId = DB.QuerySql<string>("select device_id from device_data where user_id = @UserId and user_active = 1", new { UserId = CustomerId }).FirstOrDefault();

                    string providername = DB.QuerySql<string>("select provider.first_name + ' ' + provider.last_name from booking inner join provider on  booking.provider_id = provider.id where booking.id = @Id", new { Id = model.booking_id }).FirstOrDefault();

                    //string Notification = "Your receipt generated";
                    //string Notification = "تم إنشاء فاتورتك";
                    string Notification = "تم إنشاء إيصالك لـ '" + model.booking_id + "', بواسطة '" + providername + "'";

                    SendSMS.PushNotificationAsync(DeviceId, "Kharban", Notification, APPLICATION_PAGE_NAME.INVOICE_ISSUE, 1);

                    //save notification for provider
                    new SettingRepository().SaveNotification(new NotificationModel()
                    {
                        receiver_id = CustomerId,
                        sender_id = UserId,
                        notification = Notification,
                    });
                }
                returnModel.msg = "Receipt generated successfully";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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
        public Response<BookingModel> GetActiveProviderBooking(string UserId)
        {

            string queryString = string.Empty;
            Response<BookingModel> returnModel = new Response<BookingModel>();

            try
            {
                //provider_status

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<BookingModel>(@"  select booking.id,booking.id booking_id, booking.booking_code, coupon_id, booking_amount, address_title, 
                                    landmark, booking.longitude, booking.latitude, booking.description, booking.booking_status, provider_cancellation_fee, 
                                    booking.status, booking.created, booking.payment_type,
                                    service_category.name service_name, service_category.image service_image, CONCAT(provider.first_name,' ',provider.last_name) provider_name,
                                    customer.first_name, customer.last_name, customer.contact_no , customer.country_code,customer.profile_picture,  
                                    provider_rating, provider.id provider_id, provider_rating_description, distance, booking_receipt.receipt_reject_count
                                    from booking
                                    left join customer on customer.id = customer_id
                                    left join provider on provider.id = provider_id
                                    left join service_category on service_category.id = service_id
        	left join booking_provider on booking_provider.booking_id = booking.id
                                    left join booking_receipt on booking_receipt.booking_id = booking.id
                                    where booking.is_deleted = 0 and booking_provider.provider_status = 1 and provider.id = @UserId
                                    and (booking.booking_status = 3 or booking.booking_status = 5 or booking.booking_status = 6 or booking.booking_status = 9) ", new
                    {
                        UserId = UserId,
                    }).FirstOrDefault();
                }
                returnModel.msg = "Active booking";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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
        public Response<BookingModel> GetActiveCustomerBooking(string UserId)
        {
            string queryString = string.Empty;
            Response<BookingModel> returnModel = new Response<BookingModel>();

            try
            {
                //provider_status

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<BookingModel>(@"select booking.id, booking.booking_code, coupon_id, booking_amount, address_title, 
                                                landmark, booking.longitude booking_longitude, booking.latitude booking_latitude, booking.description, booking.booking_status, provider_cancellation_fee, 
                                                booking.status, booking.created, booking.payment_type, provider.latitude, provider.longitude,
                                                service_category.name service_name, CONCAT(provider.first_name,' ',provider.last_name) provider_name, provider.id provider_id,
        		        concat(provider.country_code,'',provider.contact_no) ProviderContactNo, provider.profile_picture ProviderProfilePicture,
                                                CONCAT(customer.first_name,' ',customer.last_name) customer_name,booking_receipt.id booking_receipt_id,
                                                provider_rating, provider_rating_description, distance, booking_receipt.receipt_amount, booking_receipt.provider_receipt_description,booking_receipt.trip_snapshot,
                                                booking_receipt.receipt_status
                                                from booking
                                                left join customer on customer.id = customer_id
                                                left join provider on provider.id = provider_id
        				left join booking_receipt on booking.id = booking_receipt.booking_id
                                                left join service_category on service_category.id = service_id
                                                where booking.is_deleted = 0  
                                                and customer_id= @UserId
                                                and (booking.booking_status = 3 or booking.booking_status = 5 or booking.booking_status = 6 or booking.booking_status = 9 )", new
                    {
                        UserId = UserId,
                    }).FirstOrDefault();
                }
                returnModel.msg = "Active booking";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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
        public Response<List<BookingModel>> GetPastProviderBooking(string UserId)
        {

            string queryString = string.Empty;
            Response<List<BookingModel>> returnModel = new Response<List<BookingModel>>();

            try
            {
                //provider_status

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {

                    String strQuery = @"select booking.id, booking.booking_code, coupon_id, booking_amount, address_title, 
                                    landmark, booking.longitude, booking.latitude, CAST(booking.description AS NVARCHAR(1000)), booking.booking_status, provider_cancellation_fee, 
                                    booking.status, booking.created, booking.payment_type,
                                    service_category.name service_name, CONVERT(nvarchar(max), service_category.image) service_image, CONCAT(p.first_name, ' ', p.last_name) provider_name,
                                    CONCAT(customer.first_name, ' ', customer.last_name) customer_name, 
                                    provider_rating, provider_rating_description, distance,customer.profile_picture,customer.contact_no, booking_receipt.trip_snapshot,
                                    booking_receipt.customer_receipt_description
                                    from booking
                                    left join customer on customer.id = customer_id
                                    left join provider p on p.id = provider_id
                                    left join service_category on service_category.id = service_id

                                    left join booking_provider on booking_provider.booking_id = booking.id
                                    left join booking_receipt on booking_receipt.booking_id = booking.id
                                    where booking.is_deleted = 0 and p.id = @UserId
                                    Group By booking.id, booking.booking_code, coupon_id, booking_amount, address_title,
                                    landmark, booking.longitude, booking.latitude, CAST(booking.description AS NVARCHAR(1000)), booking.booking_status, provider_cancellation_fee, customer_receipt_description,
                                    booking.status, booking.created, booking.payment_type,
                                    service_category.name, CONVERT(nvarchar(max), service_category.image) , CONCAT(p.first_name, ' ', p.last_name),CONCAT(customer.first_name, ' ', customer.last_name),
                                    provider_rating, provider_rating_description, distance,customer.profile_picture,customer.contact_no,booking_receipt.trip_snapshot";

                    returnModel.result = DB.QuerySql<BookingModel>(strQuery, new
                    {
                        UserId = UserId,

                    }).OrderByDescending(x => x.created).ToList();
                }
                returnModel.msg = "Past bookings";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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
        public Response<List<BookingModel>> GetPastCustomerBooking(string UserId)
        {

            string queryString = string.Empty;
            Response<List<BookingModel>> returnModel = new Response<List<BookingModel>>();

            try
            {
                //provider_status

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<BookingModel>(@"select booking.id, booking.booking_code, coupon_id, booking_amount, address_title, 
                                        landmark, booking.longitude as booking_longitude, booking.latitude as booking_latitude, booking.description, booking.booking_status, provider_cancellation_fee, 
                                        booking.status, booking.created, booking.payment_type,
                                        service_category.name service_name, CONCAT(provider.first_name,' ',provider.last_name) provider_name,
        		provider.contact_no ProviderContactNo, provider.profile_picture ProviderProfilePicture,
                                        CONCAT(customer.first_name,' ',customer.last_name) customer_name,customer.latitude1 as latitude, customer.longitude1 as longitude, 
                                        provider_rating, provider_rating_description, distance,booking_receipt.trip_snapshot
                                        from booking
                                        left join customer on customer.id = customer_id
                                        left join provider on provider.id = provider_id
                                        left join service_category on service_category.id = service_id
                                        left join booking_receipt on booking_receipt.booking_id = booking.id
                                        where booking.is_deleted = 0 
                                        and booking.provider_id !=''
                                        and customer.id = @UserId 
                                        and (booking.booking_status != 3 and booking.booking_status != 5 and booking.booking_status != 6) order by booking.created desc", new
                    {
                        UserId = UserId,
                    }).OrderByDescending(x => x.created).ToList();
                }
                returnModel.msg = "Past Customer bookings";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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
        public Response<BookingModel> GetBookingDetail(BookingModel model, string UserId)
        {

            string queryString = string.Empty;
            Response<BookingModel> returnModel = new Response<BookingModel>();

            try
            {
                //provider_status

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<BookingModel>(@"  select booking.id, booking.booking_code, coupon_id, booking_amount, address_title, 
                                    landmark, booking.longitude, booking.latitude, booking.description, booking.booking_status, provider_cancellation_fee, 
                                    booking.status, booking.created, booking.payment_type,
                                    service_category.name service_name, CONCAT(provider.first_name,' ',provider.last_name) provider_name,
                                    CONCAT(customer.first_name,' ',customer.last_name) customer_name,
                                    provider_rating, provider_rating_description, distance
                                    from booking
                                    left join customer on customer.id = customer_id
                                    left join provider on provider.id = provider_id
                                    left join service_category on service_category.id = service_id
        	left join booking_provider on booking_provider.booking_id = booking.id
                                    where booking.is_deleted = 0 and booking_provider.provider_status = 1 and booking.id = @Id ", new
                    {
                        Id = model.booking_id,
                    }).FirstOrDefault();
                }
                returnModel.msg = "booking details";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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
        public Response<List<BookingModel>> GetEarning(string UserId)
        {

            string queryString = string.Empty;
            Response<List<BookingModel>> returnModel = new Response<List<BookingModel>>();

            try
            {
                //provider_status

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<BookingModel>(@"select booking_earning.amount booking_amount, booking_earning.created, service_category.name service_name
                                            from booking_earning
                                            left join booking on booking_earning.booking_id = booking.id 
        			left join service_category on booking.service_id = service_category.id 
                                            where booking_earning.is_deleted = 0 and booking_earning.status = 1 and booking.provider_id = @UserId ", new
                    {
                        UserId = UserId,
                    }).ToList();
                }
                returnModel.msg = "provider earning";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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
        public Response<BookingModel> GetTotalEarning(string UserId)
        {

            string queryString = string.Empty;
            Response<BookingModel> returnModel = new Response<BookingModel>();

            try
            {
                //provider_status

                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    returnModel.result = DB.QuerySql<BookingModel>(@"select sum(booking_earning.amount) booking_amount from booking_earning
                                            left join booking on booking_earning.booking_id = booking.id 
        			left join service_category on booking.service_id = service_category.id 
                                            where booking_earning.is_deleted = 0 and booking_earning.status = 1 and booking.provider_id = @UserId ", new
                    {
                        UserId = UserId,
                    }).FirstOrDefault();
                }
                returnModel.msg = "provider total earning";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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

        public Response<int> SendBookingReminderToProvider(BookingModel model, string UserId)
        {
            string queryString = string.Empty;
            Response<int> returnModel = new Response<int>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {

                    dynamic data = DB.QuerySql<dynamic>(@"select booking_provider.id, provider.id providerId, provider.first_name from booking_provider 
                                                        left join booking on booking.id = booking_provider.booking_id
                                                        left join provider on booking.provider_id = provider.id
                                                        where booking_provider.booking_id = @BookingId", new { BookingId = model.booking_id }).ToList();
                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            string DeviceId = DB.QuerySql<string>("select device_id from device_data where user_id = @UserId and user_active = 1", new { UserId = item.providerId }).FirstOrDefault();
                            SendSMS.PushNotificationAsync(DeviceId, "Kharban", "Dear " + item.first_name + ", This is the final reminder for you job", APPLICATION_PAGE_NAME.ORDER_TRACKING, 1);
                        }
                    }


                }
                returnModel.msg = "Job Request";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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

        public Response<List<BookingModel>> GetCompleteBookings(Searchfilter filter, string UserId)
        {
            string queryString = string.Empty;

            Response<List<BookingModel>> returnModel = new Response<List<BookingModel>>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {

                    queryString = @"select booking_code, booking_amount, booking.created, booking.modified,
                    service_category.image service_image, service_category.name service_name, booking_receipt.trip_snapshot, booking.provider_cancellation_fee from booking 
                    left join service_category on service_category.id = booking.service_id 
                    left join booking_receipt on booking_receipt.booking_id = booking.id
                    where provider_id = @ProviderId and (booking.booking_status = 4 or booking.booking_status=1)";
                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        queryString += " and Booking.Created >='" + filter.StartDate + "' and Booking.Created <= '" + filter.EndDate + "' order by Booking.created desc";
                    }

                    returnModel.result = DB.QuerySql<BookingModel>(queryString, new
                    {
                        ProviderId = UserId
                    }).OrderByDescending(x => x.created).ToList();
                }
                returnModel.msg = "Complete bookings";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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

        public Response<CusProvModel> GetCusProvInfo(BookingModel model, string UserId)
        {
            CusProvModel obj = new CusProvModel();
            string queryString = string.Empty;
            Response<CusProvModel> returnModel = new Response<CusProvModel>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {

                    obj.latitude = DB.QuerySql<string>("select latitude from booking where id = @id", new
                    {
                        id = model.booking_id
                    }).FirstOrDefault();
                    obj.longitude = DB.QuerySql<string>("select longitude from booking where id = @id", new
                    {
                        id = model.booking_id
                    }).FirstOrDefault();
                    string serviceCategoryId = DB.QuerySql<string>("Select service_id from booking where id = @id", new
                    {
                        id = model.booking_id
                    }).FirstOrDefault();
                    obj.image = DB.QuerySql<string>("select image from service_category inner join booking on service_category.id = @serviceId", new
                    {
                        serviceId = serviceCategoryId
                    }).FirstOrDefault();

                    queryString = @"select * from provider where service_category_id = @serviceCatId";

                    obj.Providers = DB.QuerySql<ProviderModel>(queryString, new
                    {
                        serviceCatId = serviceCategoryId
                    }).ToArray();

                    returnModel.result = obj;
                }
                returnModel.msg = "Customer Provider Information has been listed.";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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

        public Response<string> CompleteByProvider(BookingModel model, string UserId)
        {
            Response<string> returnModel = new Response<string>();

            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql("update booking set booking_status = @booking_status where id = @id ", new
                    {
                        id = model.booking_id,
                        booking_status = 4,
                    });
                }

                List<string> adminList = new List<string>(); string body = "";
                AdminMailModel bookingModel = new AdminMailModel();
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    adminList = DB.QuerySql<string>(@"select email from admin_user").ToList();
                }
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    bookingModel = DB.QuerySql<AdminMailModel>(@"select (isnull(customer.first_name,'')+' '+isnull(customer.last_name,'')) as CustomerName,                         (customer.contact_no) as CustomerContactNo,(isnull(provider.first_name,'')+' '+isnull(provider.last_name,''))  as ProviderName,                (provider.contact_no) as ProviderContactNo,(booking.booking_code) as BookingCode,(booking.booking_status) as BookingStatus from                 booking,customer,provider where customer_id=customer.id and booking.provider_id=provider.id and booking.id = @Id ", new
                    {
                        Id = model.booking_id,
                    }).FirstOrDefault();
                }
                foreach (var item in adminList)
                {
                    body += "<p><b>Customer Detail</b></p>";
                    body += "<p>Name: " + bookingModel.CustomerName + "</p>";
                    body += "<p>Contact No.: " + bookingModel.CustomerContactNo + "</p>";
                    body += "</br>";
                    body += "<p><b>Provider Detail</b></p>";
                    body += "<p>Name: " + bookingModel.ProviderName + "</p>";
                    body += "<p>Contact No.: " + bookingModel.ProviderContactNo + "</p>";
                    body += "</br>";
                    body += "<p><b>Booking Detail</b></p>";
                    body += "<p>Booking code: " + bookingModel.BookingCode + "</p>";
                    body += "<p>Booking Status: " + bookingModel.BookingStatus + "</p>";
                    SendMail(item, body);
                }


                returnModel.result = model.booking_id;
                returnModel.status = (int)EnumClass.ResponseState.Success;
                returnModel.msg = "The customer has declined the receipt. So, we have marked booking as completed from our end. The Admin will contact you for further queries.";
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

        public ResponseList<List<LocationModel>> GetRecentLocation(string UserId)
        {

            string queryString = string.Empty;
            ResponseList<List<LocationModel>> returnModel = new ResponseList<List<LocationModel>>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {

                    returnModel.result = DB.QuerySql<LocationModel>(@"select Top 5 customer_id, address, latitude, longitude,location.created from location inner join customer on customer.id = location.customer_id where customer.id = @id order by created desc", new
                    {
                        id = UserId
                    }).ToList();
                }
                returnModel.msg = "Recent Locations";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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

        private string GetBookingCode()
        {
            string AccountCode = "";

            using (SqlConnection Db = new SqlConnection(SiteKey.ConnectionString))
            {
                AccountCode = Db.QuerySql<string>("select booking_code from genrel_configuration where id is not null ").FirstOrDefault();
            }
            if (AccountCode != null)
            {
                string code = AccountCode.Substring(0, 4);
                string year = AccountCode.Substring(4, 4);
                string count = AccountCode.Substring(8, 5);
                if (DateTime.Now.Year.ToString() == year)
                {
                    count = (int.Parse(count) + 1).ToString("00000");
                }
                else
                {
                    year = DateTime.Now.Year.ToString();
                    count = "00001";
                }
                AccountCode = code + year + count;
            }
            else
            {
                AccountCode = "BOOK" + DateTime.Now.Year + "00001";
            }
            return AccountCode;
        }

        public Response<UserLoginModel> GetPlatformFee(string UserId)
        {
            UserLoginModel obj = new UserLoginModel();
            Response<UserLoginModel> returnModel = new Response<UserLoginModel>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {

                    obj.first_name = DB.QuerySql<string>("select customer.first_name from customer where id= @id", new
                    {
                        id = UserId
                    }).FirstOrDefault();

                    obj.last_name = DB.QuerySql<string>("select customer.last_name from customer where id= @id", new
                    {
                        id = UserId
                    }).FirstOrDefault();

                    obj.platformFee = Convert.ToInt32(DB.QuerySql<decimal>("select platform_fee from setting").FirstOrDefault());
                }

                returnModel.result = obj;
                returnModel.msg = "Platform Fee";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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

        public Response<LocationModel> SaveRecentLocation(LocationModel model, string UserId)
        {

            Response<LocationModel> returnModel = new Response<LocationModel>();
            try
            {
                string guid = Guid.NewGuid().ToString();
                var perameter = new
                {
                    id = guid,
                    customerid = UserId,
                    address = model.address,
                    longitude = model.longitude,
                    latitude = model.latitude,
                    is_deleted = 0,
                    created = DateTime.Now
                };
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    DB.ExecuteSql(@"insert into location(id,customer_id,address,longitude,latitude,is_deleted,created) 
                                    Values(@id,@customerid,@address,@longitude,@latitude,@is_deleted,@created)", perameter);
                }

                returnModel.result = model;
                returnModel.msg = "Recent Location Saved";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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
        public Response<int> AutoCancelBookings(string UserId)
        {
            string queryString = string.Empty;
            Response<int> returnModel = new Response<int>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    dynamic data = DB.QuerySql<dynamic>(@"select booking_provider.id, customer.id customerId, customer.first_name,booking.id as bookingId from                                     booking_provider 
                                                        left join booking on booking.id = booking_provider.booking_id
                                                        left join customer on booking.customer_id = customer.id
                                                        where provider_status = 0 and CURRENT_TIMESTAMP > DATEADD(minute,30,booking_provider.created) ").ToList();
                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            DB.ExecuteSql("update booking_provider set provider_status = 4 where id = @Id ", new { Id = item.id });

                            DB.ExecuteSql("update booking set booking_status = 10 where id = @Id ", new { Id = item.bookingId });

                            string DeviceId = DB.QuerySql<string>("select device_id from device_data where user_id = @UserId and user_active = 1", new { UserId = item.customerId }).FirstOrDefault();

                            var Notification = "Your Booking is canceled";

                            SendSMS.PushNotificationAsync(DeviceId, "Kharban", Notification, APPLICATION_PAGE_NAME.HOME, 1);

                        }
                    }

                }
                returnModel.msg = "Job Request";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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

        public Response<int> SaveProviderAllCompleteCanceled(BookingModel model, string UserId)
        {

            Response<int> returnModel = new Response<int>();
            try
            {
                using (SqlConnection DB = new SqlConnection(SiteKey.ConnectionString))
                {
                    dynamic data = DB.QuerySql<dynamic>(@"select provider_id from booking_provider where booking_id=@Id and provider_status=2", new
                    {
                        Id = model.booking_id,
                    }).ToList();

                    var count = DB.QuerySql<dynamic>(@"select provider_id from booking_provider where booking_id=@Id", new
                    {
                        Id = model.booking_id,
                    }).Count();

                    if (count == data.Count())
                    {
                        if (data != null)
                        {
                            foreach (var item in data)
                            {
                                DB.ExecuteSql("update booking_provider set provider_status = 4 where id = @Id ", new { Id = item.id });

                                DB.ExecuteSql("update booking set booking_status = 10 where id = @Id ", new { Id = item.bookingId });

                            }
                            string DeviceId = DB.QuerySql<string>("select device_id from device_data where user_id = @UserId and user_active = 1", new { UserId = model.customer_id }).FirstOrDefault();
                            var Notification = "Your Booking is canceled";
                            SendSMS.PushNotificationAsync(DeviceId, "Kharban", Notification, APPLICATION_PAGE_NAME.HOME, 1);
                        }
                    }
                }

                //returnModel.result = model;
                returnModel.msg = "Booking is canceled Saved";
                returnModel.status = (int)EnumClass.ResponseState.Success;
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






        /// <summary>
        /// Mail Send
        /// </summary>
        /// <param name="toMail"></param>
        /// <param name="mailBody"></param>
        public static void SendMail(string toMail, string mailBody)
        {
            string Website = SiteKey.SiteUrl;
            MailSend mail = new MailSend(Website);
            bool IsDone = mail.SendMail(toMail, "Subject Test Mail ", mailBody);
        }


    }
}
