using Insight.Database;
using Kharban_WebAPI.Helper;
using Kharban_WebAPI.Helpers;
using Kharban_WebAPI.Models;
using Kharban_WebAPI.Repository;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;


namespace Kharban_WebAPI.Authentication
{
   
    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
       
        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public AuthenticateResponse Authorization(AuthenticateRequest model)
        {
            Accounts acc = new Accounts();
            UserLoginResponse userLoginResponse = acc.getValidate(model.username, model.password);
            if (userLoginResponse.Status==false) return null;
            var token = generateJwtToken(userLoginResponse);

            return new AuthenticateResponse(userLoginResponse, token);
        }

        public UserLoginResponse GetById(string UserId)
        {
            UserLoginResponse resModel = new UserLoginResponse();
            using (SqlConnection con = new SqlConnection(SiteKey.ConnectionString))
            {
                resModel = con.QuerySql<UserLoginResponse>(@"select id, concat(first_name,' ', last_name) fullname, email from customer where id = @UserId", new
                {
                    UserId = UserId

                }).FirstOrDefault();

                if (resModel == null)
                    resModel = con.QuerySql<UserLoginResponse>(@"select id, concat(first_name,' ', last_name) fullname, email from provider where id = @UserId", new 
                    {
                        UserId = UserId
                    }).FirstOrDefault();


               
            }
            return resModel;
        }
        // helper methods

        private string generateJwtToken(UserLoginResponse userLoginResponse)
        {
            // generate token that is valid for 1 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    
                    new Claim("userId", userLoginResponse.Id.ToString()),
                    new Claim("username", userLoginResponse.fullname.ToString()),
                    new Claim(ClaimTypes.Name,userLoginResponse.fullname.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}