namespace Api.Catalog.Application.Models;

public record UserDto(
    Guid UserId,
    string Name,
    string Login,
    string Email,
    string PasswordHash,
    bool Enabled,
    IEnumerable<RoleInfoDto> Roles
);