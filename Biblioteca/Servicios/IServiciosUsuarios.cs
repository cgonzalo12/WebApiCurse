using Biblioteca.Entidades;
using Microsoft.AspNetCore.Identity;

namespace Biblioteca.Servicios
{
    public interface IServiciosUsuarios
    {
        Task<Usuario?> ObtenerUsuario();
    }
}