using JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Auth;
using JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Application.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<Domain.Entities.Usuario?> GetUsuarioByIdAsync(int usuarioId);
        Task<bool> CambiarContrasenaAsync(int usuarioId, string contrasenaActual, string nuevaContrasena);
        Task<string> GenerarContrasenaTemporalAsync(string email);
    }
}

