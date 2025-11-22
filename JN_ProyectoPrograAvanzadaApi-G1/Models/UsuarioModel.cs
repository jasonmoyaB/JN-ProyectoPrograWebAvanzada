namespace JN_ProyectoPrograAvanzadaApi_G1.Models
{
    public class UsuarioModel
    {
        public int UsuarioID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? CorreoElectronico { get; set; }
        public int RolID { get; set; }
        public bool Activo { get; set; }
        public string? RolNombre { get; set; }
        public string? ContrasenaHash { get; set; }
    }
}



