using UnityEngine;

namespace Dev.OverlayDebugView
{
    public class AgentSelectorInput : MonoBehaviour
    {
        private Camera _playerCamera;

        private void Start()
        {
            _playerCamera = Camera.main;
            if (!_playerCamera)
                UnityEngine.Debug.LogError("No camera found!");
        }
        
        void Update()
        {
            if (Input.GetMouseButtonDown(0)) // 0 Left Click
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    var overlay = hit.collider.GetComponentInParent<DebugOverlayView>();
                    if (overlay)
                    {
                        AgentSelectionManager.Instance.Select(overlay);
                    }
                    else
                    {
                        AgentSelectionManager.Instance.Deselect();
                    }
                }
                else
                {
                    AgentSelectionManager.Instance.Deselect();
                }
            }
        }
    }
}