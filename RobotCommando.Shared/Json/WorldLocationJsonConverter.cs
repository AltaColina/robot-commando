using System.Text.Json;
using System.Text.Json.Serialization;
using RobotCommando.Models.Pages;

namespace RobotCommando.Json;

public sealed class WorldLocationJsonConverter : JsonConverter<WorldLocation>
{
    private static readonly Dictionary<string, WorldLocation> NameToKey = new()
    {
        ["Unknown"] = WorldLocation.Unknwown,
        ["Current"] = WorldLocation.Current,
        ["Farm"] = WorldLocation.Farm,
        ["Capital City"] = WorldLocation.CapitalCity,
        ["City of Industry"] = WorldLocation.CityOfIndustry,
        ["City of Knowledge"] = WorldLocation.CityOfKnowledge,
        ["City of Pleasure"] = WorldLocation.CityOfPleasure,
        ["City of Storms"] = WorldLocation.CityOfStorms,
        ["City of the Guardians"] = WorldLocation.CityOfTheGuardians,
        ["City of the Jungle"] = WorldLocation.CityOfTheJungle,
        ["City of Worship"] = WorldLocation.CityOfWorship
    };

    private static readonly Dictionary<WorldLocation, string> KeyToName = new()
    {
        [WorldLocation.Unknwown] = "Unknown",
        [WorldLocation.Current] = "Current",
        [WorldLocation.Farm] = "Farm",
        [WorldLocation.CapitalCity] = "Capital City",
        [WorldLocation.CityOfIndustry] = "City of Industry",
        [WorldLocation.CityOfKnowledge] = "City of Knowledge",
        [WorldLocation.CityOfPleasure] = "City of Pleasure",
        [WorldLocation.CityOfStorms] = "City of Storms",
        [WorldLocation.CityOfTheGuardians] = "City of the Guardians",
        [WorldLocation.CityOfTheJungle] = "City of the Jungle",
        [WorldLocation.CityOfWorship] = "City of Worship"
    };

    public override WorldLocation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException();
        return NameToKey[reader.GetString()!];
    }

    public override void Write(Utf8JsonWriter writer, WorldLocation value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(KeyToName[value]);
    }
}