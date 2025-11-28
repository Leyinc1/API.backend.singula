namespace Singula.Core.Services.Dto
{
    public class PrioridadCatalogoDto
    {
        public int IdPrioridad { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int Nivel { get; set; }
        public decimal SlaMultiplier { get; set; }
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public bool Activo { get; set; }
    }
}
