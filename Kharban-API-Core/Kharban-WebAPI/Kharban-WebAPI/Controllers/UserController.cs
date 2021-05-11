using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Kharban_WebAPI.Models.User;
using Microsoft.Extensions.Configuration;
using Kharban_WebAPI.Models;
using Kharban_WebAPI.Repository;
using Kharban_WebAPI.Authentication;

namespace Kharban_WebAPI.Controllers
{
    [Route("api/v1")]
    public class UserController : BaseController
    {
        private IConfiguration Configuration;
        private IUserService userService;
        public UserController(IConfiguration _configuration, IUserService _userService)
        {
            this.Configuration = _configuration;
            this.userService = _userService;
        }


        [HttpPost]
        [Route("/authorization")]
        public IActionResult Authorization([FromBody]AuthenticateRequest model)
        {
            var authorizationDetail = userService.Authorization(model);
            if (authorizationDetail == null)
                return BadRequest(new { message = "Error" });
            return Ok(authorizationDetail);
        }

        [Authorize]
        [HttpPost, Route(APIURL.SAVE_DEVICE_INFO)]
        public ActionResult SaveDeviceInfo(DeviceInfoModel Model)
        {
            new UserRepository().SaveDeviceInfo(Model,  GetUserId());
            return Ok("");
        }
        [Authorize]
        [HttpPost,Route("saveDeviceToken")]
        public ActionResult SaveDeviceToken([FromBody]DeviceInfoModel Model)
        {
            new UserRepository().SaveDeviceInfo(Model, GetUserId());
            return Ok("");
        }
        
        [Authorize]
        [HttpPost, Route(APIURL.GET_LOGIN_USER_INFO)]
        public ActionResult GetLoginUserInfo()
        {
            UserLoginModel model = new UserLoginModel();
            model.userid = GetUserId();
            return Ok(model);
        }

        [HttpPost, Route(APIURL.GET_OTP)]
        public ActionResult GetOTP([FromBody]UserLoginModel Model)
        {
            UserLoginModel Result = new UserRepository().GetOTP(Model);
            return Ok(Result);
        }

        [HttpPost, Route(APIURL.ADD_CUSTOMER)]
        public ActionResult AddCustomer([FromBody]UserLoginModel Model)
        {
            UserLoginModel Result = new UserRepository().AddCustomer(Model);
            return Ok(Result);
        }

        [Authorize]
        [HttpPost, Route(APIURL.UPDATE_CUSTOMER)]
        public ActionResult UpdateCustomer([FromBody]UserLoginModel Model)
        {
            UserLoginModel Result = new UserRepository().UpdateCustomer(Model,GetUserId());
            return Ok(Result);
        }

        [HttpPost, Route(APIURL.ADD_PROVIDER)]
        public ActionResult AddProvider([FromBody]UserLoginModel Model)
        {
            UserLoginModel Result = new UserRepository().AddProvider(Model);
            return Ok(Result);
        }

        [Authorize]
        [HttpPost, Route(APIURL.GET_CUSTOMER)]
        public ActionResult GetCustomer(UserLoginModel Model)
        {
            Model = Model ?? new UserLoginModel();
            Response<UserLoginModel> Result = new UserRepository().GetCustomer(Model,  GetUserId());
            return Ok(Result);
        }

        [Authorize]
        [HttpPost, Route(APIURL.GET_PROVIDERS)]
        public ActionResult GetProviders([FromBody]UserLoginModel Model)
        {
            Response<List<UserLoginModel>> Result = new UserRepository().GetProviders(Model);
            return Ok(Result);
        }

        [Authorize]
        [HttpPost, Route(APIURL.GET_PROVIDER_INFORMATION)]
        public ActionResult GetProviderInformation([FromBody]UserLoginModel Model)
        {
            Model = Model ?? new UserLoginModel();
            Response<UserLoginModel> Result = new UserRepository().GetProviderInformation(Model,  GetUserId());
            return Ok(Result);
        }

        [Authorize]
        [HttpPost, Route(APIURL.Mark_FAVOURITE_LOCATION)]
        public ActionResult MarkFavouriteLocation([FromBody]UserLoginModel Model)
        {
            Response<UserLoginModel> Result = new UserRepository().MarkFavouriteLocation(Model,  GetUserId());
            return Ok(Result);
        }

        [Authorize]
        [HttpPost, Route(APIURL.UPDATE_PROVIDER_INFORMATION)]
        public ActionResult UpdateProviderInformation(ProviderModel Model)
        {
            Response<ProviderModel> Result = new UserRepository().UpdateProviderInformation(Model,  GetUserId());
            return Ok(Result);
        }

        [Authorize]
        [HttpPost, Route(APIURL.UPDATE_PROVIDER_LOCATION)]
        public ActionResult UpdateProviderLocation([FromBody]ProviderModel Model)
        {
            Response<ProviderModel> Result = new UserRepository().UpdateProviderLocation(Model,  GetUserId());
            return Ok(Result);
        }
        
        [Authorize]
        [HttpPost, Route(APIURL.UPDATE_PROVIDER_ONLINE_STATUS)]
        public ActionResult UpdateProviderOnlineStatus([FromBody]ProviderModel Model)
        {
            Response<ProviderModel> Result = new UserRepository().UpdateProviderOnlineStatus(Model,  GetUserId());
            return Ok(Result);
        }
        
        
        [HttpPost, Route(APIURL.GET_COUNTRY)]
        public ActionResult GetCountry(ProviderModel Model)
        {
            Response<List<CountryModel>> Result = new UserRepository().GetCountry();
            return Ok(Result);
        }

        [Authorize]
        [HttpPost, Route(APIURL.INSERT_CUSTOMER_CARD)]
        public ActionResult InsertCustomerCard([FromBody]CustomerCardDetails Model)
        {
            Response<CustomerCardDetails> Result = new UserRepository().InsertCustomerCard(Model, GetUserId());
            return Ok(Result);
        }

        [HttpPost,Route("customerCardInsert")]
        public ActionResult CustomerCardInsert([FromBody]CustomerCardDetails Model)
        {
            return Ok();
        }


        [Authorize]
        [HttpPost, Route(APIURL.VIEW_CUSTOMER_CARD)]
        public ActionResult ViewCustomerCard(CustomerCardDetails Model)
        {
            Response<List<CustomerCardDetails>> Result = new UserRepository().ViewCustomerCard(Model,  GetUserId());
            return Ok(Result);
        }
        [Authorize]
        [HttpPost, Route(APIURL.DELETE_CUSTOMER_CARD)]
        public ActionResult DeleteCustomerCard([FromBody]CustomerCardDetails Model)
        {
            Response<CustomerCardDetails> Result = new UserRepository().DeleteCustomerCard(Model);
            return Ok(Result);
        }


        
    }
}