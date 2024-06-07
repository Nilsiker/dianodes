using System.Diagnostics;
using Godot;

namespace Nilsiker.GodotTools.Dialogue.Convenience
{
    public static partial class NodeExtensions
    {
        public static void Log(this Node node, object? message, bool simple = false)
        {
            var trace = new StackTrace();
            var frame = trace.GetFrame(1);
            if (simple)
            {
                GD.Print(message);
            }
            else
            {
                GD.PrintRich($"[color=#808080]{node.Name} ({node.GetType().Name}::{frame?.GetMethod()?.Name}):[color=WHITE] {message}");
            }

        }
        public static void Warn(this Node _, object message) => GD.PushWarning(message);
        public static void Err(this Node _, object message) => GD.PushError(message);
    }

    public static class Rnd
    {
        public static bool Chance(int percentage)
        {
            return GD.Randf() < percentage / 100.0;
        }

    }

}
