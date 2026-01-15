using System.Net.Http.Json;
using FCG.Application.Contracts.Users;
using Microsoft.Extensions.Logging;

namespace FCG.Infrastructure.ExternalServices.UsersApi;

/// <summary>
/// Implementação do serviço de comunicação com a API de Usuários
/// </summary>
public class UserApiService : IUserApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserApiService> _logger;

    public UserApiService(HttpClient httpClient, ILogger<UserApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<UserDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Buscando usuário {UserId} na API de Usuários", userId);

            var response = await _httpClient.GetAsync($"api/users/{userId}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Usuário {UserId} não encontrado na API de Usuários", userId);
                    return null;
                }

                _logger.LogError("Erro ao buscar usuário {UserId}. Status: {StatusCode}", userId, response.StatusCode);
                response.EnsureSuccessStatusCode();
            }

            var user = await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken);
            _logger.LogInformation("Usuário {UserId} encontrado com sucesso", userId);
            
            return user;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de comunicação ao buscar usuário {UserId}", userId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao buscar usuário {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Verificando existência do usuário {UserId}", userId);

            var response = await _httpClient.GetAsync($"api/users/{userId}/exists", cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }

            response.EnsureSuccessStatusCode();
            
            var exists = await response.Content.ReadFromJsonAsync<bool>(cancellationToken);
            _logger.LogInformation("Usuário {UserId} existe: {Exists}", userId, exists);
            
            return exists;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de comunicação ao verificar existência do usuário {UserId}", userId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao verificar existência do usuário {UserId}", userId);
            throw;
        }
    }
}
