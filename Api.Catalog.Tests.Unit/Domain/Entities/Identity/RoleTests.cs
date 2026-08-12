using Api.Catalog.Domain.Entities;
using Api.Catalog.Domain.ValueObjects;
using Api.Catalog.Tests.Unit.Domain.ValueObjects;
using Bogus;

namespace Api.Catalog.Tests.Unit.Domain.Entities.Identity;

public class RoleTests
{
    private static readonly Faker faker = new();
    public static Role CreateValid() => Role.Create(RoleInfoTests.CreateValid()).Value;
}
