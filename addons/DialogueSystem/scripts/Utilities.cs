
using System;
using System.Collections.Generic;
using Godot;

namespace Nilsiker.GodotTools.Dialogue
{
    public class Connection
    {
        public StringName FromNode { get; set; }
        public int FromPort { get; set; }
        public StringName ToNode { get; set; }
        public int ToPort { get; set; }
    }

    public class Utilities
    {
        public static string PluginPath => "res://addons/DialogueSystem";
        public static string GetScenePath(string sceneName) => $"{PluginPath}/scenes/{sceneName}.tscn";
        public static string GetSvgPath(string iconName) => $"{PluginPath}/icons/{iconName}.svg";
        public static Connection ParseConnection(Godot.Collections.Dictionary connection) => new()
        {
            FromNode = (string)connection["from_node"],
            FromPort = (int)connection["from_port"],
            ToNode = (string)connection["to_node"],
            ToPort = (int)connection["to_port"],
        };


        public class EventBlackboardEntry
        {
            public string EventName { get; set; }
            public Action Emit { get; set; }
        }

        public static Dictionary<string, Action> CreateEventBlackboard(params EventBlackboardEntry[] entries)
        {
            var blackboard = new Dictionary<string, Action>();
            foreach (var entry in entries)
            {
                blackboard.Add(entry.EventName, entry.Emit);
            }

            return blackboard;
        }
    }
}
