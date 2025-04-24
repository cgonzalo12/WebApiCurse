namespace Biblioteca.Servicios
{
    public interface IAlmacenadorArchivos
    {
        Task Borrar(string? ruta, string contenedor);
        Task<string> almacenar(string contenerdor, IFormFile archivo);
        async Task<string> Editar(string? ruta, string contenedor,IFormFile archivo )
        {
            await Borrar(ruta, contenedor);
            return await almacenar(contenedor, archivo);
        }
    }
}
