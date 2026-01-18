using System.Net.Http.Json;
using FCG.Api.Catalog.Application.Contracts.Users;
using Microsoft.Extensions.Logging;

namespace FCG.Api.Catalog.Infrastructure.ExternalServices.UsersApi;

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
    public async Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Buscando usuário por email {Email} na API de Usuários", email);

            var response = await _httpClient.GetAsync($"api/accounts/{Uri.EscapeDataString(email)}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Usuário com email {Email} não encontrado na API de Usuários", email);
                    return null;
                }

                _logger.LogError("Erro ao buscar usuário por email {Email}. Status: {StatusCode}", email, response.StatusCode);
                response.EnsureSuccessStatusCode();
            }

            var user = await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken);
            _logger.LogInformation("Usuário com email {Email} encontrado com sucesso", email);
            
            return user;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de comunicação ao buscar usuário por email {Email}", email);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao buscar usuário por email {Email}", email);
            throw;
        }
    }
}
