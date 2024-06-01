using Godot;
using Nilsiker.GodotTools.Dialogue.Channels;
using Nilsiker.GodotTools.Dialogue.Editor.Models;
using Nilsiker.GodotTools.Dialogue.Models;
using Nilsiker.GodotTools.Extensions;
using System;

namespace Nilsiker.GodotTools.Dialogue.Example
{
	public partial class ExampleDialogueContainer : PanelContainer
	{
		[Export] Label _nameLabel;
		[Export] Label _lineLabel;
		[Export] TextureRect _portrait;
		[Export] float _lineSpeed = 20;
		float _lineProgress = 0;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			DialogueChannel.DialogueLineUpdated += _OnDialogueLineUpdated;
			DialogueChannel.DialogueEnded += _OnDialogueEnded;
			DialogueChannel.Data = GD.Load<DialogueResource>("res://addons/DialogueSystem/examples/example_dialogue.tres");
		}

		public override void _ExitTree()
		{
			DialogueChannel.DialogueLineUpdated -= _OnDialogueLineUpdated;
			DialogueChannel.DialogueEnded -= _OnDialogueEnded;
		}

		public override void _Process(double delta)
		{
			base._Process(delta);
			_lineProgress += (float)delta * _lineSpeed;

			_lineLabel.VisibleCharacters = Mathf.FloorToInt(_lineProgress);
		}

		private void _OnDialogueEnded()
		{
			GetTree().Quit();
		}

		private void _OnDialogueLineUpdated(NodeData data)
		{
			_lineProgress = 0;
			if (data is LineData lineData)
			{
				_nameLabel.Text = lineData.name;
				_lineLabel.Text = lineData.line;
				_portrait.Texture = lineData.portrait;
			}
		}

		public override void _GuiInput(InputEvent @event)
		{
			base._GuiInput(@event);
			if (@event is InputEventMouseButton mouseButton && @event.IsReleased() && mouseButton.ButtonIndex == MouseButton.Left)
			{
				DialogueChannel.Progress();
			}
		}
	}

}
