namespace JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities
{
    public class Usuario
    {
        public int UsuarioID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public string ContrasenaHash { get; set; } = string.Empty;
        public int RolID { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public int? BodegaID { get; set; } // Bodega asignada al técnico

        // Navegación
        public JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities.Rol? Rol { get; set; }
        public JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities.Bodega? Bodega { get; set; }
    }
}

