using System;
using TinyRogue.Engine;

namespace TinyRogue.Godot.Helpers;

public static class StringifyExtensions
{
    public static string Stringify(this TinyRogue.Engine.Actions.ExecutedAction executedAction) =>
        executedAction switch
        {
            Actions.ExecutedAction.DealtDamage dealtDamage =>
                $"[Dealt Damage] {dealtDamage.attacker} -> {dealtDamage.defender} ({dealtDamage.damage})",
            Actions.ExecutedAction.Moved moved =>
                $"[Moved] {moved.actor} -> {moved.position.x} {moved.position.y}",
            {IsNoAction: true} => $"[No Action]",
            _ => throw new ArgumentOutOfRangeException(nameof(executedAction)),
        };
}
