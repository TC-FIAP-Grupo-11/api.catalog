# FCG API Catalog

**Tech Challenge - Fase 4**  
Plataforma de venda de jogos digitais.

> **Este microsserviço faz parte de um sistema maior.**  
> Para executar toda a plataforma (Docker Compose ou Kubernetes), veja: [FCG.Infra.Orchestration](../FCG.Infra.Orchestration/README.md)

## Sobre o Projeto

API REST em .NET 8 para gerenciar catálogo de jogos, promoções e biblioteca pessoal. Na Fase 4, foram adicionadas três camadas de infraestrutura: **cache distribuído (Redis)**, **persistência NoSQL (MongoDB)** e **busca avançada com fuzzy search (Elasticsearch)**.

## Arquitetura

```
FCG.Api.Catalog/                              # Controllers, DI, Middlewares
FCG.Api.Catalog.Application/                 # CQRS (Commands/Queries), Cache, DTOs
FCG.Api.Catalog.Domain/                      # Entidades, Interfaces
FCG.Api.Catalog.Infrastructure.Data/         # EF Core, MongoDB, Elasticsearch, Redis
FCG.Api.Catalog.Infrastructure.ExternalServices/  # HTTP clients (UsersAPI)
```

### Padrões
- **CQRS** via MediatR — commands e queries separados
- **Repository Pattern** — interfaces no Domain, implementações na Infrastructure
- **Cache-aside** — Redis com fallback in-memory para dev local
- **Dual-write** — SQL Server como fonte de verdade; Elasticsearch sincronizado nos commands

## Tecnologias

| Tecnologia | Uso |
|---|---|
| .NET 8 / ASP.NET Core | Framework base |
| SQL Server + EF Core | Persistência principal |
| MongoDB | Avaliações de jogos (NoSQL) |
| Redis | Cache de listagens (TTL 5 min) |
| Elasticsearch / OpenSearch | Busca fuzzy no catálogo |
| AWS Cognito | Autenticação JWT |
| RabbitMQ + MassTransit | Mensageria assíncrona |
| MediatR + FluentValidation | Pipeline CQRS |

## Variáveis de Ambiente

### Obrigatórias

```bash
# Banco de Dados
ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=FCG_Catalog;..."

# Auth
Authentication__JwtBearer__Authority="https://cognito-idp.<REGION>.amazonaws.com/<POOL_ID>"

# RabbitMQ
Messaging__RabbitMQ__Host="localhost"
Messaging__RabbitMQ__Username="guest"
Messaging__RabbitMQ__Password="<senha>"

# Users API
ExternalServices__UserApi__BaseUrl="http://localhost:5001"
ExternalServices__UserApi__TimeoutSeconds="30"
```

### Fase 4 — Novas variáveis

```bash
# Redis (opcional — usa memória local se não configurado)
Redis__ConnectionString="localhost:6379"

# MongoDB (opcional — usa repositório in-memory se não configurado)
MongoDB__ConnectionString="mongodb://localhost:27017"
MongoDB__DatabaseName="fcg_catalog"

# Elasticsearch (opcional — usa NoOp se não configurado)
Elasticsearch__Url="http://localhost:9200"
Elasticsearch__Username=""
Elasticsearch__Password=""
```

## Setup Local

### Pré-requisitos
- .NET 8 SDK
- Docker Desktop

### 1. Subir infraestrutura (SQL Server, RabbitMQ, Redis, MongoDB, Elasticsearch)

```bash
cd FCG.Infra.Orchestration/docker
cp .env.example .env   # preencher credenciais AWS/Cognito
docker compose up -d sqlserver rabbitmq redis mongodb elasticsearch
```

### 2. Rodar a API

```bash
cd src/FCG.Api.Catalog
dotnet run
```

Acesse: http://localhost:5002/swagger

### 3. Rodar testes

```bash
dotnet test
```

## Endpoints Fase 4

### Avaliações (MongoDB)
```
POST /api/reviews              # Criar avaliação de jogo (autenticado)
GET  /api/reviews/game/{id}    # Listar avaliações de um jogo
```

### Busca Avançada (Elasticsearch)
```
GET /api/games/search?q={termo}   # Fuzzy search com relevância
```

Exemplos:
```bash
# Busca exata
GET /api/games/search?q=cyberpunk

# Busca com typo (fuzzy)
GET /api/games/search?q=cyberpunkk

# Busca por gênero
GET /api/games/search?q=rpg
```

Retorna resultados ordenados por `_score` (relevância), apenas jogos ativos.  
Campos indexados: `title^3`, `genre^2`, `description`, `publisher`.

## Kubernetes

Manifests em `k8s/`:

| Arquivo | Descrição |
|---|---|
| `deployment.yaml` | Deployment com 2 réplicas |
| `service.yaml` | Service interno (NLB / API Gateway) |
| `configmap.yaml` | Variáveis não sensíveis |
| `secret.yaml.example` | Template para o Secret K8s |

**Secrets necessários** (`catalog-api-secret`):
- `CONNECTION_STRING` — SQL Server
- `JWT_AUTHORITY` — Cognito
- `REDIS_CONNECTION_STRING` — ElastiCache
- `MONGODB_CONNECTION_STRING` — Atlas M0
- `ELASTICSEARCH_URL` / `ELASTICSEARCH_USERNAME` / `ELASTICSEARCH_PASSWORD` — OpenSearch
- `RABBITMQ_PASSWORD` — RabbitMQ

## CI/CD (GitHub Actions)

| Workflow | Trigger | Ação |
|---|---|---|
| `ci.yml` | push / PR na `main` | Build + testes unitários |
| `cd.yml` | CI passou | Build Docker → push ECR → deploy EKS |

**Secrets obrigatórios no repositório GitHub:**
- `AWS_ACCESS_KEY_ID`
- `AWS_SECRET_ACCESS_KEY`
- `AWS_SESSION_TOKEN` (se AWS Academy)
- `ECR_REGISTRY`
- `EKS_CLUSTER_NAME`

---

## Fase 4 — Novidades

### Cache Distribuído (Redis)
- `GetAllGames` e `GetActiveGames` cacheados com TTL de 5 minutos
- Cache invalidado automaticamente em `CreateGame`, `UpdateGame`, `ActivateGame`, `DeactivateGame`
- Fallback para `IMemoryCache` quando Redis não está configurado (dev local)

### Persistência NoSQL (MongoDB)
- Avaliações de jogos (`GameReview`) armazenadas como documentos no MongoDB
- Validação: jogo deve existir no SQL Server antes de aceitar avaliação
- `InMemoryGameReviewRepository` usado automaticamente em dev local

### Busca Avançada (Elasticsearch)
- Índice `games` criado/atualizado automaticamente ao criar, editar, ativar ou desativar jogos
- Multi-match com `fuzziness: AUTO` — tolera erros de digitação
- `bool` query com `filter` em `isActive: true` — jogos inativos excluídos
- `NoOpGameSearchService` usado automaticamente quando Elasticsearch não configurado
