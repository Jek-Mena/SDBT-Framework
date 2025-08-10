// Assets/Editor/BlackboardAnalyzerTool.cs

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Editor.Dev
{
    public static class BlackboardAnalyzerTool
    {
        [MenuItem("Tools/Blackboard/Analyze Usage")]
        public static void AnalyzeUsage()
        {
            // Scan all of Assets (not just Assets/Scripts)
            var root = Application.dataPath;
            var files = Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories);

            // Slightly safer patterns; handle optional generic and spacing
            var setPattern = @"Blackboard\.Set(?:<[^>]+>)?\s*\(\s*([^,\)]+)";
            var getPattern = @"Blackboard\.Get(?:<([^>]+)>)?\s*\(\s*([^,\)]+)";

            var usage = new Dictionary<string, HashSet<string>>();

            foreach (var file in files)
            {
                var content = File.ReadAllText(file);
                var fileName = Path.GetFileName(file);

                foreach (Match m in Regex.Matches(content, setPattern))
                {
                    var key = m.Groups[1].Value.Trim().Trim('"');
                    if (!usage.TryGetValue(key, out var set))
                        usage[key] = set = new HashSet<string>();
                    set.Add($"SET in {fileName}");
                }

                foreach (Match m in Regex.Matches(content, getPattern))
                {
                    var type = m.Groups[1].Success ? m.Groups[1].Value.Trim() : "?";
                    var key  = m.Groups[2].Value.Trim().Trim('"');
                    if (!usage.TryGetValue(key, out var set))
                        usage[key] = set = new HashSet<string>();
                    set.Add($"GET<{type}> in {fileName}");
                }
            }

            Debug.Log("=== BLACKBOARD KEY USAGE ===");
            foreach (var kvp in usage.OrderBy(k => k.Key))
            {
                Debug.Log($"KEY: {kvp.Key}");
                foreach (var u in kvp.Value)
                    Debug.Log($"  - {u}");
            }
            Debug.Log($"\nTotal unique keys: {usage.Count}");
        }
    }
}
