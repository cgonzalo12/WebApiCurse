using Biblioteca.DTOs;

namespace Biblioteca.Servicios.V1
{
    public interface IGeneradorEnlaces
    {
        Task GenerarEnlaces(AutorDTO autorDTO);
    }
}