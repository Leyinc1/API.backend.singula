using Singula.Core.Core.Entities;

namespace Singula.Core.Core.DTOs.ArchivoExcel;

public class ArchivoExcelDTO : BaseDTO
{
    public string NombreArchivo { get; set; } = string.Empty;
    public string RutaArchivo { get; set; } = string.Empty;
    public long TamanoBytes { get; set; }
    public DateTime FechaCarga { get; set; }
    public string UsuarioCarga { get; set; } = string.Empty;
    public EstadoProcesamiento Estado { get; set; }
    public string? MensajeError { get; set; }
    public int CantidadRegistros { get; set; }
}

public class CreateArchivoExcelDTO
{
    public string NombreArchivo { get; set; } = string.Empty;
    public long TamanoBytes { get; set; }
    public string UsuarioCarga { get; set; } = string.Empty;
}

public class UpdateArchivoExcelDTO
{
    public EstadoProcesamiento Estado { get; set; }
    public string? MensajeError { get; set; }
}

// DTO para subir archivo en la capa de aplicación UI. 
// Core usa un DTO agnóstico: contenido binario + nombre.
public class ArchivoExcelUploadDTO
{
    public byte[] Contenido { get; set; } = Array.Empty<byte>();
    public string NombreArchivo { get; set; } = string.Empty;
    public string UsuarioCarga { get; set; } = string.Empty;
}
