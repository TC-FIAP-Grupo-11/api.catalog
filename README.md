# FCG API Catalog

**Tech Challenge - Fase 3**
Plataforma de venda de jogos digitais.

## Sobre o Projeto

A FCG API Catalog é uma API REST desenvolvida em .NET 8 para gerenciar jogos e promoções. Este projeto implementa catálogo de jogos e biblioteca pessoal de jogos adquiridos.

> **⚠️ Este microsserviço faz parte de um sistema maior.**  
> Para executar toda a plataforma (Docker Compose ou Kubernetes), veja: [FCG.Infra.Orchestration](../FCG.Infra.Orchestration/README.md)
 

## Arquitetura

O projeto segue **Clean Architecture** com as seguintes camadas:

```
├── FCG.Api.Catalog/                    # Controllers, Middlewares
├── FCG.Api.Catalog.Application/        # Commands, Queries (CQRS)
├── FCG.Api.Catalog.Domain/             # Entidades, Regras de Negócio
├── FCG.Api.Catalog.Infrastructure.Data/    # EF Core, Repositories
└── FCG.Api.Catalog.Infrastructure.ExternalServices/     # Apis
```

### Padrões Utilizados
- **CQRS** - Separação de Commands e Queries
- **DDD** - Domain-Driven Design
- **Repository Pattern** - Abstração de dados
- **Mediator Pattern** - MediatR
- **Dependency Injection** - Inversão de controle

## Autenticação

Sistema de autenticação via **AWS Cognito** com JWT Bearer:
- **Cadastro** com validação de senha segura
- **Confirmação** de email
- **Login** com token JWT
- **Dois níveis de acesso**: Admin e User

## Tecnologias

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- AWS Cognito
- MediatR + FluentValidation
- xUnit + FluentAssertions

## Variáveis de Ambiente

```bash
# Banco de Dados
ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=FCG_Catalog;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"

# JWT
Authentication__JwtBearer__Authority="https://cognito-idp.<REGION>.amazonaws.com/<USER_POOL_ID>"

# RabbitMQ
Messaging__RabbitMQ__Host="localhost"
Messaging__RabbitMQ__Username="guest"
Messaging__RabbitMQ__Password="guest"
```

## Como Executar

### Localmente
```bash
cd src/FCG.Api.Catalog
dotnet run
```

Acesse: http://localhost:5002/swagger

### Docker
```bash
docker build -t fcg-catalog .
docker run -p 5002:80 fcg-catalog
```

## Funcionalidades

### Jogos
- Cadastro de jogos (Admin)
- Listagem de jogos ativos
- Ativação/Desativação (Admin)
- Compra de jogos (User)
- Biblioteca pessoal

### Promoções
- Criação de promoções com desconto (Admin)
- Validação de período
- Listagem de promoções

## Estrutura do Banco

**Entidades principais:**
- `Games` - Catálogo de jogos
- `UserGames` - Biblioteca (relacionamento N:N)
- `Promotions` - Descontos em jogos

## Domain-Driven Design (DDD)

### Event Storming - Fluxo Principal

![alt text](image.png)

## Testes

Execute os testes unitários:
```bash
dotnet test
```

Testes implementados em:
- `FCG.Domain.Tests` - Entidades e regras de negócio

---

## Fase 3 — Novidades

### Temporal Tables
`Games`, `UserGames` e `Promotions` usam `IsTemporal()` via EF Core — o SQL Server mantém histórico automático das alterações em tabelas `*History`.

### Observabilidade
- **AWS X-Ray**: middleware `app.UseXRay("fcg-catalog-api")` habilitado — rastreamento distribuído via CloudWatch

### CI/CD (GitHub Actions)
- **CI** (`.github/workflows/ci.yml`): build + testes em push/PR na `main`
- **CD** (`.github/workflows/cd.yml`): build Docker → push ECR → `kubectl set image` no EKS

**Secrets obrigatórios no repositório GitHub:**
- `AWS_ACCESS_KEY_ID`
- `AWS_SECRET_ACCESS_KEY`

### Kubernetes
Manifests em `k8s/`:
- `deployment.yaml` — Deployment com 2 réplicas
- `service.yaml` — Service NLB interno (integrado ao AWS API Gateway via VPC Link)
- `configmap.yaml` — Variáveis não sensíveis
- `secret.yaml` — Connection string, Cognito
