using UnityEngine;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("MovementLogic Settings")]
        [SerializeField] private float _moveSpeed = 5f;

        private CharacterController _characterController;
        private Camera _mainCamera;
        private Vector3 _movement;

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            // Reads the raw input axes from Unity's Input system.
            var h = Input.GetAxisRaw("Horizontal"); // "Horizontal" = A/D or Left/Right Arrow Keys (or joystick X)
            var v = Input.GetAxisRaw("Vertical"); // "Vertical" = W/S or Up/Down Arrow Keys (or joystick Y)

            // Get camera-aligned direction (isometric-friendly)
            var camForward = _mainCamera.transform.forward;
            var camRight = _mainCamera.transform.right;

            //Zeroes out the vertical (Y) component** so movement stays on the horizontal plane.
            camForward.y = 0f;
            camRight.y = 0f;

            //Normalize the vectors to have a magnitude of 1, so they don't unintentionally scale movement.
            camForward.Normalize();
            camRight.Normalize();

            // Calculates the input direction relative to the camera's orientation:
            // Forward/back is aligned with the camera’s forward. Left / right is aligned with the camera’s right
            var direction = (camForward * v + camRight * h).normalized;

            // Move in the direction vector as input (not position!), at this impulseStrength, and make sure the object slide along the ground and obey collisions.
            _characterController.SimpleMove(direction * _moveSpeed);

            // Turn the player to face the same direction they’re moving
            if (direction != Vector3.zero)
                transform.forward = direction;
        }
    }
}
