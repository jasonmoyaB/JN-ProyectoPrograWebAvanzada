using JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Auth;
using JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities;

namespace JN_ProyectoPrograAvanzadaApi_G1.Application.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<Usuario?> GetUsuarioByIdAsync(int usuarioId);
        Task<bool> CambiarContrasenaAsync(int usuarioId, string contrasenaActual, string nuevaContrasena);
        Task<string> GenerarContrasenaTemporalAsync(string email);
    }
}

