using JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Solicitudes;

namespace JN_ProyectoPrograAvanzadaApi_G1.Application.Services
{
    public interface ISolicitudService
    {
        Task<List<SolicitudDto>> GetAllAsync();
        Task<List<SolicitudDto>> GetByUsuarioAsync(int usuarioId);
        Task<List<SolicitudDto>> GetByBodegaAsync(int bodegaId);
        Task<SolicitudDto?> GetByIdAsync(int solicitudId);
        Task<int> CreateAsync(CrearSolicitudDto dto, int usuarioId);
        Task<int> GetCountPendientesByUsuarioAsync(int usuarioId);
        Task AprobarAsync(int solicitudId, int usuarioAprobadorId, string? comentarios = null);
        Task RechazarAsync(int solicitudId, int usuarioAprobadorId, string? comentarios = null);
        Task EnviarAsync(int solicitudId, string? comentarios = null);
        Task EntregarAsync(int solicitudId, int bodegaOrigenId, int bodegaDestinoId, int usuarioId, string? comentarios = null);
        Task EstablecerCantidadesEnviadasAsync(int solicitudId, EstablecerCantidadesEnviadasDto dto);
        Task<bool> TieneCantidadesEnviadasAsync(int solicitudId);
    }
}

