using Api.Catalog.Application.Helpers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api.Catalog.Api.Helpers;

internal sealed class OptionalFieldJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(OptionalField<>);
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var valueType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(OptionalFieldJsonConverter<>).MakeGenericType(valueType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}
internal sealed class OptionalFieldJsonConverter<TValue> : JsonConverter<OptionalField<TValue>>
{
    public override OptionalField<TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = JsonSerializer.Deserialize<TValue>(ref reader, options);
        return OptionalField<TValue>.Of(value!);
    }

    public override void Write(Utf8JsonWriter writer, OptionalField<TValue> value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value.Value, options);
}
