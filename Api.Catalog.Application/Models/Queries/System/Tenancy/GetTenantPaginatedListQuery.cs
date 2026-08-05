using Api.Catalog.Application.Helpers;
using Api.Catalog.Domain;
using Api.Catalog.Domain.Entities;
using Mediator;

namespace Api.Catalog.Application.Models;

public record GetTenantPaginatedListQuery(
    int PageIndex,
    int PageSize,
    string? Search = null,
    List<SortParam>? Sort = null
) : PaginatedQueryDto(PageIndex, PageSize), IRequest<AppResult<PaginatedResponseDto<Tenant>>>;