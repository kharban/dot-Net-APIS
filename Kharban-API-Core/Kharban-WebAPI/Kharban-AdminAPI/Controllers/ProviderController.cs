using Microsoft.AspNetCore.Mvc;
using Kharban_AdminAPI.Repository;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Kharban_AdminAPI.Controllers
{
    
    [Route("api/v1")]
    public class ProviderController : BaseController
    {

        public IHostingEnvironment hostingEnvironment;
        public ProviderController(IHostingEnvironment _hostingEnvironment)
        {
            hostingEnvironment = _hostingEnvironment;
        }
        [Authorize]
        [HttpPost, Route(APIURL.PROVIDER_LIST)]
        public IActionResult GetProviderList([FromBody]RequestModel model)
        {
            


            if (model == null)
                model = new RequestModel();
            ResponseList<List<ProviderModel>> returnModel = new ProviderRepository().GetProviders(model);
            return Ok(returnModel);
        }
        [Authorize]
        [HttpPost, Route(APIURL.GET_PROVIDER_REQUEST_LIST)]
        public IActionResult GetProviderRequestList([FromBody]RequestModel model)
        {
            if (model == null)
                model = new RequestModel();
            ResponseList<List<ProviderModel>> returnModel = new ProviderRepository().GetProviderRequestList(model);
            return Ok(returnModel);
        }
        [Authorize]
        [HttpPost, Route(APIURL.DELETE_PROVIDER)]
        public IActionResult DeleteProvider([FromBody]ProviderModel model)
        {
            if (model == null)
                model = new ProviderModel();
            Response<ProviderModel> returnModel = new ProviderRepository().DeleteProvider(model);

            return Ok(returnModel);
        }
        [Authorize]
        [HttpPost, Route(APIURL.VIEW_PROVIDER)]
        public IActionResult ViewProvider([FromBody]ProviderModel model)
        {
            if (model == null)
                model = new ProviderModel();
            Response<ProviderModel> returnModel = new ProviderRepository().GetProvider(model);
            return Ok(returnModel);
        }
        [Authorize]
        [HttpPost, Route(APIURL.EDIT_PROVIDER)]
        public IActionResult EditProvider([FromBody]ProviderModel model)
        {
            Response<ProviderModel> returnModel = new Response<ProviderModel>();
            string imageURL = string.Empty;
            string imgPath = string.Empty;
            byte[] imageBytes;

            try
            {
                if (model == null)
                    model = new ProviderModel();

                if (!string.IsNullOrEmpty(model.image) && !string.IsNullOrEmpty(model.image_extension))
                {
                    if (model.image_extension.ToLower() != "jpg" && model.image_extension.ToLower() != "jpeg" && model.image_extension.ToLower() != "png")
                    {
                        returnModel.success = false;
                        returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                        returnModel.msg = Resource_Kharban.SelectImageValidation;
                        return Ok(returnModel);
                    }

                    var path = hostingEnvironment.WebRootPath + "/images/provider_profile"; //Path

                    //Check if directory exist
                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
                    }

                    string imageName = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + model.image_extension;

                    //set the image path
                    imgPath = Path.Combine(path, imageName);
                    imageURL = "https://kharban.net:2096/images/provider_profile/" + imageName;
                    //imageURL = "http://localhost:59039/images/provider_profile/" + imageName;

                }
                returnModel = new ProviderRepository().UpdateProvider(model, imgPath, imageURL);


            }
            catch(Exception ex)
            {
                returnModel.msg = ex.Message;
                returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                LoggingRepository.SaveException(ex);
            }
            return Ok(returnModel);
        }
        [Authorize]
        [HttpPost, Route(APIURL.UPDATE_STATUS_PROVIDER)]
        public IActionResult UpdateStatusProvider([FromBody]ProviderModel model)
        {
            if (model == null)
                model = new ProviderModel();
            Response<ProviderModel> returnModel = new ProviderRepository().UpdateProviderStatus(model);

            return Ok(returnModel);
        }
        [Authorize]
        [HttpPost, Route(APIURL.APPROVE_RETAILER)]
        public IActionResult ApproveRetailer([FromBody]ProviderModel model)
        {
            if (model == null)
                model = new ProviderModel();
            Response<ProviderModel> returnModel = new ProviderRepository().ApproveRetailer(model);
            return Ok(returnModel);
        }

        

        

    }
}