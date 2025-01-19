using UnityEngine;
using Zenject;

namespace Player
{
    class Player : MonoBehaviour
    {
        [Inject]
        readonly PlayerInputSystem _input;
        [Inject]
        readonly PlayerStatsConfig _stats;
        [Inject]
        readonly PlayerCameraSystem _vision;
        [Inject]
        readonly Rigidbody _rb;

        void OnEnable()
        {
            _input.OnMovementInputFixedUpdate += MovementInputFixedUpdate;
        }

        void OnDisable()
        {
            _input.OnMovementInputFixedUpdate -= MovementInputFixedUpdate;
        }
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        void MovementInputFixedUpdate(Vector2 moveDir)
        {
            Vector3 moveVelocity = _vision.GetLookDir(moveDir) * _stats.Speed * Time.fixedDeltaTime;
            _rb.MovePosition(_rb.position + new Vector3(moveVelocity.x, 0, moveVelocity.z));
        }
    }
}
