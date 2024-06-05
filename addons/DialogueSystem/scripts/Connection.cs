
using Godot;

namespace Nilsiker.GodotTools.Dialogue
{
    [Tool, GlobalClass]
    public partial class Connection : Resource
    {
        [Export] public StringName FromNode = "";
        [Export] public int FromPort;
        [Export] public StringName ToNode = "";
        [Export] public int ToPort;
    }
}
