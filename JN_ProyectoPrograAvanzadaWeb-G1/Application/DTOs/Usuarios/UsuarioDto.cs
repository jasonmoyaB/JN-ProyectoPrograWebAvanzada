namespace JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Usuarios
{
    public class UsuarioDto
    {
        public int UsuarioID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public int RolID { get; set; }
        public string RolNombre { get; set; } = string.Empty;
        public int? BodegaID { get; set; }
        public string? BodegaNombre { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}




