﻿using AutoMapper;
using Biblioteca.Datos;
using Biblioteca.DTOs;
using Biblioteca.Entidades;
using Biblioteca.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace Biblioteca.Controllers.V1
{
    [ApiController]
    [Route("api/v1/libros/{libroId:int}/comentarios")]
    [Authorize]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IServiciosUsuarios serviciosUsuarios;
        private readonly IOutputCacheStore outputCacheStore;
        private const string cache = "comentarios-obtener";

        public ComentariosController(ApplicationDbContext context,IMapper mapper,
            IServiciosUsuarios serviciosUsuarios,IOutputCacheStore outputCacheStore)
        {
            this.context = context;
            this.mapper = mapper;
            this.serviciosUsuarios = serviciosUsuarios;
            this.outputCacheStore = outputCacheStore;
        }

        [HttpGet(Name ="ObtenerComentariosV1")]
        [AllowAnonymous]
        [OutputCache(Tags = [cache])]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {
            var existeLibro = await context.Libros.AnyAsync(x=>x.Id==libroId);
            if (!existeLibro)
            {
                return NotFound();
            }
            var comentarios = await context.comentarios
                .Include(x=>x.Usuario)
                .Where(x=>x.LibroId==libroId)
                .OrderByDescending(libro=>libro.FechaPublicacion)
                .ToListAsync();
            return mapper.Map<List<ComentarioDTO>>(comentarios);
        }

        [HttpGet("{id}",Name ="ObtenerComentarioV1")]
        [AllowAnonymous]
        [OutputCache(Tags = [cache])]
        public async Task<ActionResult<ComentarioDTO>> Get(Guid id)
        {
            var comentario= await context.comentarios
                .Include(x=>x.Usuario)
                .FirstOrDefaultAsync(x=>x.Id==id);
            if (comentario is null)
            {
                return NotFound();
            }

            return mapper.Map<ComentarioDTO>(comentario);
        }

        [HttpPost(Name ="CrearComentarioV1")]
        public async Task<ActionResult> Post(int libroId,ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);
            if (!existeLibro)
            {
                return NotFound();
            }
            var usuario = await serviciosUsuarios.ObtenerUsuario();
            if (usuario is null)
            {
                return NotFound();
            }

            var comentario= mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.LibroId = libroId;
            comentario.FechaPublicacion = DateTime.UtcNow;
            comentario.UsuarioId=usuario.Id;
            context.Add(comentario);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cache,default);
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return CreatedAtRoute("ObtenerComentarioV1", new { id = comentario.Id, libroId }, comentarioDTO);
        }

        [HttpPatch("{id}",Name ="PatchComentarioV1")]
        public async Task<ActionResult> Patch(Guid id,int libroId ,JsonPatchDocument<ComentarioPatchDTO> patchDoc)
        {
            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);
            if (!existeLibro)
            {
                return NotFound();
            }
            var usuario = await serviciosUsuarios.ObtenerUsuario();
            if (usuario is null)
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

            if (comentarioDB.UsuarioId!=usuario.Id)
            {
                return Forbid();
            }


            var comentarioPatchDTO = mapper.Map<ComentarioPatchDTO>(comentarioDB);

            patchDoc.ApplyTo(comentarioPatchDTO, ModelState);

            var esValido = TryValidateModel(comentarioPatchDTO);

            if (!esValido)
            {
                return ValidationProblem();
            }
            mapper.Map(comentarioPatchDTO, comentarioDB);
            await outputCacheStore.EvictByTagAsync(cache, default);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}",Name ="BorrarComentarioV1")]
        public async Task<ActionResult> Delete(int libroId, Guid id)
        {
            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);
            if (!existeLibro)
            {
                return NotFound();
            }

            var usuario = await serviciosUsuarios.ObtenerUsuario();
            if (usuario is null)
            {
                return NotFound();
            }


            var comentarioDB= await context.comentarios.FirstOrDefaultAsync(x => x.Id == id);

            if (comentarioDB is null)
            {
                return NotFound();
            }
            if (comentarioDB.UsuarioId!=usuario.Id)
            {
                return Forbid();
            }
            comentarioDB.EsBorrado = true;
            context.Update(comentarioDB);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cache, default);
            return NoContent();
        }



    }
}
