using Biblioteca.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="el campo nombre es requerido!")]
        [StringLength(150,ErrorMessage ="el campo {0} debe tener {1} caracteres o menos")]
        [PrimeraLetraMAtuscula]
        public required string Nombre { get; set; }

        public List<Libro> Libros { get; set; } = new List<Libro>();
    }
}
