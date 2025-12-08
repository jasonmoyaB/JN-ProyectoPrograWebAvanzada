using System.Net.Http.Json;
using System.Text.Json;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Services
{
    public class ApiMovimientoService : IApiMovimientoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiMovimientoService> _logger;

        public ApiMovimientoService(IHttpClientFactory httpClientFactory, ILogger<ApiMovimientoService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _logger = logger;
        }

        public async Task<List<MovimientoDto>> GetByBodegaAsync(int bodegaId, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            try
            {
                var url = $"api/movimientos/bodega/{bodegaId}";
                if (fechaDesde.HasValue || fechaHasta.HasValue)
                {
                    var queryParams = new List<string>();
                    if (fechaDesde.HasValue)
                        queryParams.Add($"fechaDesde={Uri.EscapeDataString(fechaDesde.Value.ToString("yyyy-MM-dd"))}");
                    if (fechaHasta.HasValue)
                        queryParams.Add($"fechaHasta={Uri.EscapeDataString(fechaHasta.Value.ToString("yyyy-MM-dd"))}");
                    
                    if (queryParams.Any())
                        url += "?" + string.Join("&", queryParams);
                }

                var response = await _httpClient.GetFromJsonAsync<List<MovimientoDto>>(url);
                return response ?? new List<MovimientoDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener movimientos por bodega {BodegaId}", bodegaId);
                return new List<MovimientoDto>();
            }
        }

        public async Task<List<MovimientoDto>> GetByUsuarioAsync(int usuarioId, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            try
            {
                var url = $"api/movimientos/usuario/{usuarioId}";
                if (fechaDesde.HasValue || fechaHasta.HasValue)
                {
                    var queryParams = new List<string>();
                    if (fechaDesde.HasValue)
                        queryParams.Add($"fechaDesde={Uri.EscapeDataString(fechaDesde.Value.ToString("yyyy-MM-dd"))}");
                    if (fechaHasta.HasValue)
                        queryParams.Add($"fechaHasta={Uri.EscapeDataString(fechaHasta.Value.ToString("yyyy-MM-dd"))}");
                    
                    if (queryParams.Any())
                        url += "?" + string.Join("&", queryParams);
                }

                var response = await _httpClient.GetFromJsonAsync<List<MovimientoDto>>(url);
                return response ?? new List<MovimientoDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener movimientos por usuario {UsuarioId}", usuarioId);
                return new List<MovimientoDto>();
            }
        }

        public async Task<MovimientoDto?> GetByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<MovimientoDto>($"api/movimientos/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener movimiento {Id}", id);
                return null;
            }
        }

        public async Task<int> GetCountMovimientosHoyByBodegaAsync(int bodegaId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>($"api/movimientos/bodega/{bodegaId}/count-hoy");
                if (response.TryGetProperty("count", out var count))
                {
                    return count.GetInt32();
                }
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener conteo de movimientos hoy por bodega {BodegaId}", bodegaId);
                return 0;
            }
        }

        public async Task<int> GetCountMovimientosHoyByUsuarioAsync(int usuarioId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>($"api/movimientos/usuario/{usuarioId}/count-hoy");
                if (response.TryGetProperty("count", out var count))
                {
                    return count.GetInt32();
                }
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener conteo de movimientos hoy por usuario {UsuarioId}", usuarioId);
                return 0;
            }
        }
    }
}

