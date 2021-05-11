using Microsoft.AspNetCore.Mvc;
using Kharban_AdminAPI.Repository;
using System.Collections.Generic;

namespace Kharban_AdminAPI.Controllers
{
    [Authorize]
    [Route("api/v1")]
    public class FaqController : BaseController
    {
        [HttpPost, Route(APIURL.GET_FAQ_LIST)]
        public IActionResult GetFaqList([FromBody]FaqModel model)
        {
            if (model == null)
                model = new FaqModel();
            Response<List<FaqModel>> returnModel = new FaqRepository().GetFaqList(model);
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_FAQ)]
        public IActionResult GetFaq([FromBody]FaqModel model)
        {
            if (model == null)
                model = new FaqModel();
            Response<FaqModel> returnModel = new FaqRepository().GetFaq(model);
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.DELETE_FAQ)]
        public IActionResult DeleteFaq([FromBody]FaqModel model)
        {
            if (model == null)
                model = new FaqModel();
            Response<FaqModel> returnModel = new FaqRepository().DeleteFaq(model);

            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.ADD_FAQ)]
        public IActionResult AddFaq([FromBody]FaqModel model)
        {
            if (model == null)
                model = new FaqModel();
            Response<FaqModel> returnModel = new FaqRepository().AddFaq(model);
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.EDIT_FAQ)]
        public IActionResult EditFaq([FromBody]FaqModel model)
        {
            if (model == null)
                model = new FaqModel();
            Response<FaqModel> returnModel = new FaqRepository().UpdateFaq(model);

            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.UPDATE_STATUS_FAQ)]
        public IActionResult UpdateStatusFaq([FromBody]FaqModel model)
        {
            if (model == null)
                model = new FaqModel();
            Response<FaqModel> returnModel = new FaqRepository().UpdateStatusFaq(model);

            return Ok(returnModel);
        }

    }
}