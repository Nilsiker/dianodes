using Godot;
using Nilsiker.GodotTools.Dialogue.Editor.Models;

namespace Nilsiker.GodotTools.Dialogue.Editor.Views
{
	[Tool]
	public partial class EventNode : DialogueNode, IDialogueNode
	{

		[Export] public LineEdit _eventName;
		[Export] EventNodeResource _data;

		public override NodeResource Data
		{
			get => _data; set => _data = (EventNodeResource)value;
		}

		public override void _Ready()
		{
			base._Ready();
			_eventName.TextChanged += (text) => _data.EventName = text;

			if(_data == null) return;
			_eventName.Text = _data.EventName;
		}
	}
}
