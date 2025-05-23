﻿using Biblioteca.Entidades;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Datos
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Comentario>().HasQueryFilter(comentario => !comentario.EsBorrado);
        }

        public DbSet<Autor> Autores { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Comentario> comentarios { get; set; }
        public DbSet<AutorLibro> AutoresLibros { get; set; }

        public DbSet<Error> Errores { get; set; }

    }
}
