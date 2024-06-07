using Godot;
using System;

namespace Nilsiker.GodotTools.Dialogue.Editor.Models
{
    [Tool, GlobalClass]
    public partial class LineNodeResource : NodeResource
    {
        [Export] public string Name = "";
        [Export] public string Line = "";
        [Export] public Texture2D? Portrait;
    }
}
