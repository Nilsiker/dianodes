using Godot;

namespace Nilsiker.GodotTools.Dialogue.Editor.Views
{
	[Tool]
	public partial class NodeCreationPopup : PopupMenu
	{
		[Signal] public delegate void NodeCreationRequestedEventHandler();

		// NOTE must be aligned with configured options on popup menu node!
		public enum NodeCreationOption
		{
			Line,
			Options,
			Event
		}

		public override void _Ready()
		{
			IndexPressed += _OnIndexPressed;
		}

		private void _OnIndexPressed(long index)
		{
			string s = (NodeCreationOption)index switch
			{
				NodeCreationOption.Line => null,
				NodeCreationOption.Options => null,
				NodeCreationOption.Event => null,
				_ => null
			};
		}
	}
}
