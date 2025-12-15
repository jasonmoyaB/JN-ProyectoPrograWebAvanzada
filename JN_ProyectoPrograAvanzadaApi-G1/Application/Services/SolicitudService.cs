using JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Solicitudes;
using JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Movimientos;
using JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities;
using JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Repositories;
using JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Data;
using Dapper;

namespace JN_ProyectoPrograAvanzadaApi_G1.Application.Services
{
    public class SolicitudService : ISolicitudService
    {
        private readonly ISolicitudRepository _solicitudRepository;
        private readonly IMovimientoService _movimientoService;
        private readonly IInventarioService _inventarioService;
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<SolicitudService> _logger;

        public SolicitudService(
            ISolicitudRepository solicitudRepository,
            IMovimientoService movimientoService,
            IInventarioService inventarioService,
            IDbConnectionFactory connectionFactory,
            ILogger<SolicitudService> logger)
        {
            _solicitudRepository = solicitudRepository;
            _movimientoService = movimientoService;
            _inventarioService = inventarioService;
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<List<SolicitudDto>> GetAllAsync()
        {
            var solicitudes = await _solicitudRepository.GetAllAsync();
            return solicitudes.Select(s => MapToDto(s)).ToList();
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

        private async Task<int?> GetEstadoIdByCodigoAsync(string codigo)
        {
           
            using var connection = _connectionFactory.CreateConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            var sql = "SELECT EstadoSolicitudID FROM inv.EstadosSolicitud WHERE Codigo = @Codigo";
            var estadoId = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { Codigo = codigo });
            return estadoId;
        }

        public async Task AprobarAsync(int solicitudId, int usuarioAprobadorId, string? comentarios = null)
        {
            var estadoAprobadaId = await GetEstadoIdByCodigoAsync("Aprobada");
            if (!estadoAprobadaId.HasValue)
            {
                throw new Exception("No se encontró el estado 'Aprobada' en el sistema");
            }

            var resultado = await _solicitudRepository.UpdateEstadoAsync(
                solicitudId,
                estadoAprobadaId.Value,
                usuarioAprobadorId: usuarioAprobadorId,
                fechaAprobacion: DateTime.UtcNow,
                comentarios: comentarios);

            if (!resultado)
            {
                throw new Exception("No se pudo actualizar el estado de la solicitud");
            }
        }

        public async Task RechazarAsync(int solicitudId, int usuarioAprobadorId, string? comentarios = null)
        {
            var estadoRechazadaId = await GetEstadoIdByCodigoAsync("Rechazada");
            if (!estadoRechazadaId.HasValue)
            {
                throw new Exception("No se encontró el estado 'Rechazada' en el sistema");
            }

            var resultado = await _solicitudRepository.UpdateEstadoAsync(
                solicitudId,
                estadoRechazadaId.Value,
                usuarioAprobadorId: usuarioAprobadorId,
                fechaAprobacion: DateTime.UtcNow,
                comentarios: comentarios);

            if (!resultado)
            {
                throw new Exception("No se pudo actualizar el estado de la solicitud");
            }
        }

        public async Task EnviarAsync(int solicitudId, string? comentarios = null)
        {
            
            var estadoEnProcesoId = await GetEstadoIdByCodigoAsync("EnProceso");
            if (!estadoEnProcesoId.HasValue)
            {
                throw new Exception("No se encontró el estado 'EnProceso' en el sistema");
            }

            var resultado = await _solicitudRepository.UpdateEstadoAsync(
                solicitudId,
                estadoEnProcesoId.Value,
                fechaEnvio: DateTime.UtcNow,
                comentarios: comentarios);

            if (!resultado)
            {
                throw new Exception("No se pudo actualizar el estado de la solicitud");
            }
        }

        public async Task EntregarAsync(int solicitudId, int bodegaOrigenId, int bodegaDestinoId, int usuarioId, string? comentarios = null)
        {
            
            var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
            if (solicitud == null)
            {
                throw new Exception("Solicitud no encontrada");
            }

          
            var tieneCantidades = await _solicitudRepository.TieneCantidadesEnviadasAsync(solicitudId);
            if (!tieneCantidades)
            {
                throw new Exception("La solicitud debe tener cantidades establecidas antes de ser entregada");
            }

            
            var inventarioOrigen = await _inventarioService.GetSaldoByBodegaAsync(bodegaOrigenId);
            foreach (var detalle in solicitud.Detalles.Where(d => d.CantidadEnviada.HasValue && d.CantidadEnviada.Value > 0))
            {
                var saldo = inventarioOrigen.FirstOrDefault(i => i.ProductoID == detalle.ProductoID);
                if (saldo == null || saldo.Cantidad < detalle.CantidadEnviada.Value)
                {
                    throw new Exception($"No hay suficiente stock disponible para el producto {detalle.Producto?.Nombre ?? detalle.ProductoID.ToString()}");
                }
            }

            
            var dto = new MovimientoTrasladoDto
            {
                BodegaOrigenID = bodegaOrigenId,
                BodegaDestinoID = bodegaDestinoId,
                TipoMovimientoID = 0, 
                Referencia = $"SOL-{solicitudId}-{DateTime.UtcNow:yyyyMMddHHmmss}",
                Observaciones = comentarios,
                Detalles = solicitud.Detalles
                    .Where(d => d.CantidadEnviada.HasValue && d.CantidadEnviada.Value > 0)
                    .Select(d =>
                    {
                        
                        var saldo = inventarioOrigen.FirstOrDefault(i => i.ProductoID == d.ProductoID);
                        return new MovimientoDetalleDto
                        {
                            ProductoID = d.ProductoID,
                            Cantidad = d.CantidadEnviada.Value,
                            UnidadID = saldo?.UnidadID ?? 0
                        };
                    }).ToList()
            };

            await _movimientoService.CreateTrasladoAsync(dto, usuarioId);

            // Actualizar el estado a Finalizadaaa
            var estadoFinalizadaId = await GetEstadoIdByCodigoAsync("Finalizada");
            if (!estadoFinalizadaId.HasValue)
            {
                throw new Exception("No se encontró el estado 'Finalizada' en el sistema");
            }

            var resultado = await _solicitudRepository.UpdateEstadoAsync(
                solicitudId,
                estadoFinalizadaId.Value,
                fechaEntrega: DateTime.UtcNow,
                comentarios: comentarios);

            if (!resultado)
            {
                throw new Exception("No se pudo actualizar el estado de la solicitud");
            }
        }

        public async Task EstablecerCantidadesEnviadasAsync(int solicitudId, EstablecerCantidadesEnviadasDto dto)
        {
            var cantidades = dto.Detalles.ToDictionary(d => d.ProductoID, d => d.CantidadEnviada);
            var resultado = await _solicitudRepository.EstablecerCantidadesEnviadasAsync(solicitudId, cantidades);
            
            if (!resultado)
            {
                throw new Exception("No se pudieron establecer las cantidades a enviar");
            }
        }

        public async Task<bool> TieneCantidadesEnviadasAsync(int solicitudId)
        {
            return await _solicitudRepository.TieneCantidadesEnviadasAsync(solicitudId);
        }
    }
}

