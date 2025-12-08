using System.Net.Http.Json;
using System.Text.Json;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Services
{
    public class ApiUsuarioService : IApiUsuarioService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiUsuarioService> _logger;

        public ApiUsuarioService(IHttpClientFactory httpClientFactory, ILogger<ApiUsuarioService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _logger = logger;
        }

        public async Task<List<UsuarioDto>> GetAllAsync(bool? activo = null)
        {
            try
            {
                var url = activo.HasValue ? $"api/usuarios?activo={activo.Value}" : "api/usuarios";
                var response = await _httpClient.GetFromJsonAsync<List<UsuarioDto>>(url);
                return response ?? new List<UsuarioDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                return new List<UsuarioDto>();
            }
        }

        public async Task<UsuarioDto?> GetByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UsuarioDto>($"api/usuarios/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario {Id}", id);
                return null;
            }
        }

        public async Task<int> CreateAsync(CrearUsuarioDto dto)
        {
            try
            {
                _logger.LogInformation("Intentando crear usuario: {Email}", dto.CorreoElectronico);
                var response = await _httpClient.PostAsJsonAsync("api/usuarios", dto);
                
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Respuesta del API - Status: {StatusCode}, Content: {Content}", 
                    response.StatusCode, responseContent);
                
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var result = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(responseContent);
                        if (result.TryGetProperty("id", out var id))
                        {
                            var usuarioId = id.GetInt32();
                            _logger.LogInformation("Usuario creado exitosamente con ID: {UsuarioId}", usuarioId);
                            return usuarioId;
                        }
                        _logger.LogWarning("La respuesta del API no contiene el campo 'id'. Respuesta: {Response}", responseContent);
                        return 0;
                    }
                    catch (Exception jsonEx)
                    {
                        _logger.LogError(jsonEx, "Error al deserializar la respuesta del API: {Response}", responseContent);
                        return 0;
                    }
                }
                else
                {
                    _logger.LogError("Error al crear usuario. Status: {StatusCode}, Response: {ErrorContent}", 
                        response.StatusCode, responseContent);
                    return 0;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al crear usuario. Verifique que el API esté ejecutándose en {BaseUrl}", 
                    _httpClient.BaseAddress);
                throw new InvalidOperationException($"No se pudo conectar con el API en {_httpClient.BaseAddress}. Verifique que el servicio esté ejecutándose.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear usuario: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(int id, EditarUsuarioDto dto)
        {
            try
            {
                _logger.LogInformation("Intentando actualizar usuario {Id} con datos: Nombre={Nombre}, RolID={RolID}, BodegaID={BodegaID}", 
                    id, dto.Nombre, dto.RolID, dto.BodegaID);
                
                var response = await _httpClient.PutAsJsonAsync($"api/usuarios/{id}", dto);
                
                var statusCode = response.StatusCode;
                var statusCodeInt = (int)statusCode;
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Respuesta del API al actualizar usuario - Status: {StatusCode} ({StatusCodeInt}), IsSuccessStatusCode: {IsSuccess}, Content: {Content}", 
                    statusCode, statusCodeInt, response.IsSuccessStatusCode, responseContent);
                

                if (response.IsSuccessStatusCode || 
                    statusCode == System.Net.HttpStatusCode.OK || 
                    (statusCodeInt >= 200 && statusCodeInt < 300))
                {
                    _logger.LogInformation("Usuario {Id} actualizado exitosamente. Status: {StatusCode} ({StatusCodeInt})", id, statusCode, statusCodeInt);
                    return true;
                }
                
             
                if (statusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Usuario {Id} no encontrado en el API. Respuesta: {Content}", id, responseContent);
                    return false;
                }
                else if (statusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    _logger.LogWarning("Error de validación al actualizar usuario {Id}. Respuesta: {Content}", id, responseContent);
                 
                    string errorMessage = "Error de validación al actualizar el usuario";
                    try
                    {
                        var errorObj = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(responseContent);
                        if (errorObj.TryGetProperty("message", out var message))
                        {
                            errorMessage = message.GetString() ?? errorMessage;
                            _logger.LogWarning("Mensaje de error extraído: {ErrorMessage}", errorMessage);
                        }
                       
                        if (errorObj.TryGetProperty("errors", out var errors))
                        {
                            var errorList = new List<string>();
                            if (errors.ValueKind == System.Text.Json.JsonValueKind.Array)
                            {
                                foreach (var error in errors.EnumerateArray())
                                {
                                    if (error.ValueKind == System.Text.Json.JsonValueKind.String)
                                    {
                                        errorList.Add(error.GetString() ?? "");
                                    }
                                }
                            }
                            if (errorList.Any())
                            {
                                errorMessage = string.Join("; ", errorList);
                            }
                        }
                    }
                    catch (Exception parseEx)
                    {
                        _logger.LogWarning(parseEx, "No se pudo deserializar el error. Respuesta: {Content}", responseContent);
                    }
                    throw new InvalidOperationException(errorMessage);
                }
                
                
                _logger.LogError("Error al actualizar usuario {Id}. Status: {StatusCode} ({StatusCodeInt}), Response: {ErrorContent}", 
                    id, statusCode, statusCodeInt, responseContent);
                return false;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al actualizar usuario {Id}. Verifique que el API esté ejecutándose en {BaseUrl}", 
                    id, _httpClient.BaseAddress);
                throw new InvalidOperationException($"No se pudo conectar con el API en {_httpClient.BaseAddress}. Verifique que el servicio esté ejecutándose.", ex);
            }
            catch (InvalidOperationException)
            {
                
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar usuario {Id}: {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<bool> ToggleActivoAsync(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"api/usuarios/{id}/toggle-activo", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado de usuario {Id}", id);
                return false;
            }
        }
    }
}

