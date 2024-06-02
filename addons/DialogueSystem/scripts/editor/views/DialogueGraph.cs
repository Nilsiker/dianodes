using System.Linq;
using Godot;
using Nilsiker.GodotTools.Dialogue.Editor.Models;
using Nilsiker.GodotTools.Dialogue.Models;
using Nilsiker.GodotTools.Convenience;
using System;
using System.Text.Json;

namespace Nilsiker.GodotTools.Dialogue.Editor.Views
{
    [Tool]
    public partial class DialogueGraph : GraphEdit
    {
        public bool HidePortraits => _hidePortraitsButton.ButtonPressed;

        [Export] DialogueEditor _editor;
        [Export] Label _editing;
        [Export] PopupMenu _nodeCreationMenu;
        [Export] CheckButton _hidePortraitsButton;

        DialogueResource _data;
        PackedScene _lineNode = GD.Load<PackedScene>(Utilities.GetScenePath("line_node"));
        PackedScene _eventNode = GD.Load<PackedScene>(Utilities.GetScenePath("event_node"));
        PackedScene _conditionNode = GD.Load<PackedScene>(Utilities.GetScenePath("condition_node"));
        Vector2 _lastRightClickPosition = Vector2.Zero;

        public override void _Ready()
        {
            if (GetTree().CurrentScene == this) return;
            base._Ready();

            _editor.DataChanged += _OnEditorDataChanged;

            ConnectionRequest += _OnConnectionRequest;
            DisconnectionRequest += _OnDisconnectionRequest;
            ConnectionToEmpty += _OnConnectionToEmpty;

            DeleteNodesRequest += _OnDeleteNodesRequest;

            PopupRequest += _OnPopupRequested;

            _hidePortraitsButton.Toggled += _OnHidePortraitsButtonToggled;

            _nodeCreationMenu.IndexPressed += _OnNodeCreationPopupIndexPressed;
            ScrollOffsetChanged += offset =>
            {
                if (_data == null) return;
                _data.ScrollOffset = offset;
            };
        }

        private void _OnEditorDataChanged(DialogueResource resource)
        {
            _data = resource;   // TODO remove this and handle all data fetches/updates through signals?
            LoadFromResource(resource);
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

        private void LoadFromResource(DialogueResource resource)
        {
            _hidePortraitsButton.ButtonPressed = _data.HidingPortraits;

            foreach (var child in GetChildren().OfType<GraphElement>())
            {
                child.QueueFree();
            }

            Zoom = resource.Zoom;
            ScrollOffset = resource.ScrollOffset;  // FIXME scroll offset resets to 0 on positive axes. WHY?!
            foreach (var data in resource.Nodes)
            {
                switch (data)
                {
                    case LineData lineData:
                        CreateNode<LineNode, LineData>(_lineNode, lineData);
                        break;
                    case EventData eventData:
                        CreateNode<EventNode, EventData>(_eventNode, eventData);
                        break;
                    case ConditionData conditionData:
                        CreateNode<ConditionNode, ConditionData>(_conditionNode, conditionData);
                        break;
                        // TODO add load for other data types
                }
            }

            foreach (var connection in resource.Connections)
            {
                this.Log($"Requesting to create connection {connection}");
                var parsed = Utilities.ParseConnection(connection);
                var res = ConnectNode(parsed.FromNode, parsed.FromPort, parsed.ToNode, parsed.ToPort);
                this.Log($"Creating connection {JsonSerializer.Serialize(parsed)}: {res}");
            }

            _editing.Text = "Editing " + (resource.Name ?? resource.ResourceName);

            _UpdatePortraitVisibility(!resource.HidingPortraits);
        }

        private void _OnConnectionRequest(StringName fromNode, long fromPort, StringName toNode, long toPort)
        {
            ConnectNode(fromNode, (int)fromPort, toNode, (int)toPort);
            this.Log("Connection request: " + fromNode + ":" + fromPort + " -> " + toNode + ":" + toPort);
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

        private T CreateNode<T, U>(PackedScene scene, NodeData data = null, bool register = false)
        where T : DialogueNode
        where U : NodeData, new()
        {
            var created = scene.Instantiate<T>();
            created.Data = data ?? new U();
            this.Log("created " + created.Name);
            AddChild(created);

            created.Name = created.Data.Guid;
            this.Log("added to graph as " + created.Name);


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
                    var node = CreateNode<LineNode, LineData>(_lineNode);
                    node.PositionOffset = _lastRightClickPosition;
                    break;
                case NodeCreationPopup.NodeCreationOption.Event:
                    var event_node = CreateNode<EventNode, EventData>(_eventNode);
                    event_node.PositionOffset = _lastRightClickPosition;
                    break;
                case NodeCreationPopup.NodeCreationOption.Condition:
                    var condition_node = CreateNode<ConditionNode, ConditionData>(_conditionNode);
                    condition_node.PositionOffset = _lastRightClickPosition;
                    break;
            }
        }
    }
}
