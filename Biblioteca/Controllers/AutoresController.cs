using AutoMapper;
using Biblioteca.Datos;
using Biblioteca.DTOs;
using Biblioteca.Entidades;
using Biblioteca.Servicios;
using Biblioteca.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;

namespace Biblioteca.Controllers
{
    [ApiController]
    [Route("api/autores")]
    [Authorize(Policy = "esadmin")]
    public class AutoresController :ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private string contenedor = "autores";

        public AutoresController(ApplicationDbContext context,IMapper mapper,IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }
        [HttpGet]
        [HttpGet("/listado-de-autores")]
        [AllowAnonymous]
        public async Task<IEnumerable<AutorDTO>> Get([FromQuery]PaginacionDTO paginacionDTO)
        {
            var queryable = context.Autores.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionCabecera(queryable);
            var autores=await queryable.OrderBy(x=>x.Nombres).Paginar(paginacionDTO).ToListAsync();
            var autoresDTO = mapper.Map<IEnumerable<AutorDTO>>(autores);
            return autoresDTO;
        }



        [HttpPost]
        public async Task<ActionResult> Post( AutorCreacionDTO autorCreacionDTO)
        {
            var autor = mapper.Map<Autor>(autorCreacionDTO);
            context.Add(autor);
            await context.SaveChangesAsync();
            var autorDTO = mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("ObtenerAutor", new { id = autor.Id }, autorDTO);
        }



        [HttpPost("con-foto")]
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
            var autorDTO = mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("ObtenerAutor", new { id = autor.Id }, autorDTO);
        }




        [HttpGet("{id:int}",Name ="ObtenerAutor")]//api/autores/id?incluirLibros=True|false
        [AllowAnonymous]
        [EndpointSummary("Obtiene autor por id")]
        [EndpointDescription("Obtiene un autor por id. Incluye sus libros. Si el autor no existe retorna 404.")]
        [ProducesResponseType<AutorConLibrosDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AutorConLibrosDTO>> Get([Description("El Id del autor")]int id)
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
            return autorDTO;
        }
        [HttpPut("{id:int}")]
        
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
            return NoContent();


        }


        [HttpPatch("{id:int}")]
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

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var autor= await context.Autores.FirstOrDefaultAsync(x => x.Id == id);
            if (autor is null)
            {
                return NotFound();
            }
            context.Remove(autor);
            await context.SaveChangesAsync();
            await almacenadorArchivos.Borrar(autor.Foto, contenedor);
            return NoContent();
        }



    }
}
