using UnityEngine;

namespace Dev.OverlayDebugView
{
    public class SelectedIndicator : MonoBehaviour
    {
        private DebugOverlayView _overlay;
        private MeshRenderer _renderer;

        void Start()
        {
            _overlay = GetComponentInParent<DebugOverlayView>();
            _renderer = GetComponent<MeshRenderer>();
        }

        void Update()
        {
            if (!_overlay)
                _overlay = GetComponentInParent<DebugOverlayView>();
            if (!_renderer)
                _renderer = GetComponent<MeshRenderer>();

            var shouldShow = AgentSelectionManager.Instance.IsSelected(_overlay);
            if (_renderer)
                _renderer.enabled = shouldShow;

            // Rotate to face camera
            return;
            if (shouldShow)
            {
                var cam = Camera.main;
                if (cam)
                    transform.rotation = Quaternion.LookRotation(cam.transform.forward);
            }
        }
    }
}