namespace Api.Catalog.Domain;

public static class AppResultExtensions
{
    public static async Task<TResult> FoldAsync<TValue, TResult>(
        this Task<AppResult<TValue>> resultTask,
        Func<TValue, TResult> onSuccess,
        Func<AppFailure, TResult> onFailure
    )
    {
        var result = await resultTask;
        return result.IsSuccess
            ? onSuccess(result.Value)
            : onFailure(result.Failure);
    }
}