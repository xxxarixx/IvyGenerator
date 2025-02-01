using UnityEngine;

namespace Vines
{
    public class UniversalVineGenerator
    {
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private float maxSteeringAngle = 30f;
        [SerializeField] private float stepDistance = 1f;
        [SerializeField] private int maxIterations = 100;

        private Vector3 currentPosition;
        private Vector3 currentDirection;
        private Vector3 currentNormal;
        private Vector3 previousNormal;

        public void GenerateVine(Vector3 startPosition, Vector3 startDirection, Vector3 startNormal)
        {
            Debug.Log("Generating vine");
            currentPosition = startPosition;
            currentDirection = startDirection.normalized;
            currentNormal = startNormal;
            previousNormal = Vector3.up; // Initial default

            for (int i = 0; i < maxIterations; i++)
            {
                // 1. Project direction onto the current surface's tangent plane
                Vector3 tangentDirection = Vector3.ProjectOnPlane(currentDirection, currentNormal).normalized;
                Debug.DrawRay(currentPosition, tangentDirection, Color.red, 10f);

                // 2. Calculate reflection (simulate "bounce" off the surface)
                Vector3 reflection = Vector3.Reflect(tangentDirection, currentNormal);
                Debug.DrawRay(currentPosition + tangentDirection, reflection, Color.magenta, 10f);
                // 3. Calculate steering angle based on curvature
                float curvature = Vector3.Angle(currentNormal, previousNormal);
                float steeringAngle = Mathf.Lerp(0f, maxSteeringAngle, curvature / 180f);
                previousNormal = currentNormal;

                // 4. Steer the reflection direction around the normal
                Quaternion steerRotation = Quaternion.AngleAxis(steeringAngle, currentNormal);
                Vector3 steeredDirection = steerRotation * reflection;

                // 5. Raycast to find the next point
                if (Physics.Raycast(currentPosition, steeredDirection, out RaycastHit hit, stepDistance, targetMask))
                {
                    currentPosition = hit.point;
                    currentNormal = hit.normal;
                    currentDirection = steeredDirection;

                    Debug.DrawLine(currentPosition, hit.point, Color.green, 10f);
                }
                else
                {
                    break; // Stop if no surface is hit
                }
            }
        }
    }

}