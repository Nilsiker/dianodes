using Godot;
using Nilsiker.GodotTools.Dialogue.Editor.Models;
using System;

namespace Nilsiker.GodotTools.Dialogue.Editor.Views
{
    public abstract partial class DialogueNode : GraphNode, IDialogueNode
    {
        [Export] NodeResource _data;

        public virtual NodeResource Data { get; set; }

        public override void _Ready()
        {
            PositionOffsetChanged += _OnPositionOffsetChanged;

            if (Data == null) return;
            PositionOffset = Data.Position;
            CallDeferred("set_size", Data.Size);
        }

        private void _OnPositionOffsetChanged() => Data.Position = PositionOffset;
    }

}
