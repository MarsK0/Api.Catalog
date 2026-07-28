using Api.Catalog.Application.Entities;
using Api.Catalog.Application.Enums;
using Api.Catalog.Application.Models;
using Api.Catalog.Domain.Enums;

namespace Api.Catalog.Application.Mappers;

public static class UserMapper
{
    public static UserDto Dto(this PlatformUser user)
    {
        return new UserDto(
            user.Id,
            user.Name,
            user.Login,
            user.Email,
            user.PasswordHash,
            user.Status == EPlatformUserStatus.Enabled,
            user.Roles.Select(s => s.RoleInfo).Dto()
        );
    }

    public static UserDto Dto(this Account account)
    {
        return new UserDto(
            account.Person.Id,
            account.Person.Name,
            account.Login,
            account.Person.Email,
            account.PasswordHash,
            (
                account.Status == EAccountStatus.Enabled &&
                account.Person.Status == EPersonStatus.Enabled
            ),
            account.Person.Roles.Select(s => s.RoleInfo).Dto()
        );
    }
}
