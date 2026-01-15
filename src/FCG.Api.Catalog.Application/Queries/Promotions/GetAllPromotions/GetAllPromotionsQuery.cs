using FCG.Lib.Shared.Application.Common.Models;
using FCG.Api.Catalog.Domain.Entities;
using MediatR;

namespace FCG.Api.Catalog.Application.Queries.Promotions.GetAllPromotions;

public record GetAllPromotionsQuery(
    int PageNumber = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<Promotion>>>;
