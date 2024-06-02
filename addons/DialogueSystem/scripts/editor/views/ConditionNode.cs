using Godot;
using Nilsiker.GodotTools.Dialogue.Editor.Models;
using System;

namespace Nilsiker.GodotTools.Dialogue.Editor.Views
{
	[Tool]
	public partial class ConditionNode : DialogueNode
	{
		[Export] LineEdit _conditionName;
		[Export] ConditionData _data;
		public int TrueSlot = 1;
		public int FalseSlot = 2;

		public override NodeData Data { get => _data; set => _data = (ConditionData)value; }

		public override void _Ready()
		{
			base._Ready();
			_conditionName.TextChanged += (text) => _data.ConditionName = text;

			if (_data == null) return;
			_conditionName.Text = _data.ConditionName;
		}
	}
}
