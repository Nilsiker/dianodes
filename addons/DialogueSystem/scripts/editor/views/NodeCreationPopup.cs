using Godot;

namespace Nilsiker.GodotTools.Dialogue.Editor.Views
{
	[Tool]
	public partial class NodeCreationPopup : PopupMenu
	{
		public enum NodeCreationOption
		{
			Line,
			Options,
			Event,
			Condition,
		}
	}
}
