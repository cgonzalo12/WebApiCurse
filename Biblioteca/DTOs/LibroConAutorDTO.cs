namespace Biblioteca.DTOs
{
    public class LibroConAutoresDTO  :LibroDTO
    {
        public List<AutorDTO> Autores { get; set; } = [];
    }
}
