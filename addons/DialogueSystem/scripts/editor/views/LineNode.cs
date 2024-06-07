using System;
using Godot;
using Nilsiker.GodotTools.Dialogue.Editor.Models;

namespace Nilsiker.GodotTools.Dialogue.Editor.Views
{
	[Tool]
	public partial class LineNode : DialogueNode, IDialogueNode, IHasPortrait
	{
		[Export] LineEdit _name;
		[Export] TextEdit _line;
		[Export] Control _portraitContainer;
		[Export] TextureButton _portraitButton;
		[Export] FileDialog _portraitFileDialog;
		[Export] Button _addOptionButton;
		[Export] LineNodeResource _data;

		public override NodeResource Data
		{
			get => _data; set => _data = (LineNodeResource)value;
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			base._Ready();

			_name.TextChanged += name => _data.Name = name;
			_line.TextChanged += () => _data.Line = _line.Text;

			_portraitButton.Pressed += () => _portraitFileDialog.Popup();
			_portraitFileDialog.FileSelected += (file) =>
			{
				var portrait = GD.Load<Texture2D>(file);
				_portraitButton.TextureNormal = portrait;
				_data.Portrait = portrait;
			};
			_addOptionButton.Pressed += _OnAddOptionButtonPressed;
			
			Resized += _OnResized;

			if (_data == null) return;
			_name.Text = _data.Name;
			_line.Text = _data.Line;

			if (_data.Portrait != null)
			{
				_portraitButton.TextureNormal = _data.Portrait;
			}
			if (GetTree().CurrentScene == this) return;
			_portraitContainer.Visible = !GetParent<DialogueGraph>().HidePortraits;
		}	

        private void _OnAddOptionButtonPressed()
        {
            
        }

        public void SetPortraitVisibility(bool visible)
		{
			_portraitContainer.Visible = visible;
		}

		private void _OnResized()
		{
			if (Data == null) return;
			Data.Size = Size;
		}


		private void _OnPortraitClear() => _portraitButton.TextureNormal = null;

		private void _OnPortraitLoad(string texturePath)
		{
			_portraitButton.TextureNormal = GD.Load<Texture2D>(texturePath);
		}
	}
}
