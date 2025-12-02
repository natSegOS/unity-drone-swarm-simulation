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
        [SerialField]
        private bool enableOrbitTestPattern = true;

        [Tooltiop("World-space center oint of the oribit, in meters")]
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
        }

        private void Update()
        {
            float deltaTimeSeconds = Time.deltaTime;

            droneState.UpdateState(deltaTimeSeconds);

            transform.position = droneState.positionMeters;
            transform.forward = droneState.forwardDirection;
        }
    }
}
