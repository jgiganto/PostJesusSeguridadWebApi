using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;
using System.Security.Claims;


namespace PostJesusSeguridadWebApi.Credentials
{
    public class AutorizacionCredencialesToken:OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            var acceso = ("admin" == context.UserName && context.Password == "123");
            if (acceso == false)
            {
                context.SetError("Sin acceso", "Usuario o Pw incorrecto, use admin + 123");
                return;
            }
           

            ClaimsIdentity identidad = new ClaimsIdentity(context.Options.AuthenticationType);
            identidad.AddClaim(new Claim(ClaimTypes.Name, context.Password));
            identidad.AddClaim(new Claim(ClaimTypes.Role, "ADMINISTRADOR"));
            context.Validated(identidad);
        }
    }
}