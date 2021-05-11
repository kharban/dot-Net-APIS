using Kharban_WebAPI.Models;
using Newtonsoft.Json;
using System;
using System.Security.Principal;


namespace Kharban_WebAPI.Helpers
{
    public class CustomPrincipal : IPrincipal
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public int RoleID { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }

        [JsonIgnore]
        public IIdentity Identity { get; private set; }

        public CustomPrincipal() { }

        public CustomPrincipal(string userName, params int[] roleTypes)
        {
            this.Identity = new GenericIdentity(userName);
            this.UserName = userName;
            this.UserType = "WebUser";
        }

        public CustomPrincipal(UserLoginResponse user, params int[] roleTypes)
        {
            this.Identity = new GenericIdentity(user.Id);
            this.UserID = user.Id;
            this.UserName = user.fullname;
            this.UserType = "WebUser";
        }

        public bool IsInRole(string role)
        {
            return false;
        }
        public bool IsInRole(int role)
        {
            return true;
        }
    }
}