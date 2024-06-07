using System.Linq;
using Godot;
using Nilsiker.GodotTools.Dialogue.Editor.Models;
using Nilsiker.GodotTools.Dialogue.Models;
using Nilsiker.GodotTools.Dialogue.Convenience;
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

        DialogueResource _data = GD.Load<DialogueResource>(Constants.Paths.TempDialogueResource);
        string LoadedPath => (string)ProjectSettings.Singleton.GetSetting("dialogue_loaded_path");

        PackedScene _lineNode = GD.Load<PackedScene>(Utilities.GetScenePath("line_node"));
        PackedScene _eventNode = GD.Load<PackedScene>(Utilities.GetScenePath("event_node"));
        PackedScene _conditionNode = GD.Load<PackedScene>(Utilities.GetScenePath("condition_node"));
        Vector2 _lastRightClickPosition;

        StringName? _connectToNode;
        int? _connectToPort;

        public void Refresh()
        {
            _Clear();
            this.Log($"refreshing to reflect data: {_data.Name} ({_data.ResourcePath})");
            Zoom = _data.Zoom;
            ScrollOffset = _data.ScrollOffset;  // FIXME scroll offset resets to 0 on positive axes. WHY?!
            foreach (var data in _data.Nodes)
            {
                var node = data switch
                {
                    LineNodeResource ld => _CreateNode<LineNode, LineNodeResource>(_lineNode, ld),
                    EventNodeResource ed => _CreateNode<EventNode, EventNodeResource>(_eventNode, ed),
                    ConditionNodeResource cd => _CreateNode<ConditionNode, ConditionNodeResource>(_conditionNode, cd),
                    _ => throw new NotImplementedException(),
                };
                AddChild(node);
                this.Log($"Added node to graph as {node?.Name}");
            }

            foreach (var connection in _data.Connections)
            {
                ConnectNode(connection.FromNode, connection.FromPort, connection.ToNode, connection.ToPort);
                this.Log($"Added connection {connection.FromNode}:{connection.FromPort} -> {connection.ToNode}:{connection.ToPort}");
            }

            _dialogueNameEdit.Text = _data.Name ?? _data.ResourceName;
            _OnDataShowPortraitsChanged(_data.ShowPortraits);
        }

        #region Godot Methods
        public override void _Ready()
        {
            if (GetTree().CurrentScene == this) return;
            base._Ready();
            _Clear();
            // UI
            ConnectionRequest += _OnConnectionRequested;
            DisconnectionRequest += _OnDisconnectionRequested;
            ConnectionToEmpty += _OnConnectionToEmpty;

            _dialogueNameEdit.TextChanged += _OnDialogueNameEditTextChanged;
            _hidePortraitsButton.Toggled += _OnHidePortraitsButtonToggled;
            ScrollOffsetChanged += _OnScrollOffsetChanged;

            PopupRequest += _OnPopupRequested;

            _nodeCreationMenu.IndexPressed += _OnNodeCreationPopupIndexPressed;
            DeleteNodesRequest += _OnDeleteNodesRequest;

            // Data
            _RegisterData();

            Refresh();
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            _UnregisterCurrentData();
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
        }
        #endregion

        #region Data Event Handlers
        private void _RegisterData()
        {
            _data.NodeAdded += _OnDataNodeAdded;
            _data.NodeRemoved += _OnDataNodeRemoved;
            _data.ConnectionAdded += _OnConnectionAdded;
            _data.ConnectionRemoved += _OnConnectionRemoved;
            _data.ShowPortraitsChanged += _OnDataShowPortraitsChanged;
            this.Log("registered new resource " + _data.Name);
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

        private void _OnDataNodeAdded(NodeResource data)
        {
            var node = data switch
            {
                LineNodeResource ld => _CreateNode<LineNode, LineNodeResource>(_lineNode, ld),
                EventNodeResource ed => _CreateNode<EventNode, EventNodeResource>(_eventNode, ed),
                ConditionNodeResource cd => _CreateNode<ConditionNode, ConditionNodeResource>(_conditionNode, cd),
                _ => throw new NotImplementedException(),
            };
            AddChild(node);
            this.Log($"Added node {node?.Name} to graph.");
        }

        private void _OnDataNodeRemoved(NodeResource data)
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
            this.Log("Connection removed: " + connection.FromNode + ":" + connection.FromPort + " -> " + connection.ToNode + ":" + connection.ToPort);

        }

        private void _OnDataShowPortraitsChanged(bool visible)
        {
            foreach (var child in GetChildren().OfType<IHasPortrait>())
            {
                child.SetPortraitVisibility(visible);
            }
        }
        #endregion

        #region Data Helpers
        private DialogueNode? _CreateNode<T, U>(PackedScene scene, NodeResource data)
        where T : DialogueNode
        where U : NodeResource, new()
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
            _connectToNode = null;
            _connectToPort = null;
        }

        private void _OnNodeCreationPopupIndexPressed(long option)
        {
            if (_data is null) return;

            NodeResource data = (NodeCreationPopup.NodeCreationOption)option switch
            {
                NodeCreationPopup.NodeCreationOption.Line => new LineNodeResource(),
                NodeCreationPopup.NodeCreationOption.Event => new EventNodeResource(),
                NodeCreationPopup.NodeCreationOption.Condition => new ConditionNodeResource(),
                _ => throw new NotImplementedException(),
            };

            data.Position = _lastRightClickPosition;
            _data.AddNode(data);

            if (_connectToNode != null && _connectToPort != null)
            {
                _data.AddConnection(new Connection
                {
                    FromNode = _connectToNode,
                    FromPort = _connectToPort.Value,
                    ToNode = data.Guid,
                    ToPort = 0,
                });
            }
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
            _nodeCreationMenu.Popup();
            var targetPosition = GetScreenPosition() + releasePosition;
            _nodeCreationMenu.Position = new((int)targetPosition.X, (int)targetPosition.Y);
            _lastRightClickPosition = (GetLocalMousePosition() + ScrollOffset) / Zoom;
            _connectToNode = fromNode;
            _connectToPort = (int)fromPort;
        }

        private void _OnHidePortraitsButtonToggled(bool hiding)
        {
            if (_data == null) return;
            _data.ShowPortraits = hiding;
        }
        #endregion

        #region UI Helpers
        private void _Clear()
        {
            ClearConnections();
            foreach (var child in GetChildren().OfType<GraphElement>())
            {
                child.Free();
            }
        }
        #endregion
    }
}
