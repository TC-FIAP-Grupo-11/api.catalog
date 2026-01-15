using FCG.Lib.Shared.Application.Common.Models;
using FCG.Api.Catalog.Domain.Entities;
using MediatR;

namespace FCG.Api.Catalog.Application.Queries.Promotions.GetPromotionById;

public record GetPromotionByIdQuery(Guid PromotionId) : IRequest<Result<Promotion>>;
