using System;
using Godot;
using Nilsiker.GodotTools.Convenience;
using Nilsiker.GodotTools.Dialogue.Models;

namespace Nilsiker.GodotTools.Dialogue.Editor.Views
{
	[Tool]
	public partial class DialogueEditor : Control
	{
		[Signal] public delegate void RefreshClickedEventHandler();

		public Action<DialogueResource?>? DataChanged { get; set; }
		public DialogueResource? Data
		{
			get => _resource; set
			{
				if (_resource == value) return;
				_resource = value;
				DataChanged?.Invoke(_resource);
			}
		}
		[Export] DialogueGraph _graph = null!;
		[Export] Control _preview = null!;
		[Export] Button _runButton = null!;

		DialogueResource? _resource;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			_runButton.Pressed += _OnTestButtonPressed;
		}

		private void _OnTestButtonPressed()
		{
			this.Log("Running " + _resource?.Name + " in editor.");
			this.Log("Dialogue Info: " + _resource?.Nodes);

			var path = "res://addons/DialogueSystem/examples/example_dialogue.tres";

			ResourceSaver.Singleton.Save(_resource, path);
			EditorInterface.Singleton.GetResourceFilesystem().UpdateFile(path);
			EditorInterface.Singleton.GetResourceFilesystem().ReimportFiles(new[] { path });

			var scene = GD.Load<PackedScene>("res://addons/DialogueSystem/examples/example_dialogue.tscn").Instantiate();
			_preview.AddChild(scene);
			_preview.Visible = true;
			scene.TreeExited += () => _preview.Visible = false;
		}
	}
}
