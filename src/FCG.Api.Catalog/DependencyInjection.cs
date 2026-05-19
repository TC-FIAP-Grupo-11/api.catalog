using FCG.Lib.Shared.Infrastructure.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace FCG.Api.Catalog;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        var redisConnectionString = configuration["Redis:ConnectionString"];
        if (!string.IsNullOrEmpty(redisConnectionString))
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "fcg-catalog:";
            });
        else
            services.AddDistributedMemoryCache();

        services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
        });

        services.AddJwtAuthentication(configuration);
        services.AddDefaultAuthorization();

        services.AddSwaggerWithJwt("FCG API Catalog", "v1");

        var swaggerBasePath = configuration["SWAGGER_BASE_PATH"];
        if (!string.IsNullOrEmpty(swaggerBasePath))
            services.AddSwaggerGen(c => c.AddServer(new OpenApiServer { Url = swaggerBasePath }));

        services.AddDefaultCors();

        return services;
    }
}
