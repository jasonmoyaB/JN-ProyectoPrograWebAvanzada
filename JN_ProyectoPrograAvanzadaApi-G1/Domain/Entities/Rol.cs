namespace JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities
{
    public class Rol
    {
        public int RolID { get; set; }
        public string NombreRol { get; set; } = string.Empty;

        
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}

