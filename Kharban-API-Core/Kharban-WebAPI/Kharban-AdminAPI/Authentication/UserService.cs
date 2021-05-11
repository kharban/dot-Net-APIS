using Insight.Database;
using Kharban_AdminAPI.Helper;
using Kharban_AdminAPI.Helpers;
using Kharban_AdminAPI.Models;
using Kharban_AdminAPI.Repository;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;


namespace Kharban_AdminAPI.Authentication
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
            if (userLoginResponse.Status == false) return null;
            var token = generateJwtToken(userLoginResponse);

            return new AuthenticateResponse(userLoginResponse, token);
        }

        public UserLoginResponse GetById(string username,string password)
        {
            UserLoginResponse resModel = new UserLoginResponse();
            using (SqlConnection con = new SqlConnection(SiteKey.ConnectionString))
            {
                resModel = con.QuerySql<UserLoginResponse>(@"SELECT username , email Email, first_name , last_name , mobile, country_code,password  FROM admin_user where email = @email and password = @password", new { email = username, password = password }).FirstOrDefault();
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
                    new Claim("emailid", userLoginResponse.email),
                    new Claim("firstname", userLoginResponse.first_name),
                    new Claim("lastname", userLoginResponse.last_name),
                    new Claim("countrycode", userLoginResponse.country_code),
                    new Claim("mobile", userLoginResponse.mobile),
                    new Claim("username", userLoginResponse.email.ToString()),
                    new Claim("password", userLoginResponse.password.ToString()),
                    new Claim(ClaimTypes.Name,userLoginResponse.email.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}