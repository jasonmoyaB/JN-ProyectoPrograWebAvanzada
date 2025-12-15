

namespace JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities
{
    public class StockMinimo
    {
        public int StockMinimoID { get; set; }
        public int ProductoID { get; set; }
        public int BodegaID { get; set; }
        public decimal CantidadMinima { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime? FechaActualizacion { get; set; } = DateTime.UtcNow;

        
        public Producto? Producto { get; set; }
        public Bodega? Bodega { get; set; }
    }
}

