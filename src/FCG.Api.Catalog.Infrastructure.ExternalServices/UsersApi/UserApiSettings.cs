namespace FCG.Api.Catalog.Infrastructure.ExternalServices.UsersApi;

/// <summary>
/// Configurações para a API de Usuários
/// </summary>
public class UserApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
}
