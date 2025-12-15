

namespace JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities
{
    public class UnidadMedida
    {
        public int UnidadID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Abreviacion { get; set; }
        public bool Activo { get; set; } = true;

       
        public ICollection<MovimientoDetalle> MovimientoDetalles { get; set; } = new List<MovimientoDetalle>();
    }
}

