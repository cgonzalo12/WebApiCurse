﻿using AutoMapper;
using Biblioteca.Datos;
using Biblioteca.DTOs;
using Biblioteca.Entidades;
using Biblioteca.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Controllers
{
    [ApiController]
    [Route("api/libros")]
    [Authorize(Policy = "esadmin")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IOutputCacheStore outputCacheStore;
        private const string cache= "libros-obtener";

        public LibrosController(ApplicationDbContext context,IMapper mapper,
            IOutputCacheStore outputCacheStore)
        {
            this.context = context;
            this.mapper = mapper;
            this.outputCacheStore = outputCacheStore;
        }

        [HttpGet]
        [AllowAnonymous]
        [OutputCache(Tags = [cache])]
        public async Task<IEnumerable<LibroDTO>> Get([FromQuery]PaginacionDTO paginacionDTO)
        { 
            var queryable = context.Libros.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionCabecera(queryable);
            var libros= await queryable
                .OrderBy(x=>x.Titulo)
                .Paginar(paginacionDTO)
                .ToListAsync();
            var librosDTO = mapper.Map<IEnumerable<LibroDTO>>(libros);
            return librosDTO;
        }
        
        [HttpGet("{id:int}",Name ="ObtenerLibro")]
        [AllowAnonymous]
        [OutputCache(Tags = [cache])]
        public async Task<ActionResult<LibroConAutoresDTO>> Get(int id)
        {
            var libro = await context.Libros
                .Include(x=>x.Autores)
                    .ThenInclude(x=>x.Autor)
                .FirstOrDefaultAsync(x=>x.Id==id);
            if (libro == null)
            {
                return NotFound();
            }
            var libroDTO = mapper.Map<LibroConAutoresDTO>(libro);
            return Ok(libroDTO);
        }
        [HttpPost]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            if (libroCreacionDTO.AutoresIds is null || libroCreacionDTO.AutoresIds.Count==0)
            {
                ModelState.AddModelError(nameof(libroCreacionDTO.AutoresIds), "No se puede crear un libro sin autores");
                return ValidationProblem();
            }
            var autoresIdsExisten=await context.Autores
                .Where(x=>libroCreacionDTO.AutoresIds.Contains(x.Id))
                .Select(x=>x.Id).ToListAsync();
            if (autoresIdsExisten.Count != libroCreacionDTO.AutoresIds.Count)
            {
                var autoresNoExisten=libroCreacionDTO.AutoresIds.Except(autoresIdsExisten);
                var autoresnoexistenString = string.Join(",", autoresNoExisten);
                var mensajeDeError = $"Los siguientes autores no existen:{autoresnoexistenString}";
                ModelState.AddModelError(nameof(libroCreacionDTO.AutoresIds),mensajeDeError);
                return ValidationProblem();
            }
            var libro = mapper.Map<Libro>(libroCreacionDTO);
            AsignarOrdenAutores(libro);
            context.Add(libro);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cache, default);
            var libroDTO = mapper.Map<LibroDTO>(libro);
            return CreatedAtRoute("ObtenerLibro", new { id = libro.Id }, libroDTO);
        }
        


        [HttpPut("{id:int}")]

        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            if (libroCreacionDTO.AutoresIds is null || libroCreacionDTO.AutoresIds.Count == 0)
            {
                ModelState.AddModelError(nameof(libroCreacionDTO.AutoresIds), "No se puede crear un libro sin autores");
                return ValidationProblem();
            }
            var autoresIdsExisten = await context.Autores
                .Where(x => libroCreacionDTO.AutoresIds.Contains(x.Id))
                .Select(x => x.Id).ToListAsync();
            if (autoresIdsExisten.Count != libroCreacionDTO.AutoresIds.Count)
            {
                var autoresNoExisten = libroCreacionDTO.AutoresIds.Except(autoresIdsExisten);
                var autoresnoexistenString = string.Join(",", autoresNoExisten);
                var mensajeDeError = $"Los siguientes autores no existen:{autoresnoexistenString}";
                ModelState.AddModelError(nameof(libroCreacionDTO.AutoresIds), mensajeDeError);
                return ValidationProblem();
            }

            var libroDB=await context.Libros
                .Include(libro=>libro.Autores)
                .FirstOrDefaultAsync(libro=>libro.Id==id);

            if (libroDB is null)
            {
                return NotFound();
            }
            libroDB = mapper.Map(libroCreacionDTO, libroDB);


            AsignarOrdenAutores(libroDB);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cache, default);
            return NoContent();


        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var registrosBorrados=await context.Libros.Where(context=>context.Id==id).ExecuteDeleteAsync();
            if (registrosBorrados==0)
            {
                return NotFound();
            }
            await outputCacheStore.EvictByTagAsync(cache, default);
            return NoContent();
        }

        private void AsignarOrdenAutores(Libro libro)
        {
            if (libro.Autores is not null)
            {
                for (int i = 0; i < libro.Autores.Count; i++)
                {
                    libro.Autores[i].Orden = i;
                }
            }
        }
    }
}
