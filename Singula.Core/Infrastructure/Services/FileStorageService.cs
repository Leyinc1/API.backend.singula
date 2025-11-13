using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Singula.Core.Core.Interfaces.Services;

namespace Singula.Core.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly IHostEnvironment _env;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(IHostEnvironment env, ILogger<FileStorageService> logger)
    {
        _env = env;
        _logger = logger;
    }

    public async Task<string> GuardarArchivoAsync(Stream stream, string nombreArchivo, string carpeta = "uploads")
    {
        var webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var basePath = Path.Combine(webRoot, carpeta);
        if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);

        var ruta = Path.Combine(basePath, nombreArchivo);
        using var fs = new FileStream(ruta, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fs);
        return ruta;
    }

    public async Task<byte[]> ObtenerArchivoAsync(string rutaArchivo)
    {
        if (!File.Exists(rutaArchivo)) return Array.Empty<byte>();
        return await File.ReadAllBytesAsync(rutaArchivo);
    }

    public Task<bool> EliminarArchivoAsync(string rutaArchivo)
    {
        if (File.Exists(rutaArchivo))
        {
            File.Delete(rutaArchivo);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> ExisteArchivoAsync(string rutaArchivo)
    {
        return Task.FromResult(File.Exists(rutaArchivo));
    }

    public Task<long> ObtenerTamanoArchivoAsync(string rutaArchivo)
    {
        if (!File.Exists(rutaArchivo)) return Task.FromResult(0L);
        var info = new FileInfo(rutaArchivo);
        return Task.FromResult(info.Length);
    }
}
