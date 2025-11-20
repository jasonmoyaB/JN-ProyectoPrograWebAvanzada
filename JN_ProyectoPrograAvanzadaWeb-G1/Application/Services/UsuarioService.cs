using JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Usuarios;
using JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities;
using JN_ProyectoPrograAvanzadaWeb_G1.Helpers;
using JN_ProyectoPrograAvanzadaWeb_G1.Infrastructure.Repositories;
using DomainUsuario = JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities.Usuario;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRolRepository _rolRepository;
        private readonly IBodegaRepository _bodegaRepository;

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IRolRepository rolRepository,
            IBodegaRepository bodegaRepository)
        {
            _usuarioRepository = usuarioRepository;
            _rolRepository = rolRepository;
            _bodegaRepository = bodegaRepository;
        }

        public async Task<List<UsuarioDto>> GetAllAsync(bool? activo = null)
        {
            var usuarios = await _usuarioRepository.GetAllAsync(activo);
            var roles = await _rolRepository.GetAllAsync();
            var bodegas = await _bodegaRepository.GetAllAsync(true);

            return usuarios.Select(u => new UsuarioDto
            {
                UsuarioID = u.UsuarioID,
                Nombre = u.Nombre,
                CorreoElectronico = u.CorreoElectronico,
                RolID = u.RolID,
                RolNombre = u.Rol?.NombreRol ?? roles.FirstOrDefault(r => r.RolID == u.RolID)?.NombreRol ?? "Sin rol",
                BodegaID = u.BodegaID,
                BodegaNombre = u.Bodega?.Nombre ?? bodegas.FirstOrDefault(b => b.BodegaID == u.BodegaID)?.Nombre,
                Activo = u.Activo,
                FechaRegistro = u.FechaRegistro
            }).ToList();
        }

        public async Task<UsuarioDto?> GetByIdAsync(int usuarioId)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null) return null;

            return new UsuarioDto
            {
                UsuarioID = usuario.UsuarioID,
                Nombre = usuario.Nombre,
                CorreoElectronico = usuario.CorreoElectronico,
                RolID = usuario.RolID,
                RolNombre = usuario.Rol?.NombreRol ?? "Sin rol",
                BodegaID = usuario.BodegaID,
                BodegaNombre = usuario.Bodega?.Nombre,
                Activo = usuario.Activo,
                FechaRegistro = usuario.FechaRegistro
            };
        }

        public async Task<int> CreateAsync(CrearUsuarioDto dto)
        {
            
            var usuarioExistente = await _usuarioRepository.GetByEmailAsync(dto.CorreoElectronico);
            if (usuarioExistente != null)
            {
                throw new InvalidOperationException("El correo electrónico ya está registrado");
            }

            var usuario = new DomainUsuario
            {
                Nombre = dto.Nombre,
                CorreoElectronico = dto.CorreoElectronico,
                ContrasenaHash = PasswordHelper.HashPassword(dto.Contrasena),
                RolID = dto.RolID,
                BodegaID = dto.BodegaID,
                Activo = true
            };

            return await _usuarioRepository.CreateAsync(usuario);
        }

        public async Task<bool> UpdateAsync(int usuarioId, EditarUsuarioDto dto)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null) return false;

            
            var usuarioConEmail = await _usuarioRepository.GetByEmailAsync(dto.CorreoElectronico);
            if (usuarioConEmail != null && usuarioConEmail.UsuarioID != usuarioId)
            {
                throw new InvalidOperationException("El correo electrónico ya está registrado en otro usuario");
            }

            usuario.Nombre = dto.Nombre;
            usuario.CorreoElectronico = dto.CorreoElectronico;
            usuario.RolID = dto.RolID;
            usuario.BodegaID = dto.BodegaID;
            usuario.Activo = dto.Activo;

            return await _usuarioRepository.UpdateAsync(usuario);
        }

        public async Task<bool> ToggleActivoAsync(int usuarioId)
        {
            return await _usuarioRepository.ToggleActivoAsync(usuarioId);
        }

        public async Task<bool> AssignarBodegaAsync(int usuarioId, int? bodegaId)
        {
            if (bodegaId.HasValue)
            {
                return await _usuarioRepository.AssignarBodegaAsync(usuarioId, bodegaId.Value);
            }
            
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null) return false;
            
            usuario.BodegaID = null;
            return await _usuarioRepository.UpdateAsync(usuario);
        }
    }
}

