using Microsoft.AspNetCore.Mvc;
using Kharban_AdminAPI.Repository;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace Kharban_AdminAPI.Controllers
{
    [Authorize]
    [Route("api/v1")]
    public class SettingController : BaseController
    {
        public IHostingEnvironment hostingEnvironment;
        public SettingController(IHostingEnvironment _hostingEnvironment)
        {
            hostingEnvironment = _hostingEnvironment;
        }
        [HttpPost, Route(APIURL.GET_SETTING)]
        public IActionResult GetSetting([FromBody]RequestModel model)
        {
            if (model == null)
                model = new RequestModel();
            Response<SettingModel> returnModel = new SettingRepository().GetSetting(model);
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.UPDATE_SETTING)]
        public IActionResult UpdateSetting([FromBody]SettingModel model)
        {
            if (model == null)
                model = new SettingModel();
            Response<int> returnModel = new Response<int>();
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

                var path = hostingEnvironment.WebRootPath + "~/images/kharban_logo"; //Path

                //Check if directory exist
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
                }

                string imageName = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + model.image_extension;

                //set the image path
                imgPath = Path.Combine(path, imageName);
                imageURL = "https://kharban.net:2096/images/kharban_logo/" + imageName;
            }


            returnModel = new SettingRepository().UpdateSetting(model, imgPath, imageURL);
            return Ok(returnModel);
        }
    }
}