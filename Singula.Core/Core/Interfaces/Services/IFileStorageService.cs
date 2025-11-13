namespace Singula.Core.Core.Interfaces.Services;

/// <summary>
/// Servicio para almacenamiento de archivos
/// </summary>
public interface IFileStorageService
{
    Task<string> GuardarArchivoAsync(Stream stream, string nombreArchivo, string carpeta = "uploads");
    Task<byte[]> ObtenerArchivoAsync(string rutaArchivo);
    Task<bool> EliminarArchivoAsync(string rutaArchivo);
    Task<bool> ExisteArchivoAsync(string rutaArchivo);
    Task<long> ObtenerTamanoArchivoAsync(string rutaArchivo);
}
