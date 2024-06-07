using Godot;
using Nilsiker.GodotTools.Dialogue.Convenience;
using Nilsiker.GodotTools.Dialogue.Models;
using System;
using System.Linq;
using System.Security.Principal;

namespace Nilsiker.GodotTools.Dialogue.Editor.Views
{
	[Tool]
	public partial class DialogueFiles : ItemList
	{
		public Action<DialogueResource>? FilePicked { get; set; }

		public bool AnyUnsavedFiles => _GetAnyUnsavedFileStatus();
		public DialogueResource OpenedFile => _files[_selected];
		public string OpenedDialoguePath => _paths[_selected];

		[Export] Godot.Collections.Dictionary<long, DialogueResource> _files = new();
		[Export] Godot.Collections.Dictionary<long, string> _paths = new();

		private long _selected;

		public override void _Ready()
		{
			base._Ready();
			ItemSelected += _OnItemSelected;
		}

		private void _OnItemSelected(long index)
		{
			_selected = index;
			var fileName = GetItemText((int)_selected);
			var path = _paths[_selected];
			FilePicked?.Invoke(_files[_selected]);
		}

		private bool _GetAnyUnsavedFileStatus()
		{
			for (int i = 0; i < ItemCount; ++i)
			{
				if (GetItemText(i).EndsWith(Constants.UnsavedSuffix))
				{
					return true;
				}
			}
			return false;
		}


		public void Load(DialogueResource file)
		{
			int index;
			if (_paths.Values.Contains(file.ResourcePath))
			{

				index = (int)_paths
					.Where(entry => entry.Value == file.ResourcePath)
					.First().Key;
			}
			else
			{
				var path = file.ResourcePath;
				string name = file.ResourcePath.Split('/').Last();
				var data = (DialogueResource)file.Duplicate(true);
				data.ResourceLocalToScene = true;
				_paths.Add(ItemCount, path);
				_files.Add(ItemCount, data);

				index = AddItem(name);
			}

			Select(index);
			_OnItemSelected(index);
		}

		public void MarkCurrentAsModified()
		{
			this.Log($"Setting unsaved on {_selected}");
			var text = GetItemText((int)_selected);
			if (text.EndsWith(Constants.UnsavedSuffix)) return;
			SetItemText((int)_selected, text + Constants.UnsavedSuffix);
		}

		public void SaveFile(long? index = null)
		{
			var idx = index ?? _selected;
			var path = _paths[idx];
			var data = _files[idx];

			var res = ResourceSaver.Singleton.Save(data, path);
			ResourceLoader.Load<DialogueResource>(path, "DialogueResource", ResourceLoader.CacheMode.ReplaceDeep);

			_MarkAsSaved((int)idx);
			this.Log($"{res}: Saving {data.Name} to {path}");
		}

		private void _MarkAsSaved(int index)
		{
			var text = GetItemText(index);
			var unsavedSuffixIndex = text.FindN(Constants.UnsavedSuffix);
			if (unsavedSuffixIndex >= 0)
			{
				SetItemText(index, text.Remove(unsavedSuffixIndex));
			}
		}

		public void SaveAll()
		{
			for (int i = 0; i < ItemCount; ++i)
			{
				SaveFile(i);
			}
		}
	}
}
