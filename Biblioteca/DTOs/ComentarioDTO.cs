using System.ComponentModel.DataAnnotations;

namespace Biblioteca.DTOs
{
    public class ComentarioDTO
    {
        public Guid Id { get; set; }
        [Required]
        public required string Cuerpo { get; set; }
        public DateTime FechaPublicacion { get; set; }
    }
}
