using System;
using System.Collections.Generic;
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
                current = _data.Nodes.FirstOrDefault().Guid;
                UpdateLine(_data.Nodes.FirstOrDefault());
                GD.Print(_data.Nodes.Count);
            }
        }

        public static Dictionary<string, Delegate> blackboard;

        public static void Load(DialogueResource data, Dictionary<string, Delegate> blackboard)
        {
            Data = data;
            DialogueChannel.blackboard = blackboard;
        }


        public static void Progress(int slot)
        {
            var next = _data.GetNode(current, slot);
            GD.Print("Next is: ", Json.Stringify(next));
            if (next != null)
            {
                current = next.Guid;
                switch (next)
                {
                    case EventData ed:
                        if (blackboard.TryGetValue(ed.EventName, out Delegate action))
                        {
                            var ret = action.DynamicInvoke();
                            GD.Print(ret);
                        }
                        else
                        {
                            GD.PushWarning($@"Event ""{ed.EventName}"" not found in dialogue blackboard.");

                        }
                        Progress(0);
                        return;
                    case ConditionData cd:
                        GD.Print(cd.ConditionName);
                        if (blackboard.TryGetValue(cd.ConditionName, out action))
                        {
                            GD.Print("condition returned ", (bool)action.DynamicInvoke());
                            Progress((bool)action.DynamicInvoke() ? 0 : 1);
                            return;
                        }
                        else
                        {
                            GD.PushWarning($@"Condition ""{cd.ConditionName}"" not found in dialogue blackboard.");
                        }
                        break;
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
