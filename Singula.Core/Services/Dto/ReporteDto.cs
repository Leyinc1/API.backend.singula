using System;
using System.Collections.Generic;

namespace Singula.Core.Services.Dto
{
    public class ReporteDto
    {
        public int IdReporte { get; set; }
        public string? TipoReporte { get; set; }
        public string? Formato { get; set; }
        public string? FiltrosJson { get; set; }
        public int GeneradoPor { get; set; }
        public DateTime? FechaGeneracion { get; set; }
        public string? RutaArchivo { get; set; } // Nullable - ahora guarda nombre del archivo descargado
        public string? NombreArchivo { get; set; } // Nuevo: nombre del PDF descargado por el usuario
        public ICollection<int>? IdSolicituds { get; set; }
    }
}
