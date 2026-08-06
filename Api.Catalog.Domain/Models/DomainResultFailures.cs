namespace Api.Catalog.Domain.Models;

public static class DomainFailureCodes
{
    public const string Validation = "DOMAIN_VALIDATION";
    public const string EntityNotFound = "ENTITY_NOT_FOUND";
}

public static class DomainResultFailures
{
    public static Failure Validation(string message) =>
        new(DomainFailureCodes.Validation, message);
    public static Failure EntityNotFound(string message) =>
        new(DomainFailureCodes.EntityNotFound, message);
}