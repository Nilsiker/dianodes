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
        public Action<NodeResource>? NodeAdded;
        public Action<NodeResource>? NodeRemoved;
        public Action<Connection>? ConnectionAdded;
        public Action<Connection>? ConnectionRemoved;

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
        [Export]
        public bool ShowPortraits
        {
            get => _showPortraits; set
            {
                _showPortraits = value;
                ShowPortraitsChanged?.Invoke(_showPortraits);
            }
        }
        [Export] public Godot.Collections.Array<Connection> Connections = new();
        [Export] public Godot.Collections.Array<NodeResource> Nodes = new();

        private string _name = "";
        private bool _showPortraits;

        public void AddNode(NodeResource node)
        {
            Nodes.Add(node);
            NodeAdded?.Invoke(node);
        }

        public void RemoveNode(NodeResource node)
        {
            Nodes.Remove(node);
            _RemoveConnectionsByNode(node);
            NodeRemoved?.Invoke(node);
        }

        public void AddConnection(Connection connection)
        {
            GD.Print("adding ", connection);
            Connections.Add(connection);
            ConnectionAdded?.Invoke(connection);
            GD.Print(Connections);
        }

        public void RemoveConnection(Connection connection)
        {
            Connections.Remove(connection);
            ConnectionRemoved?.Invoke(connection);
        }

        // TODO move this to logic class, making this POD-friendly
        public NodeResource? GetNode(StringName guid, int port = -1)
        {
            GD.Print("count ", Connections.Count);
            if (port < 0)
            {
                return Nodes.Where(n => n.Guid == guid).FirstOrDefault();
            }
            GD.Print("from ", guid, " via ", port);

            foreach (Connection cconn in Connections)
            {
                GD.Print("conn ", cconn);
                GD.Print(cconn.FromNode, " comparing to ", guid);
            }

            GD.Print(Connections);
            var conn = Connections
                .Where(c => c.FromNode == guid && c.FromPort == port)
                .FirstOrDefault();
            return Nodes.Where(n => n.Guid == conn?.ToNode).FirstOrDefault();
        }

        private void _RemoveConnectionsByNode(NodeResource node)
        {
            for (int i = Connections.Count - 1; i >= 0; i--)
            {
                var connection = Connections[i];
                if (connection.FromNode == node.Guid || connection.ToNode == node.Guid)
                {
                    RemoveConnection(connection);
                }
            }
        }
    }
}
