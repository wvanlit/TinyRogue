using System.Linq;
using Godot;
using TinyRogue.Engine;
using static TinyRogue.Engine.Actions;
using static TinyRogue.Engine.Actor;
using static TinyRogue.Engine.Engine;

namespace TinyRogue.Godot;

[GlobalClass]
public partial class RoguelikeEngine : Node
{
	private TinyRogueEngine _engine = CreateEngine();

	[Export] public DungeonMap DungeonMap;
	[Export] public Node2D Player;

	[Signal]
	public delegate void UpdateEventHandler();

	public override void _Ready()
	{
		DungeonMap.Setup(_engine.Dungeon);

		var playerActor = _engine.Actors.First(a => a.role.Equals(Role.Player));
		Player.Position = new Vector2(playerActor.position.x, playerActor.position.y) * 48;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsAction("quick_exit")) GetTree().Quit();
		else if (@event.IsPressed())
		{
			if (@event.IsAction("move_up")) MovePlayerUp();
			else if (@event.IsAction("move_down")) MovePlayerDown();
			else if (@event.IsAction("move_left")) MovePlayerLeft();
			else if (@event.IsAction("move_right")) MovePlayerRight();
		}
	}

	public void PlayerAction(Action playerAction)
	{
		Print($"Executing {playerAction}");
		_engine = Core.ExecutePlayerAction(_engine, playerAction);

		var playerActor = _engine.Actors.First(a => a.role.Equals(Role.Player));
		Player.Position = new Vector2(playerActor.position.x, playerActor.position.y) * 48;
	}

	public void MovePlayerUp() => PlayerAction(Action.NewMove(0, -1));
	public void MovePlayerDown() => PlayerAction(Action.NewMove(0, 1));
	public void MovePlayerLeft() => PlayerAction(Action.NewMove(-1, 0));
	public void MovePlayerRight() => PlayerAction(Action.NewMove(1, 0));
}
