using Godot;
using Nilsiker.GodotTools.Dialogue.Editor.Models;
using System;
using System.Linq;

namespace Nilsiker.GodotTools.Dialogue.Models
{
    [GlobalClass, Tool]
    public partial class DialogueResource : Resource
    {
        [Export] public Vector2 scrollOffset;
        [Export] public float zoom;
        [Export] public bool hidingPortraits;
        [Export] public Godot.Collections.Array<Godot.Collections.Dictionary> connections;
        [Export] public Godot.Collections.Array<NodeData> nodes;

        public NodeData GetNode(string guid, int port = -1)
        {
            if (port < 0)
            {
                return nodes.Where(n => n.guid == guid).FirstOrDefault();
            }

            var conn = connections.Where(c => (string)c["from_node"] == guid && (int)c["to_port"] == port).FirstOrDefault();
            if (conn == null) return null;
            
            var parsed = Utilities.ParseConnection(conn);
            return nodes.Where(n => n.guid == parsed.ToNode).FirstOrDefault();
        }
    }
}
