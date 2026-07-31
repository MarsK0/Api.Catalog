namespace Api.Catalog.Application.Helpers;

public readonly struct OptionalField<TValue>
{
    public bool IsSet { get; }
    private readonly TValue _value;
    public readonly TValue Value
        => IsSet ? _value : throw new InvalidOperationException("Não é possível acessar o valor de um campo opcional onde \"IsSet\" é false.");

    private OptionalField(bool isSet, TValue value)
    {
        IsSet = isSet;
        _value = value;
    }

    public static OptionalField<TValue> Unset() => new(false, default!);
    public static OptionalField<TValue> Of(TValue value) => new(true, value);
}