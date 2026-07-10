namespace Api.Catalog.Domain;

public class AppResult
{
    private readonly AppFailure? _failure;

    public bool IsSuccess { get; init; }

    public AppFailure Failure => IsSuccess
        ? throw new InvalidOperationException("Não há falha em resultado de sucesso.")
        : _failure!;

    protected AppResult(AppFailure? failure = null)
    {
        IsSuccess = failure is null;
        _failure = failure;
    }

    public static AppResult Success => new();
    public static AppResult Fail(AppFailure failure) => new(failure);

    public static implicit operator AppResult(AppFailure failure) => Fail(failure);
}
public class AppResult<TValue> : AppResult
{
    private readonly TValue? _value;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Não é possivel acessar o valor de um resultado de falha.");

    public AppResult(TValue value) : base()
    {
        _value = value;
    }
    public AppResult(AppFailure failure) : base(failure) { }

    public static new AppResult<TValue> Success(TValue value) => new(value);
    public static new AppResult<TValue> Fail(AppFailure failure) => new(failure);

    public static implicit operator AppResult<TValue>(TValue value) => new(value);
    public static implicit operator AppResult<TValue>(AppFailure failure) => new(failure);
}