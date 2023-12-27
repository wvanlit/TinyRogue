using Godot;
using TinyRogue.Engine;
using static TinyRogue.Engine.Actions;
using static TinyRogue.Engine.Engine;

namespace TinyRogue.Godot;

[GlobalClass]
public partial class RoguelikeEngine : Node
{
    private Types.TinyRogueEngine _engine;

    [Export] public DungeonMap DungeonMap;
    [Export] public PuppetMaster PuppetMaster;
    private uint _playerId;

    [Signal]
    public delegate void UpdateEventHandler();

    public override void _Ready()
    {
        _engine = CreateEngine();
        DungeonMap.Setup(_engine.Dungeon);

        PuppetMaster.Setup();
        foreach (var actor in _engine.Actors)
        {
            if (actor.role.IsPlayer) _playerId = actor.id;

            PuppetMaster.SpawnActor(actor);
        }
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
        var tuple = Core.ExecutePlayerAction(_engine, playerAction);

        _engine = tuple.Item1;
        foreach (var action in tuple.Item2)
        {
            PuppetMaster.VisualiseAction(action);
        }
    }

    public void MovePlayerUp() => PlayerAction(Action.NewMove(0, -1));
    public void MovePlayerDown() => PlayerAction(Action.NewMove(0, 1));
    public void MovePlayerLeft() => PlayerAction(Action.NewMove(-1, 0));
    public void MovePlayerRight() => PlayerAction(Action.NewMove(1, 0));
    public void ResetEngine() => _Ready();
}