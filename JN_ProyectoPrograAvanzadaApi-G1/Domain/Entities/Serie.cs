namespace JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities
{
    public class Serie
    {
        public int SerieID { get; set; }
        public int ProductoID { get; set; }
        public string NumeroSerie { get; set; } = string.Empty;
        public DateTime? FechaRegistro { get; set; } = DateTime.UtcNow;

       
        public Producto? Producto { get; set; }
        public ICollection<MovimientoDetalleSerie> MovimientoDetalleSeries { get; set; } = new List<MovimientoDetalleSerie>();
    }
}

