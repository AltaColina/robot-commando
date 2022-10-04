using RobotCommando.Models.Expressions;
using RobotCommando.Models.Humanoids;
using RobotCommando.Models.Items;
using RobotCommando.Models.Mechas;
using System.Text.Json.Serialization;

namespace RobotCommando.Models.Pages;

public sealed class Page : Entity
{
    public int Number { get; init; }
    
    public WorldLocation Location { get; init; }
    
    public string Text { get; init; } = null!;
    
    public string? Text2 { get; init; }
    
    [JsonIgnore]
    public bool IsVisited { get; set; }
    
    public List<Choice> Choices { get; init; } = new();
    
    public List<Item>? Items { get; init; }
    
    public List<Robot>? Robots { get; init; }
    
    public List<Monster>? Monsters { get; init; }
    
    public List<Enemy>? Enemies { get; init; }
    
    public List<Effect<Game>>? Effects { get; init; }
}
