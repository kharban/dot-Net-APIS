using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Kharban_WebAPI.Models;
using Kharban_WebAPI.Repository;

namespace Kharban_WebAPI
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        //public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        //{
        //    context.Validated();
        //}

        //public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        //{
        //    // Change authentication ticket for refresh token requests  
        //    var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
        //    newIdentity.AddClaim(new Claim("newClaim", "newValue"));
        //    var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
        //    context.Validated(newTicket);
        //    return Task.FromResult<object>(null);
        //}

        //public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        //{
        //    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
        //    Accounts acc = new Accounts();
        //    //Authenticate the user credentials
        //    UserLoginResponse userLoginResponse = acc.getValidate(context.UserName, context.Password);
        //    if (userLoginResponse.Status == true)
        //    {
        //        identity.AddClaim(new Claim("userId", userLoginResponse.Id));
        //        identity.AddClaim(new Claim("username", context.UserName));
        //        //identity.AddClaim(new Claim("email", userLoginResponse.Email));
        //        //identity.AddClaim(new Claim("ipaddress", userLoginResponse.IPAddress.ToString()));
        //        identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
        //        context.Validated(identity);
        //        return;
        //    }
        //    else
        //    {
        //        context.SetError(userLoginResponse.StatusMessage);
        //        return;
        //    }
        //}
    }

    //public class RefreshTokenProvider : IAuthenticationTokenProvider
    //{
    //    private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();

    //    //public async Task CreateAsync(AuthenticationTokenCreateContext context)
    //    //{
    //    //    var guid = Guid.NewGuid().ToString();
    //    //    // copy all properties and set the desired lifetime of refresh token  
    //    //    var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
    //    //    {
    //    //        IssuedUtc = context.Ticket.Properties.IssuedUtc,
    //    //        ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
    //    //    };
    //    //    var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);
    //    //    _refreshTokens.TryAdd(guid, refreshTokenTicket);
    //    //    // consider storing only the hash of the handle  
    //    //    context.SetToken(guid);
    //    //}
    //    //public void Create(AuthenticationTokenCreateContext context)
    //    //{
    //    //    throw new NotImplementedException();
    //    //}
    //    //public void Receive(AuthenticationTokenReceiveContext context)
    //    //{
    //    //    throw new NotImplementedException();
    //    //}
    //    //public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
    //    //{
    //    //    // context.DeserializeTicket(context.Token);
    //    //    AuthenticationTicket ticket;
    //    //    string header = context.OwinContext.Request.Headers["Authorization"];
    //    //    if (_refreshTokens.TryRemove(context.Token, out ticket))
    //    //    {
    //    //        context.SetTicket(ticket);
    //    //    }
    //    //}
    //}
}