using Godot;
using Godot.Collections;
using System;

namespace Nilsiker.GodotTools.Dialogue.Editor.Models
{
    [Tool, GlobalClass]
    public partial class EventData : NodeData
    {
        [Export] public string EventName = "";
    }
}
