using Kharban_WebAPI.CommonClasses;
using Kharban_WebAPI.Helper;
using Kharban_WebAPI.Models;
using Kharban_WebAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kharban_WebAPI.Controllers
{
    [Authorize]
    [Route("api/v1")]
    [ApiController]
    public class BookingController : BaseController
    {
        private IConfiguration Configuration;
        public BookingController(IConfiguration _configuration)
        {
            this.Configuration = _configuration;
        }
        
        [HttpPost, Route(APIURL.GET_BOOKINGS)]
        public ActionResult GetBookings(RequestModel model)
        {
            //string connStrings = Configuration.GetConnectionString("DevelopmentConnectionString");
           
            if (model == null)
                model = new RequestModel();
            ResponseList<List<BookingModel>> returnModel =new BookingRepository().GetBookings(model,  GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_COMPLETE_BOOKINGS)]
        public ActionResult GetCompleteBookings(Searchfilter model)
        {
            Response<List<BookingModel>> returnModel = new BookingRepository().GetCompleteBookings(model, GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.SAVE_BOOKING_REQUEST)]
        public async System.Threading.Tasks.Task<ActionResult> SaveBookingRequest([FromBody]BookingModel model)
        {
            Response<string> returnModel = await new BookingRepository().SaveBookingRequest(model, GetUserId());

            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.CUSTOMER_CANCEL_JOB)]
        public ActionResult CustomerCancelJob(BookingModel model)
        {
            Response<int> returnModel = new BookingRepository().CustomerCancelJob(model,  GetUserId());

            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_CONNECTED_PROVIDERS)]
        public ActionResult GetConnectedProviders(BookingModel model)
        {
            Response<List<UserLoginModel>> returnModel = new BookingRepository().GetConnectedProviders(model,  GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_ACCEPTED_PROVIDER)]
        public ActionResult GetAcceptedProvider(BookingModel model)
        {
            Response<UserLoginModel> returnModel = new BookingRepository().GetAcceptedProvider(model,  GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_PENDING_RATING_BOOKING)]
        public ActionResult GetPendingRatingBooking()
        {
            Response<BookingModel> returnModel = new BookingRepository().GetPendingRatingBooking(GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.SUBMIT_RATING)]
        public ActionResult SubmitRating(BookingModel model)
        {
            Response<int> returnModel = new BookingRepository().SubmitRating(model,  GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.RECEIPT_ACCEPT_DECLINE)]
        public ActionResult ReceiptAcceptDecline(BookingModel model)
        {
            Response<int> returnModel = new BookingRepository().ReceiptAcceptDecline(model, GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_JOBREQUEST)]
        public ActionResult GetJobRequest()
        {
            Response<List<BookingModel>> returnModel = new BookingRepository().GetJobRequest(GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_JOBDETAIL)]
        public ActionResult GetJobDetail(BookingModel model)
        {
            Response<BookingModel> returnModel = new BookingRepository().GetJobDetail(model, GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.JOB_ACCEPT_DECLINE)]
        public ActionResult JobAcceptDecline(BookingProviderModel model)
        {
            Response<UserLoginModel> returnModel = new BookingRepository().JobAcceptDecline(model, GetUserId());
            return Ok(returnModel);
        }


        [HttpPost, Route(APIURL.GENERATE_RECEIPT)]
        public ActionResult GenerateReceipt(BookingReceiptModel model)
        {
            Response<int> returnModel = new BookingRepository().GenerateReceipt(model, GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_ACTIVE_PROVIDER_BOOKING)]
        public ActionResult GetActiveProviderBooking()
        {
            Response<BookingModel> returnModel = new BookingRepository().GetActiveProviderBooking(GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_ACTIVE_CUSTOMER_BOOKING)]
        public ActionResult GetActiveCustomerBooking()
        {
            Response<BookingModel> returnModel = new BookingRepository().GetActiveCustomerBooking(GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_PAST_PROVIDER_BOOKING)]
        public ActionResult GetPastProviderBooking()
        {
            Response<List<BookingModel>> returnModel = new BookingRepository().GetPastProviderBooking(GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_PAST_CUSTOMER_BOOKING)]
        public ActionResult GetPastCustomerBooking()
        {
            Response<List<BookingModel>> returnModel = new BookingRepository().GetPastCustomerBooking(GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_BOOKING_DETAIL)]
        public ActionResult GetBookingDetail(BookingModel model)
        {
            Response<BookingModel> returnModel = new BookingRepository().GetBookingDetail(model,  GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_EARNING)]
        public ActionResult GetEarning()
        {
            Response<List<BookingModel>> returnModel = new BookingRepository().GetEarning(GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_TOTAL_EARNING)]
        public ActionResult GetTotalEarning()
        {
            Response<BookingModel> returnModel = new BookingRepository().GetTotalEarning(GetUserId());
            return Ok(returnModel);
        }

        [AllowAnonymous]
        [HttpPost, Route(APIURL.AUTO_CANCEL_BOOKINGS)]
        public ActionResult AutoCancelBookings()
        {
            Response<int> returnModel = new BookingRepository().AutoCancelBookings(GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.SEND_BOOKING_REMINDER_TO_PROVIDER)]
        public ActionResult SendBookingReminderToProvider(BookingModel model)
        {
            Response<int> returnModel = new BookingRepository().SendBookingReminderToProvider(model, GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_CUS_PROV_INFO)]
        public ActionResult GetCusProvInfo(BookingModel model)
        {
            Response<CusProvModel> returnModel = new BookingRepository().GetCusProvInfo(model, GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.COMPLETE_BY_PROVIDER)]
        public ActionResult CompleteByProvider(BookingModel model)
        {
            Response<string> returnModel = new BookingRepository().CompleteByProvider(model, GetUserId());
            return Ok(returnModel);
        }
        [HttpPost, Route(APIURL.RECENT_LOCATION)]
        public ActionResult GetRecentLocation()
        {
            ResponseList<List<LocationModel>> returnModel = new BookingRepository().GetRecentLocation(GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_PLATFORM_FEE)]
        public ActionResult GetPlatformFee()
        {
            Response<UserLoginModel> returnModel = new BookingRepository().GetPlatformFee(GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_SAVERECENT_LOCATION)]
        public ActionResult SaveRecentLocation(LocationModel model)
        {
            Response<LocationModel> returnModel = new BookingRepository().SaveRecentLocation(model, GetUserId());
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.All_PROVIDER_CANCEL_BOOKING)]
        public ActionResult SaveProviderAllCompleteCanceled(BookingModel model)
        {
            Response<int> returnModel = new Response<int>();
            returnModel = new BookingRepository().SaveProviderAllCompleteCanceled(model, GetUserId());
            return Ok(returnModel);
        }
    }
}
