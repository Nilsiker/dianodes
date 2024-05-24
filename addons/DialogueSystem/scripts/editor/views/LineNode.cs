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
		[Export] LineData _resource;

		public NodeData Data
		{
			get => _resource; set
			{
				var data = (LineData)value;
				_resource = data;
				_name.Text = data.name;
				_line.Text = data.line;
				_portraitButton.TextureNormal = data.portrait;
				
				PositionOffset = data.position;
				CallDeferred("set_size", data.size);
			}
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Resized += _OnResized;
			PositionOffsetChanged += _OnPositionOffsetChanged;

			_name.TextChanged += name => _resource.name = name;
			_line.TextChanged += () => _resource.line = _line.Text;
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
