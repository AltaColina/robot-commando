using RobotCommando.Models.Abilities;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace RobotCommando.Json;

public sealed class AbilityJsonConverter : JsonConverter<Ability>
{
    public override Ability? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();
        var node = JsonNode.Parse(ref reader)!.AsObject();
        var tag = node["tag"]!.AsValue().GetValue<string>();
        return tag switch
        {
            nameof(ActiveAbility) => JsonSerializer.Deserialize<ActiveAbility>(node, options),
            nameof(PassiveAbility) => JsonSerializer.Deserialize<PassiveAbility>(node, options),
            nameof(TriggerAbility) => JsonSerializer.Deserialize<TriggerAbility>(node, options),
            _ => throw new JsonException(),
        };
    }

    public override void Write(Utf8JsonWriter writer, Ability value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}