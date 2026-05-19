using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FCG.Api.Catalog.Application.Contracts.Repositories;
using FCG.Api.Catalog.Infrastructure.Data.Context;
using FCG.Api.Catalog.Infrastructure.Data.MongoDB;
using FCG.Api.Catalog.Infrastructure.Data.Repositories;
using FCG.Lib.Shared.Infrastructure.DependencyInjection;

namespace FCG.Api.Catalog.Infrastructure.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabaseInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSqlServerDatabase<ApplicationDbContext>(configuration);
        services.AddRepositories();
        services.AddMongoDb(configuration);

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IPromotionRepository, PromotionRepository>();

        return services;
    }

    private static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["MongoDB:ConnectionString"];
        if (!string.IsNullOrEmpty(connectionString))
        {
            services.AddSingleton<MongoDbContext>();
            services.AddScoped<IGameReviewRepository, GameReviewRepository>();
        }
        else
        {
            services.AddSingleton<IGameReviewRepository, InMemoryGameReviewRepository>();
        }

        return services;
    }
}
