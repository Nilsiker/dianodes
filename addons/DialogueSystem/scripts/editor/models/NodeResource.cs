using Godot;
using System;

namespace Nilsiker.GodotTools.Dialogue.Editor.Models
{
    [GlobalClass, Tool]
    public partial class NodeResource : Resource
    {
        [Export] public StringName Guid = System.Guid.NewGuid().ToString();
        [Export] public Vector2 Size;
        [Export] public Vector2 Position;

        public NodeResource()
        {
            Guid = System.Guid.NewGuid().ToString();
            ResourceLocalToScene = true;
        }
    }
}
