using Biblioteca.Datos;
using Biblioteca.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public LibrosController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Libro>> Get()
        {
            return await context.Libros.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Libro>> Get(int id)
        {
            var libro = await context.Libros
                .Include(x=>x.Autor)
                .FirstOrDefaultAsync(x=>x.Id==id);
            if (libro == null)
            {
                return NotFound();
            }
            return Ok(libro);
        }
        [HttpPost]
        public async Task<ActionResult> Post(Libro libro)
        {
            var existeAutor = await context.Autores.AnyAsync(x=>x.Id==libro.AutorId);
            if (!existeAutor)
            {
                ModelState.AddModelError(nameof(libro.AutorId), $"no existe el autor con ese Id: {libro.AutorId}");
                return ValidationProblem();
            }
            context.Add(libro);
            await context.SaveChangesAsync();
            return Ok();
        }



        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id,Libro libro)
        {
            if (id != libro.Id)
            {
                return BadRequest("no coinciden los ids");
            }
            var existeAutor = await context.Autores.AnyAsync(x=>x.Id==libro.AutorId);
            if (!existeAutor)
            {
                return BadRequest($"no existe el autor con ese Id {libro.AutorId}");
            }
            context.Update(libro);
            await context.SaveChangesAsync();
            return Ok();


        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var registrosBorrados=await context.Libros.Where(context=>context.Id==id).ExecuteDeleteAsync();
            if (registrosBorrados==0)
            {
                return NotFound();
            }
            return Ok();
        }


    }
}
