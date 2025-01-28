using Unity.VisualScripting;
using UnityEngine;
using Vines;
using Zenject;

namespace Player
{
    class Player : MonoBehaviour
    {
        [SerializeField]
        Transform _head;
        [Inject]
        readonly PlayerInputSystem _input;
        [Inject]
        readonly PlayerStatsConfig _stats;
        [Inject]
        readonly PlayerCameraSystem _vision;
        [Inject]
        readonly Rigidbody _rb;
        // Vines
        [Inject]
        readonly VinesSystem _vinesSystem;
        LayerMask _vinesTarget = 1 << 0;

        Vector3 _visualizationPoint;

        void OnEnable()
        {
            _input.OnMovementInputFixedUpdate += MovementInputFixedUpdate;
            _input.OnPrimaryBtnStart += PrimaryBtnPressed;
        }

        void OnDisable()
        {
            _input.OnMovementInputFixedUpdate -= MovementInputFixedUpdate;
            _input.OnPrimaryBtnStart -= PrimaryBtnPressed;
        }

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_visualizationPoint, 0.1f);
            _vinesSystem.GizmosDebug();
        }

        void MovementInputFixedUpdate(Vector2 moveDir)
        {
            Vector3 moveVelocity = _vision.GetLookDir(moveDir) * _stats.Speed * Time.fixedDeltaTime;
            _rb.MovePosition(_rb.position + new Vector3(moveVelocity.x, 0, moveVelocity.z));
        }

        void PrimaryBtnPressed()
        {
            var hit = ShootRaycastForward(5f, _vinesTarget);
            _visualizationPoint = hit.point;
            _vinesSystem.Invoke(shootDirection:_vision.CameraForward,
                                worldPosition:hit.point,
                                normal:hit.normal, 
                                targetMask:_vinesTarget);
        }

        RaycastHit ShootRaycastForward(float length,LayerMask layerMask)
        {
            Physics.Raycast(_head.position, _vision.CameraForward, out RaycastHit hit, length, layerMask);
            return hit;
        }

    }
}
