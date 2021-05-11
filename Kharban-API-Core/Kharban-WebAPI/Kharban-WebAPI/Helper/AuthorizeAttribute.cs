using Kharban_WebAPI.Helpers;
using Kharban_WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Security.Claims;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    
    static object  UserIdn;
    public void OnAuthorization(AuthorizationFilterContext context)
    {

        var user = (UserLoginResponse)context.HttpContext.Items["UserLoginResponse"];
        if (user == null)
        {
            // not logged in
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
        
    }

    
}

