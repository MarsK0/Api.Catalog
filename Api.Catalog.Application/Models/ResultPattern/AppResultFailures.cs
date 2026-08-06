using Api.Catalog.Domain;

namespace Api.Catalog.Application;

public static class AppFailureCodes
{
    public const string Validation = "APPLICATION_VALIDATION";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string InvalidRequest = "INVALID_REQUEST";
}

public static class AppResultFailures
{
    public static Failure Validation(string message) =>
        new(AppFailureCodes.Validation, message);
    public static Failure Unauthorized(string message) =>
        new(AppFailureCodes.Unauthorized, message);
    public static Failure InvalidRequest(string message) =>
        new(AppFailureCodes.InvalidRequest, message);
}