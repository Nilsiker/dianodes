#if TOOLS
using Godot;
using Nilsiker.GodotTools.Dialogue.Editor.Views;
using static Nilsiker.GodotTools.Dialogue.Utilities;

namespace Nilsiker.GodotTools.Dialogue
{
	[Tool]
	public partial class Plugin : EditorPlugin
	{
		PackedScene _mainControl = GD.Load<PackedScene>(GetScenePath("dialogue_editor"));
		Texture2D _icon = GD.Load<Texture2D>(GetSvgPath("dialogue"));
		GraphEdit _instance;

		public override void _EnablePlugin()
		{
			base._EnablePlugin();
		}

		public override void _DisablePlugin()
		{
			base._DisablePlugin();
			RemoveAutoloadSingleton("DialogueManager");
		}

		public override void _EnterTree()
		{
			base._EnterTree();
			_instance = _mainControl.Instantiate<DialogueEditor>();
			_instance.Visible = false;
			EditorInterface.Singleton.GetEditorMainScreen().AddChild(_instance);
		}

		public override void _ExitTree()
		{
			base._ExitTree();
			_instance?.QueueFree();
		}

		public override bool _HasMainScreen() => true;
		public override string _GetPluginName() => "Dialogues";
		public override Texture2D _GetPluginIcon() => _icon;

		public override void _MakeVisible(bool visible)
		{
			if (_instance == null) return;
			if (visible)
			{
				_Reload();
			}
			_instance.Visible = visible;
		}

		private void _Reload()
		{
			if (_instance == null) return;
			_instance.QueueFree();
			_instance = _mainControl.Instantiate<DialogueEditor>();
			EditorInterface.Singleton.GetEditorMainScreen().AddChild(_instance);
		}
	}
}

#endif
