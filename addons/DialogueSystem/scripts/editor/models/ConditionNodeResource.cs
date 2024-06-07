using Godot;
using System;

namespace Nilsiker.GodotTools.Dialogue.Editor.Models
{
    [Tool, GlobalClass]
    public partial class ConditionNodeResource : NodeResource
    {
        [Export]
        public string ConditionName { get; set; } = "";

    }

}
