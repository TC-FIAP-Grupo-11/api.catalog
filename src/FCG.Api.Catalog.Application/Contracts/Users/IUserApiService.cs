namespace FCG.Api.Catalog.Application.Contracts.Users;

/// <summary>
/// Service para comunicação com a API de Usuários (FCG.Api.Users)
/// </summary>
public interface IUserApiService
{
    /// <summary>
    /// Obtém um usuário por email da API de Usuários
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do usuário ou null se não encontrado</returns>
    Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
