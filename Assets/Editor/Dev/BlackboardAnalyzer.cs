using UnityEngine;

namespace Dev
{
// TemporaryBlackboardAnalyzer.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;

public class BlackboardAnalyzer : MonoBehaviour
{
    [ContextMenu("Analyze Blackboard Usage")]
    void AnalyzeUsage()
    {
        var projectPath = Application.dataPath + "/Scripts";
        var files = System.IO.Directory.GetFiles(projectPath, "*.cs", System.IO.SearchOption.AllDirectories);
        
        var setPattern = @"Blackboard\.Set(?:<[^>]+>)?\s*\(\s*([^,]+)";
        var getPattern = @"Blackboard\.Get(?:<([^>]+)>)?\s*\(\s*([^,\)]+)";
        
        var usage = new Dictionary<string, HashSet<string>>();
        
        foreach (var file in files)
        {
            var content = System.IO.File.ReadAllText(file);
            var fileName = System.IO.Path.GetFileName(file);
            
            // Find Set calls
            var setMatches = Regex.Matches(content, setPattern);
            foreach (Match match in setMatches)
            {
                var key = match.Groups[1].Value.Trim();
                if (!usage.ContainsKey(key))
                    usage[key] = new HashSet<string>();
                usage[key].Add($"SET in {fileName}");
            }
            
            // Find Get calls
            var getMatches = Regex.Matches(content, getPattern);
            foreach (Match match in getMatches)
            {
                var type = match.Groups[1].Value.Trim();
                var key = match.Groups[2].Value.Trim();
                if (!usage.ContainsKey(key))
                    usage[key] = new HashSet<string>();
                usage[key].Add($"GET<{type}> in {fileName}");
            }
        }
        
        Debug.Log("=== BLACKBOARD KEY USAGE ===");
        foreach (var kvp in usage.OrderBy(x => x.Key))
        {
            Debug.Log($"KEY: {kvp.Key}");
            foreach (var use in kvp.Value)
                Debug.Log($"  - {use}");
        }
        
        Debug.Log($"\nTotal unique keys: {usage.Count}");
    }
}
}