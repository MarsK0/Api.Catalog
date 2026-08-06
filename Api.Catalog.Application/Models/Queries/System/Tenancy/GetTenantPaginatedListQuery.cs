using Api.Catalog.Domain;
using Api.Catalog.Domain.Entities;
using Mediator;

namespace Api.Catalog.Application.Models;

public record GetTenantPaginatedListQuery(
    int PageIndex,
    int PageSize,
    string? Search = null
) : PaginatedQueryDto(PageIndex, PageSize), IRequest<Result<PaginatedResponseDto<Tenant>>>;