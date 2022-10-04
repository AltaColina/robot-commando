using RobotCommando.Json;
using RobotCommando.Models;
using RobotCommando.Models.Abilities;
using RobotCommando.Models.Battling;
using RobotCommando.Models.Expressions;
using RobotCommando.Models.Humanoids;
using RobotCommando.Models.Items;
using RobotCommando.Models.Mechas;
using RobotCommando.Models.Pages;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace RobotCommando.Shared.Tests;

public class Test { public int Value; }
public class Serialization
{
    private static Player CreatePlayer()
    {
        var player = new Player
        {
            Tag = "Player",
            Name = "Player 1",
            Description = "Hero of the story",
            Icon = "avatar_icon.png",
            StaminaMax = 20,
            Stamina = 20,
            SkillMax = 10,
            Skill = 10,
            LuckMax = 11,
            Luck = 11
        };
        return player;
    }

    [Fact]
    public void Compile_Condition()
    {
        var game = new Game { Player = CreatePlayer() };
        Condition<Game> condition = "c.Player.Luck >= 10 && c.Player.StaminaMax <= 24";
        Assert.True(condition.IsTrue(game));
    }

    [Fact]
    public void Compile_Effect()
    {
        var game = new Game { Player = CreatePlayer() };
        Effect<Game> effect = "c.Player.Luck = 0";

        effect.Execute(game);
        Assert.Equal(0, game.Player.Luck);
    }

    private static readonly JsonSerializerOptions DefaultSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
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
            new WorldLocationJsonConverter(),
            new JsonStringEnumConverter(),
        }
    };

    [Fact]
    public void Items()
    {
        var acquireMap = new Dictionary<string, ItemTrigger>()
        {
            ["Interface Transponder"] = new ItemTrigger
            {
                Effect = "c.Player.RobotSkill++"
            },
            ["Luck Amulet"] = new ItemTrigger
            {
                Effect = "c.Player.Luck++"
            }
        };
        var discardMap = new Dictionary<string, ItemTrigger>()
        {
            ["Interface Transponder"] = new ItemTrigger
            {
                Effect = "c.Player.RobotSkill--"
            },
            ["Luck Amulet"] = new ItemTrigger
            {
                Effect = "c.Player.Luck--"
            }
        };
        var useMap = new Dictionary<string, ItemTrigger>
        {
            ["Armor Plate"] = new ItemTrigger
            {
                Condition = "c.Robot is not null && c.Robot.Armor < c.Robot.ArmorMax",
                Effect = "c.Robot.Armor++"
            },
            ["Medikit"] = new ItemTrigger
            {
                Condition = "c.Player.Stamina < c.Player.StaminaMax",
                Effect = "c.Player.Stamina++"
            },
            ["Robot Destroyer"] = new ItemTrigger
            {
                Condition = "c.Robot is not null && c.Robot.Armor > 0",
                Effect = "c.Robot.Armor--"
            },
            ["Seeker Missile"] = new ItemTrigger
            {
                Condition = "c.Robot is not null",
                Effect = "c.Robot.Abilities.Add(new ActiveAbility { Name = \"Seeker Missile\", Description = \"Charges: 1. Use: deal 10 damage to enemy unit.\", Icon = \"\", Usages = 1, Effects = new() { [BattlePhase.BeforeRoll] = \"c.Other.Armor -= 10\" } }"
            }

        };
        var source = @"E:\Users\Cerquido\source\repos\RobotCommando\RobotCommando.Shared\Resources\items.json";
        using var stream = File.OpenRead(source);
        var document = JsonNode.Parse(stream)!;
        var items = new List<Item>();
        foreach (var (tag, obj) in document.Root.AsObject())
        {
            var item = new Item
            {
                Tag = tag,
                Name = (string)obj!["id"]!,
                Description = (string)obj!["description"]!,
                Icon = Path.GetFileName((string)obj!["icon"]!).ToLowerInvariant().Replace(' ', '_'),
                OnAcquire = acquireMap.GetValueOrDefault(tag),
                OnDiscard = discardMap.GetValueOrDefault(tag),
                OnUse = useMap.GetValueOrDefault(tag),
            };
            items.Add(item);
        }
        Console.WriteLine();
        File.WriteAllText(@"D:\Development\robot-commando\RobotCommando.Shared.Tests\Json\items.json", JsonSerializer.Serialize(items.ToDictionary(k => k.Name), DefaultSerializerOptions));
    }

    [Fact]
    public void GetItems()
    {
        var file = @"D:\Development\robot-commando\RobotCommando.Shared.Tests\Json\items.json";

        var items = JsonSerializer.Deserialize<Dictionary<string, Item>>(File.ReadAllText(file), DefaultSerializerOptions);
    }

    [Fact]
    public void Abilities()
    {
        var abilities = new List<Ability>
        {
            // Passives
            new PassiveAbility
            {
                Name = "Disarmed",
                Description = "Disarmed. Cannot attack.",
                Icon = "",
                Effects = new()
                {
                    [BattlePhase.BeforeBattle] = "c.This.Statuses.Add(MechaBattlerStatus.Disarmed)",
                    [BattlePhase.AfterBattle] = "c.This.Status.Remove(MechaBattlerStatus.Disarmed)"
                }
            },
            new PassiveAbility
            {
                Name = "Double Damage",
                Description = "Deals double damage on hit.",
                Icon = "",
                Effects = new()
                {
                    [BattlePhase.BeforeBattle] = "c.This.Damage *= 2",
                    [BattlePhase.AfterBattle] = "c.This.Damage /= 2"
                }
            },
            new PassiveAbility
            {
                Name = "Flying",
                Description = "Can fly.",
                Icon = "",
                Effects = new()
                {
                    [BattlePhase.BeforeBattle] = "c.This.Statuses.Add(MechaBattlerStatus.Flying)",
                    [BattlePhase.AfterBattle] = "c.This.Statuses.Remove(MechaBattlerStatus.Flying)"
                }
            },
            new PassiveAbility
            {
                Name = "Retaliate",
                Description = "At the end of each round, deals 1 damage to the enemy robot.",
                Icon = "",
                Effects = new()
                {
                    [BattlePhase.AfterRound] = "c.Other.Armor -= 1"
                }
            },

            // Trigger
            new TriggerAbility
            {
                Name = "Acceleration",
                Description = "Whenever roll exceeds enemy's roll by 4 or more, automatically win the next round.",
                Icon = "",
                Phase = BattlePhase.AfterRoll,
                Condition = "c.This.Roll >= c.Other.Roll + 4",
                Effects = new()
                {
                    [BattlePhase.AfterRound] = "c.This.Statuses.Add(MechaBattlerStatus.Accelerated)"
                }
            },
            new TriggerAbility
            {
                Name = "Anti-Flying",
                Description = "+3 skill versus flying units.",
                Icon = "",
                Phase = BattlePhase.BeforeBattle,
                Condition = "c.Other.Abilities.Any(a => a.Name == \"Flying\")",
                Effects = new()
                {
                    [BattlePhase.BeforeBattle] = "c.This.Skill += 3",
                    [BattlePhase.AfterBattle] = "c.This.Skill -= 3"
                }
            },
            new TriggerAbility
            {
                Name = "Entangle",
                Description = "Whenever roll is greater than 15, entangle target non-flying enemy. Entagled units take 1 damage each round.",
                Icon = "",
                Phase = BattlePhase.AfterRoll,
                Condition = "c.This.Roll > 15 && c.Other.Types.HasFlag(MechaTypes.Humanoid)",
                Effects = new()
                {
                    [BattlePhase.AfterRoll] = "c.Other.Statuses.Add(MechaBattlerStatus.Entangled)"
                }
            },
            new TriggerAbility
            {
                Name = "Knockdown",
                Description = "Whenever this robot hits a humanoid unit, knock it down. Knock down units deal no damage in the next turn.",
                Icon = "",
                Phase = BattlePhase.AfterHit,
                Condition = "c.Other.Types.HasFlag(MechaTypes.Humanoid)",
                Effects = new()
                {
                    [BattlePhase.AfterHit] = "c.Other.Statuses.Add(MechaBattlerStatus.KnockedDown)"
                }
            },
            new TriggerAbility
            {
                Name = "Momentum",
                Description = "Whenever roll exceeds enemy's roll by 4 or more, deal 1 extra damage that cannot be reduced.",
                Icon = "",
                Phase = BattlePhase.AfterRoll,
                Condition = "c.This.Roll > Other.Roll",
                Effects = new()
                {
                    [BattlePhase.BeforeHit] = "c.This.Damage += 1",
                    [BattlePhase.AfterHit] = "c.This.Damage -= 1"
                }
            },
            new TriggerAbility
            {
                Name = "Skilled Combat",
                Description = "Whenever roll is greater than 17, take no damage this turn.",
                Icon = "",
                Phase = BattlePhase.AfterRoll,
                Condition = "c.This.Roll > 17",
                Effects = new()
                {
                    [BattlePhase.AfterRoll] = "c.This.Statuses.Add(MechaBattlerStatus.Invencible)"
                }
            },
            new TriggerAbility
            {
                Name = "Sonic Screamer",
                Description = "-1 skill to all enemy dinossaurs.",
                Icon = "",
                Phase = BattlePhase.BeforeHit,
                Condition = "c.Other.Types.HasFlag(MechaTypes.Dinossaur)",
                Effects = new()
                {
                    [BattlePhase.BeforeBattle] = "c.Other.Skill -= 1;",
                    [BattlePhase.AfterBattle] = "c.Other.Skill += 1;"
                }
            },
            new TriggerAbility
            {
                Name = "Unfamiliar Controls",
                Description = "Whenever roll is less than 7, eject controller.",
                Condition = "c.This.Roll < 7",
                Effects = new()
                {
                    [BattlePhase.AfterRoll] = "c.This.Armor = 0"
                }
            },

            // Manual
            new ActiveAbility
            {
                Name = "Powerful Strike",
                Description = "Activate: -2 to roll. +4 damage on hit.",
                Icon = "",
                Usages = -1,
                Effects = new()
                {
                    [BattlePhase.AfterRoll] = "c.This.Roll -= 2",
                    [BattlePhase.BeforeHit] = "c.This.Damage = 6"
                }
            },
            new ActiveAbility
            {
                Name = "Seeker Missile",
                Description = "Charges: 1. Use: deal 10 damage to enemy unit.",
                Icon = "",
                Usages = 1,
                Effects = new()
                {
                    [BattlePhase.BeforeRoll] = "c.Other.Armor -= 10"
                }
            },
            new ActiveAbility
            {
                Name = "Sonic Gun",
                Description = "Charges: 3. Deal 1d6 damage to target robot or 2d6 damage to target dinossaur.",
                Icon = "",
                Usages = 3,
                Effects = new()
                {
                    [BattlePhase.BeforeRoll] = "c.Other.Armor -= c.Other.Types.HasFlag(MechaTypes.Dinossaur) ? c.Die.Roll(2) : c.Die.Roll()"
                }
            }

            // Special

        };

        File.WriteAllText(@"D:\Development\robot-commando\RobotCommando.Shared.Tests\Json\abilities.json", JsonSerializer.Serialize(abilities.ToDictionary(a => a.Name), DefaultSerializerOptions));
    }

    [Fact]
    public void GetAbilities()
    {
        var file = @"D:\Development\robot-commando\RobotCommando.Shared.Tests\Json\abilities.json";

        var abilities = JsonSerializer.Deserialize<Dictionary<string, Ability>>(File.ReadAllText(file), DefaultSerializerOptions);
    }

    [Fact]
    public void Robots()
    {
        var abilities = JsonSerializer.Deserialize<Dictionary<string, Ability>>(File.ReadAllText(@"D:\Development\robot-commando\RobotCommando.Shared.Tests\Json\abilities.json"), DefaultSerializerOptions)!;
        var robots = new List<Robot>
        {
            new Robot
            {
                Name = "Cargo Cab",
                Description = "",
                Icon = "",
                ArmorMax = 12,
                Armor = 12,
                BonusMax = 0,
                Bonus = 0,
                SpeedMax = MechaSpeed.Slow,
                Speed = MechaSpeed.Slow,
                Abilities = null!,
            },
            new Robot
            {
                Name = "Control Robot",
                Description = "",
                Icon = "",
                ArmorMax = 9,
                Armor = 9,
                BonusMax = 0,
                Bonus = 0,
                SpeedMax = MechaSpeed.Slow,
                Speed = MechaSpeed.Slow,
                Abilities = null!,
            },
            new Robot
            {
                Name = "Cowboy",
                Description = "",
                Icon = "",
                Type = MechaType.Humanoid,
                ArmorMax = 10,
                Armor = 10,
                BonusMax = 0,
                Bonus = 0,
                SpeedMax = MechaSpeed.Average,
                Speed = MechaSpeed.Average,
                Abilities = null!,
            },
            new Robot
            {
                Name = "Digger",
                Description = "",
                Icon = "",
                ArmorMax = 16,
                Armor = 16,
                BonusMax = 0,
                Bonus = 0,
                SpeedMax = MechaSpeed.Slow,
                Speed = MechaSpeed.Slow,
                Abilities = { abilities["Powerful Strike"] }
            },
            new Robot
            {
                Name = "Dragonfly",
                Description = "",
                Icon = "",
                ArmorMax = 5,
                Armor = 5,
                BonusMax = 0,
                Bonus = 0,
                SpeedMax = MechaSpeed.UltraFast,
                Speed = MechaSpeed.UltraFast,
                Abilities = { abilities["Flying"] }
            },
            new Robot
            {
                Name = "Escape Robot",
                Description = "",
                Icon = "",
                ArmorMax = 1,
                Armor = 1,
                BonusMax = 0,
                Bonus = 0,
                SpeedMax = MechaSpeed.VeryFast,
                Speed = MechaSpeed.VeryFast,
            },
            new Robot
            {
                Name = "Farm Robot",
                Description = "",
                Icon = "",
                ArmorMax = 7,
                Armor = 7,
                BonusMax = 0,
                Bonus = 0,
                SpeedMax = MechaSpeed.Average,
                Speed = MechaSpeed.Average,
            },
            new Robot
            {
                Name = "Hedgehog",
                Description = "",
                Icon = "",
                ArmorMax = 8,
                Armor = 8,
                BonusMax = 0,
                Bonus = 0,
                SpeedMax = MechaSpeed.Slow,
                Speed = MechaSpeed.Slow,
                Abilities = { abilities["Anti-Flying"] }
            },
            new Robot
            {
                Name = "Myrmidon",
                Description = "",
                Type = MechaType.Humanoid,
                Icon = "",
                ArmorMax = 12,
                Armor = 12,
                BonusMax = 1,
                Bonus = 1,
                SpeedMax = MechaSpeed.Average,
                Speed = MechaSpeed.Average,
                Abilities = { }
            },
            new Robot
            {
                Name = "Robot Car",
                Description = "",
                Icon = "",
                ArmorMax = 1,
                Armor = 1,
                BonusMax = 0,
                Bonus = 0,
                SpeedMax = MechaSpeed.Fast,
                Speed = MechaSpeed.Fast,
                Abilities = { abilities["Disarmed"] }
            },
            new Robot
            {
                Name = "Robotank",
                Description = "",
                Icon = "",
                ArmorMax = 18,
                Armor = 18,
                BonusMax = 2,
                Bonus = 2,
                SpeedMax = MechaSpeed.Slow,
                Speed = MechaSpeed.Slow,
                Abilities = { abilities["Sonic Gun"] }
            },
            new Robot
            {
                Name = "Serpent VII",
                Description = "",
                Icon = "",
                ArmorMax = 9,
                BonusMax = 1,
                SpeedMax = MechaSpeed.Fast,
                Abilities = { abilities["Entangle"] }
            },
            new Robot
            {
                Name = "Stilter",
                Description = "",
                Icon = "",
                ArmorMax = 4,
                BonusMax = -1,
                SpeedMax = MechaSpeed.Average,
            },
            new Robot
            {
                Name = "Super Cowboy",
                Description = "",
                Icon = "",
                ArmorMax = 14,
                Armor = 14,
                BonusMax = 1,
                Bonus = 1,
                SpeedMax = MechaSpeed.Average,
                Speed = MechaSpeed.Average,
            },
            new Robot
            {
                Name = "Trooper XI",
                Description = "",
                Icon = "",
                Type = MechaType.Humanoid,
                ArmorMax = 12,
                Armor = 12,
                BonusMax = 2,
                Bonus = 2,
                SpeedMax = MechaSpeed.Average,
                Speed = MechaSpeed.Average,
                Abilities = { }
            },
            new Robot
            {
                Name = "Utility Robot",
                Description = "",
                Icon = "",
                ArmorMax = 4,
                Armor = 4,
                BonusMax = -2,
                Bonus = -2,
                SpeedMax = MechaSpeed.Slow,
                Speed = MechaSpeed.Slow,
            },
            new Robot
            {
                Name = "Walker",
                Description = "",
                Icon = "",
                ArmorMax = 6,
                Armor = 6,
                BonusMax = -1,
                Bonus = -1,
                SpeedMax = MechaSpeed.Fast,
                Speed = MechaSpeed.Fast,
            },
            new Robot
            {
                Name = "Wasp Fighter",
                Description = "",
                Icon = "",
                ArmorMax = 6,
                Armor = 6,
                BonusMax = 2,
                Bonus = 2,
                SpeedMax = MechaSpeed.VeryFast,
                Speed = MechaSpeed.VeryFast,
                Abilities = { abilities["Acceleration"], abilities["Flying"] }
            }
        };

        File.WriteAllText(@"D:\Development\robot-commando\RobotCommando.Shared.Tests\Json\robots.json", JsonSerializer.Serialize(robots.ToDictionary(r => r.Name), DefaultSerializerOptions));
    }

    [Fact]
    public void GetRobots()
    {
        var file = @"D:\Development\robot-commando\RobotCommando.Shared.Tests\Json\robots.json";

        var abilities = JsonSerializer.Deserialize<Dictionary<string, Robot>>(File.ReadAllText(file), DefaultSerializerOptions);
    }

    [Fact]
    public void Monsters()
    {
        var abilities = JsonSerializer.Deserialize<Dictionary<string, Ability>>(File.ReadAllText(@"D:\Development\robot-commando\RobotCommando.Shared.Tests\Json\abilities.json"), DefaultSerializerOptions)!;
        var monsters = new List<Monster>
        {
            new Monster
            {
                Name = "Air Fighter",
                Description = "",
                Icon = "",
                ArmorMax = 7,
                Armor = 7,
                SkillMax = 9,
                Skill = 9,
                SpeedMax = MechaSpeed.Fast,
                Speed = MechaSpeed.Fast,
                Abilities = { abilities["Flying"] },
                BattleResult = { Win = 113, Lose = 136 }
            },
            new Monster
            {
                Name = "Ankylosaurus",
                Description = "",
                Icon = "",
                Type = MechaType.Dinossaur,
                ArmorMax = 12,
                Armor = 12,
                SkillMax = 9,
                Skill = 9,
                SpeedMax = MechaSpeed.Slow,
                Speed = MechaSpeed.Slow,
                Abilities = { abilities["Knockdown"] },
                BattleResult = { Win = 56, Escape = 115 }
            },
            new Monster
            {
                Name = "Battleman",
                Description = "",
                Icon = "",
                Type = MechaType.Humanoid,
                ArmorMax = 11,
                Armor = 11,
                SkillMax = 11,
                Skill = 11,
                SpeedMax = MechaSpeed.Average,
                Speed = MechaSpeed.Average,
                Abilities =  { abilities["Momentum"] },
                BattleResult = { Win = 290, Lose = 61 }
            },
            new Monster
            {
                Name = "Control Robot",
                Description = "",
                Icon = "",
                ArmorMax = 9,
                Armor = 9,
                SkillMax = 6,
                Skill = 6,
                SpeedMax = MechaSpeed.Slow,
                Speed = MechaSpeed.Slow,
                Abilities = { abilities["Unfamiliar Controls"] },
                BattleResult = { Win = 92, Lose = 61, Escape = 216 }
            },
            new Monster
            {
                Name = "Crusher",
                Description = "",
                Icon = "",
                ArmorMax = 14,
                Armor = 14,
                SkillMax = 8,
                Skill = 8,
                SpeedMax = MechaSpeed.Slow,
                Speed = MechaSpeed.Slow,
                Abilities = { abilities["Double Damage"] },
                BattleResult = { Win = 275, Lose = 384 }
            },
            new Monster
            {
                Name = "Iguanodon",
                Description = "",
                Icon = "",
                Type = MechaType.Dinossaur,
                ArmorMax = 5,
                Armor = 5,
                SkillMax = 9,
                Skill = 9,
                SpeedMax = MechaSpeed.Average,
                Speed = MechaSpeed.Average,
                BattleResult = { Win = 137, Lose = 306 }
            },
            new Monster
            {
                Name = "Man-Trap Plant",
                Description = "",
                Icon = "",
                ArmorMax = 8,
                Armor = 8,
                SkillMax = 9,
                Skill = 9,
                SpeedMax = MechaSpeed.Static,
                Speed = MechaSpeed.Static,
                BattleResult = { Win = 97, Lose = 141 }
            },
            new Monster
            {
                Name = "Mini-Robots",
                Description = "",
                Icon = "",
                ArmorMax = 9,
                Armor = 9,
                SkillMax = 10,
                Skill = 10,
                SpeedMax = MechaSpeed.Fast,
                Speed = MechaSpeed.Fast,
                Abilities = { abilities["Flying"] },
                BattleResult = { Win = 166, Lose = 278 }
            },
            new Monster
            {
                Name = "Myrmidon",
                Description = "",
                Icon = "",
                Type = MechaType.Humanoid,
                ArmorMax = 10,
                Armor = 10,
                SkillMax = 10,
                Skill = 10,
                SpeedMax = MechaSpeed.Average,
                Speed = MechaSpeed.Average,
                BattleResult = { Win = 192, Lose = 238, Escape = 308 }
            },
            new Monster
            {
                Name = "Nothosaurus",
                Description = "",
                Icon = "",
                Type = MechaType.Dinossaur,
                ArmorMax = 7,
                Armor = 7,
                SkillMax = 9,
                Skill = 9,
                SpeedMax = MechaSpeed.Slow,
                Speed = MechaSpeed.Slow,
                BattleResult = { Win = 51, Lose = 279, Escape = 331 }
            },
            new Monster
            {
                Name = "Pteranodon",
                Description = "",
                Icon = "",
                Type = MechaType.Dinossaur,
                ArmorMax = 3,
                Armor = 3,
                SkillMax = 9,
                Skill = 9,
                SpeedMax = MechaSpeed.Fast,
                Speed = MechaSpeed.Fast,
                Abilities = { abilities["Flying"] },
                BattleResult = { Win = 186, Lose = 116, Escape = 186 }
            },
            new Monster
            {
                Name = "Street Cleaner",
                Description = "",
                Icon = "",
                ArmorMax = 4,
                Armor = 4,
                SkillMax = 7,
                Skill = 7,
                SpeedMax = MechaSpeed.Average,
                Speed = MechaSpeed.Average,
                BattleResult = { Win = 35, Lose = 154 }
            },
            new Monster
            {
                Name = "Robot Tyrannosaurus",
                Description = "",
                Icon = "",
                ArmorMax = 11,
                Armor = 11,
                SkillMax = 10,
                Skill = 10,
                SpeedMax = MechaSpeed.Fast,
                Speed = MechaSpeed.Fast,
                Abilities = { abilities["Flying"] },
                BattleResult = { Win = 230, Lose = 255, Escape = 52 }
            },
            new Monster
            {
                Name = "Supertank",
                Description = "",
                Icon = "",
                ArmorMax = 16,
                Armor = 16,
                SkillMax = 12,
                Skill = 12,
                SpeedMax = MechaSpeed.Slow,
                Speed = MechaSpeed.Slow,
                Abilities = { abilities["Retaliate"] },
                BattleResult = { Win = 354, Lose = 6 }
            },
            new Monster
            {
                Name = "Towing Robot",
                Description = "",
                Icon = "",
                ArmorMax = 6,
                Armor = 6,
                SkillMax = 7,
                Skill = 7,
                SpeedMax = MechaSpeed.Average,
                Speed = MechaSpeed.Average,
                BattleResult = { Win = 35, Lose = 154 }
            },
            new Monster
            {
                Name = "Traffic Copter",
                Description = "",
                Icon = "",
                ArmorMax = 3,
                Armor = 3,
                SkillMax = 8,
                Skill = 8,
                SpeedMax = MechaSpeed.Fast,
                Speed = MechaSpeed.Fast,
                BattleResult = { Win = 35, Lose = 154 }
            },
            new Monster
            {
                Name = "Triceratops",
                Description = "",
                Icon = "",
                Type = MechaType.Dinossaur,
                ArmorMax = 9,
                Armor = 9,
                SkillMax = 9,
                Skill = 9,
                SpeedMax = MechaSpeed.Average,
                Speed = MechaSpeed.Average,
                BattleResult = { Win = 391, Lose = 310, Escape = 270 }
            },
            new Monster
            {
                Name = "Tripod",
                Description = "",
                Icon = "",
                ArmorMax = 7,
                Armor = 7,
                SkillMax = 9,
                Skill = 9,
                SpeedMax = MechaSpeed.Fast,
                Speed = MechaSpeed.Fast,
                BattleResult = { Win = 30, Lose = 336, Escape = 293 }
            },
            new Monster
            {
                Name = "Tylosaurus",
                Description = "",
                Icon = "",
                Type = MechaType.Dinossaur,
                ArmorMax = 9,
                Armor = 9,
                SkillMax = 10,
                Skill = 10,
                SpeedMax = MechaSpeed.Fast,
                Speed = MechaSpeed.Fast,
                BattleResult = { Win = 101 }
            },
            new Monster
            {
                Name = "Tyrannosaurus",
                Description = "",
                Icon = "",
                Type = MechaType.Dinossaur,
                ArmorMax = 8,
                Armor = 8,
                SkillMax = 9,
                Skill = 9,
                SpeedMax = MechaSpeed.Fast,
                Speed = MechaSpeed.Fast,
                BattleResult = { Win = 235, Lose = 258 }
            },
            new Monster
            {
                Name = "Wasp Fighter",
                Description = "",
                Icon = "",
                ArmorMax = 6,
                Armor = 6,
                SkillMax = 11,
                Skill = 11,
                SpeedMax = MechaSpeed.VeryFast,
                Speed = MechaSpeed.VeryFast,
                Abilities = { abilities["Acceleration"], abilities["Flying"] },
                BattleResult = { Win = 35, Lose = 154 }
            }
        };

        File.WriteAllText(@"D:\Development\robot-commando\RobotCommando.Shared.Tests\Json\monsters.json", JsonSerializer.Serialize(monsters.ToDictionary(a => a.Name), DefaultSerializerOptions));
    }

    [Fact]
    public void Pages()
    {
        var folder = @"D:\Development\robot-commando\RobotCommando.Shared.Tests\Json\Pages\";
        var pages = new List<Page>();
        foreach (var file in Directory.EnumerateFiles(folder, "*.json"))
        {
            var json = File.ReadAllText(file);
            var node1 = JsonNode.Parse(json)!;
            var node2 = JsonNode.Parse("{}")!;
            node2.Root["$schema"] = "https://raw.githubusercontent.com/AltaColina/robot-commando/main/RobotCommando.Shared/Json/Schemas/page-schema.json";
            node2.Root["number"] = Int32.Parse(Path.GetFileNameWithoutExtension(file));
            foreach (var property in node1.AsObject().ToList())
            {
                if (property.Key == "$schema")
                    continue;
                node1.Root.AsObject().Remove(property.Key);
                node2.Root.AsObject().Add(property);
            }
            json = node2.ToJsonString(DefaultSerializerOptions);
            File.WriteAllText(file, json);
            var page = JsonSerializer.Deserialize<Page>(json, DefaultSerializerOptions)!;
            pages.Add(page);
        }

        Console.WriteLine(pages.Count);
    }
}