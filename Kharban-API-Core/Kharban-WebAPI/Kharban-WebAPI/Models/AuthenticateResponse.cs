using System.Text.Json.Serialization;

namespace Kharban_WebAPI.Models
{
    public class AuthenticateResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string access_token { get; set; }


        public AuthenticateResponse(UserLoginResponse user, string token)
        {
            Id = user.Id;
            UserName = user.fullname;
            access_token = token;
        }

        public class User
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Username { get; set; }

            [JsonIgnore]
            public string Password { get; set; }
        }
    }
    
}
