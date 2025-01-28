using System.Collections.Generic;
using UnityEngine;

namespace Vines
{
    class VinesSystem
    {
        readonly List<Vector3> _vines = new();
        internal void Invoke(Vector3 shootDirection,Vector3 worldPosition, Vector3 normal, LayerMask targetMask)
        {
            normal = normal.normalized;
            Vector3 hitDirection = -normal;
            float rad = 0.5f;
            Debug.DrawRay(worldPosition, normal, Color.yellow, 3f);
            _vines.Clear();
            int raysPerCircle = 8;
            Vector3[] rayPosition = GenerateCirclePoints(center: worldPosition, 
                                                         radius: rad, 
                                                         numberOfPoints: raysPerCircle,
                                                         normal: normal);
            // Cast rays in all directions to find edge points
            foreach (Vector3 pos in rayPosition)
            {
                Vector3 startPosition = pos - hitDirection * (rad - 0.1f);
                RaycastHit hit;
                if (Physics.Raycast(startPosition, hitDirection, out hit, rad * 3f, targetMask))
                {
                    // The hit point is on the sphere's edge
                    Vector3 edgePoint = hit.point;
                    _vines.Add(edgePoint);
                    Debug.DrawRay(startPosition, hitDirection * rad * 3f, Color.red, 2f);
                }
            }

        }
        Vector3[] GenerateCirclePoints(Vector3 center, float radius, int numberOfPoints, Vector3 normal)
        {
            float angleIncrement = 360f / (float)numberOfPoints; // Angle between each point
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
