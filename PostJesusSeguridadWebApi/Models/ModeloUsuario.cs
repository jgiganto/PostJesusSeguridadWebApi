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