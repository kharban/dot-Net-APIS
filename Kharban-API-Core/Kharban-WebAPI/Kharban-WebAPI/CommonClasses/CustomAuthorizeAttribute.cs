using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Mvc;

namespace Kharban_WebAPI
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private IHttpContextAccessor httpContextAccessor;
        //protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        //protected override void HandleUnauthorizedRequest(AuthorizationContext actionContext)
        //{

        //    if (!httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
        //    {
        //        //ClaimsIdentity claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
        //        //string userid = string.Empty;
        //        ////// Access claims
        //        //foreach (Claim claim in claimsIdentity.Claims)
        //        //{
        //        //   if (claim.Type == "userId")
        //        //    {
        //        //        userid = claim.Value;
        //        //       HttpContext.Current.Session["userId"] = claim.Value;
        //        //    }
        //        //}
        //        base.HandleUnauthorizedRequest(actionContext);
        //    }
        //    else
        //    {
        //        //Setting error message and status fode 403 for unauthorized user
        //        actionContext.Result= new ViewResult
        //        {
        //            ViewName = "Authorization failed."
        //        };



        //        //actionContext.Result = new ViewResult(System.Net.HttpStatusCode.Forbidden) { Content = new StringContent(JsonConvert.SerializeObject(new { Message = "Authorization failed." })), StatusCode = HttpStatusCode.Forbidden };
        //    }

        //}


    }
    
}