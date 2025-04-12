using Biblioteca.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Datos
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Autor> Autores { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Comentario> comentarios { get; set; }
        public DbSet<AutorLibro> AutoresLibros { get; set; }

    }
}
