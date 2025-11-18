using JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Auth;
using JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities;
using JN_ProyectoPrograAvanzadaWeb_G1.Helpers;
using JN_ProyectoPrograAvanzadaWeb_G1.Infrastructure.Repositories;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRolRepository _rolRepository;

        public AuthService(IUsuarioRepository usuarioRepository, IRolRepository rolRepository)
        {
            _usuarioRepository = usuarioRepository;
            _rolRepository = rolRepository;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            try
            {
                string emailNormalized = request.CorreoElectronico?.Trim().ToLowerInvariant() ?? string.Empty;
                
                var usuario = await _usuarioRepository.GetByEmailAsync(emailNormalized);

                if (usuario == null || !usuario.Activo)
                {
                    await Task.Delay(Random.Shared.Next(100, 300));
                    return null;
                }

                if (string.IsNullOrWhiteSpace(usuario.ContrasenaHash))
                {
                    await Task.Delay(Random.Shared.Next(100, 300));
                    return null;
                }

                bool isPasswordValid = PasswordHelper.VerifyPassword(request.Contrasena, usuario.ContrasenaHash);

                if (!isPasswordValid)
                {
                    await Task.Delay(Random.Shared.Next(100, 300));
                    return null;
                }

                return new LoginResponseDto
                {
                    UsuarioID = usuario.UsuarioID,
                    Nombre = usuario.Nombre,
                    CorreoElectronico = usuario.CorreoElectronico,
                    RolID = usuario.RolID,
                    RolNombre = usuario.Rol?.NombreRol ?? string.Empty,
                    BodegaID = usuario.BodegaID,
                    BodegaNombre = usuario.Bodega?.Nombre,
                    Activo = usuario.Activo
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al autenticar usuario: {ex.Message}", ex);
            }
        }

        public async Task<Domain.Entities.Usuario?> GetUsuarioByIdAsync(int usuarioId)
        {
            return await _usuarioRepository.GetByIdAsync(usuarioId);
        }

        public async Task<bool> CambiarContrasenaAsync(int usuarioId, string contrasenaActual, string nuevaContrasena)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
            {
                return false;
            }

            if (!PasswordHelper.VerifyPassword(contrasenaActual, usuario.ContrasenaHash))
            {
                return false;
            }

            if (PasswordHelper.VerifyPassword(nuevaContrasena, usuario.ContrasenaHash))
            {
                return false;
            }

            string nuevoHash = PasswordHelper.HashPassword(nuevaContrasena);
            return await _usuarioRepository.UpdatePasswordAsync(usuarioId, nuevoHash);
        }

        public async Task<string> GenerarContrasenaTemporalAsync(string email)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(email);
            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado");
            }

            string nuevaClave = Guid.NewGuid().ToString("N")[..8];
            string hash = PasswordHelper.HashPassword(nuevaClave);
            
            bool actualizado = await _usuarioRepository.UpdatePasswordAsync(usuario.UsuarioID, hash);
            if (!actualizado)
            {
                throw new Exception("Error al actualizar la contraseña");
            }

            return nuevaClave;
        }
    }
}

