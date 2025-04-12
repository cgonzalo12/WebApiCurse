using AutoMapper;
using Biblioteca.Datos;
using Biblioteca.DTOs;
using Biblioteca.Entidades;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace Biblioteca.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ComentariosController(ApplicationDbContext context,IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {
            var existeLibro = await context.Libros.AnyAsync(x=>x.Id==libroId);
            if (!existeLibro)
            {
                return NotFound();
            }
            var comentarios = await context.comentarios
                .Where(x=>x.LibroId==libroId)
                .OrderByDescending(libro=>libro.FechaPublicacion)
                .ToListAsync();
            return mapper.Map<List<ComentarioDTO>>(comentarios);
        }

        [HttpGet("{id}",Name ="ObtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> Get(Guid id)
        {
            var comentario= await context.comentarios.FirstOrDefaultAsync(x=>x.Id==id);
            if (comentario is null)
            {
                return NotFound();
            }

            return mapper.Map<ComentarioDTO>(comentario);
        }

        [HttpPost]
        public async Task<ActionResult> Post(int libroId,ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);
            if (!existeLibro)
            {
                return NotFound();
            }

            var comentario= mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.LibroId = libroId;
            comentario.FechaPublicacion = DateTime.UtcNow;
            context.Add(comentario);
            await context.SaveChangesAsync();
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return CreatedAtRoute("ObtenerComentario", new { id = comentario.Id, libroId }, comentarioDTO);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(Guid id,int libroId ,JsonPatchDocument<ComentarioPatchDTO> patchDoc)
        {
            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);
            if (!existeLibro)
            {
                return NotFound();
            }

            if (patchDoc is null)
            {
                return BadRequest();
            }
            var comentarioDB = await context.comentarios.FirstOrDefaultAsync(autor => autor.Id == id);
            if (comentarioDB is null)
            {
                return NotFound();
            }
            var comentarioPatchDTO = mapper.Map<ComentarioPatchDTO>(comentarioDB);

            patchDoc.ApplyTo(comentarioPatchDTO, ModelState);

            var esValido = TryValidateModel(comentarioPatchDTO);

            if (!esValido)
            {
                return ValidationProblem();
            }
            mapper.Map(comentarioPatchDTO, comentarioDB);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int libroId, Guid id)
        {
            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);
            if (!existeLibro)
            {
                return NotFound();
            }

            var comentario = await context.comentarios.FirstOrDefaultAsync(x => x.Id == id && x.LibroId == libroId);
            if (comentario is null)
            {
                return NotFound();
            }

            context.comentarios.Remove(comentario);
            await context.SaveChangesAsync();

            return NoContent();
        }



    }
}
