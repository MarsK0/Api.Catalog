namespace Api.Catalog.Application.Models;

public record PaginatedResponseDto<TModel>(IEnumerable<TModel> Items, int TotalCount);