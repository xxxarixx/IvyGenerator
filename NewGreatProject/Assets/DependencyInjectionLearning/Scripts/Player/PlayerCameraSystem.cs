using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace Player
{
    class PlayerCameraSystem
    {
        [Inject]
        Camera _cam;

        internal Vector3 GetLookDir(Vector2 moveDir)
        {
            Vector3 cameraForward = Vector3.Scale(_cam.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 cameraRight = _cam.transform.right;

            return (cameraForward * moveDir.y + cameraRight * moveDir.x).normalized;
        }
    }
}
