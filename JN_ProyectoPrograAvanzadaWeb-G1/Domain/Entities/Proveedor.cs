namespace JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities
{
    public class Proveedor
    {
        public int ProveedorID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Contacto { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public bool Activo { get; set; } = true;

       
        public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
    }
}

