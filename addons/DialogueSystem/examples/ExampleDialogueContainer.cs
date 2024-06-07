using Godot;
using Nilsiker.GodotTools.Dialogue.Channels;
using Nilsiker.GodotTools.Dialogue.Editor.Models;
using Nilsiker.GodotTools.Dialogue.Models;
using Nilsiker.GodotTools.Dialogue.Convenience;

namespace Nilsiker.GodotTools.Dialogue.Example
{
	[Tool]
	public partial class ExampleDialogueContainer : PanelContainer
	{
		[Signal] public delegate void DummyEventEventHandler();


		[Export] Label _nameLabel = null!;
		[Export] Label _lineLabel = null!;
		[Export] TextureRect _portrait = null!;
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

			var data = GD.Load<DialogueResource>(Constants.Paths.TempDialogueResource);
			this.Log("Example Dialogue starting: " + data.Name);
			DialogueChannel.Load(data, delegates);
		}

		public override void _ExitTree()
		{
#nullable disable
			DialogueChannel.DialogueLineUpdated -= _OnDialogueLineUpdated;
			DialogueChannel.DialogueEnded -= _OnDialogueEnded;
#nullable enable
		}

		public override void _Process(double delta)
		{
			base._Process(delta);
			if (_lineLabel is null) return;
			_lineProgress += (float)delta * _lineSpeed;
			_lineLabel.VisibleCharacters = Mathf.FloorToInt(_lineProgress);
		}

		private void _OnDialogueEnded()
		{
			QueueFree();
		}

		private void _OnDialogueLineUpdated(NodeResource data)
		{
			_lineProgress = 0;
			if (data is LineNodeResource lineData)
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
