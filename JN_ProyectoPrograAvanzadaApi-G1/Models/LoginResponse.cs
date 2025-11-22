namespace JN_ProyectoPrograAvanzadaApi_G1.Models
{
    public class LoginResponse
    {
        public int UsuarioID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public int RolID { get; set; }
        public string RolNombre { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}



