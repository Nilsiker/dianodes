using Godot;
using Nilsiker.GodotTools.Dialogue.Channels;
using Nilsiker.GodotTools.Dialogue.Editor.Models;
using Nilsiker.GodotTools.Dialogue.Models;
using Nilsiker.GodotTools.Convenience;

namespace Nilsiker.GodotTools.Dialogue.Example
{
	[Tool]
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

			var delegates = Utilities.CreateBlackboard(
				new("add_10_coins", () => GD.Print("Add 10 coins!")),
				new("feeling_generous", () => Rnd.Chance(50))
			);
			var data = GD.Load<DialogueResource>("res://addons/DialogueSystem/examples/example_dialogue.tres");
			this.Log("Example Dialogue starting: " + data.Name);
			DialogueChannel.Load(data, delegates);
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
			QueueFree();
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
				DialogueChannel.Progress(0);
			}
		}
	}

}
