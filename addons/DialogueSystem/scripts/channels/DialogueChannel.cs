using System;
using System.Linq;
using Godot;
using Nilsiker.GodotTools.Dialogue.Editor.Models;
using Nilsiker.GodotTools.Dialogue.Models;
namespace Nilsiker.GodotTools.Dialogue.Channels
{
    public static class DialogueChannel
    {
        public static Action<NodeData> DialogueLineUpdated { get; set; }
        public static Action DialogueEnded { get; set; }

        public static void UpdateLine(NodeData line) => DialogueLineUpdated?.Invoke(line);

        private static string current;
        private static DialogueResource _data;
        public static DialogueResource Data
        {
            set
            {
                _data = value;
                current = _data.nodes.FirstOrDefault().guid;
                UpdateLine(_data.nodes.FirstOrDefault());
                GD.Print(_data.nodes.Count);
            }
        }


        public static void Progress()
        {
            var next = _data.GetNode(current, 0);
            if (next != null)
            {
                current = next.guid;
                UpdateLine(next);
            } else {
                DialogueEnded?.Invoke();
            }
        }
    }
}
