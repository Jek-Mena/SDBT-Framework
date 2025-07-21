using UnityEngine;

namespace Dev.OverlayDebugView
{
    public class AgentSelectionManager : MonoBehaviour
    {
        public static AgentSelectionManager Instance { get; private set; }
        public DebugOverlayView SelectedAgent { get; private set; }
        
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Select(DebugOverlayView overlay)
        {
            SelectedAgent = overlay;
        }

        public bool IsSelected(DebugOverlayView overylay)
        {
            return SelectedAgent == overylay;
        }

        public void Deselect()
        {
            SelectedAgent = null;       
        }
    }
}