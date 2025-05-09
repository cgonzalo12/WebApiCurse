using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Entidades
{
    public class Comentario
    {
        public Guid Id { get; set; }
        [Required]
        public required string Cuerpo { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public int LibroId { get; set; }

        public Libro? Libro { get; set; }
        public required string UsuarioId { get; set; }
        public bool EsBorrado { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
