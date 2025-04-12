using AutoMapper;
using Biblioteca.Datos;
using Biblioteca.DTOs;
using Biblioteca.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Controllers
{
    [ApiController]
    [Route("api/autores-coleccion")]
    public class AutoresCollecionController :ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AutoresCollecionController(ApplicationDbContext context,IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }



        [HttpPost]
        public async Task<ActionResult> Post(IEnumerable<AutorCreacionDTO> autoresCreacionDTO)
        {
            var autores = mapper.Map<IEnumerable<Autor>>(autoresCreacionDTO);
            context.AddRange(autores);
            await context.SaveChangesAsync();
            var autoresDTO = mapper.Map<IEnumerable<AutorDTO>>(autores);
            var ids= autores.Select(x => x.Id);
            var idsString = string.Join(",", ids);
            return CreatedAtRoute("ObtenerAutoresPorIds", new { ids = idsString }, autoresDTO);
        }

        [HttpGet("{ids}",Name ="ObtenerAutoresPorIds")] //api/autoresColeccion/1,2,3
        public async Task<ActionResult<List<AutorConLibrosDTO>>> Get(string ids)
        {
            var idsColeccion = new List<int>();
            foreach (var id in ids.Split(","))
            {
                if (int.TryParse(id, out int idInt))
                {
                    idsColeccion.Add(idInt);
                }
            }
            if (!idsColeccion.Any())
            {
                ModelState.AddModelError(nameof(ids), "Ningún Id válido fue encontrado");
                return ValidationProblem();
            }
            
            var autores = await context.Autores
                .Include(autor => autor.Libros)
                    .ThenInclude(autor => autor.Libro)
                .Where(x => idsColeccion.Contains(x.Id))
                .ToListAsync();

            if (autores.Count !=idsColeccion.Count)
            {
                return NotFound();
            }

            var autoresDTO = mapper.Map<List<AutorConLibrosDTO>>(autores);
            return autoresDTO;


            
        }
    }
}
