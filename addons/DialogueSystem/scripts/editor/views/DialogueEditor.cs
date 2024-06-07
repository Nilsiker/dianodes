using System;
using Godot;
using Nilsiker.GodotTools.Dialogue.Convenience;
using Nilsiker.GodotTools.Dialogue.Models;

namespace Nilsiker.GodotTools.Dialogue.Editor.Views
{
	[Tool]
	public partial class DialogueEditor : Control
	{
		[Export] DialogueGraph _graph = null!;
		[Export] DialoguePreview _preview = null!;
		[Export] Button _runButton = null!;

		DialogueResource _data = GD.Load<DialogueResource>(Constants.Paths.TempDialogueResource);

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			_runButton.Pressed += _OnTestButtonPressed;
		}

		public override void _ExitTree()
		{
			base._ExitTree();
			this.Log("editor exiting tree");
		}

		public void LoadResource(DialogueResource resource)
		{
			if (resource is null) return;
			
			_data.Name = resource.Name;
			_data.Nodes = resource.Nodes;
			_data.Connections = resource.Connections;
			_data.ScrollOffset = resource.ScrollOffset;
			_data.Zoom = resource.Zoom;
			_data.ShowPortraits = resource.ShowPortraits;

			_graph.Refresh();
		}

		public override void _UnhandledInput(InputEvent @event)
		{

			if (@event is InputEventWithModifiers modEvent && modEvent.ShiftPressed && Input.IsKeyPressed(Key.S))
			{
				_SaveDialogue();
			}
		}


		private void _SaveDialogue()
		{
			var loadedPath = ProjectSettings.Singleton.GetSetting("dialogue/loaded_path").AsString();
			var res = ResourceSaver.Singleton.Save(_data, loadedPath);
			EditorInterface.Singleton.GetResourceFilesystem().UpdateFile(loadedPath);
			EditorInterface.Singleton.GetResourceFilesystem().ReimportFiles(new[] { loadedPath });

			this.Log(res);
		}

		private void _OnTestButtonPressed()
		{
			_preview.Play();
		}
	}
}
