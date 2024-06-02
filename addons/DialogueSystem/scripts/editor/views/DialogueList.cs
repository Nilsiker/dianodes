using Godot;
using Nilsiker.GodotTools.Dialogue.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nilsiker.GodotTools.Dialogue.Editor.Views
{
	[Tool]
	public partial class DialogueList : ItemList
	{
		[Export] DialogueEditor _editor;

		HashSet<string> paths = new HashSet<string>();

		public override void _Ready()
		{
			base._Ready();
			_editor.DataChanged += _OnDataChanged;
		}

		private void _OnDataChanged(DialogueResource data)
		{
			var path = data.ResourcePath;
			if (paths.Contains(path)) return;
			var name = data.ResourcePath.Split('/').LastOrDefault();
			paths.Add(path);
			AddItem(name);
		}

		public override void _ExitTree()
		{
			_editor.DataChanged -= _OnDataChanged;
			base._ExitTree();
		}
	}
}
