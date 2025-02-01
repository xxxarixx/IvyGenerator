using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vines
{
    class VinesSystem
    {
        [System.Serializable]
        internal class Settings
        {
            [SerializeField]
            [Range(-1f,1f)]
            internal float treshHold = 0f;

            [SerializeField]
            internal float Rad = 0.5f;

            /// <summary>
            /// Its length <see cref="Rad"/> * this
            /// </summary>

            [SerializeField]
            internal float DepthMultiplayer = 2f;

            [SerializeField]
            internal float DebugTimeFade = 3f;

            [SerializeField]
            internal float TimeBetweenSpawn = 1f;

            [SerializeField]
            internal int LoopCount = 5;

            [SerializeField]
            internal int raysPerCircle = 3;
        }

        internal VinesSystem(Settings settings)
        {
            _settings = settings;
        }

        Settings _settings;

        List<Vector3> _vines = new();
        internal void Invoke(Vector3 shootDirection, Vector3 origin, Vector3 normal, LayerMask targetMask)
        {
            normal = normal.normalized;
            
            Debug.DrawRay(origin, normal, Color.yellow, _settings.DebugTimeFade);
            _vines.Clear();
            StaticCorountine.StartStaticCoruntine(ProcessVines(origin,normal,targetMask, raysPerCircle: _settings.raysPerCircle, loopCount: _settings.LoopCount));
        }
        IEnumerator ProcessVines(Vector3 origin, Vector3 normal, LayerMask targetMask, int raysPerCircle = 8, int loopCount = 2)
        {
            Vector3[] targetPoints = new Vector3[0];
            Vector3[] originsOld = default;
            // Initial setup 1
            targetPoints = AddPoints(new Vector3[0], normal, targetMask, false, raysPerCircle, origin);
            originsOld = new[] { origin };
            _vines.AddRange(targetPoints);
            yield return new WaitForSeconds(_settings.TimeBetweenSpawn);

            // Initial setup 2
            targetPoints = AddPoints(originsOld, normal, targetMask, false, raysPerCircle, targetPoints);
            _vines.AddRange(targetPoints);
            yield return new WaitForSeconds(_settings.TimeBetweenSpawn);

            // Loop for the specified number of iterations
            for (int i = 0; i < loopCount - 2; i++)
            {
                originsOld = targetPoints;
                targetPoints = AddPoints(originsOld, normal, targetMask, true, raysPerCircle, targetPoints);
                _vines.AddRange(targetPoints);
                yield return new WaitForSeconds(_settings.TimeBetweenSpawn);
            }
            yield return null;
        }
        Vector3[] AddPoints(Vector3[] originsOld, Vector3 normal, LayerMask targetMask, bool canCheckIsPointInDirection, int raysPerCircle = 8, params Vector3[] origins)
        {
            int i = 0;
            int oldI = 0;
            Vector3[] points = new Vector3[raysPerCircle * origins.Length];
            Vector3 hitDirection = -normal;
            
            foreach (Vector3 originNew in origins)
            {
                Vector3[] rayPosition = GenerateCirclePointsBasedOnNormal(center: originNew,
                                                            radius: _settings.Rad,
                                                            numberOfPoints: raysPerCircle,
                                                            normal: normal);
                // Cast rays in all directions to find edge points
                foreach (Vector3 pos in rayPosition)
                {
                    Vector3 startPosition = pos - hitDirection * (_settings.Rad - 0.02f);
                    Vector3 randPos = Vector3.zero;
                    Vector3 randDirection = Vector3.one * Random.Range(-0.2f, 0.2f);
                    float randRad = Random.Range(-0.3f, 0.3f);

                    if (Physics.Raycast(startPosition + randPos, hitDirection + randDirection, out RaycastHit hit, _settings.Rad * _settings.DepthMultiplayer + randRad, targetMask))
                    {
                        // The hit point is on the sphere's edge
                        Vector3 edgePoint = hit.point;
                        if (canCheckIsPointInDirection && IsPointInDirection(originNew, (originNew - originsOld[oldI]).normalized, edgePoint, treshHold: _settings.treshHold))
                        {
                            points[i] = edgePoint;
                            Debug.DrawRay(originNew, (originNew - originsOld[oldI]).normalized * _settings.Rad, Color.green, 5f);
                        }
                        else if(!canCheckIsPointInDirection)
                        {
                            points[i] = edgePoint;
                            Debug.DrawRay(startPosition + randPos, hitDirection + randDirection * (_settings.Rad * _settings.DepthMultiplayer + randRad), Color.red, _settings.DebugTimeFade);
                        }
                        else
                            points[i] = Vector3.zero;
                       
                    }
                    i++;
                }
                if(originsOld.Length < 2) //blocking iteration 0,1
                    oldI++;
            }   
            
            return points;
        }
        Vector3[] GenerateCirclePointsBasedOnNormal(Vector3 center, float radius, int numberOfPoints, Vector3 normal)
        {
            float angleIncrement = 360f / numberOfPoints; // Angle between each point
            Vector3[] pointsOnEdge = new Vector3[numberOfPoints];

            // Normalize the normal vector
            normal = normal.normalized;

            // Create a rotation to align the circle's plane with the normal
            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, normal);

            for (int i = 0; i < numberOfPoints; i++)
            {
                // Calculate the angle in radians
                float angle = i * angleIncrement * Mathf.Deg2Rad;

                // Calculate the position of the point on the circle (in the XY plane)
                Vector3 pointPosition = new(
                    radius * Mathf.Cos(angle),
                    radius * Mathf.Sin(angle),
                    0
                );

                // Rotate the point to align with the normal
                pointPosition = rotation * pointPosition;

                // Translate the point to the center
                pointPosition += center;

                pointsOnEdge[i] = pointPosition;
            }

            return pointsOnEdge;
        }
        private bool IsPointInDirection(Vector3 origin, Vector3 direction, Vector3 point, float treshHold = 0f)
        {
            // Calculate the vector from origin to the point
            Vector3 toPoint = point - origin;

            // Normalize the direction vector (if not already normalized)
            direction = direction.normalized;

            // Calculate the dot product
            float dotProduct = Vector3.Dot(direction, toPoint.normalized);

            // Check if the dot product is positive (point is in the same general direction)
            return dotProduct >= treshHold;
        }
        internal void GizmosDebug()
        {
            Gizmos.color = Color.green;
            foreach (var item in _vines)
            {
                Gizmos.DrawSphere(item, 0.1f);
            }
        }
    }
}
