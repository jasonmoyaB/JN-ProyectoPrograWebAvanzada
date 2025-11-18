namespace JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities
{
    public class Bodega
    {
        public int BodegaID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Ubicacion { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime? FechaCreacion { get; set; } = DateTime.UtcNow;

        // Navegación
        public ICollection<JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities.Usuario> Usuarios { get; set; } = new List<JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities.Usuario>();
        public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
        public ICollection<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();
    }
}

