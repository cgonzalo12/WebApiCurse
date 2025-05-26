using AutoMapper;
using Biblioteca.Datos;
using Biblioteca.DTOs;
using Biblioteca.Entidades;
using Biblioteca.Servicios;
using Biblioteca.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.Linq.Dynamic.Core;

namespace Biblioteca.Controllers.V1
{
    [ApiController]
    [Route("api/v1/autores")]
    [Authorize(Policy = "esadmin")]
    [FiltroAgregarCabeceras("controladores","autores")]
    public class AutoresController :ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly ILogger<AutoresController> logger;
        private readonly IOutputCacheStore outputCacheStore;
        private const string contenedor = "autores";
        private const string cache = "autores-obtener";

        public AutoresController(ApplicationDbContext context,IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos,ILogger<AutoresController> logger,
            IOutputCacheStore outputCacheStore)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
            this.logger = logger;
            this.outputCacheStore = outputCacheStore;
        }
        [HttpGet(Name ="ObtenerAutoresV1")]
        [HttpGet("/api/v1/listado-de-autores")]
        [AllowAnonymous]
        //[OutputCache(Tags = [cache])]
        [ServiceFilter<MiFiltroDeAccion>()]
        [FiltroAgregarCabeceras("accion", "obtener-autores")]

        public async Task<IEnumerable<AutorDTO>> Get([FromQuery]PaginacionDTO paginacionDTO)
        {
            var queryable = context.Autores.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionCabecera(queryable);
            var autores=await queryable.OrderBy(x=>x.Nombres).Paginar(paginacionDTO).ToListAsync();
            var autoresDTO = mapper.Map<IEnumerable<AutorDTO>>(autores);
            return autoresDTO;
        }



        [HttpPost(Name ="CrearAutorV1")]
        public async Task<ActionResult> Post( AutorCreacionDTO autorCreacionDTO)
        {
            var autor = mapper.Map<Autor>(autorCreacionDTO);
            context.Add(autor);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cache, default);
            var autorDTO = mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("ObtenerAutorV1", new { id = autor.Id }, autorDTO);
        }



        [HttpPost("con-foto",Name ="CrearAutorConFotoV1")]
        public async Task<ActionResult> PostConFoto([FromForm]AutorCreacionDTOConFoto autorCreacionDTOConFoto)
        {
            var autor = mapper.Map<Autor>(autorCreacionDTOConFoto);
            if (autorCreacionDTOConFoto.Foto is not null)
            {
                var url = await almacenadorArchivos.almacenar(contenedor, autorCreacionDTOConFoto.Foto);
                autor.Foto = url;
            }
            context.Add(autor);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cache, default);//actualiza cache
            var autorDTO = mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("ObtenerAutorV1", new { id = autor.Id }, autorDTO);
        }




        [HttpGet("{id:int}",Name ="ObtenerAutorV1")]//api/autores/id?incluirLibros=True|false
        [AllowAnonymous]
        [EndpointSummary("Obtiene autor por id")]
        [EndpointDescription("Obtiene un autor por id. Incluye sus libros. Si el autor no existe retorna 404.")]
        [ProducesResponseType<AutorConLibrosDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[OutputCache(Tags = [cache])]
        public async Task<ActionResult<AutorConLibrosDTO>> Get([Description("El Id del autor")]int id,
            [FromQuery] bool incluirHeaters=false)
        {
            var autor= await context.Autores
                .Include(x=>x.Libros)
                    .ThenInclude(x=>x.Libro)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (autor is null)
            {
                return NotFound();
            }
            var autorDTO = mapper.Map<AutorConLibrosDTO>(autor);
            if (incluirHeaters)
            {
                GenerarEnlaces(autorDTO);
            }
            
            return autorDTO;
        }

        private void GenerarEnlaces(AutorDTO autorDTO)
        {
            autorDTO.Enlaces.Add(
                new DatosHATEOASDTO(Enlace: Url.Link("ObtenerAutorV1", new {id=autorDTO.Id})!,
                Descripcion:"self",
                Metodo:"GET"));

            autorDTO.Enlaces.Add(
                new DatosHATEOASDTO(Enlace: Url.Link("ActualizarAutorV1", new { id = autorDTO.Id })!,
                Descripcion: "autor-actualizar",
                Metodo: "PUT"));

            autorDTO.Enlaces.Add(
                new DatosHATEOASDTO(Enlace: Url.Link("PatchAutorV1", new { id = autorDTO.Id })!,
                Descripcion: "autor-patch",
                Metodo: "PATCH"));

            autorDTO.Enlaces.Add(
                new DatosHATEOASDTO(Enlace: Url.Link("BorrarAutorV1", new { id = autorDTO.Id })!,
                Descripcion: "autor-borrar",
                Metodo: "DELETE"));

        }


        [HttpGet("filtrar",Name ="FiltrarAutoresV1")]
        [AllowAnonymous]
        public async Task<ActionResult> Filtrar([FromQuery] AutorFiltroDTO autorFiltroDTO)
        {
            var queryable= context.Autores.AsQueryable();

            if (!string.IsNullOrEmpty(autorFiltroDTO.Nombres))
            {
                queryable = queryable.Where(autor => autor.Nombres.Contains(autorFiltroDTO.Nombres));
            }

            if (!string.IsNullOrEmpty(autorFiltroDTO.Apellidos))
            {
                queryable = queryable.Where(autor => autor.Apellidos.Contains(autorFiltroDTO.Apellidos));
            }
            if (autorFiltroDTO.IncluirLibros)
            {
                queryable = queryable.Include(x => x.Libros).ThenInclude(x => x.Libro);
            }
            if (autorFiltroDTO.TieneFoto.HasValue)
            {
                if (autorFiltroDTO.TieneFoto.Value)
                {
                    queryable = queryable.Where(x => x.Foto != null);
                }
                else
                {
                    queryable= queryable.Where(x => x.Foto == null);
                }
            }
            if (autorFiltroDTO.TieneLibros.HasValue)
            {
                if (autorFiltroDTO.TieneLibros.Value)
                {
                    queryable = queryable.Where(x => x.Libros.Any());
                }
                else
                {
                    queryable = queryable.Where(x => !x.Libros.Any());
                }
            }
            if (!string.IsNullOrEmpty(autorFiltroDTO.TituloLibro))
            {
                queryable = queryable
                    .Where(x => x.Libros
                    .Any(x => x.Libro!.Titulo
                    .Contains(autorFiltroDTO.TituloLibro)));
            }
            if (!string.IsNullOrEmpty(autorFiltroDTO.CamposOrdenar))
            {
                var tipoOrden = autorFiltroDTO.OrdenAscendete ? "ascending" : "descending";
                try
                {
                    queryable = queryable.OrderBy($"{autorFiltroDTO.CamposOrdenar} {tipoOrden}");
                }
                catch (Exception ex)
                {
                    queryable= queryable.OrderBy(x => x.Nombres);
                    logger.LogError(ex.Message, ex);
                    return BadRequest($"No se puede ordenar por el campo {autorFiltroDTO.CamposOrdenar}");
                }
            }
            else
            {
                queryable = queryable.OrderBy(x => x.Nombres);
            }


            var autores= await queryable
                .Paginar(autorFiltroDTO.PaginacionDTO)
                .ToListAsync();
            if (autorFiltroDTO.IncluirLibros)
            {
                var autoresDTO = mapper.Map<IEnumerable<AutorConLibrosDTO>>(autores);
                return Ok(autoresDTO);
            }
            else
            {
                var autoresDTO = mapper.Map<IEnumerable<AutorDTO>>(autores);
                return Ok(autoresDTO);
            }
            
        }

        [HttpPut("{id:int}",Name ="ActuañlizarAutorV1")]
        public async Task<ActionResult> Put(int id, [FromForm] AutorCreacionDTOConFoto autorCreacionDTO)
        {
            var existeAutor = await context.Autores.AnyAsync(x => x.Id == id);
            if (!existeAutor)
            {
                return NotFound();
            }
            var autor = mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;
            if (autorCreacionDTO.Foto is not null)
            {
                var fotoActual= await context.Autores
                    .Where(x=>x.Id==id)
                    .Select(x => x.Foto)
                    .FirstOrDefaultAsync();

                var url=await almacenadorArchivos.Editar(fotoActual,contenedor,autorCreacionDTO.Foto);
                autor.Foto= url;
            }
            context.Update(autor);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cache, default);//actualiza cache
            return NoContent();


        }


        [HttpPatch("{id:int}",Name ="PatchAutorV1")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<AutorPatchDTO> patchDoc)
        {
            if (patchDoc is null)
            {
                return BadRequest();
            }
            var autorDB = await context.Autores.FirstOrDefaultAsync(autor => autor.Id == id);
            if (autorDB is null)
            {
                return NotFound();
            }
            var autorPatchDto = mapper.Map<AutorPatchDTO>(autorDB);

            patchDoc.ApplyTo(autorPatchDto,ModelState);

            var esValido=TryValidateModel(autorPatchDto);

            if (!esValido)
            {
                return ValidationProblem();
            }
            mapper.Map(autorPatchDto, autorDB);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cache, default);//actualiza cache
            return NoContent();
        }

        [HttpDelete("{id:int}",Name ="BorrarAutorV1")]
        public async Task<ActionResult> Delete(int id)
        {
            var autor= await context.Autores.FirstOrDefaultAsync(x => x.Id == id);
            if (autor is null)
            {
                return NotFound();
            }
            context.Remove(autor);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cache, default);//actualiza cache
            await almacenadorArchivos.Borrar(autor.Foto, contenedor);
            return NoContent();
        }




    }
}
