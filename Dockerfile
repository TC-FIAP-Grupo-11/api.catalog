FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copiar projetos do Catalog
COPY ["src/FCG.Api.Catalog/FCG.Api.Catalog.csproj", "src/FCG.Api.Catalog/"]
COPY ["src/FCG.Api.Catalog.Application/FCG.Api.Catalog.Application.csproj", "src/FCG.Api.Catalog.Application/"]
COPY ["src/FCG.Api.Catalog.Domain/FCG.Api.Catalog.Domain.csproj", "src/FCG.Api.Catalog.Domain/"]
COPY ["src/FCG.Api.Catalog.Infrastructure.Data/FCG.Api.Catalog.Infrastructure.Data.csproj", "src/FCG.Api.Catalog.Infrastructure.Data/"]
COPY ["src/FCG.Api.Catalog.Infrastructure.ExternalServices/FCG.Api.Catalog.Infrastructure.ExternalServices.csproj", "src/FCG.Api.Catalog.Infrastructure.ExternalServices/"]

RUN dotnet restore "src/FCG.Api.Catalog/FCG.Api.Catalog.csproj"

COPY . .
WORKDIR "/src/src/FCG.Api.Catalog"
RUN dotnet build "FCG.Api.Catalog.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FCG.Api.Catalog.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FCG.Api.Catalog.dll"]
