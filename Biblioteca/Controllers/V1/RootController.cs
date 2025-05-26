using Biblioteca.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize]
    public class RootController:ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }
        [HttpGet(Name ="ObtenerRootV1")]
        [AllowAnonymous]
        public async Task<IEnumerable<DatosHATEOASDTO>> Get()
        {
            var datosHATEOAS=new List<DatosHATEOASDTO>();

            var esAdmin = await authorizationService.AuthorizeAsync(User, "esadmin");

            //acciones que puede ralizar cualquiera

            datosHATEOAS.Add(new DatosHATEOASDTO(Url.Link("ObtenerRootV1", new { })!,
                Descripcion: "self", Metodo: "GET"));

            datosHATEOAS.Add(new DatosHATEOASDTO(Url.Link("ObtenerAutoresV1", new { })!,
                Descripcion: "Obtener-Autores", Metodo: "GET"));

            datosHATEOAS.Add(new DatosHATEOASDTO(Url.Link("LoginUsuarioV1", new { })!,
                Descripcion: "login-usuario", Metodo: "POST"));

            datosHATEOAS.Add(new DatosHATEOASDTO(Url.Link("RegistroUsuarioV1", new { })!,
                Descripcion: "usuarios-registrar", Metodo: "POST"));

            

            if (User.Identity!.IsAuthenticated)
            {
                //acciones para usuarios logueados
                datosHATEOAS.Add(new DatosHATEOASDTO(Url.Link("ActualizarUsuarioV1", new { })!,
                    Descripcion: "usuario-actualizar", Metodo: "PUT"));

                datosHATEOAS.Add(new DatosHATEOASDTO(Url.Link("RenovarTokenV1", new { })!,
                    Descripcion: "token-renovar", Metodo: "GET"));
            }



            if (esAdmin.Succeeded)
            {
                //acciones q solo pueden hacer los admin
                datosHATEOAS.Add(new DatosHATEOASDTO(Url.Link("CrearAutorV1", new { })!,
                    Descripcion: "autor-crear", Metodo: "POST"));

                datosHATEOAS.Add(new DatosHATEOASDTO(Url.Link("CrearAutoresV1", new { })!,
                    Descripcion: "autores-crear", Metodo: "POST"));

                datosHATEOAS.Add(new DatosHATEOASDTO(Url.Link("CrearlibroV1", new { })!,
                    Descripcion: "libro-crear", Metodo: "POST"));

                datosHATEOAS.Add(new DatosHATEOASDTO(Url.Link("ObtenerUsuarioV1", new { })!,
                    Descripcion: "usuarios-obtener", Metodo: "GET"));
            }

            return datosHATEOAS;
        }
    }
}
