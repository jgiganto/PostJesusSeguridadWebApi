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
            var acceso = (("admin" == context.UserName && context.Password == "123") || ("user" == context.UserName && context.Password == "222"));
            if (acceso == false)
            {
                context.SetError("Sin acceso", "Usuario o Pw incorrecto, use admin + 123 , o user + 222");

            }
            else
            {
                ClaimsIdentity identidad = new ClaimsIdentity(context.Options.AuthenticationType);
                identidad.AddClaim(new Claim(ClaimTypes.Name, context.Password));
                //
                if (("admin" == context.UserName && context.Password == "123"))
                {
                    identidad.AddClaim(new Claim(ClaimTypes.Role, "ADMINISTRADOR"));
                    context.Validated(identidad);
                }
                if (("user" == context.UserName && context.Password == "222"))
                {
                    identidad.AddClaim(new Claim(ClaimTypes.Role, "USUARIO"));
                    context.Validated(identidad);
                }

            }
           


        }
    }
}