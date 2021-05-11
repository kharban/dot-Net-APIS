using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Kharban_AdminAPI.Controllers
{
    public abstract class BaseController : Controller
    {
        protected string Email()
        {
            return this.User.Claims.First(i => i.Type == "emailid").Value;
        }
        protected string FirstName()
        {
            return this.User.Claims.First(i => i.Type == "firstname").Value;
        }
        protected string LastName()
        {
            return this.User.Claims.First(i => i.Type == "lastname").Value;
        }
        protected string UserName()
        {
            return this.User.Claims.First(i => i.Type == "username").Value;
        }
        protected string CountryCode()
        {
            return this.User.Claims.First(i => i.Type == "countrycode").Value;
        }
        protected string Mobile()
        {
            return this.User.Claims.First(i => i.Type == "mobile").Value;
        }
    }
}
