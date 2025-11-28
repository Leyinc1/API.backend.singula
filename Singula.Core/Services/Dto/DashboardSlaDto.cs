using System;
using System.Collections.Generic;

namespace Singula.Core.Services.Dto
{
    public class DashboardSlaDto
    {
        public int Id { get; set; }
        public string BloqueTech { get; set; } = string.Empty;
        public string TipoSolicitud { get; set; } = string.Empty;
        public string Prioridad { get; set; } = "Media";
        public DateTime? FechaSolicitud { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public int DiasTranscurridos { get; set; }
        public bool CumpleSla { get; set; } // Campo unificado para cumplimiento
        public bool CumpleSla1 { get; set; } // Mantener para compatibilidad
        public bool CumpleSla2 { get; set; } // Mantener para compatibilidad
        public string NombrePersonal { get; set; } = string.Empty;
        public int DiasUmbralSla { get; set; }
    }

    public class DashboardStatsDto
    {
        public int TotalSolicitudes { get; set; }
        // Estadísticas dinámicas por tipo de solicitud
        public Dictionary<string, SlaStats> EstadisticasPorTipo { get; set; } = new();
        
        // Mantener campos legacy para compatibilidad con frontend existente
        public double CumplimientoSla1 { get; set; }
        public double CumplimientoSla2 { get; set; }
        public double PromedioDiasSla1 { get; set; }
        public double PromedioDiasSla2 { get; set; }
    }

    public class SlaStats
    {
        public int TotalSolicitudes { get; set; }
        public int CumpleSla { get; set; }
        public double PorcentajeCumplimiento { get; set; }
        public double PromedioDias { get; set; }
        public int DiasUmbral { get; set; }
    }

    public class DashboardFiltersDto
    {
        public List<string> BloquesTech { get; set; } = new();
        public List<string> TiposSolicitud { get; set; } = new();
        public List<string> Prioridades { get; set; } = new();
        public List<ConfigSlaInfo> ConfiguracionesSla { get; set; } = new();
    }

    public class ConfigSlaInfo
    {
        public int Id { get; set; }
        public string CodigoSla { get; set; } = string.Empty;
        public string TipoSolicitud { get; set; } = string.Empty;
        public int DiasUmbral { get; set; }
    }
}
