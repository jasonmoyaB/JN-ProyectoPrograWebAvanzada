using JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Solicitudes;

namespace JN_ProyectoPrograAvanzadaApi_G1.Application.Services
{
    public interface ISolicitudService
    {
        Task<List<SolicitudDto>> GetByUsuarioAsync(int usuarioId);
        Task<List<SolicitudDto>> GetByBodegaAsync(int bodegaId);
        Task<SolicitudDto?> GetByIdAsync(int solicitudId);
        Task<int> CreateAsync(CrearSolicitudDto dto, int usuarioId);
        Task<int> GetCountPendientesByUsuarioAsync(int usuarioId);
    }
}

