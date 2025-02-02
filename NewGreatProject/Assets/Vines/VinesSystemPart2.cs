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
            internal float OriginOffsetMultiplayer = 1f;

            [SerializeField]
            internal Color SupportSphereGizmosColor = Color.yellow;

            [SerializeField]
            internal float DirectionRandomizerStrength = 1f;

            [SerializeField]
            internal float OriginRandomizerStrength = 1f;

            internal float RaycastDistance => Spacing * ((Spacing * 7) / MaxSpacingAngle) + OriginOffsetMultiplayer;
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
            Vector3 previousNormal = normal;
            for (int i = 0; i < _settings.IterationCount; i++)
            {
                PopulatePoint(ref shootDirection, ref shootOrigin, ref normal, ref previousDirection, targetMask);
                RandomizePoint(ref shootDirection, ref shootOrigin, normal);
                VisualizePoint(_vines.Count - 1,shootOrigin, lineRenderer);
                InBetweenPoint(shootOrigin, ref previousNormal,normal,lineRenderer, targetMask);
                yield return new WaitForSeconds(_settings.TimeBetweenSpawn);
            }
            yield return null;
        }

        void PopulatePoint(ref Vector3 shootDirection, ref Vector3 shootOrigin, ref Vector3 normal, ref Vector3? previousDirection, LayerMask targetMask)
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
            Vector3 reflectionAngled = Vector3.Lerp(reflection, -directionToReflect, adjustedSpacingAngle);
            Debug.DrawRay(forwardPoint, reflectionAngled * _settings.RaycastDistance, Color.magenta, _settings.DebugTimeFade);
            shootDirection = reflectionAngled;
            
            
            if (Physics.Raycast(forwardPoint, reflectionAngled, out RaycastHit hit, _settings.RaycastDistance, targetMask))
            {
                shootOrigin = hit.point + normal * _settings.OriginOffsetMultiplayer;
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
                    Debug.DrawRay(forwardPoint, reflectionAngled * _settings.RaycastDistance, Color.magenta, _settings.DebugTimeFade);
                    if (Physics.Raycast(forwardPoint, reflectionAngled, out hit, _settings.RaycastDistance, targetMask))
                    {
                        normal = hit.normal;
                        shootOrigin = hit.point + normal * _settings.OriginOffsetMultiplayer;
                        _vines.Add(shootOrigin);
                        shootDirection = reflectionAngled;
                        break;
                    }
                    i += increaseValue;
                }
            }

            
        }

        void RandomizePoint(ref Vector3 shootDirection, ref Vector3 shootOrigin, Vector3 normal)
        {
            shootDirection += new Vector3(Random.Range(-_settings.DirectionRandomizerStrength, _settings.DirectionRandomizerStrength), Random.Range(-_settings.DirectionRandomizerStrength, _settings.DirectionRandomizerStrength), Random.Range(-_settings.DirectionRandomizerStrength, _settings.DirectionRandomizerStrength));
            shootOrigin += new Vector3(Random.Range(-_settings.OriginRandomizerStrength, _settings.OriginRandomizerStrength), Random.Range(-_settings.OriginRandomizerStrength, _settings.OriginRandomizerStrength), Random.Range(-_settings.OriginRandomizerStrength, _settings.OriginRandomizerStrength));
        }

        void VisualizePoint(int index,Vector3 shootOrigin,LineRenderer lineRenderer)
        {
            lineRenderer.positionCount = _vines.Count;
            lineRenderer.SetPosition(index, shootOrigin);
        }

        void InBetweenPoint(Vector3 shootOrigin, ref Vector3 previousNormal, Vector3 normal, LineRenderer lineRenderer, LayerMask targetMask)
        {
            if (_vines.Count < 2)
                return;

            Vector3 previousShootOrigin = _vines[_vines.Count - 2];

            Vector3 directionToPreviousOrigin = (previousShootOrigin - shootOrigin).normalized;

            if(Physics.Raycast(shootOrigin, directionToPreviousOrigin, Vector3.Distance(shootOrigin,previousShootOrigin), targetMask))
            {
                // There is collision in between, should generate additional point inbetween to avoid line clipping to model
                Vector3 inBetweenOrigin = Vector3.Lerp(shootOrigin, previousShootOrigin, 0.5f);
                Vector3 inBetweenNormal = Vector3.Lerp(normal, previousNormal, 0.5f);
                Vector3 newDebugPointPosition = inBetweenOrigin + inBetweenNormal * _settings.RaycastDistance;
                _debugPoints.Add(newDebugPointPosition);
                if (Physics.Raycast(newDebugPointPosition, -inBetweenNormal, out RaycastHit hit, _settings.RaycastDistance, targetMask))
                {
                    Vector3 newPointShootOrigin = hit.point;
                    previousNormal = hit.normal;
                    _vines.Insert(_vines.Count - 2, newPointShootOrigin);
                    VisualizePoint(_vines.Count - 2, newPointShootOrigin, lineRenderer);
                    VisualizePoint(_vines.Count - 1, _vines[_vines.Count - 1], lineRenderer);
                }
            }
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
