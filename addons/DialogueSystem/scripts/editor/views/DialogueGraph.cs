using System.Linq;
using Godot;
using Nilsiker.GodotTools.Dialogue.Editor.Models;
using Nilsiker.GodotTools.Dialogue.Models;
using Nilsiker.GodotTools.Convenience;
using System;

namespace Nilsiker.GodotTools.Dialogue.Editor.Views
{
    [Tool]
    public partial class DialogueGraph : GraphEdit
    {
        public bool HidePortraits => _hidePortraitsButton.ButtonPressed;

        [Export] DialogueEditor _editor = null!;
        [Export] LineEdit _dialogueNameEdit = null!;
        [Export] PopupMenu _nodeCreationMenu = null!;
        [Export] CheckButton _hidePortraitsButton = null!;


        DialogueResource? _data;
        PackedScene _lineNode = GD.Load<PackedScene>(Utilities.GetScenePath("line_node"));
        PackedScene _eventNode = GD.Load<PackedScene>(Utilities.GetScenePath("event_node"));
        PackedScene _conditionNode = GD.Load<PackedScene>(Utilities.GetScenePath("condition_node"));
        Vector2 _lastRightClickPosition;


        #region Godot Methods
        public override void _Ready()
        {
            if (GetTree().CurrentScene == this) return;
            base._Ready();

            _editor.DataChanged += _OnEditorDataChanged;

            ConnectionRequest += _OnConnectionRequested;
            DisconnectionRequest += _OnDisconnectionRequested;
            ConnectionToEmpty += _OnConnectionToEmpty;

            _dialogueNameEdit.TextChanged += _OnDialogueNameEditTextChanged;
            _hidePortraitsButton.Toggled += _OnHidePortraitsButtonToggled;
            ScrollOffsetChanged += _OnScrollOffsetChanged;

            PopupRequest += _OnPopupRequested;

            _nodeCreationMenu.IndexPressed += _OnNodeCreationPopupIndexPressed;
            DeleteNodesRequest += _OnDeleteNodesRequest;
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            base._UnhandledInput(@event);
            if (_data == null) return;

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
        #endregion

        #region Data Event Handlers
        private void _OnEditorDataChanged(DialogueResource? resource)
        {
            _UnregisterCurrentData();   // always try and unregister, since the plugin might shut down and be reset

            if (resource == null) return;

            _RegisterData(resource);

            _data = resource;   // TODO remove this and handle all data fetches/updates through signals?
            this.Log("data changed to " + resource.Name);

            _LoadFromResource(resource);
        }

        private void _RegisterData(DialogueResource resource)
        {
            resource.NodeAdded += _OnDataNodeAdded;
            resource.NodeRemoved += _OnDataNodeRemoved;
            resource.ConnectionAdded += _OnConnectionAdded;
            resource.ConnectionRemoved += _OnConnectionRemoved;
            resource.ShowPortraitsChanged += _OnDataShowPortraitsChanged;
            this.Log("registered new resource " + resource.Name);
        }

        private void _UnregisterCurrentData()
        {
            if (_data is null) return;
            _data.NodeAdded -= _OnDataNodeAdded;
            _data.NodeRemoved -= _OnDataNodeRemoved;
            _data.ConnectionAdded -= _OnConnectionAdded;
            _data.ConnectionRemoved -= _OnConnectionRemoved;
            _data.ShowPortraitsChanged -= _OnDataShowPortraitsChanged;
            this.Log("unregistered current resource " + _data.Name);
        }

        private void _OnDataNodeAdded(NodeData data)
        {
            var node = data switch
            {
                LineData ld => _CreateNode<LineNode, LineData>(_lineNode, ld),
                EventData ed => _CreateNode<EventNode, EventData>(_eventNode, ed),
                ConditionData cd => _CreateNode<ConditionNode, ConditionData>(_conditionNode, cd),
                _ => throw new NotImplementedException(),
            };
            AddChild(node);
            this.Log($"Added node {node?.Name} to graph.");
        }

        private void _OnDataNodeRemoved(NodeData data)
        {
            var node = GetChildren()
                .OfType<DialogueNode>()
                .Where(node => node.Data == data)
                .First();
            node.QueueFree();
        }

        private void _OnConnectionAdded(Connection connection)
        {
            if (_data is null) return;
            ConnectNode(connection.FromNode, connection.FromPort, connection.ToNode, connection.ToPort);
            this.Log("Connection request: " + connection.FromNode + ":" + connection.FromPort + " -> " + connection.ToNode + ":" + connection.ToPort);
        }

        private void _OnConnectionRemoved(Connection connection)
        {
            if (_data is null) return;
            DisconnectNode(connection.FromNode, connection.FromPort, connection.ToNode, connection.ToPort);
        }
        #endregion

        #region Data Helpers
        private DialogueNode? _CreateNode<T, U>(PackedScene scene, NodeData data)
        where T : DialogueNode
        where U : NodeData, new()
        {
            if (_data is null) return null;

            var created = scene.Instantiate<T>();

            created.Data = data ?? new U();
            created.Name = created.Data.Guid;
            created.PositionOffset = created.Data.Position;

            return created;
        }
        #endregion

        #region UI Event Handlers
        private void _OnDialogueNameEditTextChanged(string newText)
        {
            if (_data is null) return;
            _data.Name = newText;
        }

        private void _OnScrollOffsetChanged(Vector2 offset)
        {
            if (_data == null) return;
            _data.ScrollOffset = offset;
        }


        private void _OnPopupRequested(Vector2 atPosition)
        {
            _nodeCreationMenu.Popup();
            var targetPosition = GetScreenPosition() + atPosition;
            _nodeCreationMenu.Position = new((int)targetPosition.X, (int)targetPosition.Y);
            _lastRightClickPosition = (atPosition + ScrollOffset) / Zoom;
        }

        private void _OnNodeCreationPopupIndexPressed(long option)
        {
            if (_data is null) return;

            NodeData data = (NodeCreationPopup.NodeCreationOption)option switch
            {
                NodeCreationPopup.NodeCreationOption.Line => new LineData(),
                NodeCreationPopup.NodeCreationOption.Event => new EventData(),
                NodeCreationPopup.NodeCreationOption.Condition => new ConditionData(),
                _ => throw new NotImplementedException(),
            };

            data.Position = _lastRightClickPosition;
            _data.AddNode(data);
        }

        private void _OnDeleteNodesRequest(Godot.Collections.Array nodes)
        {
            if (_data is null) return;

            foreach (string name in nodes)
            {
                var node = GetNode<IDialogueNode>(name);
                _data.RemoveNode(node.Data);
            }
        }

        private void _OnConnectionRequested(StringName fromNode, long fromPort, StringName toNode, long toPort)
        {
            if (_data is null) return;
            Connection connection = new Connection
            {
                FromNode = fromNode.ToString(),
                FromPort = (int)fromPort,
                ToNode = toNode.ToString(),
                ToPort = (int)toPort
            };
            _data.AddConnection(connection);
        }

        private void _OnDisconnectionRequested(StringName fromNode, long fromPort, StringName toNode, long toPort)
        {
            if (_data is null) return;
            Connection connection = new Connection
            {
                FromNode = fromNode.ToString(),
                FromPort = (int)fromPort,
                ToNode = toNode.ToString(),
                ToPort = (int)toPort
            };
            _data.RemoveConnection(connection);
        }

        private void _OnConnectionToEmpty(StringName fromNode, long fromPort, Vector2 releasePosition)
        {
            _nodeCreationMenu.Visible = true;
            var targetPosition = GetScreenPosition() + releasePosition;
            _nodeCreationMenu.Position = new((int)targetPosition.X, (int)targetPosition.Y);
            _lastRightClickPosition = (GetLocalMousePosition() + ScrollOffset) / Zoom;
            // TODO implement!
        }

        private void _OnHidePortraitsButtonToggled(bool hiding)
        {
            if (_data == null) return;
            _data.ShowPortraits = hiding;
        }
        #endregion

        #region UI Helpers
        private void _SaveDialogue()
        {
            var res = ResourceSaver.Singleton.Save(_data);
            this.Log(res);
        }

        private void _LoadFromResource(DialogueResource resource)
        {
            _Clear();

            Zoom = resource.Zoom;
            ScrollOffset = resource.ScrollOffset;  // FIXME scroll offset resets to 0 on positive axes. WHY?!
            foreach (var data in resource.Nodes)
            {
                var node = data switch
                {
                    LineData ld => _CreateNode<LineNode, LineData>(_lineNode, ld),
                    EventData ed => _CreateNode<EventNode, EventData>(_eventNode, ed),
                    ConditionData cd => _CreateNode<ConditionNode, ConditionData>(_conditionNode, cd),
                    _ => throw new NotImplementedException(),
                };
                AddChild(node);
            }

            foreach (var connection in resource.Connections)
            {
                ConnectNode(connection.FromNode, connection.FromPort, connection.ToNode, connection.ToPort);
            }

            _dialogueNameEdit.Text = resource.Name ?? resource.ResourceName;
            _OnDataShowPortraitsChanged(!resource.ShowPortraits);
        }

        private void _Clear()
        {
            ClearConnections();
            foreach (var child in GetChildren().OfType<GraphElement>())
            {
                child.QueueFree();
            }
        }
        #endregion

        private void _OnDataShowPortraitsChanged(bool visible)
        {
            foreach (var child in GetChildren().OfType<IHasPortrait>())
            {
                child.SetPortraitVisibility(visible);
            }
        }
    }
}
