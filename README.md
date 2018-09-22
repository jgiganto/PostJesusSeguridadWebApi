# PostJesusSeguridadWebApi
Muy buenas, en el presente Post voy a explicar qué es la seguridad Oauth 2 y cómo usarla en un servicio WebApi, haciendo uso de diferentes roles.

![Oauth logo](https://i.imgur.com/1sDdJY6.gif)


Oauth 2 es un protocolo abierto de seguridad (publicado en octubre de 2012 por el Internet Engineering Task Force (IETF)) que proporciona flujos de autorización específicos para aplicaciones Web, de escritorio, smartphones, etc. Empresas como Facebook, Google o Github, entre otras , utilizan este esquema de seguridad en sus API.  Oauth 2 describe como un usuario (consumidor del servicio) puede tener acceso la información ofrecida por un proveedor de servicio (APIs) mediante el uso de Tokens de acceso temporales. Estos dan acceso a la información del usuario de forma y tiempo limitado incluyendo código en la petición.

Hay que decir que este sistema tiene un punto débil, ya que si alguien captura el token de acceso podría usarlo para acceder a la información del usuario, aunque para evitar esto hay que tener en cuenta una serie de recomendaciones en el uso de esta tecnología:

Todo el intercambio de token con el servidor de autorización debe hacerse bajo el protocolo de comunicaciones TLS, el cual garantiza la integridad y confidencialidad de la comunicación. De esta forma evitamos que un atacante externo pueda capturar nuestros tokens de acceso, refresco o autorización, que podrían ser usados contra los servidores de autenticación o de recursos para obtener acceso a información reservada. El ámbito o acceso del token de acceso debe ser lo más limitado posible, limitando la información a la que se puede tener acceso.
El servidor de autorización es el encargado de autenticar al usuario de forma segura. El servidor debe garantizar que la contraseña (o el sistema de autenticación utilizado) sea lo suficientemente robusto para autenticar a los usuarios. En aquellos casos en los que no se pueda autenticar al cliente, el servidor debe utilizar otros métodos para la validación, como por ejemplo que sea necesario configurar la dirección URI de redireccionamiento que se utiliza.
La generación de los tokens por parte del servidor de autorización debe garantizar que no puedan ser generados, modificados o adivinados por terceras partes. La probabilidad de que un atacante genere un token debe ser inferior a 2-128 y recomendablemente inferior o igual a 2-160.
El tiempo de validez de los tokens o códigos de acceso debe ser de corto y de un solo uso. Además, si el servidor de autorización detecta múltiples intentos de obtención de un token de acceso con un mismo código de autorización, el servidor debe revocar el acceso de todos los códigos de acceso concedidos en base a este token de autorización ya que este token está comprometido.
Los clientes que se autentiquen con el servidor de autorización deben validar el certificado TLS del servidor, comprobando así la identidad del mismo y evitando posibles ataques man-in-the-middle que pueda sufrir el protocolo o posibles ataques de Phising que intenten suplantar la identidad del centro de autorización para obtener las credenciales del usuario.
Los clientes no deben almacenar los token en sitios vulnerables o accesibles, como por ejemplo serían las cookies.
El servidor de autorización debe comprobar que las URIs de redireccionamiento usadas tanto al conseguir el código de autorización como de acceso deben coincidir, evitando así que un atacante modifique la misma y obtenga acceso a la cuenta de un usuario.

![secuencia Oauth](https://i.imgur.com/WdU4Yhl.gif)


Ahora vamos a ver como implementar este esquema de seguridad en un sencillo ejemplo, para ello usaremos Visual Studio de Microsoft.
Lo primero que haremos será abrir un nuevo proyecto MVC empty y marcaremos las referencias para Web APi, para este proyecto necesitaremos agregar tres paquetes NuGet y actualizar el paquete de NewtonSoft.

![Nuget](https://i.imgur.com/q8cSTbe.png)

Sobre nuestro proyecto usando el explorador de archivos buscaremos el Glogal.asax y lo eliminaremos, además añadiremos una nueva clase OWIN y la llamaremos Startup.cs. 

![Owin](https://i.imgur.com/lFdirlF.png)

