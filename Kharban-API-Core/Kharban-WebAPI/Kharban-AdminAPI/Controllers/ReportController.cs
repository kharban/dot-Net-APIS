using Microsoft.AspNetCore.Mvc;
using Kharban_AdminAPI.Repository;
using System.Collections.Generic;

namespace Kharban_AdminAPI.Controllers
{
    [Authorize]
    [Route("api/v1")]
    public class ReportController : BaseController
    {
        [HttpPost, Route(APIURL.GET_REPORT_DATA_COUNT)]
        public IActionResult GetReportData([FromBody]RequestModel model)
        {
            if (model == null)
                model = new RequestModel();
            Response<DashBoardModel> returnModel = new ReportRepository().GetReportData(model);
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_REPORT_CUSTOMER_LIST)]
        public IActionResult GetCustomerList([FromBody]RequestModel model)
        {
            if (model == null)
                model = new RequestModel();
            Response<List<CustomerModel>> returnModel = new ReportRepository().GetCustomerList(model);
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_REPORT_PROVIDER_LIST)]
        public IActionResult GetProviderList([FromBody]RequestModel model)
        {
            if (model == null)
                model = new RequestModel();
            Response<List<ProviderModel>> returnModel = new ReportRepository().GetProviderList(model);
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_REPORT_BOOKING_LIST)]
        public IActionResult GetBookingList([FromBody]RequestModel model)
        {
            if (model == null)
                model = new RequestModel();
            Response<List<BookingModel>> returnModel = new ReportRepository().GetBookingList(model);
            return Ok(returnModel);
        }
        
        [HttpPost, Route(APIURL.GET_REPORT_TRANSICTION_LIST)]
        public IActionResult GetTransictionList([FromBody]RequestModel model)
        {
            if (model == null)
                model = new RequestModel();
            Response<List<TransactionModel>> returnModel = new ReportRepository().GetTransictionList(model);
            return Ok(returnModel);
        }
    }
}