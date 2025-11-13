using Singula.Core.Core.DTOs.RegistroKPI;

namespace Singula.Core.Core.Interfaces.Services;

/// <summary>
/// Servicio para procesar archivos Excel
/// </summary>
public interface IExcelProcessorService
{
    Task<List<CreateRegistroKPIDTO>> LeerArchivoExcelAsync(string rutaArchivo);
    Task<bool> ValidarFormatoAsync(string rutaArchivo);
    Task<byte[]> ExportarAExcelAsync<T>(List<T> data, string nombreHoja = "Datos");
}
