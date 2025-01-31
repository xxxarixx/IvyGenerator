using UnityEngine;
using System.Collections.Generic;

public class ProceduralVineGenerator : MonoBehaviour
{
    public int vineSegments = 20;
    public float segmentLength = 1f;
    public float gravity = 9.8f;
    public float initialSpeed = 5f;
    public float maxDeviation = 30f;  // Maximum angle deviation for each segment
    public LayerMask obstacleLayer; // Layer mask for objects that vines should interact with

    private List<Vector3> vinePoints;

    void Start()
    {
        GenerateVine();
    }
    [ContextMenu("Regenerate vine")]
    void GenerateVine()
    {
        vinePoints = new List<Vector3>();
        vinePoints.Add(transform.position); // Start at the object's position

        Vector3 currentDirection = transform.forward; // Initial growth direction

        for (int i = 0; i < vineSegments; i++)
        {
            Vector3 nextPoint = vinePoints[i] + currentDirection * segmentLength;

            // Raycast to detect obstacles
            RaycastHit hit;
            if (Physics.Raycast(vinePoints[i], currentDirection, out hit, segmentLength, obstacleLayer))
            {
                // Adjust the vine direction based on the hit normal
                Vector3 reflection = Vector3.Reflect(currentDirection, hit.normal);
                currentDirection = reflection.normalized;
                nextPoint = vinePoints[i] + currentDirection * segmentLength; // Recalculate next point
            }
            else
            {
                // Add some randomness to the direction
                float deviationAngle = Random.Range(-maxDeviation, maxDeviation);
                Quaternion rotation = Quaternion.AngleAxis(deviationAngle, Vector3.up); // Rotate around Y-axis
                currentDirection = rotation * currentDirection;
            }

            vinePoints.Add(nextPoint);

            // Apply "gravity" (a downward bias) - you might want to refine this
            currentDirection = (currentDirection + Vector3.down * 0.1f).normalized; // Adjust the 0.1f for strength
        }
    }

    void OnDrawGizmos()
    {
        if (vinePoints == null || vinePoints.Count == 0) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < vinePoints.Count - 1; i++)
        {
            Gizmos.DrawLine(vinePoints[i], vinePoints[i + 1]);
        }

        // Draw points for better visualization
        Gizmos.color = Color.red;
        foreach (Vector3 point in vinePoints)
        {
            Gizmos.DrawSphere(point, 0.1f); // Smaller sphere for points
        }
    }
}