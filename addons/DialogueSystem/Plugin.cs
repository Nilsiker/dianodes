#if TOOLS
using System.Xml.Resolvers;
using Godot;
using Nilsiker.GodotTools.Convenience;
using Nilsiker.GodotTools.Dialogue.Editor.Views;
using Nilsiker.GodotTools.Dialogue.Models;
using static Nilsiker.GodotTools.Dialogue.Utilities;

namespace Nilsiker.GodotTools.Dialogue
{
	[Tool]
	public partial class Plugin : EditorPlugin
	{
		PackedScene _mainControl = GD.Load<PackedScene>(GetScenePath("dialogue_editor"));
		Texture2D _icon = GD.Load<Texture2D>(GetSvgPath("dialogue"));
		Texture2D _resourceIcon = GD.Load<Texture2D>("res://addons/DialogueSystem/icons/dialogue_resource.svg");
		DialogueEditor _instance;

		public override void _EnablePlugin()
		{
			base._EnablePlugin();
			AddCustomType("DialogueResource", "Resource", _instance.GetScript().As<Script>(), _resourceIcon);
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
			this.Log("Reloading...");
			_instance.QueueFree();
			_instance = _mainControl.Instantiate<DialogueEditor>();
			EditorInterface.Singleton.GetEditorMainScreen().AddChild(_instance);
		}


		public override bool _Handles(GodotObject @object)
		{
			return @object is DialogueResource && _instance.Data != @object;
		}

		public override void _Edit(GodotObject @object)
		{
			if (@object == null) return;
			_instance.Data = (DialogueResource)@object;
			this.Log(_instance.Data);
		}

		public override void _SaveExternalData()
		{
			this.Log("Saving " + _instance.Data.Name + " to " + _instance.Data.ResourcePath);
		}
	}
}

#endif
