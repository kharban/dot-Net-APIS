using Kharban_AdminAPI.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kharban_AdminAPI.Controllers
{

    [Route("api/v1")]
    public class ImagesUploadController : BaseController
    {
        public IHostingEnvironment hostingEnvironment;
        public ImagesUploadController(IHostingEnvironment _hostingEnvironment)
        {
            hostingEnvironment = _hostingEnvironment;
        }
        [HttpPost,Route("providerImagesUpload")]
        public ActionResult UploadImagesProvoder(ProviderModel model)
        {
            if (model == null)
                model = new ProviderModel();

            string imageURL = string.Empty;
            string imageName = string.Empty;
            string path = string.Empty;
            string imgPath = string.Empty;
            Response<string> returnModel = new Response<string>();
            if (!string.IsNullOrEmpty(model.image) && !string.IsNullOrEmpty(model.image_extension))
            {
                imageName = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + model.image_extension;

                if (model.image_extension.ToLower() != "jpg" && model.image_extension.ToLower() != "jpeg" && model.image_extension.ToLower() != "png")
                {
                    returnModel.success = false;
                    returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                    returnModel.msg = Resource_Kharban.SelectImageValidation;
                    return Ok(returnModel);
                }


                if (model.ImageType == "profile")
                {
                    path = hostingEnvironment.WebRootPath + "/images/provider_profile"; //Path
                    imageURL = "https://kharban.net:2096/images/provider_profile/" + imageName;
                }
                else if (model.ImageType == "iban")
                {
                    path = hostingEnvironment.WebRootPath + "/images/provider_iban"; //Path
                    imageURL = "https://kharban.net:2096/images/provider_iban/" + imageName;
                }
                else if (model.ImageType == "document")
                {
                    path = hostingEnvironment.WebRootPath + "/images/provider_document"; //Path
                    imageURL = "https://kharban.net:2096/images/provider_document/" + imageName;
                }

                //Check if directory exist
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
                }

                //set the image path
                imgPath = Path.Combine(path, imageName);
            }
            else
            {
                returnModel.msg = Resource_Kharban.Error;
                return Ok(returnModel);
            }
            returnModel = new ProviderRepository().UploadProviderImage(model, imageURL, imgPath);
            return Ok(returnModel);
        }

        [HttpPost,Route("customerImagesUpload")]
        public ActionResult UploadImagesCustomer(CustomerModel model)
        {
            if (model == null)
                model = new CustomerModel();
            string imageName = string.Empty;
            string path = string.Empty;
            string imgPath = string.Empty;
            string imageURL = string.Empty;

            Response<string> returnModel = new Response<string>();
            if (!string.IsNullOrEmpty(model.image) && !string.IsNullOrEmpty(model.image_extension))
            {
                imageName = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + model.image_extension;

                if (model.image_extension.ToLower() != "jpg" && model.image_extension.ToLower() != "jpeg" && model.image_extension.ToLower() != "png")
                {
                    returnModel.success = false;
                    returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                    returnModel.msg = Resource_Kharban.SelectImageValidation;
                    return Ok(returnModel);
                }


                if (model.ImageType == "profile")
                {
                    path = hostingEnvironment.WebRootPath + "/images/customer_profile"; //Path
                    imageURL = "https://kharban.net:2096/images/customer_profile/" + imageName;
                }
                //Check if directory exist
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
                }

                //set the image path
                imgPath = Path.Combine(path, imageName);

                //set the image path
                imgPath = Path.Combine(path, imageName);
                imageURL = "https://kharban.net:2096/images/customer_profile/" + imageName;



            }
            returnModel = new ProviderRepository().UploadCustomerImage(model, imageURL, imgPath);
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.UPLOAD_PROVIDER_IMAGE_BY_MOBILE)]
        public ActionResult UploadProviderImage([FromBody]ProviderModel model)
        {
            if (model == null)
                model = new ProviderModel();

            string imageURL = string.Empty;
            string imageName = string.Empty;
            string path = string.Empty;
            string imgPath = string.Empty;
            Response<string> returnModel = new Response<string>();
            if (!string.IsNullOrEmpty(model.image) && !string.IsNullOrEmpty(model.image_extension))
            {
                imageName = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + model.image_extension;

                if (model.image_extension.ToLower() != "jpg" && model.image_extension.ToLower() != "jpeg" && model.image_extension.ToLower() != "png")
                {
                    returnModel.success = false;
                    returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                    returnModel.msg = Resource_Kharban.SelectImageValidation;
                    return Ok(returnModel);
                }


                if (model.ImageType == "profile")
                {
                    path = hostingEnvironment.WebRootPath + "/images/provider_profile"; //Path
                    imageURL = "https://kharban.net:2096/images/provider_profile/" + imageName;
                }
                else if (model.ImageType == "iban")
                {
                    path = hostingEnvironment.WebRootPath + "/images/provider_iban"; //Path
                    imageURL = "https://kharban.net:2096/images/provider_iban/" + imageName;
                }
                else if (model.ImageType == "document")
                {
                    path = hostingEnvironment.WebRootPath + "/images/provider_document"; //Path
                    imageURL = "https://kharban.net:2096/images/provider_document/" + imageName;
                }

                //Check if directory exist
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
                }

                //set the image path
                imgPath = Path.Combine(path, imageName);
            }
            else
            {
                returnModel.msg = Resource_Kharban.Error; 
                return Ok(returnModel);
            }
            returnModel = new ProviderRepository().UploadProviderImage(model, imageURL, imgPath);
            return Ok(returnModel);
        }

        [HttpPost, Route(APIURL.UPLOAD_CUSTOMER_IMAGE_BY_MOBILE)]
        public IActionResult UploadCustomerImage(CustomerModel model)
        {
            if (model == null)
                model = new CustomerModel();
            string imageName = string.Empty;
            string path = string.Empty;
            string imgPath = string.Empty;
            string imageURL = string.Empty;

            Response<string> returnModel = new Response<string>();
            if (!string.IsNullOrEmpty(model.image) && !string.IsNullOrEmpty(model.image_extension))
            {
                imageName = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + model.image_extension;

                if (model.image_extension.ToLower() != "jpg" && model.image_extension.ToLower() != "jpeg" && model.image_extension.ToLower() != "png")
                {
                    returnModel.success = false;
                    returnModel.status = (int)EnumClass.ResponseState.ResposityError;
                    returnModel.msg = Resource_Kharban.SelectImageValidation;
                    return Ok(returnModel);
                }


                if (model.ImageType == "profile")
                {
                    path = hostingEnvironment.WebRootPath + "/images/customer_profile"; //Path
                    imageURL = "https://kharban.net:2096/images/customer_profile/" + imageName;
                }
                //Check if directory exist
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
                }

                //set the image path
                imgPath = Path.Combine(path, imageName);

                //set the image path
                imgPath = Path.Combine(path, imageName);
                imageURL = "https://kharban.net:2096/images/provider_profile/" + imageName;



            }
            returnModel = new ProviderRepository().UploadCustomerImage(model, imageURL, imgPath);
            return Ok(returnModel);
        }
    }
}
