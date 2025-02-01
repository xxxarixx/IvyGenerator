using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening.Core;

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
            [Range(0f,5f)]
            internal float MaxSpacingAngle = 0.5f;

            [SerializeField]
            internal int IterationCount = 3;

            [SerializeField]
            internal float DebugTimeFade = 3f;

            [SerializeField]
            internal float TimeBetweenSpawn = 1f;

            [SerializeField]
            internal bool UpdateDebugMode;

            [SerializeField]
            internal int ManualCheckSegmentsCount = 4;

            [SerializeField]
            internal Color SupportSphereGizmosColor = Color.yellow;
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
        LineRenderer lastLineRenderer;
        internal void Update()
        {
            if(_settings.UpdateDebugMode)
                Invoke(lastShootDir, lastOrigin, lastNormal, lastLayerMask, lastLineRenderer);
        }

        internal void Invoke(Vector3 shootDirection, Vector3 shootOrigin, Vector3 normal, LayerMask targetMask, LineRenderer lineRenderer)
        {
            lastShootDir = shootDirection;
            lastOrigin = shootOrigin;
            lastNormal = normal;
            lastLayerMask = targetMask;
            StaticCorountine.StopStaticCoruntine(ProcessVines(shootDirection, shootOrigin, normal, targetMask, lineRenderer));

            normal = normal.normalized;
            _vines.Clear();
            _debugPoints.Clear();
            Debug.DrawRay(shootOrigin, normal, Color.yellow, _settings.DebugTimeFade);
            Debug.DrawRay(shootOrigin, -shootDirection, Color.black, _settings.DebugTimeFade);
            StaticCorountine.StartStaticCoruntine(ProcessVines(shootDirection,shootOrigin,normal,targetMask, lineRenderer));

        }

        IEnumerator ProcessVines(Vector3 shootDirection, Vector3 shootOrigin, Vector3 normal, LayerMask targetMask, LineRenderer lineRenderer)
        {
            Vector3? previousDirection = null;
            for (int i = 0; i < _settings.IterationCount; i++)
            {
                PopulatePoint(ref shootDirection, ref shootOrigin, ref normal, ref previousDirection, lineRenderer, targetMask);
                yield return new WaitForSeconds(_settings.TimeBetweenSpawn);
            }
            yield return null;
        }

        void PopulatePoint(ref Vector3 shootDirection, ref Vector3 shootOrigin, ref Vector3 normal, ref Vector3? previousDirection, LineRenderer lineRenderer, LayerMask targetMask)
        {
            // Correctly project the current direction onto the tangent plane
            Vector3 projectedForward = Vector3.ProjectOnPlane(shootDirection, normal).normalized;

            if (!previousDirection.HasValue)
                previousDirection = shootDirection;

            // Calculate the forward point on the surface
            Vector3 forwardPoint = shootOrigin + normal * _settings.Spacing + projectedForward * _settings.Spacing;
            _debugPoints.Add(forwardPoint);
            Debug.DrawRay(shootOrigin, projectedForward, Color.red, _settings.DebugTimeFade);

            // Calculate reflection direction and make reflection
            Vector3 directionToReflect = (forwardPoint - shootOrigin).normalized;
            Debug.DrawRay(shootOrigin, directionToReflect, Color.white, _settings.DebugTimeFade);
            Vector3 reflection = Vector3.Reflect(directionToReflect, normal);

            // Adjust SpacingAngle based on curvature
            float curvature = Vector3.Angle(shootDirection, previousDirection.Value);
            previousDirection = shootDirection;
            Debug.Log($"curv:{curvature} normal:{normal} prevNormal: {previousDirection}");
            // Adjust reflection angle 
            float adjustedSpacingAngle = Mathf.Lerp(0f, _settings.MaxSpacingAngle, (curvature * _settings.Spacing) / 180f);
            float raycastDistance = _settings.Spacing * ((_settings.Spacing * 10) / _settings.MaxSpacingAngle);
            Vector3 reflectionAngled = Vector3.Lerp(reflection, -directionToReflect, adjustedSpacingAngle);
            Debug.DrawRay(forwardPoint, reflectionAngled * raycastDistance, Color.magenta, _settings.DebugTimeFade);
            shootDirection = reflectionAngled;
            
            
            if (Physics.Raycast(forwardPoint, reflectionAngled, out RaycastHit hit, raycastDistance, targetMask))
            {
                shootOrigin = hit.point;
                normal = hit.normal;
                _vines.Add(shootOrigin);
            }
            else
            {
                float increaseValue = 1 / (float)_settings.ManualCheckSegmentsCount;
                float i = 0;
                // Manual check needed, propably low poly
                while (i < 0.9f)
                {
                    Debug.Log($"decreaseValue: {increaseValue} i:{i}");
                    reflectionAngled = Vector3.Lerp(reflection, -directionToReflect, i);
                    Debug.DrawRay(forwardPoint, reflectionAngled * raycastDistance, Color.magenta, _settings.DebugTimeFade);
                    if (Physics.Raycast(forwardPoint, reflectionAngled, out hit, raycastDistance, targetMask))
                    {
                        shootOrigin = hit.point;
                        normal = hit.normal;
                        _vines.Add(shootOrigin);
                        shootDirection = reflectionAngled;
                        break;
                    }
                    i += increaseValue;
                }
            }

            lineRenderer.positionCount = _vines.Count;
            lineRenderer.SetPosition(_vines.Count - 1, shootOrigin);
        }

        internal void GizmosDebug()
        {
            Gizmos.color = Color.green;
            foreach (var item in _vines)
            {
                Gizmos.DrawSphere(item, 0.1f);
            }

            Gizmos.color = _settings.SupportSphereGizmosColor;
            foreach (var item in _debugPoints)
            {
                Gizmos.DrawSphere(item, 0.1f);
            }
        }
    }
}
