
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Data.Common;

namespace Biblioteca.Servicios
{
    public class AlmacenadorArchivosAzure //: IAlmacenadorArchivos
    {
        //private readonly string connectionString;
        //public AlmacenadorArchivosAzure(IConfiguration configuration)
        //{
        //    connectionString = configuration.GetConnectionString("AzureStorageConnection")!;
        //}
        //public async Task<string> almacenar(string contenedor, IFormFile archivo)
        //{
        //    var cliente = new BlobContainerClient(connectionString, contenedor);
        //    await cliente.CreateIfNotExistsAsync();
        //    cliente.SetAccessPolicy(PublicAccessType.Blob);

        //    var extension=Path.GetExtension(archivo.FileName);
        //    var nombreArchivo = $"{Guid.NewGuid()}{extension}";
        //    var blob = cliente.GetBlobClient(nombreArchivo);
        //    var blobHttpHaeders = new BlobHttpHeaders();
        //    blobHttpHaeders.ContentType= archivo.ContentType;
        //    await blob.UploadAsync(archivo.OpenReadStream(), blobHttpHaeders);
        //    return blob.Uri.ToString();
        //}

        //public async Task Borrar(string? ruta, string contenedor)
        //{
        //    if (string.IsNullOrEmpty(ruta))
        //    {
        //        return;
        //    }

        //    var cliente = new BlobContainerClient(connectionString, contenedor);
        //    await cliente.CreateIfNotExistsAsync();
        //    var nombreArchivo = Path.GetFileName(ruta);
        //    var blob = cliente.GetBlobClient(nombreArchivo);
        //    await blob.DeleteIfExistsAsync();

        //}
    }
}
