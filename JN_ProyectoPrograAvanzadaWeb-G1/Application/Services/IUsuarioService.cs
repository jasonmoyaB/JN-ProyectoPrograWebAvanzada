using JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Usuarios;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Application.Services
{
    public interface IUsuarioService
    {
        Task<List<UsuarioDto>> GetAllAsync(bool? activo = null);
        Task<UsuarioDto?> GetByIdAsync(int usuarioId);
        Task<int> CreateAsync(CrearUsuarioDto dto);
        Task<bool> UpdateAsync(int usuarioId, EditarUsuarioDto dto);
        Task<bool> ToggleActivoAsync(int usuarioId);
        Task<bool> AssignarBodegaAsync(int usuarioId, int? bodegaId);
    }
}


