using Microsoft.CodeAnalysis.CSharp.Syntax;
using RobotCommando.Models.Abilities;
using RobotCommando.Models.Expressions;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace RobotCommando.Json;

public sealed class ConditionJsonConverter<TContext> : JsonConverter<Condition<TContext>>
{
    public override Condition<TContext>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException();
        return new Condition<TContext> { ExpressionText = reader.GetString()! };
    }

    public override void Write(Utf8JsonWriter writer, Condition<TContext> value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ExpressionText);
    }
}
