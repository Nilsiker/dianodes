using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
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
                current = _data.Nodes.FirstOrDefault().Guid;
                UpdateLine(_data.Nodes.FirstOrDefault());
                GD.Print(_data.Nodes.Count);
            }
        }

        public static Dictionary<string, Action> eventBlackboard;

        public static void Load(DialogueResource data, Dictionary<string, Action> blackboard)
        {
            Data = data;
            eventBlackboard = blackboard;
        }


        public static void Progress()
        {
            var next = _data.GetNode(current, 0);
            if (next != null)
            {
                current = next.Guid;
                if (next is EventData ed)
                {
                    if (eventBlackboard.TryGetValue(ed.EventName, out Action action))
                    {
                        action.Invoke();
                    }
                    Progress();
                    return;
                }
                UpdateLine(next);
            }
            else
            {
                DialogueEnded?.Invoke();
            }
        }
    }
}
