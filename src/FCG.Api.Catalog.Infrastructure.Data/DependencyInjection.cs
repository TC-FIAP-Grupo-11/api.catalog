using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FCG.Api.Catalog.Application.Contracts.Repositories;
using FCG.Api.Catalog.Infrastructure.Data.Context;
using FCG.Api.Catalog.Infrastructure.Data.Repositories;
using FCG.Lib.Shared.Application.Contracts.Repositories;
using FCG.Lib.Shared.Infrastructure.Data.Repositories;

namespace FCG.Api.Catalog.Infrastructure.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabaseInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext(configuration);

        services.AddRepositories();

        return services;
    }

    private static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);

                sqlOptions.CommandTimeout(30);
            });

            #if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            #endif
        });

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IPromotionRepository, PromotionRepository>();

        return services;
    }
}
