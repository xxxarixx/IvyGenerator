using UnityEngine;
using Vines;
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
        // Vines
        [Inject]
        readonly VineController _vinesController;
        [Inject]
        readonly PlayerWeaponHolderSystem _weaponHolderSystem;
        LayerMask shootTarget = 1 << 0;
        [SerializeField]
        Transform _head;

        [SerializeField]
        Transform origin;
        [SerializeField]
        Transform nextPoint;
        [SerializeField]
        Vector3 direction;

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

        private void Update()
        {
            (bool, float) final = IsPointInDirection(origin.transform.position, direction, nextPoint.transform.position);
            Debug.DrawRay(origin.transform.position, direction, Color.blue, duration: 0.01f);
            Debug.Log($"IsPointInDirection: {final.Item1} ({final.Item2})");
            _vinesController.Update();
        }
        private (bool,float) IsPointInDirection(Vector3 origin, Vector3 direction, Vector3 point)
        {
            // Calculate the vector from origin to the point
            Vector3 toPoint = point - origin;

            // Normalize the direction vector (if not already normalized)
            direction = direction.normalized;

            // Calculate the dot product
            float dotProduct = Vector3.Dot(direction, toPoint.normalized);

            // Check if the dot product is positive (point is in the same general direction)
            return (dotProduct >= 0, dotProduct);
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            _vinesController.GizmosDebug();
            _weaponHolderSystem.Gizmos();
        }
       

        void MovementInputFixedUpdate(Vector2 moveDir)
        {
            Vector3 moveVelocity = _vision.GetLookDir(moveDir) * _stats.Speed * Time.fixedDeltaTime;
            _rb.MovePosition(_rb.position + new Vector3(moveVelocity.x, 0, moveVelocity.z));
        }

        void PrimaryBtnPressed()
        {
            RaycastHit hit = ShootRaycastForward(5f, shootTarget);
            if (hit.collider != null)
                _weaponHolderSystem.Fire(_vision.CameraForward,hit);
        }
        RaycastHit ShootRaycastForward(float length, LayerMask layerMask)
        {
            Debug.Log($"head:{_head}, vision:{_vision}");
            Physics.Raycast(_head.position, _vision.CameraForward, out RaycastHit hit, length, layerMask);
            return hit;
        }

    }
}
