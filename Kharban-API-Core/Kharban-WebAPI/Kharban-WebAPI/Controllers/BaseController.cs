using Kharban_WebAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kharban_WebAPI.Controllers
{
    public abstract class BaseController : Controller
    {
        protected string GetUserId()
        {
            return this.User.Claims.First(i => i.Type == "userId").Value;
        }
    }
}
