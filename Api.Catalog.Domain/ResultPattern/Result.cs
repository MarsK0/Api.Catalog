namespace Api.Catalog.Domain;

public record Failure(string Code, string Message);
public class Result
{
    private readonly Failure? _failure;

    public bool IsSuccess { get; init; }

    public Failure Failure => IsSuccess
        ? throw new InvalidOperationException("Não há falha em resultado de sucesso.")
        : _failure!;

    protected Result(Failure? failure = null)
    {
        IsSuccess = failure is null;
        _failure = failure;
    }

    public static Result Success => new();
    public static Result Fail(Failure failure) => new(failure);

    public static implicit operator Result(Failure failure) => Fail(failure);
}
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Não é possível acessar o valor de um resultado de falha.");

    public Result(TValue value) : base()
    {
        _value = value;
    }
    public Result(Failure failure) : base(failure) { }

    public static new Result<TValue> Success(TValue value) => new(value);
    public static new Result<TValue> Fail(Failure failure) => new(failure);

    public static implicit operator Result<TValue>(TValue value) => new(value);
    public static implicit operator Result<TValue>(Failure failure) => new(failure);
}