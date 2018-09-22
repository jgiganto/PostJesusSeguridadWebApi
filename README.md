# PostJesusSeguridadWebApi
Muy buenas, en el presente Post voy a explicar qué es la seguridad Oauth 2 y cómo usarla en un servicio WebApi, haciendo uso de diferentes roles.

![Oauth logo](https://i.imgur.com/1sDdJY6.gif)

**Oauth 2** es un protocolo abierto de seguridad (publicado en octubre de 2012 por el **Internet Engineering Task Force (IETF))** que proporciona flujos de autorización específicos para aplicaciones Web, de escritorio, smartphones, etc. Empresas como **Facebook**, **Google** o **Github**, entre otras , utilizan este esquema de seguridad en sus API.  Oauth 2 describe como un usuario (consumidor del servicio) puede tener acceso la información ofrecida por un proveedor de servicio (APIs) mediante el uso de Tokens de acceso temporales. Estos dan acceso a la información del usuario de forma y tiempo limitado incluyendo código en la petición.

-Hay que decir que este sistema tiene un punto débil, ya que si alguien captura el token de acceso podría usarlo para acceder a la información del usuario, aunque para evitar esto hay que tener en cuenta una serie de recomendaciones en el uso de esta tecnología:

  -**Todo el intercambio de token con el servidor** de autorización debe hacerse bajo el protocolo de comunicaciones TLS, el cual garantiza la integridad y confidencialidad de la comunicación. De esta forma evitamos que un atacante externo pueda capturar nuestros tokens de acceso, refresco o autorización, que podrían ser usados contra los servidores de autenticación o de recursos para obtener acceso a información reservada. El ámbito o acceso del token de acceso debe ser lo más limitado posible, limitando la información a la que se puede tener acceso.
  -**El servidor de autorización es el encargado de autenticar al usuario de forma segura.** El servidor debe garantizar que la contraseña (o el sistema de autenticación utilizado) sea lo suficientemente robusto para autenticar a los usuarios. En aquellos casos en los que no se pueda autenticar al cliente, el servidor debe utilizar otros métodos para la validación, como por ejemplo que sea necesario configurar la dirección URI de redireccionamiento que se utiliza.
La generación de los tokens por parte del servidor de autorización debe garantizar que no puedan ser generados, modificados o adivinados por terceras partes. La probabilidad de que un atacante genere un token debe ser inferior a 2-128 y recomendablemente inferior o igual a 2-160.
-**El tiempo de validez de los tokens o códigos de acceso debe ser de corto y de un solo uso.** Además, si el servidor de autorización detecta múltiples intentos de obtención de un token de acceso con un mismo código de autorización, el servidor debe revocar el acceso de todos los códigos de acceso concedidos en base a este token de autorización ya que este token está comprometido.
-**Los clientes que se autentiquen con el servidor de autorización deben validar el certificado TLS del servidor**, comprobando así la identidad del mismo y evitando posibles ataques man-in-the-middle que pueda sufrir el protocolo o posibles ataques de Phising que intenten suplantar la identidad del centro de autorización para obtener las credenciales del usuario.
-**Los clientes no deben almacenar los token en sitios vulnerables o accesibles**, como por ejemplo serían las cookies.
-**El servidor de autorización** debe comprobar que las URIs de redireccionamiento usadas tanto al conseguir el código de autorización como de acceso deben coincidir, evitando así que un atacante modifique la misma y obtenga acceso a la cuenta de un usuario.

![secuencia Oauth](https://i.imgur.com/WdU4Yhl.gif)


Ahora vamos a ver como implementar este esquema de seguridad en un sencillo ejemplo, para ello usaremos Visual Studio de Microsoft.
Lo primero que haremos será abrir un nuevo proyecto MVC empty y marcaremos las referencias para Web APi, para este proyecto necesitaremos agregar tres paquetes NuGet y actualizar el paquete de NewtonSoft.

![Nuget](https://i.imgur.com/q8cSTbe.png)

Sobre nuestro proyecto usando el explorador de archivos buscaremos el Glogal.asax y lo eliminaremos, además añadiremos una nueva clase OWIN y la llamaremos Startup.cs. 

![Owin](https://i.imgur.com/lFdirlF.png)

Debemos editar el fichero AssemblyInfo.cs y añadir la siguiente linea :

```C#
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: OwinStartup(typeof(PostJesusSeguridadWebApi.Startup))]
```

En el archivo Web.config incluiremos una clave de inicio indicando la clase OWIN:


```XML
<configuration>
  <appSettings>
    <add key="owin:appStartup" value="PostJesusSeguridadWebApi.Startup,PostJesusSeguridadWebApi" />
  </appSettings>
  <system.web>
```

Ahora vamos a crear la capa MidleWare que se encargará de ver qué usuarios tienen acceso validando el usuario y Pw y en caso afirmativo les otorgará el Token de acceso. Para ello creamos una carpeta llamada Credentials y dentro de ella una clase a la que podemos llamar por ejemplo : AutorizacionCredencialesToken. en ella escribiremos las lineas siguientes:

```C#
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
```

En este ejemplo hemos creado dos roles (Administrador y usuario), cuando solicitemos un Token el sistema sabrá con que perfil lo hemos hecho y nos permitirá acceder sólo a los datos del mismo.

En la clase Startup.cs, incluiremos las lineas siguientes:

```c#
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
```

Esta clase utiliza a la capa MidleWare para saber si genera o no el token.

Ahora en la carpeta Models crearemos una clase para simular “zonas de usuarios” distintas donde tendremos acceso dependiendo del Role con el que solicitemos el Token.

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PostJesusSeguridadWebApi.Models
{
    public class ModeloUsuario
    {
        public String GetMensajeAdmin()
        {
            return "Has sido validado con éxito Sr. Administrador!!";
        }

        public String GetMensajeUsuario()
        {
            return "Has sido validado con éxito Sr. Usuario!!";
        }
    }
}
```

En el Controlador debemos decorar nuestras funciones con los Roles a los que tienen acceso, si queremos que el Role Admin por ejemplo pueda acceder a varias funciones solo tenemos

que indicarles su Authorize correspondiente justo encima de las mismas.

```C#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PostJesusSeguridadWebApi.Models;

namespace PostJesusSeguridadWebApi.Controllers
{
    public class UsuarioController : ApiController
    {
        ModeloUsuario modelo;
        public UsuarioController()
        {
            modelo = new ModeloUsuario();
        }

        [Authorize(Roles ="ADMINISTRADOR")]
        public String Get()
        {
            return modelo.GetMensajeAdmin();
        }

       [HttpGet]
        [Authorize(Roles = "USUARIO")]
        [Route("api/currito")]
        public String usuario()
        {
            return modelo.GetMensajeUsuario();
        }
    }
}
```

Ya tenemos configurada la seguridad en nuestra Api. Ahora vamos a realizar las comprobaciones necesarias para saber que todo este funcionando correctamente. Para ello nos ayudaremos de la herramienta PostMan, este software nos permitirá realizar peticiones Post y Get usando usuario y contraseña.

Primero comprobamos que si hacemos una petición Post a nuestra Url “http://localhost:58186/recuperartoken” con unas credenciales no válidas el sistema nos devuelve un mensaje de error:

![POSTMAN](https://i.imgur.com/763GHQq.png)

Si introducimos el usuario y contraseña correctos el sistema nos devolverá el Token:

![TOKEN](https://i.imgur.com/Lvd6cnG.png)

El cual usaremos para realizar la petición Get a nuestra Zona:


![Validado](https://i.imgur.com/kgI1hXE.png)

y si intentamos acceder con este Token a una zona que no nos corresponde el sistema nos deniega el acceso:

![Denegado](https://i.imgur.com/IlWUpqh.png)

Así quedaría comprobado que  la seguridad Oauth 2.0 está funcionando correctamente.

Hasta aquí este post de cómo implantar seguridad Oauth 2.0  espero que os sirva y os incluyo un enlace Github donde podéis descargar todo el código.

 

 

Enlace GitHub:

https://github.com/jgiganto/PostJesusSeguridadWebApi.git

Referencias:

www.certsi.es, Wikipedia, http://www.ietf.org.

 

Autor: Jesús Giganto Diez
