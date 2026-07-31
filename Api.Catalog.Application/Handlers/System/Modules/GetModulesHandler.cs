using Api.Catalog.Application.Models;
using Api.Catalog.Domain;
using Api.Catalog.Domain.Models;
using Mediator;

namespace Api.Catalog.Application.Handlers;

internal sealed class GetModulesHandler : IRequestHandler<GetModulesQuery, AppResult<IEnumerable<string>>>
{
    public ValueTask<AppResult<IEnumerable<string>>> Handle(GetModulesQuery query, CancellationToken _)
    {
        AppResult<IEnumerable<string>> result = Modules.All.ToList();
        return ValueTask.FromResult(result);
    }
}
