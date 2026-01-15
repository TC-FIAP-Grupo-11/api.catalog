using MediatR;
using FCG.Api.Catalog.Application.Contracts.Repositories;
using FCG.Lib.Shared.Application.Common.Models;
using FCG.Api.Catalog.Domain.Entities;

namespace FCG.Api.Catalog.Application.Queries.Promotions.GetAllPromotions;

public class GetAllPromotionsQueryHandler(IPromotionRepository promotionRepository) : IRequestHandler<GetAllPromotionsQuery, Result<PagedResult<Promotion>>>
{
    private readonly IPromotionRepository _promotionRepository = promotionRepository;

    public async Task<Result<PagedResult<Promotion>>> Handle(GetAllPromotionsQuery request, CancellationToken cancellationToken)
    {
        var pagedResult = await _promotionRepository.GetAllPagedAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return Result.Success(pagedResult);
    }
}
