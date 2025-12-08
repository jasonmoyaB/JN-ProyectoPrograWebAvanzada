using JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Solicitudes;
using JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities;
using JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Repositories;

namespace JN_ProyectoPrograAvanzadaApi_G1.Application.Services
{
    public class SolicitudService : ISolicitudService
    {
        private readonly ISolicitudRepository _solicitudRepository;

        public SolicitudService(ISolicitudRepository solicitudRepository)
        {
            _solicitudRepository = solicitudRepository;
        }

        public async Task<List<SolicitudDto>> GetByUsuarioAsync(int usuarioId)
        {
            var solicitudes = await _solicitudRepository.GetByUsuarioAsync(usuarioId);
            return solicitudes.Select(s => MapToDto(s)).ToList();
        }

        public async Task<List<SolicitudDto>> GetByBodegaAsync(int bodegaId)
        {
            var solicitudes = await _solicitudRepository.GetByBodegaAsync(bodegaId);
            return solicitudes.Select(s => MapToDto(s)).ToList();
        }

        public async Task<SolicitudDto?> GetByIdAsync(int solicitudId)
        {
            var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
            if (solicitud == null) return null;

            return MapToDto(solicitud);
        }

        public async Task<int> CreateAsync(CrearSolicitudDto dto, int usuarioId)
        {
            // Obtener el ID del estado "Pendiente"
            var estadoPendienteId = await GetEstadoPendienteIdAsync();
            if (estadoPendienteId == null)
            {
                throw new Exception("No se encontró el estado 'Pendiente' en el sistema");
            }

            var solicitud = new Solicitud
            {
                EstadoSolicitudID = estadoPendienteId.Value,
                BodegaID = dto.BodegaID,
                UsuarioID = usuarioId,
                Comentarios = dto.Comentarios,
                FechaCreacionUTC = DateTime.UtcNow
            };

            // Mapear detalles
            solicitud.Detalles = dto.Detalles.Select(d => new SolicitudDetalle
            {
                ProductoID = d.ProductoID,
                CantidadSolicitada = d.CantidadSolicitada
            }).ToList();

            return await _solicitudRepository.CreateAsync(solicitud);
        }

        public async Task<int> GetCountPendientesByUsuarioAsync(int usuarioId)
        {
            return await _solicitudRepository.GetCountPendientesByUsuarioAsync(usuarioId);
        }

        private SolicitudDto MapToDto(Solicitud s)
        {
            return new SolicitudDto
            {
                SolicitudID = s.SolicitudID,
                EstadoSolicitudID = s.EstadoSolicitudID,
                EstadoCodigo = s.EstadoSolicitud?.Codigo ?? string.Empty,
                EstadoNombre = s.EstadoSolicitud?.Codigo ?? string.Empty,
                BodegaID = s.BodegaID,
                BodegaNombre = s.Bodega?.Nombre ?? string.Empty,
                UsuarioID = s.UsuarioID,
                UsuarioNombre = s.Usuario?.Nombre ?? string.Empty,
                FechaCreacionUTC = s.FechaCreacionUTC,
                FechaAprobacionUTC = s.FechaAprobacionUTC,
                FechaEnvioUTC = s.FechaEnvioUTC,
                FechaEntregaUTC = s.FechaEntregaUTC,
                Comentarios = s.Comentarios,
                UsuarioAprobadorID = s.UsuarioAprobadorID,
                UsuarioAprobadorNombre = s.UsuarioAprobador?.Nombre,
                Detalles = s.Detalles?.Select(d => new SolicitudDetalleDto
                {
                    SolicitudDetalleID = d.SolicitudDetalleID,
                    ProductoID = d.ProductoID,
                    ProductoNombre = d.Producto?.Nombre ?? string.Empty,
                    ProductoSKU = d.Producto?.SKU ?? string.Empty,
                    CantidadSolicitada = d.CantidadSolicitada,
                    CantidadEnviada = d.CantidadEnviada
                }).ToList() ?? new List<SolicitudDetalleDto>()
            };
        }

        private async Task<int?> GetEstadoPendienteIdAsync()
        {
            
            return 1; 
        }
    }
}

