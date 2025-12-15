namespace JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities
{

    //producto    
    public class Producto
    {
        public int ProductoID { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool EsSerializado { get; set; } = false;
        public bool Activo { get; set; } = true;
        public DateTime? FechaCreacion { get; set; } = DateTime.UtcNow;

      
        public ICollection<MovimientoDetalle> MovimientoDetalles { get; set; } = new List<MovimientoDetalle>();
        public ICollection<SolicitudDetalle> SolicitudDetalles { get; set; } = new List<SolicitudDetalle>();
        public ICollection<Serie> Series { get; set; } = new List<Serie>();
        public ICollection<StockMinimo> StockMinimos { get; set; } = new List<StockMinimo>();
    }
}

