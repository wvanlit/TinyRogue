using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using TinyRogue.Engine;
using TinyRogue.Godot.Helpers;
using static TinyRogue.Engine.Types;

namespace TinyRogue.Godot;

public partial class PuppetMaster : Node2D
{
    [Export] public PackedScene EntityScene;
    [Export] public PackedScene PlayerScene;

    private Dictionary<uint, Entity> _entities;
    private uint _playerId;

    public void Setup()
    {
        Position = Vector2.Zero;
        _entities = new Dictionary<uint, Entity>();

        foreach (var child in GetChildren())
        {
            child.QueueFree();
        }
    }

    public void SpawnActor(Types.Actor actor)
    {
        if (actor.role.Equals(Role.Player))
        {
            var player = (PlayerScene.Instantiate()) as Entity;
            Debug.Assert(player is not null, "player is null");
            Print(player.Name);

            AddChild(player);

            player.MoveImmediate(new Vector2I(actor.position.x, actor.position.y));

            _entities[actor.id] = player;
            _playerId = actor.id;
        }
        else if (actor.role.Equals(Role.NPC))
        {
            var entity = EntityScene.Instantiate() as Entity;
            Debug.Assert(entity is not null);

            AddChild(entity);

            entity.MoveImmediate(new Vector2I(actor.position.x, actor.position.y));

            _entities[actor.id] = entity;
        }
    }

    public void VisualiseAction(Actions.ExecutedAction action)
    {
        if (action is Actions.ExecutedAction.Moved moved)
        {
            _entities[moved.actor].MoveSmooth(new Vector2I(moved.position.x, moved.position.y));
        }
        else if (action is Actions.ExecutedAction.DealtDamage dealtDamage)
        {
            if (dealtDamage.defender == _playerId)
            {
                // TODO - Visualise damage dealt to player
                Print("Player took damage!");
                GetViewport().GetCamera2D().Shake();
            }
            else
            {
                _entities[dealtDamage.defender].Die();

                if (dealtDamage.attacker == _playerId) // Only shake camera if player dealt damage
                    GetViewport().GetCamera2D().Shake();
            }
        }
    }

    public void SetActorVisibility(uint id, bool state)
    {
        _entities[id].SpriteVisible(state);
    }
}
