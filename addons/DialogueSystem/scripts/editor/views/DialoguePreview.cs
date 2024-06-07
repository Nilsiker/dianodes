using Godot;
using Nilsiker.GodotTools.Dialogue.Example;
using Nilsiker.GodotTools.Dialogue.Models;

namespace Nilsiker.GodotTools.Dialogue.Editor.Views
{
	[Tool]
	public partial class DialoguePreview : MarginContainer
	{
		public void Play(string path)
		{
			var scene = GD.Load<PackedScene>("res://addons/DialogueSystem/examples/preview_dialogue.tscn").Instantiate<ExampleDialogueContainer>();
			scene.DataPath = path;
			AddChild(scene);
			Visible = true;
			scene.TreeExited += () => Visible = false;
			scene.Start();
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