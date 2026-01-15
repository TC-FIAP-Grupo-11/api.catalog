using FCG.Api.Catalog.Application.Contracts.Users;
using FCG.Api.Catalog.Infrastructure.ExternalServices.UsersApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Api.Catalog.Infrastructure.ExternalServices;

/// <summary>
/// Configuração de injeção de dependência para serviços externos
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind User API Settings
        services.Configure<UserApiSettings>(options =>
            configuration.GetSection("ExternalServices:UserApi").Bind(options));

        // Configuração do HttpClient para a API de Usuários
        var userApiSettings = configuration.GetSection("ExternalServices:UserApi").Get<UserApiSettings>()
            ?? throw new InvalidOperationException("Configurações da API de Usuários não encontradas");

        if (string.IsNullOrWhiteSpace(userApiSettings.BaseUrl))
            throw new InvalidOperationException("URL base da API de Usuários não configurada");

        services.AddHttpClient<IUserApiService, UserApiService>(client =>
        {
            client.BaseAddress = new Uri(userApiSettings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(userApiSettings.TimeoutSeconds);
        });

        return services;
    }
}
