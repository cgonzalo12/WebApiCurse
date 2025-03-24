using Biblioteca.Datos;
using Biblioteca.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController :ControllerBase
    {
        private readonly ApplicationDbContext context;

        public AutoresController(ApplicationDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        [HttpGet("/listado-de-autores")]
        public async Task<IEnumerable<Autor>> Get()
        {
            return await context.Autores.ToListAsync();
        }



        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Autor autor)
        {
            context.Add(autor);
            await context.SaveChangesAsync();
            return Ok();
        }



        
        [HttpGet("{id:int}")]//api/autores/id?incluirLibros=True|false
        public async Task<ActionResult<Autor>> Get([FromRoute]int id)
        {
            var autor= await context.Autores
                .Include(x=>x.Libros)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (autor is null)
            {
                return NotFound();
            }
            return Ok(autor);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id,Autor autor)
        {
            if (id != autor.Id)
            {
                return BadRequest("los ids deben cioncidir");
            }

            context.Update(autor);
            await context.SaveChangesAsync();
            return Ok();


        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var registrosBorrados = await context.Autores.Where(context=>context.Id == id).ExecuteDeleteAsync();
            if (registrosBorrados ==0)
            {
                return NotFound();
            }
            return Ok();
        }



    }
}
