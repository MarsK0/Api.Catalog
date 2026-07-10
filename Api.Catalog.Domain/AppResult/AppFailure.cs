namespace Api.Catalog.Domain;

public static class FailureCode
{
    public const string Exception = "EXCEPTION";
    public const string DomainValidation = "DOMAIN_VALIDATION";
    public const string ApplicationValidation = "APPLICATION_VALIDATION";
    public const string InfrastructureValidation = "INFRASTRUCTURE_VALIDATION";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Conflict = "CONFLICT";
    public const string InvalidRequest = "INVALID_REQUEST";
    public const string EntityNotFound = "ENTITY_NOT_FOUND";
}

public sealed record AppFailure(string Code, string Message)
{
    public static AppFailure FromException =>
        new(FailureCode.Exception, "Ocorreu um erro inesperado.");
    public static AppFailure DomainValidation(string message) =>
        new(FailureCode.DomainValidation, message);
    public static AppFailure ApplicationValidation(string message) =>
        new(FailureCode.ApplicationValidation, message);
    public static AppFailure InfrastructureValidation(string message) =>
        new(FailureCode.InfrastructureValidation, message);
    public static AppFailure AuthValidation(string message) =>
        new(FailureCode.Unauthorized, message);
    public static AppFailure Conflict(string message) =>
        new(FailureCode.Conflict, message);
    public static AppFailure InvalidRequest(string message) =>
        new(FailureCode.InvalidRequest, message);
    public static AppFailure EntityNotFound(string message) =>
        new(FailureCode.EntityNotFound, message);
}