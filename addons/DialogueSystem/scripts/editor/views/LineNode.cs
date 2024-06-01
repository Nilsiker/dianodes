using System;
using Godot;
using Nilsiker.GodotTools.Dialogue.Editor.Models;

namespace Nilsiker.GodotTools.Dialogue.Editor.Views
{
	[Tool]
	public partial class LineNode : GraphNode, IHasPortrait, IHasNodeData
	{
		[Export] LineEdit _name;
		[Export] TextEdit _line;
		[Export] Control _portraitContainer;
		[Export] TextureButton _portraitButton;
		[Export] FileDialog _portraitFileDialog;
		[Export] LineData _data;

		public NodeData Data
		{
			get => _data; set => _data = (LineData)value;
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Resized += _OnResized;
			PositionOffsetChanged += _OnPositionOffsetChanged;

			_name.TextChanged += name => _data.name = name;
			_line.TextChanged += () => _data.line = _line.Text;

			_name.Text = _data.name;
			_line.Text = _data.line;
			if (_data.portrait != null)
			{
				_portraitButton.TextureNormal = _data.portrait;
			}
			_portraitContainer.Visible = !GetParent<DialogueEditor>().HidePortraits;

			PositionOffset = Data.position;
			CallDeferred("set_size", Data.size);

			_portraitButton.Pressed += () => _portraitFileDialog.Popup();
			_portraitFileDialog.FileSelected += (file) =>
			{
				var portrait = GD.Load<Texture2D>(file);
				_portraitButton.TextureNormal = portrait;
				_data.portrait = portrait;
			};
		}

		public void SetPortraitVisibility(bool visible)
		{
			_portraitContainer.Visible = visible;
		}

		private void Resize()
		{
			if (_portraitContainer.Visible)
			{
				Size = new(Size.X + 100, Size.Y);
			}
			else
			{
				Size = new(Size.X - 100, Size.Y);
			}
		}

		private void _OnResized()
		{
			Data.size = Size;
		}

		private void _OnPositionOffsetChanged() => Data.position = PositionOffset;

		private void _OnPortraitClear() => _portraitButton.TextureNormal = null;

		private void _OnPortraitLoad(string texturePath)
		{
			_portraitButton.TextureNormal = GD.Load<Texture2D>(texturePath);
		}
	}
}
