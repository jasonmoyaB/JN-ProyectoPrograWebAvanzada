namespace JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities
{
    public class Rol
    {
        public int RolID { get; set; }
        public string NombreRol { get; set; } = string.Empty;

        // Navegación
        public ICollection<JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities.Usuario> Usuarios { get; set; } = new List<JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities.Usuario>();
    }
}

