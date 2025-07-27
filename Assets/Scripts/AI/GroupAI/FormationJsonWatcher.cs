#if UNITY_EDITOR
using System.IO;
using UnityEditor;

namespace AI.GroupAI
{
    [InitializeOnLoad]
    public class FormationJsonWatcher {
        static FormationJsonWatcher() {
            var watcher = new FileSystemWatcher("Assets/Resources/Data", "*.json");
            watcher.Changed += (s, e) => {
                // Parse and push new params into your managers here
            };
            watcher.EnableRaisingEvents = true;
        }
    }
}
#endif