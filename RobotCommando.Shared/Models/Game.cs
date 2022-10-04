using RobotCommando.Messaging;
using RobotCommando.Models.Humanoids;
using RobotCommando.Models.Items;
using RobotCommando.Models.Mechas;
using RobotCommando.Models.Pages;

namespace RobotCommando.Models;

public sealed class Game
{
    private Robot _robot = null!;
    private Page _page = null!;

    public Player Player { get; init; } = null!;

    public Inventory Inventory { get; init; } = null!;

    public Die Die { get; init; } = null!;
    
    public Robot Robot { get => _robot; set { _robot = value; RobotChanged?.Invoke(this, new EventArgs<Robot>(value)); } }

    public Page Page { get => _page; set { _page = value; PageChanged?.Invoke(this, new EventArgs<Page>(value)); } }



    public event EventHandler<EventArgs<Page>>? PageChanged;
    public event EventHandler<EventArgs<Robot>>? RobotChanged;
}
