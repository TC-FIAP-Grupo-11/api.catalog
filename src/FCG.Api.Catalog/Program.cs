using FCG.Api.Catalog;
using FCG.Lib.Shared.Infrastructure.Middlewares;
using FCG.Api.Catalog.Infrastructure.Data.Context;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using FCG.Api.Catalog.Infrastructure.Data;
using FCG.Api.Catalog.Application;
using FCG.Api.Catalog.Infrastructure.ExternalServices;

var builder = WebApplication.CreateBuilder(args);

// API Layer - Controllers + Auth + Swagger + CORS
builder.Services.AddApiServices(builder.Configuration);

// FluentValidation Auto Validation
builder.Services.AddFluentValidationAutoValidation();

// Application Layer - CQRS + Validation
builder.Services.AddApplicationServices();

// Infrastructure - Database and Repositories
builder.Services.AddDatabaseInfrastructure(builder.Configuration);

// Infrastructure - External Services
builder.Services.AddExternalServices(builder.Configuration);

var app = builder.Build();

// Database Migration
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    
    if (app.Environment.IsDevelopment())
    {
        await context.Database.MigrateAsync();
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FCG API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseMiddleware<ExceptionMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
