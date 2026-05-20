using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FCG.Api.Catalog.Application.Contracts.Repositories;
using FCG.Api.Catalog.Application.Contracts.Services;
using FCG.Api.Catalog.Infrastructure.Data.Context;
using FCG.Api.Catalog.Infrastructure.Data.Elasticsearch;
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
        services.AddElasticsearch(configuration);

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

    private static IServiceCollection AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
    {
        var url = configuration["Elasticsearch:Url"];
        if (!string.IsNullOrEmpty(url))
        {
            var settings = new ElasticsearchClientSettings(new Uri(url));

            var username = configuration["Elasticsearch:Username"];
            var password = configuration["Elasticsearch:Password"];
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                settings.Authentication(new Elastic.Transport.BasicAuthentication(username, password));

            services.AddSingleton(new ElasticsearchClient(settings));
            services.AddScoped<IGameSearchService, GameSearchService>();
        }
        else
        {
            services.AddSingleton<IGameSearchService, NoOpGameSearchService>();
        }

        return services;
    }
}
