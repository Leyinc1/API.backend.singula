using ClosedXML.Excel;
using Singula.Core.Core.Entities;
using Singula.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Singula.Core.Services
{
    public class ExcelImportService : IExcelImportService
    {
        private readonly IRepository<Solicitud> _solicitudRepo;
        private readonly IRepository<Area> _areaRepo;
        private readonly IRepository<RolRegistro> _rolRepo;
        private readonly IRepository<ConfigSla> _slaRepo;
        private readonly IRepository<EstadoSolicitudCatalogo> _estadoRepo;
        private readonly ILogger<ExcelImportService> _logger;

        // Mapeo de columnas del Excel a propiedades (case-insensitive, solo una variante por campo)
        private static Dictionary<string, string> GetColumnMapping()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // Area (reemplaza BloqueTech) - StringComparer.OrdinalIgnoreCase maneja mayúsculas/minúsculas automáticamente
                { "area", "Area" },
                { "área", "Area" },
                
                // TipoSolicitud
                { "tipo de solicitud", "TipoSolicitud" },
                { "tipo_solicitud", "TipoSolicitud" },
                { "tiposolicitud", "TipoSolicitud" },
                { "tipo solicitud", "TipoSolicitud" },
                { "tipo de sla", "TipoSolicitud" },
                { "tipo_sla", "TipoSolicitud" },
                { "tiposla", "TipoSolicitud" },
                
                // Prioridad
                { "prioridad", "Prioridad" },
                
                // FechaSolicitud
                { "fecha solicitud", "FechaSolicitud" },
                { "fecha_solicitud", "FechaSolicitud" },
                { "fechasolicitud", "FechaSolicitud" },
                
                // FechaIngreso
                { "fecha de ingreso", "FechaIngreso" },
                { "fecha_ingreso", "FechaIngreso" },
                { "fechaingreso", "FechaIngreso" },
                { "fecha ingreso", "FechaIngreso" },
                
                // NombrePersonal
                { "nombre personal", "NombrePersonal" },
                { "nombre_personal", "NombrePersonal" },
                { "nombrepersonal", "NombrePersonal" },
                
                // Observaciones
                { "observaciones", "Observaciones" }
            };
        }

        public ExcelImportService(
            IRepository<Solicitud> solicitudRepo,
            IRepository<Area> areaRepo,
            IRepository<RolRegistro> rolRepo,
            IRepository<ConfigSla> slaRepo,
            IRepository<EstadoSolicitudCatalogo> estadoRepo,
            ILogger<ExcelImportService> logger)
        {
            _solicitudRepo = solicitudRepo;
            _areaRepo = areaRepo;
            _rolRepo = rolRepo;
            _slaRepo = slaRepo;
            _estadoRepo = estadoRepo;
            _logger = logger;
        }

        public async Task<ExcelImportResult> ImportFromExcelAsync(string filePath)
        {
            var result = new ExcelImportResult();

            if (!File.Exists(filePath))
            {
                result.Success = false;
                result.Message = "El archivo no existe";
                return result;
            }

            try
            {
                // Cargar catálogos para lookup
                var areas = (await _areaRepo.GetAllAsync()).ToList();
                var roles = (await _rolRepo.GetAllAsync()).ToList();
                var slas = (await _slaRepo.GetAllAsync()).ToList();
                var estados = (await _estadoRepo.GetAllAsync()).ToList();

                // Estado por defecto: Pendiente (ID 1)
                var estadoPendiente = estados.FirstOrDefault(e => 
                    e.Descripcion?.ToLower().Contains("pendiente") == true) ?? estados.FirstOrDefault();

                using var workbook = new XLWorkbook(filePath);
                var worksheet = workbook.Worksheets.First();
                
                _logger.LogInformation("📄 Excel abierto, leyendo encabezados...");

                // Obtener la fila de encabezados
                var headerRow = worksheet.Row(1);
                var columnIndices = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                var columnMapping = GetColumnMapping();

                foreach (var cell in headerRow.CellsUsed())
                {
                    var headerValue = cell.GetString().Trim();
                    if (columnMapping.ContainsKey(headerValue))
                    {
                        var mappedName = columnMapping[headerValue];
                        
                        // Evitar duplicados - solo tomar la primera ocurrencia
                        if (!columnIndices.ContainsKey(mappedName))
                        {
                            columnIndices[mappedName] = cell.Address.ColumnNumber;
                            _logger.LogInformation($"✅ Columna encontrada: '{headerValue}' → {mappedName}");
                        }
                        else
                        {
                            _logger.LogWarning($"⚠️ Columna duplicada ignorada: '{headerValue}' (ya mapeada como {mappedName})");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"⚠️ Columna no reconocida: '{headerValue}'");
                    }
                }

                // Validar columnas requeridas (FechaIngreso es opcional)
                var requiredColumns = new[] { "Area", "FechaSolicitud" };
                var missingColumns = requiredColumns.Where(c => !columnIndices.ContainsKey(c)).ToList();
                if (missingColumns.Any())
                {
                    result.Success = false;
                    result.Message = $"Columnas requeridas no encontradas: {string.Join(", ", missingColumns)}";
                    _logger.LogError($"❌ {result.Message}");
                    return result;
                }
                
                _logger.LogInformation($"✅ Todas las columnas requeridas encontradas");

                // Procesar filas de datos
                var dataRows = worksheet.RowsUsed().Skip(1); // Skip header
                result.TotalRows = dataRows.Count();
                
                _logger.LogInformation($"📊 Total de filas a procesar: {result.TotalRows}");

                foreach (var row in dataRows)
                {
                    try
                    {
                        var rowData = ExtractRowData(row, columnIndices);
                        
                        // Validar campos OBLIGATORIOS
                        if (string.IsNullOrWhiteSpace(rowData.Area))
                        {
                            result.FailedRows++;
                            result.Errors.Add($"Fila {row.RowNumber()}: AREA está vacía (campo obligatorio)");
                            _logger.LogWarning($"⚠️ Fila {row.RowNumber()}: AREA vacía (campo obligatorio)");
                            continue;
                        }

                        if (!rowData.FechaSolicitud.HasValue)
                        {
                            result.FailedRows++;
                            result.Errors.Add($"Fila {row.RowNumber()}: Fecha Solicitud está vacía (campo obligatorio)");
                            _logger.LogWarning($"⚠️ Fila {row.RowNumber()}: Fecha Solicitud vacía (campo obligatorio)");
                            continue;
                        }

                        // Buscar o usar valores por defecto para las FK
                        var area = FindOrCreateArea(areas, rowData.Area);
                        var sla = FindSlaBySolicitud(slas, rowData.TipoSolicitud);

                        // Normalizar prioridad a códigos válidos
                        var prioridadNormalizada = NormalizarPrioridad(rowData.Prioridad);

                        // Calcular días SLA
                        var diasSla = 0;
                        if (rowData.FechaSolicitud.HasValue && rowData.FechaIngreso.HasValue)
                        {
                            diasSla = (rowData.FechaIngreso.Value - rowData.FechaSolicitud.Value).Days;
                        }

                        // Crear la solicitud
                        var solicitud = new Solicitud
                        {
                            IdPersonal = 1, // Personal genérico
                            IdRolRegistro = 1, // RolRegistro por defecto
                            IdSla = sla?.IdSla ?? 1,
                            IdArea = area?.IdArea ?? 1,
                            IdEstadoSolicitud = estadoPendiente?.IdEstadoSolicitud ?? 1,
                            FechaSolicitud = rowData.FechaSolicitud,
                            FechaIngreso = rowData.FechaIngreso,
                            NumDiasSla = diasSla,
                            ResumenSla = $"{rowData.TipoSolicitud} - {rowData.Area}",
                            OrigenDato = "excel",
                            Prioridad = prioridadNormalizada,
                            CreadoPor = null,
                            CreadoEn = DateTime.UtcNow
                        };

                        _logger.LogInformation($"💾 Insertando fila {row.RowNumber()}: {rowData.Area} - {rowData.FechaSolicitud}");
                        await _solicitudRepo.CreateAsync(solicitud);
                        result.ImportedRows++;
                        _logger.LogInformation($"✅ Fila {row.RowNumber()} insertada exitosamente");
                    }
                    catch (Exception ex)
                    {
                        result.FailedRows++;
                        result.Errors.Add($"Fila {row.RowNumber()}: {ex.Message}");
                        _logger.LogError(ex, $"❌ Error en fila {row.RowNumber()}: {ex.Message}");
                    }
                }

                result.Success = result.ImportedRows > 0;
                result.Message = $"Importación completada: {result.ImportedRows} de {result.TotalRows} registros importados";

                if (result.FailedRows > 0)
                {
                    result.Message += $" ({result.FailedRows} errores)";
                }
                
                _logger.LogInformation($"📊 Resultado: {result.Message}");

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error al procesar el archivo: {ex.Message}";
                result.Errors.Add(ex.ToString());
                return result;
            }
        }

        private ExcelRowData ExtractRowData(IXLRow row, Dictionary<string, int> columnIndices)
        {
            var data = new ExcelRowData();

            if (columnIndices.TryGetValue("Area", out int areaCol))
                data.Area = row.Cell(areaCol).GetString()?.Trim();

            if (columnIndices.TryGetValue("TipoSolicitud", out int tipoCol))
                data.TipoSolicitud = row.Cell(tipoCol).GetString()?.Trim();

            if (columnIndices.TryGetValue("Prioridad", out int prioridadCol))
                data.Prioridad = row.Cell(prioridadCol).GetString()?.Trim();

            if (columnIndices.TryGetValue("FechaSolicitud", out int fechaSolCol))
                data.FechaSolicitud = ParseDate(row.Cell(fechaSolCol));

            if (columnIndices.TryGetValue("FechaIngreso", out int fechaIngCol))
                data.FechaIngreso = ParseDate(row.Cell(fechaIngCol));

            if (columnIndices.TryGetValue("NombrePersonal", out int nombreCol))
                data.NombrePersonal = row.Cell(nombreCol).GetString()?.Trim();

            if (columnIndices.TryGetValue("Observaciones", out int obsCol))
                data.Observaciones = row.Cell(obsCol).GetString()?.Trim();

            return data;
        }

        private DateTime? ParseDate(IXLCell cell)
        {
            if (cell.IsEmpty()) return null;

            DateTime parsedDate;

            // Intentar obtener como DateTime directamente (formato Excel)
            if (cell.DataType == XLDataType.DateTime)
            {
                parsedDate = cell.GetDateTime();
                return DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
            }

            // Intentar parsear como string
            var dateStr = cell.GetString()?.Trim();
            if (string.IsNullOrEmpty(dateStr)) return null;

            // Formatos a intentar
            var formats = new[] 
            { 
                "dd/MM/yyyy", 
                "yyyy-MM-dd", 
                "MM/dd/yyyy",
                "d/M/yyyy",
                "dd-MM-yyyy"
            };

            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(dateStr, format, CultureInfo.InvariantCulture, 
                    DateTimeStyles.None, out parsedDate))
                {
                    return DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
                }
            }

            // Intentar parse general
            if (DateTime.TryParse(dateStr, out parsedDate))
            {
                return DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
            }

            return null;
        }

        private Area? FindOrCreateArea(List<Area> areas, string areaName)
        {
            if (string.IsNullOrWhiteSpace(areaName)) return areas.FirstOrDefault();

            return areas.FirstOrDefault(a => 
                a.NombreArea?.Equals(areaName, StringComparison.OrdinalIgnoreCase) == true)
                ?? areas.FirstOrDefault();
        }

        private string NormalizarPrioridad(string? prioridad)
        {
            if (string.IsNullOrWhiteSpace(prioridad))
                return "MEDIA";

            var prioridadLower = prioridad.ToLower();

            if (prioridadLower.Contains("crítica") || prioridadLower.Contains("critica"))
                return "CRITICA";
            
            if (prioridadLower.Contains("alta"))
                return "ALTA";
            
            if (prioridadLower.Contains("media"))
                return "MEDIA";
            
            if (prioridadLower.Contains("baja"))
                return "BAJA";

            // Si no coincide con ninguno, usar MEDIA por defecto
            return "MEDIA";
        }

        private ConfigSla? FindSlaBySolicitud(List<ConfigSla> slas, string? tipoSolicitud)
        {
            if (string.IsNullOrWhiteSpace(tipoSolicitud)) return slas.FirstOrDefault();

            var tipoLower = tipoSolicitud.ToLower();

            if (tipoLower.Contains("nuevo"))
            {
                return slas.FirstOrDefault(s => 
                    s.CodigoSla?.ToLower().Contains("nuevo") == true) ?? slas.FirstOrDefault();
            }

            if (tipoLower.Contains("reemplazo"))
            {
                return slas.FirstOrDefault(s => 
                    s.CodigoSla?.ToLower().Contains("reemplazo") == true) ?? slas.FirstOrDefault();
            }

            return slas.FirstOrDefault();
        }

        private class ExcelRowData
        {
            public string? Area { get; set; }
            public string? TipoSolicitud { get; set; }
            public string? Prioridad { get; set; }
            public DateTime? FechaSolicitud { get; set; }
            public DateTime? FechaIngreso { get; set; }
            public string? NombrePersonal { get; set; }
            public string? Observaciones { get; set; }
        }
    }
}
