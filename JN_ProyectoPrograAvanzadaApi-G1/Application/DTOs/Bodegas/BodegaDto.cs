namespace JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Bodegas
{
    public class BodegaDto
    {
        public int BodegaID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Ubicacion { get; set; }
        public bool Activo { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public int? TotalUsuarios { get; set; }
    }
}

