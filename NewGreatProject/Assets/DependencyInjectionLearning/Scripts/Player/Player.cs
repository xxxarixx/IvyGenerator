using System.Collections.Generic;
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
        readonly VinesSystemPart2 _vinesSystem;

        [SerializeField]
        LineRenderer lineRenderer;

        LayerMask _vinesTarget = 1 << 0;

        Vector3 _visualizationPoint;

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
            _vinesSystem.Update();
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
            if(hit.collider != null)
            {
                _visualizationPoint = hit.point;
                _vinesSystem.Invoke(shootDirection:_vision.CameraForward,
                                    shootOrigin:hit.point,
                                    normal:hit.normal, 
                                    targetMask:_vinesTarget);
            }
        }
        RaycastHit ShootRaycastForward(float length,LayerMask layerMask)
        {
            Physics.Raycast(_head.position, _vision.CameraForward, out RaycastHit hit, length, layerMask);
            return hit;
        }

    }
}
