using Godot;
using Nilsiker.GodotTools.Dialogue.Editor.Models;
using System;

namespace Nilsiker.GodotTools.Dialogue.Editor
{
    public interface IDialogueNode
    {
    }

    public interface IHasPortrait
    {
        void SetPortraitVisibility(bool visible);
    }
    
    public interface IHasNodeData
    {
        NodeData Data { get; set; }
    }
}
