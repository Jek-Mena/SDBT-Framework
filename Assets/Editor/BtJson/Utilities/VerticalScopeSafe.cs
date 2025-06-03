using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

// TODO: ScopeSafe Utilities
// --------------------------

// Immediate:
// [x] Implement VerticalScopeSafe with context label and stopwatch support
// [x] Replace manual BeginVertical/EndVertical with using(VerticalScopeSafe) in NodeInspectorPanel.cs

// Next Scope Helpers:
// [ ] FoldoutScopeSafe
//     - Manages EditorGUI.Foldout state using a unique key or label
//     - Usage: using (new FoldoutScopeSafe("Details", ref isExpanded)) { ... }
//     - Optional: persist foldout state using EditorPrefs or internal static cache

// [ ] HorizontalScopeSafe
//     - Same pattern as VerticalScopeSafe, but wraps BeginHorizontal/EndHorizontal
//     - Add stopwatch and debug support for symmetry

// [ ] ScrollViewScopeSafe
//     - Wraps BeginScrollView/EndScrollView
//     - Automatically preserves scroll position
//     - Optional: allow external scroll vector ref for multi-panel coordination

// [ ] DisabledScopeSafe
//     - Wraps EditorGUI.BeginDisabledGroup / EndDisabledGroup
//     - Usage: using (new DisabledScopeSafe(condition)) { ... }

// Optional Enhancements:
// [ ] Create GuiScopeBase : IDisposable for shared logic (timing, logging, nesting checks)
// [ ] Add #if DEBUG_GUI conditional logging support
// [ ] Consider a GUIStyle registry for shared look-and-feel control across scopes

namespace Editor.BtJson.Utilities
{
    public class VerticalScopeSafe : IDisposable
    {
        private readonly string _contextLabel;
        private readonly Stopwatch _stopwatch;
        
        public VerticalScopeSafe(string contextLabel = null, GUIStyle style = null, params GUILayoutOption[] options)
        {
            _contextLabel = contextLabel;

            if (!string.IsNullOrEmpty(_contextLabel))
            {
                _stopwatch = Stopwatch.StartNew();
                Debug.Log($"[UI Scope Start] {_contextLabel}");
            }

            if (style != null)
                EditorGUILayout.BeginVertical(style, options);
            else
                EditorGUILayout.BeginVertical(options);
        }
        
        public void Dispose()
        {
            if (!string.IsNullOrEmpty(_contextLabel))
            {
                _stopwatch.Stop();
                Debug.Log($"[UI Scope End] {_contextLabel} — {_stopwatch.ElapsedMilliseconds}ms");
            }

            EditorGUILayout.EndVertical();
        }
    }
}