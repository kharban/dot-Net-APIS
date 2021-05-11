using Microsoft.AspNetCore.Mvc;
using Kharban_AdminAPI.Repository;
using System.Collections.Generic;
using System;
using Kharban_AdminAPI.Helper;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Kharban_AdminAPI.Controllers
{
    [Authorize]
    [Route("api/v1")]
    public class CategoryController : BaseController
    {
        public IHostingEnvironment hostingEnvironment;
        public CategoryController(IHostingEnvironment _hostingEnvironment)
        {
            hostingEnvironment = _hostingEnvironment;
        }

        [HttpPost, Route(APIURL.GET_CATEGORIES)]
        public IActionResult GetCategories([FromBody]RequestModel model)
        {
            string nm = model.keyword;
            if (!string.IsNullOrEmpty(nm))
            {
                nm = nm.Substring(0, 1).ToUpper() + nm.Substring(1);
            }



            if (model == null)
                model = new RequestModel();
            ResponseList<List<CategoryModel>> returnModel = new CategoryRepository().GetCategories(model);
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_CATEGORY_LIST)]
        public IActionResult GetCategoryList()
        {
            Response<List<CategoryModel>> returnModel = new CategoryRepository().GetCategoryList();
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.GET_CATEGORY)]
        public IActionResult GetCategory([FromBody]CategoryModel model)
        {
            if (model == null)
                model = new CategoryModel();
            Response<CategoryModel> returnModel = new CategoryRepository().GetCategory(model);
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.DELETE_CATEGORY)]
        public IActionResult DeleteCategory([FromBody]CategoryModel model)
        {
            if (model == null)
                model = new CategoryModel();
            Response<CategoryModel> returnModel = new CategoryRepository().DeleteCategory(model);

            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.ADD_CATEGORY)]
        public IActionResult AddCategory([FromBody]CategoryModel model)
        {
            if (model == null)
                model = new CategoryModel();
            Response<CategoryModel> returnModel = new Response<CategoryModel>();
            string imageURL = string.Empty;
            string imgPath = string.Empty;
            if (!string.IsNullOrEmpty(model.image) && !string.IsNullOrEmpty(model.image_extension))
            {
                if (model.image_extension.ToLower() != "jpg" && model.image_extension.ToLower() != "jpeg" && model.image_extension.ToLower() != "png")
                {
                    returnModel.success = false;
                    returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                    returnModel.msg = Resource_Kharban.SelectImageValidation;
                    return Ok(returnModel);
                }
                var path = hostingEnvironment.WebRootPath + "images/service_category";//Path

                //Check if directory exist
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
                }

                string imageName = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + model.image_extension;

                //set the image path
                imgPath = Path.Combine(path, imageName);

                imageURL = "https://kharban.net:2096/images/service_category/" + imageName;
            }

            returnModel = new CategoryRepository().AddCategory(model, imgPath, imageURL);
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.EDIT_CATEGORY)]
        public IActionResult EditCategory([FromBody]CategoryModel model)
        {
            if (model == null)
                model = new CategoryModel();
            Response<CategoryModel> returnModel = new Response<CategoryModel>();
            string imageURL = string.Empty;
            string imgPath = string.Empty;
            if (!string.IsNullOrEmpty(model.image) && !string.IsNullOrEmpty(model.image_extension))
            {
                if (model.image_extension.ToLower() != "jpg" && model.image_extension.ToLower() != "jpeg" && model.image_extension.ToLower() != "png")
                {
                    returnModel.success = false;
                    returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                    returnModel.msg = Resource_Kharban.SelectImageValidation;
                    return Ok(returnModel);
                }

                var path = hostingEnvironment.WebRootPath + "/images/service_category"; //Path

                //Check if directory exist
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
                }

                string imageName = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + model.image_extension;

                //set the image path
                imgPath = Path.Combine(path, imageName);

                imageURL = "https://kharban.net:2096/images/service_category/" + imageName;
            }


            returnModel = new CategoryRepository().UpdateCategory(model, imgPath, imageURL);
            return Ok(returnModel);

        }

        [HttpPost, Route(APIURL.UPDATE_STATUS_CATEGORY)]
        public IActionResult UpdateStatusCategory([FromBody]CategoryModel model)
        {
            if (model == null)
                model = new CategoryModel();
            Response<CategoryModel> returnModel = new CategoryRepository().UpdateStatusCategory(model);

            return Ok(returnModel);
        }

    }
}