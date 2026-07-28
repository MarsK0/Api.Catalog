using Api.Catalog.Application.Entities;

namespace Api.Catalog.Application.Contracts;

public interface IRefreshTokenRepo
{
    void Add(RefreshToken refreshToken);
    Task<RefreshToken?> GetByHashAsync(string hash, CancellationToken ct, bool track = false);
    Task<List<RefreshToken>> GetByFamilyIdAsync(Guid familyId, CancellationToken ct, bool track = false);
}
