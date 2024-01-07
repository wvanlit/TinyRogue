using System;
using Godot;

namespace TinyRogue.Godot;

public partial class Entity : Node2D
{
    public Vector2I TilePosition { get; private set; } = Vector2I.Zero;

    private Sprite2D _sprite;
    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite") ?? throw new ArgumentNullException(nameof(_sprite));
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer") ??
                           throw new ArgumentNullException(nameof(_animationPlayer));

        _animationPlayer.Play("RESET");
    }

    public void SpriteVisible(bool state)
    {
        _sprite.Visible = state;
    }

    public void Die()
    {
        _animationPlayer.Play("die");
    }

    public void MoveImmediate(Vector2I destination)
    {
        TilePosition = destination;
        Position = TilePosition * 48;
    }

    public void MoveSmooth(Vector2I destination)
    {
        const double duration = 0.2;
        const double halfDuration = duration / 2;

        // Movement
        CreateTween()
            .TweenProperty(
                this,
                new NodePath("position"),
                new Vector2(destination.X, destination.Y) * 48,
                duration)
            .FromCurrent()
            .SetEase(Tween.EaseType.InOut);

        // Tilt
        const float tiltFactor = 0.15f;
        var tilt = tiltFactor * Math.Clamp(destination.X - TilePosition.X, -1, 1);

        CreateTween()
            .TweenProperty(
                _sprite,
                new NodePath("rotation"),
                tilt,
                0.01)
            .From(0)
            .SetEase(Tween.EaseType.In);

        CreateTween()
            .TweenProperty(
                _sprite,
                new NodePath("rotation"),
                0,
                duration)
            .From(tilt)
            .SetEase(Tween.EaseType.In);

        // Hop
        const float spriteBaseValue = 24;
        var baseSpriteLocation = new Vector2(spriteBaseValue, spriteBaseValue);
        var arcSpriteLocation = new Vector2(spriteBaseValue, spriteBaseValue - 8);

        CreateTween()
            .TweenProperty(
                _sprite,
                new NodePath("position"),
                arcSpriteLocation,
                halfDuration)
            .From(baseSpriteLocation)
            .SetEase(Tween.EaseType.Out);

        CreateTween()
            .TweenProperty(
                _sprite,
                new NodePath("position"),
                baseSpriteLocation,
                halfDuration)
            .From(arcSpriteLocation)
            .SetDelay(halfDuration)
            .SetEase(Tween.EaseType.Out);

        TilePosition = destination;
    }
}
