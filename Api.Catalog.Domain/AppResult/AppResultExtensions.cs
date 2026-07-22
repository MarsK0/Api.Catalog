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
    public static async Task FoldAsync<TValue>(
        this Task<AppResult<TValue>> resultTask,
        Func<TValue, Task> onSuccess,
        Func<AppFailure, Task> onFailure
    )
    {
        var result = await resultTask;
        if (result.IsSuccess)
            await onSuccess(result.Value);
        else
            await onFailure(result.Failure);
    }
    public static async Task<TResult> FoldAsync<TValue, TResult>(
        this Task<AppResult<TValue>> resultTask,
        Func<TValue, Task<TResult>> onSuccess,
        Func<AppFailure, Task<TResult>> onFailure
    )
    {
        var result = await resultTask;
        return result.IsSuccess
            ? await onSuccess(result.Value)
            : await onFailure(result.Failure);
    }
}