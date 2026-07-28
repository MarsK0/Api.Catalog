using Api.Catalog.Application.Entities;

namespace Api.Catalog.Application.Contracts;

public interface IPlatformUserRepo
{
    Task<PlatformUser?> FindByUserIdAsync(Guid userId, CancellationToken ct, bool includes = true, bool track = false);
    Task<PlatformUser?> FindByLoginAsync(string login, CancellationToken ct, bool includes = true, bool track = false);
}
