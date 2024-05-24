using System;
using System.Linq;
using Godot;
using Nilsiker.GodotTools.Dialogue.Editor.Models;
using Nilsiker.GodotTools.Dialogue.Models;

namespace Nilsiker.GodotTools.Dialogue.Editor.Views
{
    [Tool]
    public partial class DialogueEditor : GraphEdit
    {
        [Export] PopupMenu _nodeCreationMenu;
        [Export] CheckButton _hidePortraitsButton;
        [Export] DialogueResource _data;

        PackedScene _lineNode = GD.Load<PackedScene>(Utilities.GetScenePath("line_node"));
        Vector2 _lastRightClickPosition = Vector2.Zero;

        public override void _Ready()
        {
            base._Ready();

            ScrollOffsetChanged += offset => _data.scrollOffset = offset;

            ConnectionRequest += _OnConnectionRequest;
            DisconnectionRequest += _OnDisconnectionRequest;
            ConnectionToEmpty += _OnConnectionToEmpty;

            DeleteNodesRequest += _OnDeleteNodesRequest;

            _hidePortraitsButton.ButtonPressed = _data.hidingPortraits;
            _hidePortraitsButton.Toggled += _OnHidePortraitsButtonToggled;

            _nodeCreationMenu.IndexPressed += _OnNodeCreationPopupIndexPressed;

            LoadFromResource();
        }

        private void LoadFromResource()
        {
            Zoom = _data.zoom;
            ScrollOffset = _data.scrollOffset;

            foreach (var data in _data.nodes)
            {
                if (data is LineData lineData)
                {
                    CreateLineNode(lineData);
                }
                // TODO add load for other data types
            }

            foreach (var connection in _data.connections)
            {
                var parsed = Utilities.ParseConnection(connection);
                ConnectNode(parsed.FromNode, parsed.FromPort, parsed.ToNode, parsed.ToPort);
            }

            _UpdatePortraitVisibility(!_data.hidingPortraits);
        }

        private void _OnConnectionRequest(StringName fromNode, long fromPort, StringName toNode, long toPort)
        {
            ConnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
            _data.connections = GetConnectionList();
        }

        private void _OnDisconnectionRequest(StringName fromNode, long fromPort, StringName toNode, long toPort)
        {
            DisconnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
            _data.connections = GetConnectionList();
        }

        private void _OnDeleteNodesRequest(Godot.Collections.Array nodes)
        {
            foreach (string name in nodes)
            {
                foreach (var connection in GetConnectionList())
                {
                    var parsed = Utilities.ParseConnection(connection);
                    _OnDisconnectionRequest(parsed.FromNode, parsed.FromPort, parsed.ToNode, parsed.ToPort);
                }

                var node = GetNode<LineNode>(name);
                _data.nodes.Remove(node.Data);
                node.QueueFree();
            }
            _data.connections = GetConnectionList();
        }


        private void _OnConnectionToEmpty(StringName fromNode, long fromPort, Vector2 releasePosition)
        {
            _nodeCreationMenu.Visible = true;
            var targetPosition = GetScreenPosition() + releasePosition;
            _nodeCreationMenu.Position = new((int)targetPosition.X, (int)targetPosition.Y);
        }

        private void _OnHidePortraitsButtonToggled(bool hiding)
        {
            _data.hidingPortraits = hiding;
            _UpdatePortraitVisibility(!hiding);
        }

        private void _UpdatePortraitVisibility(bool visible)
        {
            foreach (var child in GetChildren().OfType<IHasPortrait>())
            {
                child.SetPortraitVisibility(visible);
            }
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            base._UnhandledInput(@event);
            if (@event is InputEventMouseButton button)
            {
                if (button.ButtonIndex == MouseButton.WheelDown || button.ButtonIndex == MouseButton.WheelUp)
                {
                    _data.zoom = Zoom;
                }
            }
        }

        public override void _GuiInput(InputEvent @event)
        {
            base._GuiInput(@event);
            if (@event is InputEventMouseButton button && button.IsReleased() && button.ButtonIndex == MouseButton.Right)
            {
                _nodeCreationMenu.Visible = true;
                var targetPosition = GetScreenPosition() + button.Position;
                _nodeCreationMenu.Position = new((int)targetPosition.X, (int)targetPosition.Y);
                _lastRightClickPosition = (GetLocalMousePosition() + ScrollOffset) / Zoom;
            }
        }

        private LineNode CreateLineNode(LineData data = null, bool register = false)
        {
            var created = _lineNode.Instantiate<LineNode>();
            created.Data = data ?? new()
            {
                guid = Guid.NewGuid().ToString()
            };
            created.Name = created.Data.guid;
            AddChild(created);

            if (data == null)
            {
                _data.nodes.Add(created.Data);
            }

            return created;
        }

        private void _OnNodeCreationPopupIndexPressed(long option)
        {
            switch ((NodeCreationPopup.NodeCreationOption)option)
            {
                case NodeCreationPopup.NodeCreationOption.Line:
                    var node = CreateLineNode();
                    node.PositionOffset = _lastRightClickPosition;
                    break;
            }
        }
    }
}
