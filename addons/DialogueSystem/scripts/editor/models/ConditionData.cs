using Godot;
using System;

namespace Nilsiker.GodotTools.Dialogue.Editor.Models
{
    [Tool, GlobalClass]
    public partial class ConditionData : NodeData
    {

        [Export]
        public string ConditionName { get; set; } = "";

    }
}
