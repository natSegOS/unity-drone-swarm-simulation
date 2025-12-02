using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using DroneSwarmSimulation.Core.Simulation;

namespace DroneSwarmSimulation.Presentation
{
    /// <summary>
    /// Unity-facing controller that owns a DroneState instance
    /// and applies its simulated state to the GameObject's transform
    /// </summary>
    public class DroneController : MonoBehaviour
    {
        [Header("Identification")]
        
        [Tooltip("Logical identifier for this drone instance (for debugging and tracking)")]
        [SerializeField]
        private string droneIdentifier = "Drone_01";

        [Header("Initial Conditions")]
        
        [Tooltip("Initial position of the drone in world space, in meters")]
        [SerializeField]
        private Vector3 initialPositionMeters = Vector3.zero;

        [Tooltip("Initial velocity  of the drone in meters per second")]
        [SerializeField]
        private Vector3 initialVelocityMetersPerSecond = new Vector3(0f, 0f, 5f);

        [Tooltip("Initial forward direction of the drone. Will be normalized")]
        [SerializeField]
        private Vector3 initialForwardDirection = Vector3.forward;

        [Header("Test Movement - Orbit")]
        
        [Tooltip("Enable a simple orbiting test pattern around a fixed point")]
        [SerializeField]
        private bool enableOrbitTestPattern = true;

        [Tooltip("World-space center oint of the oribit, in meters")]
        [SerializeField]
        private Vector3 orbitCenterPositionMeters = Vector3.zero;

        [Tooltip("Radius of the orbit in meters")]
        [SerializeField]
        private float orbitRadiusMeters = 10f;

        [Tooltip("Angular speed of the orbit in degrees per second")]
        [SerializeField]
        private float orbitAngularSpeedDegreesPerSecond = 30f;

        [Tooltip("Height (Y coordinate) at which the drone orbits, in meters")]
        [SerializeField]
        private float orbitHeightMeters = 5f;

        private float currentOrbitAngleDegrees;

        private DroneState droneState;

        private void Awake()
        {
            transform.position = initialPositionMeters;

            droneState = new DroneState(
                droneIdentifier,
                initialPositionMeters,
                initialVelocityMetersPerSecond,
                initialForwardDirection
            );

            if (enableOrbitTestPattern)
            {
                // Initialize orbit starting angle based on initial position
                Vector3 flatFromCenter = transform.position - new Vector3(
                    orbitCenterPositionMeters.x,
                    transform.position.y,
                    orbitCenterPositionMeters.z
                );

                if (flatFromCenter.sqrMagnitude > 0.0001f)
                {
                    currentOrbitAngleDegrees = Mathf.Atan2(flatFromCenter.x, flatFromCenter.z) * Mathf.Rad2Deg;
                }
                else
                {
                    currentOrbitAngleDegrees = 0f;
                }
            }
        }

        private void Update()
        {
            float deltaTimeSeconds = Time.deltaTime;

            if (enableOrbitTestPattern)
            {
                UpdateOrbitTestPattern(deltaTimeSeconds);
            }

            droneState.UpdateState(deltaTimeSeconds);

            transform.position = droneState.positionMeters;
            transform.forward = droneState.forwardDirection;
        }

        /// <summary>
        /// Updates the drone's velocity and positino target to follow
        /// a simple circular orbit around a fixed point in world space
        /// </summary>
        /// <param name="deltaTimeSeconds"></param>
        private void UpdateOrbitTestPattern(float deltaTimeSeconds)
        {
            currentOrbitAngleDegrees += orbitAngularSpeedDegreesPerSecond * deltaTimeSeconds;
            
            // Keep the angle in [0, 360) range to avoid it growing unbounded
            if (currentOrbitAngleDegrees >= 360f)
            {
                currentOrbitAngleDegrees -= 360f;
            }

            // Compute the desired position on orbit circle
            float angleRadians = currentOrbitAngleDegrees * Mathf.Deg2Rad;

            float offsetX = Mathf.Sin(angleRadians) * orbitRadiusMeters;
            float offsetZ = Mathf.Cos(angleRadians) * orbitRadiusMeters;

            Vector3 orbitCenterAtHeight = new Vector3(
                orbitCenterPositionMeters.x,
                orbitHeightMeters,
                orbitCenterPositionMeters.z
            );

            Vector3 desiredPositionOnOrbit = orbitCenterAtHeight + new Vector3(offsetX, 0f, offsetZ);
            
            // Compute velocity that will move the drone toward desired position
            Vector3 toTarget = desiredPositionOnOrbit - droneState.positionMeters;

            // If very close to target, avoid tiny velocities
            const float minimumDistanceBeforeMovingMeters = 0.01f;
            if (toTarget.sqrMagnitude < minimumDistanceBeforeMovingMeters * minimumDistanceBeforeMovingMeters)
            {
                droneState.velocityMetersPerSecond = Vector3.zero;
                return;
            }
            
            Vector3 desiredDirection = toTarget.normalized;
            
            // Use existing speed or fall back to initial speed if nearly zero
            float currentSpeedMetersPerSecond = droneState.velocityMetersPerSecond.magnitude;
            float fallbackMetersPerSecond = initialVelocityMetersPerSecond.magnitude;

            if (currentSpeedMetersPerSecond < 0.01f)
            {
                currentSpeedMetersPerSecond = fallbackMetersPerSecond;
            }
            
            droneState.velocityMetersPerSecond = desiredDirection * currentSpeedMetersPerSecond;
        }
    }
}
