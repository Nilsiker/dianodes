using System;
using Godot;
using Nilsiker.GodotTools.Dialogue.Convenience;
using Nilsiker.GodotTools.Dialogue.Models;

namespace Nilsiker.GodotTools.Dialogue.Editor.Views
{
	[Tool]
	public partial class DialogueEditor : Control
	{
		public DialogueFiles Files => _files;
		public DialogueGraph Graph => _graph;

		[Export] DialogueFiles _files = null!;
		[Export] DialogueGraph _graph = null!;
		[Export] DialoguePreview _preview = null!;
		[Export] Button _runButton = null!;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			_runButton.Pressed += _OnTestButtonPressed;
			_files.FilePicked += _OnFilePicked;
			_graph.DataModified += _OnGraphDataModified;
		}

		public override void _ExitTree()
		{
			base._ExitTree();
			this.Log("editor exiting tree");
		}

		public override void _UnhandledInput(InputEvent @event)
		{

			if (@event is InputEventWithModifiers modEvent && modEvent.ShiftPressed && Input.IsKeyPressed(Key.S))
			{
				Files.SaveFile();
			}
		}

		public void LoadResource(DialogueResource resource)
		{
			if (resource is null) return;
			_graph.RegisterData(resource);
		}

		private void _OnTestButtonPressed()
		{
			_preview.Play(_files.OpenedDialoguePath);
		}

		private void _OnFilePicked(DialogueResource resource)
		{
			this.Log($"picked {resource.Name}");
			LoadResource(resource);
		}

		private void _OnGraphDataModified()
		{
			_files.MarkCurrentAsModified();
		}
	}
}
