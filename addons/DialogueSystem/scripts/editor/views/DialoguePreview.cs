using Godot;

namespace Nilsiker.GodotTools.Dialogue.Editor.Views
{
	[Tool]
	public partial class DialoguePreview : MarginContainer
	{
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