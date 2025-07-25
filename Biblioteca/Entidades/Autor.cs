﻿using Biblioteca.Validaciones;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="el campo nombre es requerido!")]
        [StringLength(150,ErrorMessage ="el campo {0} debe tener {1} caracteres o menos")]
        [PrimeraLetraMatuscula]
        public required string Nombres { get; set; }
        [Required(ErrorMessage = "el campo apellido es requerido!")]
        [StringLength(20, ErrorMessage = "el campo {0} debe tener {1} caracteres o menos")]
        public required string Apellidos { get; set; }
        [Unicode(false)]
        public string? Foto { get; set; }
        public List<AutorLibro> Libros { get; set; } = [];
    }
}
