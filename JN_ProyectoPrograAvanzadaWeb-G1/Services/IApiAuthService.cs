using JN_ProyectoPrograAvanzadaWeb_G1.Models.ViewModels;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Services
{
    public interface IApiAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<bool> CambiarContrasenaAsync(int userId, string currentPassword, string newPassword);
        Task<string> RecuperarClaveAsync(string email);
    }

    public class LoginRequestDto
    {
        public string CorreoElectronico { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public int UsuarioID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public int RolID { get; set; }
        public string RolNombre { get; set; } = string.Empty;
        public int? BodegaID { get; set; }
        public string? BodegaNombre { get; set; }
        public bool Activo { get; set; }
    }
}

