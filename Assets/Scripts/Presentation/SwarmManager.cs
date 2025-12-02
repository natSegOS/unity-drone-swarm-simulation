using UnityEngine;
using DroneSwarmSimulation.Core.Simulation;

namespace DroneSwarmSimulation.Presentation
{
    /// <summary>
    /// Unity-facing manager responsible for coordinating a swarm of drones
    /// in the scene. Owns a SwarmSimulation instance and will later be
    /// responsible for spawning and wiring DroneController
    /// </summary>
    public class SwarmManager : MonoBehaviour
    {
        [Header("Swarm Setup")]
        
        [Tooltip("Prefab used to spawn individual drone GameObjects")]
        [SerializeField]
        private GameObject dronePrefab;

        [Tooltip("Number of drones to eventually spawn into the swarm")]
        [SerializeField]
        private int initialDroneCount = 10;

        [Tooltip("Seed used for any random swarm initialization logic")]
        [SerializeField]
        private int randomSeed = 12345;

        [Header("Spawn Area")]

        [Tooltip("Center of the area in which drones will be spawned, in world space")]
        [SerializeField]
        private Vector3 spawnAreaCenterMeters = Vector3.zero;

        [Tooltip("Size of the axis-aligned box (in meters) used to randomize initial spawn positions")]
        [SerializeField]
        private Vector3 spawnAreaSizeMeters = new Vector3(50f, 10f, 50f);

        [Tooltip("Initial speed of drones when spawned, in meters per second")]
        [SerializeField]
        private float initialSpawnSpeedMetersPerSecond = 5f;

        [Header("Flocking Parameters")]

        [Tooltip("Radius within which neighbors contribute to separation, in meters")]
        [SerializeField]
        private float separationRadiusMeters = 5f;

        [Tooltip("Strength of separation steering. Higher values push drones apart more aggressively")]
        [SerializeField]
        private float separationStrength = 1f;

        [Tooltip("Radius within which neighbors contribute to alignment, in meters")]
        [SerializeField]
        private float alignmentRadiusMeters = 8f;

        [Tooltip("Strength of alignment steering. Higher values make drones match neighbor direction faster")]
        [SerializeField]
        private float alignmentStrength = 0.5f;

        [Tooltip("Radius within which neighbors contribute to cohesion, in meters")]
        [SerializeField]
        private float cohesionRadiusMeters = 10f;

        [Tooltip("Strength of cohesion steering. Higher values pull drones toward neighbor centers more strongly")]
        [SerializeField]
        private float cohesionStrength = 0.3f;

        /// <summary>
        /// Core simulation object that owns and updates all DroneStates
        /// </summary>
        private SwarmSimulation swarmSimulation;

        private void Awake()
        {
            swarmSimulation = new SwarmSimulation();

            if (dronePrefab == null)
            {
                Debug.LogWarning("SwarmManager has no dronePrefab assigned. No drones will be spawned.");
                return;
            }

            if (initialDroneCount <= 0)
            {
                Debug.LogWarning("SwarmManager initialDroneCount is not positive. No drones will be spawned.");
                return;
            }

            Random.InitState(randomSeed);

            for (int i = 0; i < initialDroneCount; i++)
            {
                SpawnAndRegisterSingleDrone(i);
            }
        }

        /// <summary>
        /// Spawns a single drone GameObject, creates its DroneState,
        /// registers it with the swarm simulation, and connects the controller
        /// </summary>
        /// <param name="droneIndex">The index of the drone, used as an identifier</param>
        private void SpawnAndRegisterSingleDrone(int droneIndex)
        {
            Vector3 halfSize = spawnAreaCenterMeters * 0.5f;

            float randomX = Random.Range(-halfSize.x, halfSize.x);
            float randomY = Random.Range(-halfSize.y, halfSize.y);
            float randomZ = Random.Range(-halfSize.z, halfSize.z);

            Vector3 spawnPosition = spawnAreaCenterMeters + new Vector3(randomX, randomY, randomZ);
            Vector3 initialForwardDirection = Vector3.forward;
            Vector3 initialVelocity = initialForwardDirection * initialSpawnSpeedMetersPerSecond;

            string droneIdentifier = $"Drone_{droneIndex:D3}";

            GameObject droneInstance = Instantiate(dronePrefab, spawnPosition, Quaternion.LookRotation(initialForwardDirection, Vector3.up));

            var controller = droneInstance.GetComponent<DroneController>();
            if (controller == null)
            {
                Debug.LogWarning("Spawned dronePrefab does not have a DroneController component");
                return;
            }

            var droneState = new DroneState(
                droneIdentifier,
                spawnPosition,
                initialVelocity,
                initialForwardDirection
            );

            swarmSimulation.AddDrone(droneState);
            controller.Initialize(droneState);
        }

        private void Update()
        {
            if (swarmSimulation == null) return;

            float deltaTimeSeconds = Time.deltaTime;

            swarmSimulation.ApplyFlockingToAllDrones(
                separationRadiusMeters,
                separationStrength,
                alignmentRadiusMeters,
                alignmentStrength,
                cohesionRadiusMeters,
                cohesionStrength
            );
            
            swarmSimulation.UpdateAllDrones(deltaTimeSeconds);
        }
    }
}
