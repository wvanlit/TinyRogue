using Godot;

namespace TinyRogue.Godot.Helpers;

public static class CameraExtensions
{
    public static void Shake(this Camera2D camera) =>
        camera.GetNode<AnimationPlayer>("AnimationPlayer").Play("shake");
}
