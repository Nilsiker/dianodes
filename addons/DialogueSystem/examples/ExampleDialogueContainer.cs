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
		[Signal] public delegate void DummyEventEventHandler();

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
			var data = GD.Load<DialogueResource>("res://addons/DialogueSystem/examples/example_dialogue.tres");

			var blackboard = Utilities.CreateEventBlackboard(new Utilities.EventBlackboardEntry()
			{
				EventName = "add_10_coins",
				Emit = () => GD.Print($"This is a dummy placeholder for logic that grants the player 10 coins!")
			});

			DialogueChannel.Load(data, blackboard);
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
				_nameLabel.Text = lineData.Name;
				_lineLabel.Text = lineData.Line;
				_portrait.Texture = lineData.Portrait;
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
