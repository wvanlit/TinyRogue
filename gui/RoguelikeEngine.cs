using System.Linq;
using Godot;
using TinyRogue.Engine;
using TinyRogue.Godot.Helpers;
using static TinyRogue.Engine.Actions;
using static TinyRogue.Engine.Engine;

namespace TinyRogue.Godot;

[GlobalClass]
public partial class RoguelikeEngine : Node
{
    private Types.TinyRogueEngine _engine;

    [Export] public DungeonMap DungeonMap;
    [Export] public PuppetMaster PuppetMaster;
    [Export] public VisionMap VisionMap;

    private uint _playerId;
    private bool _killedAllEnemies;

    public override void _Ready()
    {
        _engine = CreateEngine();

        DungeonMap.Setup(_engine.Dungeon);
        VisionMap.Setup(_engine.ShadowMap);

        PuppetMaster.Setup();

        foreach (var actor in _engine.Actors)
        {
            if (actor.role.IsPlayer)
                _playerId = actor.id;

            PuppetMaster.SpawnActor(actor);
        }

        UpdateActorVisibility();

        _killedAllEnemies = false;
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
        foreach (var executedAction in tuple.Item2)
        {
            PuppetMaster.VisualiseAction(executedAction);
        }

        Print("Executed Actions: \n\t", string.Join("\n\t", tuple.Item2.Select(x => x.Stringify())));

        // TODO - Only update changed tiles instead of all, this isn't very efficient!
        VisionMap.Setup(_engine.ShadowMap);
        UpdateActorVisibility();

        if (_engine.Actors.Length == 1 && !_killedAllEnemies) // Only the player is left
        {
            _killedAllEnemies = true;
            GetTree().CreateTimer(2.0).Connect("timeout", new(this, nameof(ResetEngine)));
        }
    }

    private void UpdateActorVisibility()
    {
        foreach (var actor in _engine.Actors)
        {
            if (actor.role.IsPlayer)
            {
                PuppetMaster.SetActorVisibility(actor.id, true);
            }
            else
            {
                var (visible, _) = VisionMap.CheckVisibility(new Vector2I(actor.position.x, actor.position.y));
                PuppetMaster.SetActorVisibility(actor.id, visible);
            }
        }
    }

    public void MovePlayerUp() => PlayerAction(Action.NewMove(0, -1));

    public void MovePlayerDown() => PlayerAction(Action.NewMove(0, 1));

    public void MovePlayerLeft() => PlayerAction(Action.NewMove(-1, 0));

    public void MovePlayerRight() => PlayerAction(Action.NewMove(1, 0));

    public void ResetEngine() => _Ready();
}