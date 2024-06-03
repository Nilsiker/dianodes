using Godot;
using Nilsiker.GodotTools.Dialogue.Editor.Models;
using System;
using System.Linq;

namespace Nilsiker.GodotTools.Dialogue.Models
{
    [GlobalClass, Tool, Icon("res://addons/DialogueSystem/icons/dialogue_resource.svg")]
    public partial class DialogueResource : Resource
    {
        public Action<string>? NameChanged;
        public Action<bool>? ShowPortraitsChanged;
        public Action<NodeData>? NodeAdded;
        public Action<NodeData>? NodeRemoved;
        public Action<Godot.Collections.Dictionary>? ConnectionAdded;
        public Action<Godot.Collections.Dictionary>? ConnectionRemoved;

        [Export]
        public string Name
        {
            get => _name; set
            {
                _name = value;
                NameChanged?.Invoke(_name);
            }
        }
        [Export] public Vector2 ScrollOffset;
        [Export] public float Zoom;
        [Export] public bool ShowPortraits;
        [Export] public Godot.Collections.Array<Godot.Collections.Dictionary> Connections = new();
        [Export] public Godot.Collections.Array<NodeData> Nodes = new();

        private string _name = "";

        public void AddNode(NodeData node)
        {
            Nodes.Add(node);
            NodeAdded?.Invoke(node);
        }

        public void RemoveNode(NodeData node)
        {
            Nodes.Remove(node);
            NodeRemoved?.Invoke(node);
        }

        public void AddConnection(Godot.Collections.Dictionary connection)
        {
            Connections.Add(connection);
            ConnectionAdded?.Invoke(connection);
        }

        public void RemoveConnection(Godot.Collections.Dictionary connection)
        {
            Connections.Remove(connection);
            ConnectionRemoved?.Invoke(connection);
        }

        // TODO move this to logic class, making this POD-friendly
        public NodeData? GetNode(string guid, int port = -1)
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
