using System.ComponentModel.DataAnnotations;

namespace Kharban_AdminAPI.Models
{
    public class AuthenticateRequest
    {
        public string username { get; set; }

        public string password { get; set; }
    }
}