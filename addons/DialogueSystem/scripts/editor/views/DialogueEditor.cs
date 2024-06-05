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
		[Export] DialoguePreview _preview = null!;
		[Export] Button _runButton = null!;

		DialogueResource? _resource;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			_runButton.Pressed += _OnTestButtonPressed;
		}

		public override void _ExitTree()
		{
			base._ExitTree();
			Data = null;
		}

		private void _OnTestButtonPressed()
		{
			_SaveResourceToPreview();
			_preview.Play();
		}

		private void _SaveResourceToPreview()
		{
			var path = Constants.Paths.PreviewDialogueResource;
			ResourceSaver.Singleton.Save(_resource, path);
			EditorInterface.Singleton.GetResourceFilesystem().UpdateFile(path);
			EditorInterface.Singleton.GetResourceFilesystem().ReimportFiles(new[] { path });
		}
	}
}
