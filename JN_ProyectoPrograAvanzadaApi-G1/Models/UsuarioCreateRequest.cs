namespace JN_ProyectoPrograAvanzadaApi_G1.Models
{
    public class UsuarioCreateRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string? CorreoElectronico { get; set; }
        public string? Contrasena { get; set; }
        public int RolID { get; set; }
        public bool Activo { get; set; } = true;
    }
}



