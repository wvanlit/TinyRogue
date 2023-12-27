using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using TinyRogue.Engine;
using static TinyRogue.Engine.Types;

namespace TinyRogue.Godot;

public partial class PuppetMaster : Node2D
{
    [Export] public PackedScene EntityScene;
    [Export] public PackedScene PlayerScene;

    private Dictionary<uint, Entity> _entities;

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
            var player = PlayerScene.Instantiate() as Entity;
            Debug.Assert(player is not null);

            AddChild(player);

            player.MoveImmediate(new Vector2I(actor.position.x, actor.position.y));

            _entities[actor.id] = player;
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
    }
}