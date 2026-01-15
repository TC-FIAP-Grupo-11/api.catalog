namespace FCG.Application.Contracts.Users;

/// <summary>
/// Service para comunicação com a API de Usuários (FCG.Api.Users)
/// </summary>
public interface IUserApiService
{
    /// <summary>
    /// Obtém um usuário por ID da API de Usuários
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do usuário ou null se não encontrado</returns>
    Task<UserDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se um usuário existe
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se o usuário existe</returns>
    Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken = default);
}
