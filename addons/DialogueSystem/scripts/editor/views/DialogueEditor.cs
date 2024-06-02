using System;
using System.Linq;
using System.Text.Json;
using Godot;
using Nilsiker.GodotTools.Dialogue.Editor.Models;
using Nilsiker.GodotTools.Dialogue.Models;
using Nilsiker.GodotTools.Extensions;

namespace Nilsiker.GodotTools.Dialogue.Editor.Views
{
    [Tool]
    public partial class DialogueEditor : GraphEdit
    {
        [Export] Label _editing;
        [Export] PopupMenu _nodeCreationMenu;
        [Export] CheckButton _hidePortraitsButton;
        [Export] DialogueResource _data;

        PackedScene _lineNode = GD.Load<PackedScene>(Utilities.GetScenePath("line_node"));
        PackedScene _eventNode = GD.Load<PackedScene>(Utilities.GetScenePath("event_node"));
        // PackedScene _optionsNode = GD.Load<PackedScene>(Utilities.GetScenePath("options_node"));
        Vector2 _lastRightClickPosition = Vector2.Zero;
        public bool HidePortraits => _hidePortraitsButton.ButtonPressed;

        public override void _Ready()
        {
            base._Ready();


            ConnectionRequest += _OnConnectionRequest;
            DisconnectionRequest += _OnDisconnectionRequest;
            ConnectionToEmpty += _OnConnectionToEmpty;

            DeleteNodesRequest += _OnDeleteNodesRequest;

            PopupRequest += _OnPopupRequested;

            _hidePortraitsButton.ButtonPressed = _data.HidingPortraits;
            _hidePortraitsButton.Toggled += _OnHidePortraitsButtonToggled;

            _nodeCreationMenu.IndexPressed += _OnNodeCreationPopupIndexPressed;

            LoadFromResource();
            ScrollOffsetChanged += offset =>
            {
                _data.ScrollOffset = offset;
                this.Log(offset);
            };
        }

        private void _OnFilesDropped(string[] files)
        {
            if (files.Length == 1)
            {
                var file = files.FirstOrDefault();
                var dialogue = GD.Load<DialogueResource>(file);
                GD.Print(dialogue);
            }
        }


        private void _SaveDialogue()
        {
            var res = ResourceSaver.Singleton.Save(_data);
            this.Log(res);
        }

        private void LoadFromResource()
        {
            foreach (var child in GetChildren().OfType<GraphElement>())
            {
                child.QueueFree();
            }

            Zoom = _data.Zoom;
            ScrollOffset = _data.ScrollOffset;  // FIXME scroll offset resets to 0 on positive axes. WHY?!
            foreach (var data in _data.Nodes)
            {
                if (data is LineData lineData)
                {
                    CreateLineNode(lineData);
                }
                else if (data is EventData eventData)
                {
                    CreateEventNode(eventData);
                }
                // TODO add load for other data types
            }

            foreach (var connection in _data.Connections)
            {
                var parsed = Utilities.ParseConnection(connection);
                var res = ConnectNode(parsed.FromNode, parsed.FromPort, parsed.ToNode, parsed.ToPort);
            }

            _editing.Text = "Editing " + (_data.Name ?? _data.ResourceName);

            _UpdatePortraitVisibility(!_data.HidingPortraits);
        }

        private void _OnConnectionRequest(StringName fromNode, long fromPort, StringName toNode, long toPort)
        {
            ConnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
            _data.Connections = GetConnectionList();
        }

        private void _OnDisconnectionRequest(StringName fromNode, long fromPort, StringName toNode, long toPort)
        {
            DisconnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
            _data.Connections = GetConnectionList();
        }

        private void _OnDeleteNodesRequest(Godot.Collections.Array nodes)
        {
            foreach (string name in nodes)
            {
                foreach (var connection in GetConnectionList())
                {
                    var parsed = Utilities.ParseConnection(connection);
                    if (parsed.FromNode == name || parsed.ToNode == name)
                    {
                        _OnDisconnectionRequest(parsed.FromNode, parsed.FromPort, parsed.ToNode, parsed.ToPort);
                    }
                }

                var node = GetNode<IDialogueNode>(name);
                _data.Nodes.Remove(node.Data);
                node.QueueFree();
            }
            _data.Connections = GetConnectionList();
        }


        private void _OnConnectionToEmpty(StringName fromNode, long fromPort, Vector2 releasePosition)
        {
            _nodeCreationMenu.Visible = true;
            var targetPosition = GetScreenPosition() + releasePosition;
            _nodeCreationMenu.Position = new((int)targetPosition.X, (int)targetPosition.Y);
            _lastRightClickPosition = (GetLocalMousePosition() + ScrollOffset) / Zoom;
        }

        private void _OnHidePortraitsButtonToggled(bool hiding)
        {
            _data.HidingPortraits = hiding;
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
                    _data.Zoom = Zoom;
                }
            }
            else if (@event is InputEventWithModifiers modEvent && modEvent.ShiftPressed && Input.IsKeyPressed(Key.S))
            {
                _SaveDialogue();
            }
        }

        private LineNode CreateLineNode(LineData data = null, bool register = false)
        {
            var created = _lineNode.Instantiate<LineNode>();
            created.Data = data ?? new()
            {
                Guid = Guid.NewGuid().ToString(),
            };
            created.Name = created.Data.Guid;
            this.Log("created " + created.Name);
            AddChild(created);

            if (data == null)
            {
                _data.Nodes.Add(created.Data);
            }

            return created;
        }

        private EventNode CreateEventNode(EventData data = null, bool register = false)
        {
            var created = _eventNode.Instantiate<EventNode>();
            created.Data = data ?? new()
            {
                Guid = Guid.NewGuid().ToString(),
            };
            created.Name = created.Data.Guid;
            this.Log("created " + created.Name);
            AddChild(created);

            if (data == null)
            {
                _data.Nodes.Add(created.Data);
            }

            return created;
        }


        private void _OnPopupRequested(Vector2 atPosition)
        {
            _nodeCreationMenu.Visible = true;
            var targetPosition = GetScreenPosition() + atPosition;
            _nodeCreationMenu.Position = new((int)targetPosition.X, (int)targetPosition.Y);
            _lastRightClickPosition = (atPosition + ScrollOffset) / Zoom;
        }


        private void _OnNodeCreationPopupIndexPressed(long option)
        {
            switch ((NodeCreationPopup.NodeCreationOption)option)
            {
                case NodeCreationPopup.NodeCreationOption.Line:
                    var node = CreateLineNode();
                    node.PositionOffset = _lastRightClickPosition;
                    break;
                case NodeCreationPopup.NodeCreationOption.Event:
                    var event_node = CreateEventNode();
                    event_node.PositionOffset = _lastRightClickPosition;
                    break;
            }
        }

        public override bool _CanDropData(Vector2 atPosition, Variant data)
        {
            var dict = data.AsGodotDictionary();
            if (dict.TryGetValue("files", out Variant files))
            {
                if (files.AsStringArray().Length != 1) return false;
                var resource = GD.Load(files.AsStringArray().First());
                return resource is DialogueResource && resource != _data;
            }

            return false;
        }
        public override void _DropData(Vector2 atPosition, Variant data)
        {
            // be bold about this ,since _CanDropData has validated the variant
            base._DropData(atPosition, data);
            var dict = data.AsGodotDictionary();
            var dialogue = GD.Load<DialogueResource>(dict["files"].AsStringArray().First());

            _data = dialogue;
            LoadFromResource();
        }
    }
}
