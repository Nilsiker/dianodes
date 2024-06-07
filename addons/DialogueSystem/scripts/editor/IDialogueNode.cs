using Godot;
using Nilsiker.GodotTools.Dialogue.Editor.Models;
using System;

namespace Nilsiker.GodotTools.Dialogue.Editor
{
    public interface IDialogueNode : IHasNodeData
    {
        void QueueFree();
    }

    public interface IHasPortrait
    {
        void SetPortraitVisibility(bool visible);
    }
    
    public interface IHasNodeData
    {
        NodeResource Data { get; set; }
    }
}
