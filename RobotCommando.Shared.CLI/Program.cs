using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;
using RobotCommando.Json;
using RobotCommando.Models;
using RobotCommando.Models.Battling;
using RobotCommando.Models.Mechas;
using RobotCommando.Models.Items;
using RobotCommando.Models.Humanoids;
using System.Diagnostics;
using RobotCommando.Shared.CLI;
using RobotCommando.Models.Abilities;
using RobotCommando.Models.Pages;

var defaultSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
{
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    Converters =
    {
        new AbilityJsonConverter(),
        new EffectJsonConverter<Game>(),
        new ConditionJsonConverter<Game>(),
        new EffectJsonConverter<MechaBattle>(),
        new ConditionJsonConverter<MechaBattle>(),
    }
};

var player = new Player
{
    Name = "Player",
    Description = "Mah boy",
    Icon = "",
    StaminaMax = 20,
    Stamina = 20,
    SkillMax = 10,
    Skill = 10,
    LuckMax = 10,
    Luck = 10,
};

var folder = @"D:\Development\robot-commando\RobotCommando.Shared.Tests\Json\Pages\";
var game = new Game
{
    Die = new Die { Sides = 6 },
    Player = player,
};
foreach (var file in Directory.EnumerateFiles(folder, "*.json"))
{
    var json = File.ReadAllText(file);
    var page = JsonSerializer.Deserialize<Page>(json, defaultSerializerOptions)!;
    game.Page = page;

    var redraw = true;
    while (redraw)
    {
        redraw = false;
        Console.WriteLine($"# {page.Number}; Location: {JsonSerializer.Serialize(page.Location, defaultSerializerOptions)}");
        Console.WriteLine(page.Text);
        Console.WriteLine();
        foreach (var choice in page.Choices)
            Console.WriteLine($"- {choice.Text} ({choice.Link})");
        var line = Console.ReadLine();
        if (line is not null && Int32.TryParse(line, out var number) && number < page.Choices.Count)
        {
            var choice = page.Choices[number];
            Console.WriteLine($"Condition: {choice.Condition?.ExpressionText}");
            if (choice.Condition is not null)
                Console.WriteLine($"Codition is met: {choice.Condition.IsTrue(game)}");
            Console.WriteLine($"Effect: {choice.Effect?.ExpressionText}");
            if (choice.Effect is not null)
                choice.Effect.Execute(game);
            Console.ReadLine();
            redraw = true;
        }
        Console.Clear();
    }
}

var robotsJson = File.ReadAllText(@"D:\Development\robot-commando\RobotCommando.Shared.Tests\Json\robots.json");
var monstersJson = File.ReadAllText(@"D:\Development\robot-commando\RobotCommando.Shared.Tests\Json\monsters.json");

var robots = new Dictionary<string, Robot>(JsonSerializer.Deserialize<Dictionary<string, Robot>>(robotsJson, defaultSerializerOptions)!, StringComparer.InvariantCultureIgnoreCase);
var monsters = new Dictionary<string, Monster>(JsonSerializer.Deserialize<Dictionary<string, Monster>>(monstersJson, defaultSerializerOptions)!, StringComparer.InvariantCultureIgnoreCase);

await Task.WhenAll(robots.SelectMany(r => r.Value.Abilities).Concat(monsters.SelectMany(m => m.Value.Abilities)).Select(a => a.CompileAsync()));

var robot = default(Robot);

while (robot is null)
{
    Console.WriteLine("Pick robot (type its name)");
    foreach (var (name, r) in robots)
        Console.WriteLine($"- {name} ({r.Type}) [{r.Armor}, {r.Bonus}, {r.Speed}] {{{String.Join(", ", r.Abilities.Select(a => a.Name))}}}");
    var line = Console.ReadLine();
    if (line is null || !robots.TryGetValue(line, out robot))
        Console.WriteLine("invalid input");
    Console.Clear();
}

var monster = default(Monster);
while (monster is null)
{
    Console.WriteLine("Pick monster (type its name)");
    foreach (var (name, m) in monsters)
        Console.WriteLine($"- {name} ({m.Type}) [{m.Armor}, {m.Skill}, {m.Speed}] {{{String.Join(", ", m.Abilities.Select(a => a.Name))}}}");
    var line = Console.ReadLine();
    if (line is null || !monsters.TryGetValue(line, out monster))
        Console.WriteLine("invalid input");
    Console.Clear();
}
Console.WriteLine($"Selected robot: '{robot.Name}'");
Console.WriteLine($"Selected monster: '{monster.Name}'");

var battle = new MechaBattle
{
    Die = new Die { Sides = 6 },
    Player = new MechaBattler(robot, player),
    Enemy = new MechaBattler(monster),
    PossibleResult = monster.BattleResult,
};

Trace.Listeners.Add(new DebugListener());
do
{
    Console.WriteLine(battle.Phase);
    var line = Console.ReadLine();
    if (!String.IsNullOrWhiteSpace(line))
    {
        if (line == "escape")
        {
            Console.WriteLine($"Escaping..");
            battle.AttemptEscape = battle.CanEscape;
            if (!battle.AttemptEscape)
                Console.WriteLine("Cannot espace right now");
        }
        else if (battle.Phase < BattlePhase.BeforeRoll && battle.Player.Abilities.FirstOrDefault(a => StringComparer.InvariantCultureIgnoreCase.Equals(a.Name, line)) is ActiveAbility ability)
        {
            if (!ability.TryActivate(battle))
                Console.WriteLine($"Failed to activate '{ability.Name}'");
        }
        else
        {
            Console.WriteLine($"Invalid command '{line}'");
        }
    }
}
while (battle.MoveNext());

Console.WriteLine($"Result: {battle.Result}");
