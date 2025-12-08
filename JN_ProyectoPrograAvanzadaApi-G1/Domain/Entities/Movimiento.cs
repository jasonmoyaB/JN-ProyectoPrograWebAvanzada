namespace JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities
{
    public class Movimiento
    {
        public int MovimientoID { get; set; }
        public int BodegaID { get; set; }
        public int TipoMovimientoID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime FechaMovimientoUTC { get; set; } = DateTime.UtcNow;
        public string? Referencia { get; set; }
        public string? Observaciones { get; set; }
        public int? BodegaRelacionadaID { get; set; } 
        public int? ProveedorID { get; set; }
        public int? SolicitudID { get; set; }

        
        public Bodega? Bodega { get; set; }
        public TipoMovimiento? TipoMovimiento { get; set; }
        public Usuario? Usuario { get; set; }
        public Bodega? BodegaRelacionada { get; set; }
        public Proveedor? Proveedor { get; set; }
        public Solicitud? Solicitud { get; set; }
        public ICollection<MovimientoDetalle> Detalles { get; set; } = new List<MovimientoDetalle>();
    }
}

