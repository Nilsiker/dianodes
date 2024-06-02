using Godot;
using Nilsiker.GodotTools.Dialogue.Editor.Models;
using System;
using System.Linq;

namespace Nilsiker.GodotTools.Dialogue.Models
{
    [GlobalClass, Tool, Icon("res://addons/DialogueSystem/icons/dialogue_resource.svg")]
    public partial class DialogueResource : Resource
    {
        [Export] public string Name;
        [Export] public Vector2 ScrollOffset;
        [Export] public float Zoom;
        [Export] public bool HidingPortraits;
        [Export] public Godot.Collections.Array<Godot.Collections.Dictionary> Connections;
        [Export] public Godot.Collections.Array<NodeData> Nodes;

        // TODO move this to logic class, making this POD-friendly
        public NodeData GetNode(string guid, int port = -1)
        {
            if (port < 0)
            {
                return Nodes.Where(n => n.Guid == guid).FirstOrDefault();
            }
            var conn = Connections.Where(c => (string)c["from_node"] == guid && (int)c["from_port"] == port).FirstOrDefault();
            if (conn == null) return null;

            var parsed = Utilities.ParseConnection(conn);
            return Nodes.Where(n => n.Guid == parsed.ToNode).FirstOrDefault();
        }
    }
}
