using Microsoft.AspNetCore.Mvc;
using Kharban_AdminAPI.Repository;
using System.Collections.Generic;

namespace Kharban_AdminAPI.Controllers
{
    [Authorize]
    [Route("api/v1")]
    public class PaymentController : BaseController
    {
        [HttpPost, Route(APIURL.GET_PAYMENTS)]
        public IActionResult GetPayments([FromBody]RequestModel model)
        {
            if (model == null)
                model = new RequestModel();


            ResponseList<PaymentModel> returnModel = new PaymentRepository().GetPayments(model);
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_PAYMENT_EXPORT)]
        public IActionResult GetPaymentExport([FromBody]RequestModel model)
        {
            if (model == null)
                model = new RequestModel();
            Response<List<PaymentsModel>> returnModel = new PaymentRepository().GetPaymentExport(model);
            return Ok(returnModel);
        }
        
        [HttpPost, Route(APIURL.PROVIDER_PAYMENT_SETTLE)]
        public IActionResult ProviderPaymentSettle([FromBody]RequestModel model)
        {
            Response<int> returnModel = new PaymentRepository().ProviderPaymentSettle(model);
            return Ok(returnModel);
        }
    }
}