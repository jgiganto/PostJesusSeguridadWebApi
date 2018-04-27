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
 