using Api.Catalog.Application.Entities;

namespace Api.Catalog.Application.Contracts;

public interface IRefreshTokenRepo
{
    Task AddAsync(RefreshToken refreshToken, CancellationToken ct);
    Task<RefreshToken?> GetByHashAsync(string hash, CancellationToken ct);
    Task<List<RefreshToken>> GetByFamilyIdAsync(Guid familyId, CancellationToken ct);
}
