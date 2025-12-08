namespace JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities
{
    public class TipoMovimiento
    {
        public int TipoMovimientoID { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public short Naturaleza { get; set; } 
        public bool Activo { get; set; } = true;

       
        public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
    }
}

