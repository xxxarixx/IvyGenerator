using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Vines
{
    class VinesSystemPart2
    {
        [System.Serializable]
        internal class Settings
        {
            [SerializeField]
            internal float Spacing = 1f;

            [SerializeField]
            [Range(0f,1f)]
            internal float SpacingAngle = 0.5f;

            [SerializeField]
            [Range (0.1f,2f)]
            internal float UpwardBias = 0.5f;

            [SerializeField]
            internal int IterationCount = 3;

            [SerializeField]
            internal float DebugTimeFade = 3f;

            [SerializeField]
            internal float TimeBetweenSpawn = 1f;

        }

        internal VinesSystemPart2(Settings settings)
        {
            _settings = settings;
        }

        Settings _settings;

        List<Vector3> _vines = new();
        List<Vector3> _debugPoints = new();

        Vector3 lastShootDir;
        Vector3 lastOrigin;
        Vector3 lastNormal;
        LayerMask lastLayerMask;
        internal void Update()
        {
            //``Invoke(lastShootDir, lastOrigin, lastNormal, lastLayerMask);
        }

        internal void Invoke(Vector3 shootDirection, Vector3 shootOrigin, Vector3 normal, LayerMask targetMask)
        {
            lastShootDir = shootDirection;
            lastOrigin = shootOrigin;
            lastNormal = normal;
            lastLayerMask = targetMask;

            normal = normal.normalized;
            _vines.Clear();
            _debugPoints.Clear();

            Debug.DrawRay(shootOrigin, normal, Color.yellow, _settings.DebugTimeFade);
            Debug.DrawRay(shootOrigin, -shootDirection, Color.black, _settings.DebugTimeFade);
            StaticCorountine.StartStaticCoruntine(ProcessVines(shootDirection,shootOrigin,normal,targetMask));
        }

        IEnumerator ProcessVines(Vector3 shootDirection, Vector3 shootOrigin, Vector3 normal, LayerMask targetMask)
        {
            for (int i = 0; i < _settings.IterationCount; i++)
            {
                PopulatePoint(ref shootDirection, ref shootOrigin, ref normal,targetMask);
                yield return new WaitForSeconds(_settings.TimeBetweenSpawn);
            }
            yield return null;
        }

        void PopulatePoint(ref Vector3 shootDirection, ref Vector3 shootOrigin, ref Vector3 normal, LayerMask targetMask)
        {
            // Correctly project the current direction onto the tangent plane
            Vector3 projectedForward = Vector3.ProjectOnPlane(shootDirection, normal).normalized;

            // Calculate the forward point on the surface
            Vector3 forwardPoint = shootOrigin + normal * _settings.Spacing + projectedForward * _settings.Spacing;
            _debugPoints.Add(forwardPoint);
            Debug.DrawRay(shootOrigin, projectedForward, Color.red, _settings.DebugTimeFade);

            // Calculate reflection direction and make reflection
            Vector3 directionToReflect = (forwardPoint - shootOrigin).normalized;
            Debug.DrawRay(shootOrigin, directionToReflect, Color.white, _settings.DebugTimeFade);
            Vector3 reflection = Vector3.Reflect(directionToReflect, normal);

            // Adjust reflection angle 
            Vector3 reflectionAngled = Vector3.Slerp(reflection, -directionToReflect, _settings.SpacingAngle);
            Debug.DrawRay(forwardPoint, reflectionAngled, Color.magenta, _settings.DebugTimeFade);

            shootDirection = reflectionAngled;
            if (Physics.Raycast(forwardPoint, reflectionAngled, out RaycastHit hit, 1f, targetMask))
            {
                shootOrigin = hit.point;
                normal = hit.normal;
                _vines.Add(shootOrigin);
            }
        }

        internal void GizmosDebug()
        {
            Gizmos.color = Color.green;
            foreach (var item in _vines)
            {
                Gizmos.DrawSphere(item, 0.1f);
            }

            Gizmos.color = Color.yellow;
            foreach (var item in _debugPoints)
            {
                Gizmos.DrawSphere(item, 0.1f);
            }
        }
    }
}
