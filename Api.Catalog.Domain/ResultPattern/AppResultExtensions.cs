namespace Api.Catalog.Domain;

public static class AppResultExtensions
{
    public static async Task<TResult> FoldAsync<TValue, TResult>(
        this ValueTask<Result<TValue>> resultTask,
        Func<TValue, TResult> onSuccess,
        Func<Failure, TResult> onFailure
    )
    {
        var result = await resultTask;
        return result.IsSuccess
            ? onSuccess(result.Value)
            : onFailure(result.Failure);
    }
    public static async Task FoldAsync<TValue>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, Task> onSuccess,
        Func<Failure, Task> onFailure
    )
    {
        var result = await resultTask;
        if (result.IsSuccess)
            await onSuccess(result.Value);
        else
            await onFailure(result.Failure);
    }
    public static async Task<TResult> FoldAsync<TResult>(
        this ValueTask<Result> resultTask,
        Func<TResult> onSuccess,
        Func<Failure, TResult> onFailure
    )
    {
        var result = await resultTask;
        return result.IsSuccess
            ? onSuccess()
            : onFailure(result.Failure);
    }
}