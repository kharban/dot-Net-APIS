using Microsoft.AspNetCore.Mvc;
using Kharban_AdminAPI.Repository;
using System.Collections.Generic;

namespace Kharban_AdminAPI.Controllers
{
    [Authorize]
    [Route("api/v1")]
    public class BookingController : BaseController
    {
        [HttpPost, Route(APIURL.GET_BOOKINGS)]
        public IActionResult GetBookings([FromBody]RequestModel model)
        {
            if (model == null)
                model = new RequestModel();
            ResponseList<List<BookingModel>> returnModel = new BookingRepository().GetBookings(model);
            return Ok(returnModel);
        }
    }
}