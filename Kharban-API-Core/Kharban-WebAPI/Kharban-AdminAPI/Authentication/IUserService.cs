using Kharban_AdminAPI.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Kharban_AdminAPI.Authentication
{
    public interface IUserService
    {
        AuthenticateResponse Authorization(AuthenticateRequest model);
        UserLoginResponse GetById(string username, string password);
    }
    
}