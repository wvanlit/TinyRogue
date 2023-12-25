using System.Linq;
using Godot;
using TinyRogue.Engine;
using static TinyRogue.Engine.Actions;
using static TinyRogue.Engine.Engine;

namespace TinyRogue.Godot;

[GlobalClass]
public partial class RoguelikeEngine : Node
{
    private Types.TinyRogueEngine _engine = CreateEngine();

    [Export] public DungeonMap DungeonMap;
    [Export] public Node2D Player;

    [Signal]
    public delegate void UpdateEventHandler();

    public override void _Ready()
    {
        DungeonMap.Setup(_engine.Dungeon);

        var playerActor = _engine.Actors.First(a => a.role.Equals(Types.Role.Player));
        Player.Position = new Vector2(playerActor.position.x, playerActor.position.y) * 48;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("quick_exit")) GetTree().Quit();
        else if (@event.IsActionPressed("move_up", allowEcho: true)) MovePlayerUp();
        else if (@event.IsActionPressed("move_down", allowEcho: true)) MovePlayerDown();
        else if (@event.IsActionPressed("move_left", allowEcho: true)) MovePlayerLeft();
        else if (@event.IsActionPressed("move_right", allowEcho: true)) MovePlayerRight();
        else if (@event.IsActionPressed("reset_engine", allowEcho: true)) ResetEngine();
    }

    public void PlayerAction(Action playerAction)
    {
        _engine = Core.ExecutePlayerAction(_engine, playerAction);

        var playerActor = _engine.Actors.First(a => a.role.Equals(Types.Role.Player));
        Player.Position = new Vector2(playerActor.position.x, playerActor.position.y) * 48;
    }

    public void MovePlayerUp() => PlayerAction(Action.NewMove(0, -1));
    public void MovePlayerDown() => PlayerAction(Action.NewMove(0, 1));
    public void MovePlayerLeft() => PlayerAction(Action.NewMove(-1, 0));
    public void MovePlayerRight() => PlayerAction(Action.NewMove(1, 0));

    public void ResetEngine()
    {
        _engine = CreateEngine();
        _Ready();
    }
}