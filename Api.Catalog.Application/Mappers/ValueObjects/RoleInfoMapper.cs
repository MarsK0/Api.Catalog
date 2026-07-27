using Api.Catalog.Application.Models;
using Api.Catalog.Domain.ValueObjects;

namespace Api.Catalog.Application.Mappers;

public static class RoleInfoMapper
{
    public static IEnumerable<RoleInfoDto> Dto(this IEnumerable<RoleInfo> roles)
    {
        var dto = new List<RoleInfoDto>();

        foreach (var role in roles)
            dto.Add(role.Dto());

        return dto;
    }
    public static RoleInfoDto Dto(this RoleInfo roleInfo)
    {
        return new RoleInfoDto(
            roleInfo.Name,
            roleInfo.Description,
            roleInfo.Permissions.Select(s => s.Value)
        );
    }
}
