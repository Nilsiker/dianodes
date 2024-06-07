using Godot;
using Godot.Collections;
using System;

namespace Nilsiker.GodotTools.Dialogue.Editor.Models
{
    [Tool, GlobalClass]
    public partial class EventNodeResource : NodeResource
    {
        [Export] public string EventName = "";
    }
}
