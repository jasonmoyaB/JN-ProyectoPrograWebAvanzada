using JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Usuarios;
using JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities;
using JN_ProyectoPrograAvanzadaApi_G1.Helpers;
using JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Repositories;
using DomainUsuario = JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities.Usuario;
using Microsoft.Extensions.Logging;

namespace JN_ProyectoPrograAvanzadaApi_G1.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRolRepository _rolRepository;
        private readonly IBodegaRepository _bodegaRepository;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IRolRepository rolRepository,
            IBodegaRepository bodegaRepository,
            ILogger<UsuarioService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _rolRepository = rolRepository;
            _bodegaRepository = bodegaRepository;
            _logger = logger;
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

            var resultado = await _usuarioRepository.UpdateAsync(usuario);
            
            _logger.LogInformation("UsuarioService: UpdateAsync retornó {Resultado} para usuario {UsuarioId}", resultado, usuarioId);
            
           
            if (!resultado)
            {
                _logger.LogWarning("UsuarioService: UpdateAsync retornó false, verificando si los datos se actualizaron realmente para usuario {UsuarioId}", usuarioId);
                
               
                var usuarioActualizado = await _usuarioRepository.GetByIdAsync(usuarioId);
                if (usuarioActualizado != null)
                {
                    
                    if (usuarioActualizado.Nombre == dto.Nombre &&
                        usuarioActualizado.CorreoElectronico == dto.CorreoElectronico &&
                        usuarioActualizado.RolID == dto.RolID &&
                        usuarioActualizado.BodegaID == dto.BodegaID &&
                        usuarioActualizado.Activo == dto.Activo)
                    {
                        _logger.LogInformation("UsuarioService: Los datos se actualizaron correctamente aunque UpdateAsync retornó false. Retornando true para usuario {UsuarioId}", usuarioId);
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("UsuarioService: Los datos no coinciden. Esperado: Nombre={Nombre}, RolID={RolID}, BodegaID={BodegaID}, Activo={Activo}. Actual: Nombre={NombreActual}, RolID={RolIDActual}, BodegaID={BodegaIDActual}, Activo={ActivoActual}",
                            dto.Nombre, dto.RolID, dto.BodegaID, dto.Activo,
                            usuarioActualizado.Nombre, usuarioActualizado.RolID, usuarioActualizado.BodegaID, usuarioActualizado.Activo);
                    }
                }
                else
                {
                    _logger.LogError("UsuarioService: Usuario {UsuarioId} no encontrado después de UpdateAsync", usuarioId);
                }
            }
            
            return resultado;
        }

        public async Task<bool> ToggleActivoAsync(int usuarioId)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null) return false;
            
            var estadoAnterior = usuario.Activo;
            
           
            var resultado = await _usuarioRepository.ToggleActivoAsync(usuarioId);
            
            _logger.LogInformation("UsuarioService: ToggleActivoAsync retornó {Resultado} para usuario {UsuarioId}", resultado, usuarioId);
            
           
            if (!resultado)
            {
                _logger.LogWarning("UsuarioService: ToggleActivoAsync retornó false, verificando si el estado se cambió realmente para usuario {UsuarioId}", usuarioId);
                
                var usuarioActualizado = await _usuarioRepository.GetByIdAsync(usuarioId);
                if (usuarioActualizado != null)
                {
                    
                    if (usuarioActualizado.Activo != estadoAnterior)
                    {
                        _logger.LogInformation("UsuarioService: El estado se cambió correctamente aunque ToggleActivoAsync retornó false. Estado anterior: {EstadoAnterior}, Estado actual: {EstadoActual}. Retornando true para usuario {UsuarioId}", 
                            estadoAnterior, usuarioActualizado.Activo, usuarioId);
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("UsuarioService: El estado no cambió. Estado anterior: {EstadoAnterior}, Estado actual: {EstadoActual} para usuario {UsuarioId}",
                            estadoAnterior, usuarioActualizado.Activo, usuarioId);
                    }
                }
                else
                {
                    _logger.LogError("UsuarioService: Usuario {UsuarioId} no encontrado después de ToggleActivoAsync", usuarioId);
                }
            }
            
            return resultado;
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

