using System.Text.Json.Serialization;
using RobotCommando.Json;

namespace RobotCommando.Models.Pages;

[JsonConverter(typeof(WorldLocationJsonConverter))]
public enum WorldLocation
{
    Unknwown,
    Current,
    CapitalCity,
    CityOfIndustry,
    CityOfKnowledge,
    CityOfPleasure,
    CityOfStorms,
    CityOfTheGuardians,
    CityOfTheJungle,
    CityOfWorship,
}
