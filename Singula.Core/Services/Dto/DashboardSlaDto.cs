using System;

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
        public bool CumpleSla1 { get; set; }
        public bool CumpleSla2 { get; set; }
        public string NombrePersonal { get; set; } = string.Empty;
        public int DiasUmbralSla { get; set; }
    }

    public class DashboardStatsDto
    {
        public int TotalSolicitudes { get; set; }
        public double CumplimientoSla1 { get; set; }
        public double CumplimientoSla2 { get; set; }
        public double PromedioDiasSla1 { get; set; }
        public double PromedioDiasSla2 { get; set; }
    }
}
