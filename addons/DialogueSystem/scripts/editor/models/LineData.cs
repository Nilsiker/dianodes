using Godot;
using System;

namespace Nilsiker.GodotTools.Dialogue.Editor.Models
{
    [Tool, GlobalClass]
    public partial class LineData : NodeData
    {
        [Export] public string name;
        [Export] public string line;
        [Export] public Texture2D portrait;
    }
}
