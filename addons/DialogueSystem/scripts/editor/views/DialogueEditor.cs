using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Godot;
using Nilsiker.GodotTools.Dialogue.Editor.Models;
using Nilsiker.GodotTools.Dialogue.Models;

namespace Nilsiker.GodotTools.Dialogue.Editor.Views
{
    [Tool]
    public partial class DialogueEditor : GraphEdit
    {
        [Export] DialogueResource _data;
        [Export] PopupMenu _nodeCreationMenu;
        [Export] ConfirmationDialog _deleteNodeDialog;
        [Export] CheckButton _hidePortraitsButton;

        PackedScene _lineNode = GD.Load<PackedScene>(Utilities.GetScenePath("line_node"));

        public override void _Ready()
        {
            base._Ready();

            ScrollOffsetChanged += offset => _data.scrollOffset = offset;

            ConnectionToEmpty += _OnConnectionToEmpty;
            ConnectionFromEmpty += _OnConnectionToEmpty;

            ConnectionRequest += _OnConnectionRequest;
            DeleteNodesRequest += _OnDeleteNodesRequest;

            _hidePortraitsButton.ButtonPressed = _data.hidingPortraits;
            _hidePortraitsButton.Toggled += _OnHidePortraitsButtonToggled;

            LoadFromResource();
        }


        private void LoadFromResource()
        {
            Zoom = _data.zoom;
            ScrollOffset = _data.scrollOffset;

            foreach (var data in _data.nodes)
            {
                if (data is LineData lineResource)
                {
                    var created = _lineNode.Instantiate<LineNode>();
                    created.Data = lineResource;
                    AddChild(created);
                }
                // TODO add load for other data types
            }

            _UpdatePortraitVisibility(!_data.hidingPortraits);
        }

        private void _OnConnectionRequest(StringName fromNode, long fromPort, StringName toNode, long toPort)
        {
            ConnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
        }


        private void _OnDeleteNodesRequest(Godot.Collections.Array nodes)
        {
            foreach (string name in nodes)
            {
                foreach (var connection in GetConnectionList())
                {
                    DisconnectNode(
                        (string)connection["from_node"],
                        (int)connection["from_port"],
                        (string)connection["to_node"],
                        (int)connection["to_port"]
                    );
                }
                GetNode<GraphNode>(name).QueueFree();
            }
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
            if (@event is InputEventMouseButton button && (button.ButtonIndex == MouseButton.WheelDown || button.ButtonIndex == MouseButton.WheelUp))
            {
                _data.zoom = Zoom;
            }
        }
    }
}
