using Biblioteca.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    public class RootController:ControllerBase
    {
        [HttpGet(Name = "obtenerRootV1")]
        public IEnumerable<datosHATEOASDTO> Get()
        {
            var datosHETEOAS = new List<datosHATEOASDTO>();

            datosHETEOAS.Add(new datosHATEOASDTO(Enlace: Url.Link("obtenerRootV1", new { })!,
                Descripcion: "self", Metodo: "GET"));

            return datosHETEOAS;
        }
    }
}
