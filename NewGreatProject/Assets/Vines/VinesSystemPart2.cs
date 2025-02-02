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
            internal float Spacing = 0.45f;

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
            float SpacingRandomizerStrength = 1f;

            [SerializeField]
            internal LineRenderer LineRendererPrefab;

            internal float RandomSpacing => Spacing + Random.Range(-SpacingRandomizerStrength, SpacingRandomizerStrength);

            internal float RaycastDistance
            {
                get
                {
                    var randSpacing = RandomSpacing;
                    return randSpacing * ((randSpacing * 7) / MaxSpacingAngle) + OriginOffsetMultiplayer;
                }
            }
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
        LineRenderer? lastLineRenderer;
        bool _vinesArePlaying;
        internal void Update()
        {
            if(_settings.UpdateDebugMode)
                Invoke(lastShootDir, lastOrigin, lastNormal, lastLayerMask);
        }

        internal void Invoke(Vector3 shootDirection, Vector3 shootOrigin, Vector3 normal, LayerMask targetMask)
        {
            if (_vinesArePlaying)
                return;


            lastShootDir = shootDirection;
            lastOrigin = shootOrigin;
            lastNormal = normal;
            lastLayerMask = targetMask;
            _vinesArePlaying = true;
            CreateLineRenderer();

            normal = normal.normalized;
            _vines.Clear();
            _debugPoints.Clear();
            Debug.DrawRay(shootOrigin, normal, Color.yellow, _settings.DebugTimeFade);
            Debug.DrawRay(shootOrigin, -shootDirection, Color.black, _settings.DebugTimeFade);
            StaticCorountine.StartStaticCoruntine(ProcessVines(shootDirection,shootOrigin,normal,targetMask, lastLineRenderer));

        }

        IEnumerator ProcessVines(Vector3 shootDirection, Vector3 shootOrigin, Vector3 normal, LayerMask targetMask, LineRenderer lineRenderer)
        {
            Vector3? previousDirection = null;
            Vector3 previousNormal = normal;
            float spacing = _settings.Spacing;
            for (int i = 0; i < _settings.IterationCount; i++)
            {
                PopulatePoint(ref shootDirection, ref shootOrigin, ref normal, ref previousDirection, ref spacing, targetMask);
                RandomizePoint(ref shootDirection, ref spacing);
                VisualizePoint(_vines.Count - 1,shootOrigin, lineRenderer);
                InBetweenPoint(shootOrigin, ref previousNormal,normal,lineRenderer, targetMask);
                previousNormal = normal;
                yield return new WaitForSeconds(_settings.TimeBetweenSpawn);
            }
            _vinesArePlaying = false;
            yield return null;
        }

        void CreateLineRenderer()
        {
            if (lastLineRenderer != null)
                return;

            lastLineRenderer = GameObject.Instantiate(_settings.LineRendererPrefab, Vector3.zero, Quaternion.identity);
        }

        void PopulatePoint(ref Vector3 shootDirection, ref Vector3 shootOrigin, ref Vector3 normal, ref Vector3? previousDirection, ref float Spacing, LayerMask targetMask)
        {
            // Correctly project the current direction onto the tangent plane
            Vector3 projectedForward = Vector3.ProjectOnPlane(shootDirection, normal).normalized;

            if (!previousDirection.HasValue)
                previousDirection = shootDirection;

            // Calculate the forward point on the surface
            Vector3 forwardPointOrigin = shootOrigin + normal * Spacing + projectedForward * Spacing;

            // Check if ray to debug point has any obstacle, check in order to prevent going through obstacle.
            if(Physics.Raycast(shootOrigin, (forwardPointOrigin - shootOrigin).normalized, out RaycastHit hit, Vector3.Distance(shootOrigin,forwardPointOrigin), targetMask))
            {
                normal = hit.normal;
                shootOrigin = hit.point + normal * _settings.OriginOffsetMultiplayer;
                _vines.Add(shootOrigin);
                return;
            }
            else
            {
                _debugPoints.Add(forwardPointOrigin);
            }

            Debug.DrawRay(shootOrigin, projectedForward, Color.red, _settings.DebugTimeFade);

            // Calculate reflection direction and make reflection
            Vector3 directionToReflect = (forwardPointOrigin - shootOrigin).normalized;
            Debug.DrawRay(shootOrigin, directionToReflect, Color.white, _settings.DebugTimeFade);
            Vector3 reflection = Vector3.Reflect(directionToReflect, normal);

            // Adjust SpacingAngle based on curvature
            float curvature = Vector3.Angle(shootDirection, previousDirection.Value);
            previousDirection = shootDirection;
            // Adjust reflection angle 
            float adjustedSpacingAngle = Mathf.Lerp(0f, _settings.MaxSpacingAngle, (curvature * Spacing) / 180f);
            Vector3 reflectionAngled = Vector3.Lerp(reflection, -directionToReflect, adjustedSpacingAngle);
            Debug.DrawRay(forwardPointOrigin, reflectionAngled * _settings.RaycastDistance, Color.magenta, _settings.DebugTimeFade);
            shootDirection = reflectionAngled;
            
            
            if (Physics.Raycast(forwardPointOrigin, reflectionAngled, out hit, _settings.RaycastDistance, targetMask))
            {
                normal = hit.normal;
                shootOrigin = hit.point + normal * _settings.OriginOffsetMultiplayer;
                _vines.Add(shootOrigin);
            }
            else
            {
                float increaseValue = 1 / (float)_settings.ManualCheckSegmentsCount;
                float i = 0;
                // Manual check needed, propably low poly
                while (i < 0.9f)
                {
                    reflectionAngled = Vector3.Lerp(reflection, -directionToReflect, i);
                    Debug.DrawRay(forwardPointOrigin, reflectionAngled * _settings.RaycastDistance, Color.magenta, _settings.DebugTimeFade);
                    if (Physics.Raycast(forwardPointOrigin, reflectionAngled, out hit, _settings.RaycastDistance, targetMask))
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

        void RandomizePoint(ref Vector3 shootDirection, ref float Spacing)
        {
            shootDirection += new Vector3(Random.Range(-_settings.DirectionRandomizerStrength, _settings.DirectionRandomizerStrength), Random.Range(-_settings.DirectionRandomizerStrength, _settings.DirectionRandomizerStrength), Random.Range(-_settings.DirectionRandomizerStrength, _settings.DirectionRandomizerStrength));
            Spacing = _settings.RandomSpacing;
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

            Vector3 previousShootOrigin = _vines[^2];

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
                    _vines.Insert(_vines.Count - 1, newPointShootOrigin);
                    VisualizePoint(_vines.Count - 2, newPointShootOrigin, lineRenderer);
                    VisualizePoint(_vines.Count - 1, _vines[^1], lineRenderer);
                    Debug.Log($"inbetweenPoint generated at: {_vines.Count - 2}");
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
