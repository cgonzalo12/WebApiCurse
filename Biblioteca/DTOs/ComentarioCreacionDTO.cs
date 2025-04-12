using System.ComponentModel.DataAnnotations;

namespace Biblioteca.DTOs
{
    public class ComentarioCreacionDTO
    {
        [Required]
        public required string Cuerpo { get; set; }
        
    }
}
