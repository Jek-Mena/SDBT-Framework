using AI.BehaviorTree.Keys;
using AI.BehaviorTree.Runtime.Context;
using AI.BehaviorTree.Stimulus;
using AI.SquadAI;
using UnityEngine;

namespace Dev.OverlayDebugView
{
    /// <summary>
    /// MonoBehaviour: attaches to agent root, handles overlay rendering and selection.
    /// Uses DebugOverlayData for all debug info generation.
    /// </summary>
    public class DebugOverlayView : MonoBehaviour
    {
        public DebugOverlayData OverlayData { get; private set; }

        /// <summary>
        /// Call this from your agent/AI setup code.
        /// </summary>
        public void Initialize(BtContext context)
        {
            // Use gameObject.name for agent label, or override as needed
            OverlayData = new DebugOverlayData(context)
            {
                SquadAgent = context.Blackboard.Get<ISquadAgent>(BlackboardKeys.Group.SquadAgent)
            };
        }

        void OnGUI()
        {
            // Only draw overlay if selected
            if (!AgentSelectionManager.Instance.IsSelected(this))
                return;

            var debugString = OverlayData.BuildDebugString();

            // Upper left label
            const float labelWidth = 400;
            const float labelHeight = 1000;
            const float labelX = 20;
            const float labelY = 20;

            // Upper right graph
            const float graphWidth = 400;
            const float graphHeight = 300;
            float graphX = Screen.width - graphWidth - 20, graphY = 20;

            GUI.Label(new Rect(labelX, labelY, labelWidth, labelHeight), debugString);
            DrawStimuliModulationGraph(graphX, graphY, graphWidth, graphHeight);
        }
        
        void DrawStimuliModulationGraph(float x, float y, float width, float height)
        {
            // Get curves and stimulus value from data
            var curves = OverlayData.GetCurveProfiles(); // List<CurveProfileEntry>
            float stimulus = OverlayData.GetCurrentStimulusValue(); // [0..1]

            int N = 60; // Sample count
            int margin = 30;

            // Axis ranges
            float minX = 0f, maxX = 1f;
            float minY = 0f, maxY = 1.25f; // y max depends on your highest "max" value

            // Draw axes
            GUI.color = Color.white;
            GUI.Box(new Rect(x, y, width, height), GUIContent.none);

            // Draw curves
            for (int c = 0; c < curves.Count; c++)
            {
                var curve = curves[c];
                Color curveColor = Color.HSVToRGB((float)c / curves.Count, 0.8f, 0.8f);
                Vector2? last = null;
                for (int i = 0; i < N; i++)
                {
                    var stim = Mathf.Lerp(minX, maxX, i / (float)(N - 1));
                    var val = EvaluateCurve(curve, stim);

                    var px = x + margin + (stim - minX) / (maxX - minX) * (width - 2 * margin);
                    var py = y + height - margin - (val - minY) / (maxY - minY) * (height - 2 * margin);

                    if (last.HasValue)
                        Drawing.DrawLine(last.Value, new Vector2(px, py), curveColor, 2f);
                    last = new Vector2(px, py);
                }
                // Curve label
                GUI.color = curveColor;
                GUI.Label(new Rect(x + width - margin + 4, y + margin + 18 * c, 100, 18), curve.CurveName + " (" + curve.CurveType + ")");
            }

            // Draw marker for current stimulus
            var stimX = x + margin + (stimulus - minX) / (maxX - minX) * (width - 2 * margin);
            Drawing.DrawLine(new Vector2(stimX, y + margin), new Vector2(stimX, y + height - margin), Color.white, 3f);
            
            // Optionally: show value
            GUI.color = Color.white;
            GUI.Label(new Rect(stimX - 24, y + height - margin + 2, 48, 18), $"Input: {stimulus:0.00}");
            GUI.color = Color.white;
        }

        // Evaluates a curve for visualization (matches PersonaBtSwitcher logic)
        float EvaluateCurve(CurveProfileEntry curve, float x)
        {
            return curve.CurveType switch
            {
                "Linear" => curve.Max * Mathf.Clamp01(x),
                "Gaussian" => curve.Max * Mathf.Exp(-curve.Sharpness * Mathf.Pow(x - curve.Center, 2f)),
                "Sigmoid" => curve.Max / (1f + Mathf.Exp(-curve.Sharpness * (x - curve.Center))),
                _ => 0f
            };
        }
        
        private void OnDrawGizmos()
        {
            // Suppose you have a way to get the SquadAgent runtime object.
            // This can be via Data, Context, or another reference injected at init.
            var squadAgent = OverlayData?.SquadAgent; // You implement this.
            if (squadAgent == null) return;

            var slotWorldPos = squadAgent.GetSlotWorldPosition();
            Gizmos.color = (squadAgent.IsLeader) ? Color.red : Color.goldenRod;
            Gizmos.DrawSphere(slotWorldPos, 0.75f);

            // Optionally: Draw a line from agent to slot
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, slotWorldPos);
        }
    }
}