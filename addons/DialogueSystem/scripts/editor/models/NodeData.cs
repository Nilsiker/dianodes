using Godot;
using System;

namespace Nilsiker.GodotTools.Dialogue.Editor.Models
{
    [GlobalClass, Tool]
    public partial class NodeData : Resource
    {
        [Export] public string guid = Guid.NewGuid().ToString();
        [Export] public Vector2 size;
        [Export] public Vector2 position;
    }
}
