using System;
using System.Security.Claims;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Kharban_WebAPI;
using Kharban_WebAPI.Models;
using Kharban_WebAPI.Repository;
using Microsoft.Extensions.Configuration;

namespace Kharban_WebAPI.Controllers
{
    [Route("api/v1")]
    public class CategoryController : ControllerBase
    {
        private IConfiguration Configuration;
        public CategoryController(IConfiguration _configuration)
        {
            this.Configuration = _configuration;
        }

        //[Authorize]
        [HttpPost, Route(APIURL.GET_CATEGORY_LIST)]
        public ActionResult GetCategoryList(RequestModel model)
        {
            if (model == null)
                model = new RequestModel();
            Response<List<CategoryModel>> returnModel = new CategoryRepository().GetCategoryList(model);
            return Ok(returnModel);
        }
    }
}