using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(PostJesusSeguridadWebApi.Startup))]

namespace PostJesusSeguridadWebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            ConfigurarOauth(app);
            app.UseWebApi(config);

        }

        public void ConfigurarOauth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions opcionesautorizacion =
               new OAuthAuthorizationServerOptions()
               {
                   AllowInsecureHttp = true,
                   TokenEndpointPath = new PathString("/recuperartoken"),
                   AccessTokenExpireTimeSpan = TimeSpan.FromHours(1),
                   Provider = new Credentials.AutorizacionCredencialesToken()
               };
            app.UseOAuthAuthorizationServer(opcionesautorizacion);
            OAuthBearerAuthenticationOptions opcionesoauth =
                new OAuthBearerAuthenticationOptions()
                {
                    AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active
                };

            app.UseOAuthBearerAuthentication(opcionesoauth);
        }
    }
}
