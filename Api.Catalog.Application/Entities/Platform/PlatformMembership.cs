using Api.Catalog.Domain;
using Api.Catalog.Domain.Entities;

namespace Api.Catalog.Application.Entities;

public class PlatformMembership : BaseEntity
{
    public Guid PersonId { get; private set; }
    private Person _person = null!;
    public Person Person => _person;
    private PlatformMembership() { }

    public static AppResult<PlatformMembership> Create(
        Guid personId
    )
    {
        return new PlatformMembership
        {
            PersonId = personId
        };
    }

}
