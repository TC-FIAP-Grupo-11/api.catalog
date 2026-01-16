FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG GITHUB_USERNAME
ARG GITHUB_TOKEN

WORKDIR /src

# Adicionar fonte do GitHub Packages
RUN dotnet nuget add source "https://nuget.pkg.github.com/TC-FIAP-Grupo-11/index.json" --name "TC-FIAP-Grupo-11" --username $GITHUB_USERNAME --password $GITHUB_TOKEN --store-password-in-clear-text

# Copiar projetos do Catalog
COPY ["FCG.Api.Catalog/src/FCG.Api.Catalog/FCG.Api.Catalog.csproj", "FCG.Api.Catalog/src/FCG.Api.Catalog/"]
COPY ["FCG.Api.Catalog/src/FCG.Api.Catalog.Application/FCG.Api.Catalog.Application.csproj", "FCG.Api.Catalog/src/FCG.Api.Catalog.Application/"]
COPY ["FCG.Api.Catalog/src/FCG.Api.Catalog.Domain/FCG.Api.Catalog.Domain.csproj", "FCG.Api.Catalog/src/FCG.Api.Catalog.Domain/"]
COPY ["FCG.Api.Catalog/src/FCG.Api.Catalog.Infrastructure.Data/FCG.Api.Catalog.Infrastructure.Data.csproj", "FCG.Api.Catalog/src/FCG.Api.Catalog.Infrastructure.Data/"]
COPY ["FCG.Api.Catalog/src/FCG.Api.Catalog.Infrastructure.ExternalServices/FCG.Api.Catalog.Infrastructure.ExternalServices.csproj", "FCG.Api.Catalog/src/FCG.Api.Catalog.Infrastructure.ExternalServices/"]

RUN dotnet restore "FCG.Api.Catalog/src/FCG.Api.Catalog/FCG.Api.Catalog.csproj"

COPY . .
WORKDIR "/src/FCG.Api.Catalog/src/FCG.Api.Catalog"
RUN dotnet build "FCG.Api.Catalog.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FCG.Api.Catalog.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FCG.Api.Catalog.dll"]
