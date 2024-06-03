using Godot;
using Nilsiker.GodotTools.Dialogue.Models;

namespace Nilsiker.GodotTools.Dialogue.Editor.Views
{
	[Tool]
	public partial class DialoguePreview : MarginContainer
	{
		public void Play() {
			var scene = GD.Load<PackedScene>("res://addons/DialogueSystem/examples/preview_dialogue.tscn").Instantiate();
			AddChild(scene);
			Visible = true;
			scene.TreeExited += () => Visible = false;
		}

		public override void _GuiInput(InputEvent @event)
		{
			base._GuiInput(@event);
			if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.ButtonIndex == MouseButton.Left)
			{
				GetChild(0).QueueFree();
			}
		}
	}

}