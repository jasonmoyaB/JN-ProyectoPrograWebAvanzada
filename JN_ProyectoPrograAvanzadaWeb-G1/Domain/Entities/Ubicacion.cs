namespace JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities
{
    public class Ubicacion
    {
        public int UbicacionID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int? BodegaID { get; set; }
        public bool Activo { get; set; } = true;

        // Navegación
        public Bodega? Bodega { get; set; }
        public ICollection<MovimientoDetalle> MovimientoDetalles { get; set; } = new List<MovimientoDetalle>();
    }
}

