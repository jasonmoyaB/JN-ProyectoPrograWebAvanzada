namespace JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities
{
    public class MotivoAjuste
    {
        public int MotivoAjusteID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;

        public ICollection<MovimientoDetalle> MovimientoDetalles { get; set; } = new List<MovimientoDetalle>();
    }
}

