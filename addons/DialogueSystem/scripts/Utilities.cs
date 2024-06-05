
using System;
using System.Collections.Generic;
using Godot;

namespace Nilsiker.GodotTools.Dialogue
{
    public class Utilities
    {
        public static string PluginPath => "res://addons/DialogueSystem";
        public static string GetScenePath(string sceneName) => $"{PluginPath}/scenes/{sceneName}.tscn";
        public static string GetSvgPath(string iconName) => $"{PluginPath}/icons/{iconName}.svg";
        public static Connection ParseConnection(Godot.Collections.Dictionary connection) => new()
        {
            FromNode = connection["from_node"].ToString(),
            FromPort = (int)connection["from_port"],
            ToNode = connection["to_node"].ToString(),
            ToPort = (int)connection["to_port"],
        };

        public static Dictionary<string, Delegate> CreateBlackboard(params KeyValuePair<string, Delegate>[] entries)
        {
            var blackboard = new Dictionary<string, Delegate>();
            foreach (var entry in entries)
            {
                blackboard.Add(entry.Key, entry.Value);
            }

            return blackboard;
        }
    }
}
