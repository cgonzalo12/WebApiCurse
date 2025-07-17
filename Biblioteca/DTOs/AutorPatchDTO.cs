using Biblioteca.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.DTOs
{
    public class AutorPatchDTO
    {
        [Required(ErrorMessage = "el campo nombre es requerido!")]
        [StringLength(150, ErrorMessage = "el campo {0} debe tener {1} caracteres o menos")]
        [PrimeraLetraMatuscula]
        public required string Nombres { get; set; }
        [Required(ErrorMessage = "el campo apellido es requerido!")]
        [StringLength(20, ErrorMessage = "el campo {0} debe tener {1} caracteres o menos")]
        public required string Apellidos { get; set; }
    }
}
