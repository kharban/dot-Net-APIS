using Microsoft.AspNetCore.Mvc;
using Kharban_AdminAPI.Repository;
using System.Collections.Generic;

namespace Kharban_AdminAPI.Controllers
{
    [Authorize]
    [Route("api/v1")]
    public class CustomerController : BaseController
    {
        [HttpPost, Route(APIURL.GET_CUSTOMERS)]
        public IActionResult GetCustomerList([FromBody]RequestModel model)
        {
            if (model == null)
                model = new RequestModel();
            ResponseList<List<CustomerModel>> returnModel = new CustomerRepository().GetCustomers(model);

            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.DELETE_CUSTOMER)]
        public IActionResult DeleteCustomer([FromBody]CustomerRequestModel model)
        {
            if (model == null)
                model = new CustomerRequestModel();
            Response<CustomerModel> returnModel = new CustomerRepository().DeleteCustomers(model);

            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.EDIT_CUSTOMER)]
        public IActionResult UpdateCustomer([FromBody]CustomerRequestModel model)
        {
            if (model == null)
                model = new CustomerRequestModel();
            Response<CustomerModel> returnModel = new CustomerRepository().UpdateCustomer(model);

            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.UPDATE_STATUS_CUSTOMER)]
        public IActionResult UpdateStatusCustomer([FromBody]CustomerRequestModel model)
        {
            if (model == null)
                model = new CustomerRequestModel();
            Response<CustomerModel> returnModel = new CustomerRepository().UpdateCustomerStatus(model);

            return Ok(returnModel);
        }

    }
}