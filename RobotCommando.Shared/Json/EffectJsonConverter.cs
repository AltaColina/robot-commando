using RobotCommando.Models.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RobotCommando.Json;

public sealed class EffectJsonConverter<TContext> : JsonConverter<Effect<TContext>>
{
    public override Effect<TContext>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException();
        return new Effect<TContext> { ExpressionText = reader.GetString()! };
    }

    public override void Write(Utf8JsonWriter writer, Effect<TContext> value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ExpressionText);
    }
}
