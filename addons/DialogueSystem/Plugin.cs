#if TOOLS
using System;
using Godot;
using Nilsiker.GodotTools.Dialogue.Convenience;
using Nilsiker.GodotTools.Dialogue.Editor.Views;
using Nilsiker.GodotTools.Dialogue.Models;
using static Nilsiker.GodotTools.Dialogue.Utilities;
namespace Nilsiker.GodotTools.Dialogue
{
	/// <summary>
	/// The main plugin class for the DialogueSystem.
	/// It handles the plugin registration, instantiating and showing the editor, and keeps the reference to the currently edited resource.
	/// </summary>
	[Tool]
	public partial class Plugin : EditorPlugin
	{
		PackedScene _mainControl = GD.Load<PackedScene>(GetScenePath("dialogue_editor"));
		Texture2D _icon = GD.Load<Texture2D>(GetSvgPath("dialogue"));
		Texture2D _resourceIcon = GD.Load<Texture2D>("res://addons/DialogueSystem/icons/dialogue_resource.svg");
		DialogueEditor? _editor;

		public override void _EnablePlugin()
		{
			base._EnablePlugin();
		}

		public override void _DisablePlugin()
		{
			base._DisablePlugin();
		}
		public override void _ExitTree()
		{
			base._ExitTree();
			if (_editor == null) return;
			_editor?.QueueFree();
		}

		public override void _Ready()
		{
			base._Ready();
			_editor = _mainControl.Instantiate<DialogueEditor>();
			_editor.Visible = false;
			EditorInterface.Singleton.GetEditorMainScreen().AddChild(_editor);
		}

		public override bool _HasMainScreen() => true;
		public override string _GetPluginName() => "Dialogues";
		public override Texture2D _GetPluginIcon() => _icon;

		public override void _MakeVisible(bool visible)
		{
			if (_editor == null) return;
			if (visible)
			{
				_Reload();
			}
			_editor.Visible = visible;
		}

		private void _Reload()
		{
			if (_editor == null) return;
			this.Log("Reloading...");
			_editor.QueueFree();
			_editor = _mainControl.Instantiate<DialogueEditor>();
			EditorInterface.Singleton.GetEditorMainScreen().AddChild(_editor);
		}

		public override bool _Handles(GodotObject @object)
		{
			return @object is DialogueResource;
		}

		public override void _Edit(GodotObject @object)
		{
			if (_editor is null || @object is null) return;
			var resource = (DialogueResource)@object;
			_editor.Files.Load(resource);
		}

		public override string _GetUnsavedStatus(string forScene)
		{
			if (_editor.Files.AnyUnsavedFiles) return "There are unsaved dialogues. Would you like to save them?";
			return base._GetUnsavedStatus(forScene);
		}

        public override void _SaveExternalData()
        {
            base._SaveExternalData();
			_editor.Files.SaveAll();
        }
    }
}

#endif
