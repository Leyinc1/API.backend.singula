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
        public string? RutaArchivo { get; set; }
        public ICollection<int>? IdSolicituds { get; set; }
    }
}
