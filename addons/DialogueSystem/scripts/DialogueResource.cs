using Godot;
using Nilsiker.GodotTools.Dialogue.Editor.Models;
using System;

namespace Nilsiker.GodotTools.Dialogue.Models
{
    [GlobalClass, Tool]
    public partial class DialogueResource : Resource
    {
        [Export] public Vector2 scrollOffset;
        [Export] public float zoom;
        [Export] public bool hidingPortraits;
        [Export] public Godot.Collections.Dictionary connections;
        [Export] public NodeData[] nodes;
    }
}
