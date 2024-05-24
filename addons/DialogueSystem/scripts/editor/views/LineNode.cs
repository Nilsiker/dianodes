using System;
using Godot;
using Nilsiker.GodotTools.Dialogue.Editor.Models;

namespace Nilsiker.GodotTools.Dialogue.Editor.Views
{
	[Tool]
	public partial class LineNode : GraphNode, IHasPortrait, IHasNodeData
	{
		[Export] LineData _resource;
		[Export] LineEdit _characterNameLineEdit;
		[Export] Control _portraitContainer;
		[Export] TextureButton _portraitButton;
		[Export] FileDialog _portraitFileDialog;

		public NodeData Data
		{
			get => _resource; set
			{
				var data = (LineData)value;
				_resource = data;
				_characterNameLineEdit.Text = data.name;
				_portraitButton.TextureNormal = data.portrait;
				CallDeferred("set_size", data.size);
				PositionOffset = data.position;
			}
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Resized += _OnResized;
			PositionOffsetChanged += _OnPositionOffsetChanged;
		}


		public void SetPortraitVisibility(bool visible)
		{
			_portraitContainer.Visible = visible;
			CallDeferred("Resize");
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

		private void _OnResized() => Data.size = Size;

		private void _OnPositionOffsetChanged() => Data.position = PositionOffset;

		private void _OnPortraitClear() => _portraitButton.TextureNormal = null;

		private void _OnPortraitLoad(string texturePath)
		{
			_portraitButton.TextureNormal = GD.Load<Texture2D>(texturePath);
		}
	}
}
