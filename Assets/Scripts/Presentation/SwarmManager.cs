using UnityEngine;
using DroneSwarmSimulation.Core.Formation;
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
        [Header("Simulation")]

        [Tooltip("Multiplier applied to Time.deltaTime when updating the swarm simulation. Values greater than 1 speed up the simulation")]
        [SerializeField]
        private float simulationTimeScale = 1.0f;

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

        [Header("Formation Settings")]

        [Tooltip("Blend between pure flocking (0) and pure formation (1)")]
        [Range(0f, 1f)]
        [SerializeField]
        private float formationWeight = 0f;

        [Tooltip("Base strength of formation steering when formationWeight is 1. Actual strength is scaled by formationWeight each frame")]
        [SerializeField]
        private float formationStrength = 1f;

        [Tooltip("Optional anchor transform used to position and orient the active formation in world space. If not assigned, the swarm manager's own transform will be used as the anchor")]
        [SerializeField]
        private Transform formationAnchorTransform;

        /// <summary>
        /// Currently active formation definition used when formation steering is enabled.
        /// May be null if no formation is active
        /// </summary>
        private FormationDefinition activeFormationDefinition;

        [Header("Obstacle Avoidance")]

        [Tooltip("Base radius around each drone within which obstacles contrinbute to avoidance, in meters")]
        [SerializeField]
        private float obstacleAvoidanceRadiusMeters = 15f;

        [Tooltip("Strength of obstacle avoidance steering. Higher values push drones away from obstacles more aggressively")]
        [SerializeField]
        private float obstacleAvoidanceStrength = 1.5f;

        [Header("Debug Gizmos")]

        [Tooltip("Draw neighbor radius gizmos for each drone")]
        [SerializeField]
        private bool drawNeighborRadiusGizmos = true;

        [Tooltip("Draw separation steering vectors for each drone")]
        [SerializeField]
        private bool drawSeparationVectors = true;

        [Tooltip("Draw alignment steering vectors for each drone")]
        [SerializeField]
        private bool drawAlignmentVectors = true;

        [Tooltip("Draw cohesion steering vectors for each drone")]
        [SerializeField]
        private bool drawCohesionVectors = true;

        [Tooltip("Scale factor applied to steering vectors when drawing gizmos")]
        [SerializeField]
        private float gizmoVectorScale = 1.0f;

        /// <summary>
        /// Core simulation object that owns and updates all DroneStates
        /// </summary>
        private SwarmSimulation swarmSimulation;
        public SwarmSimulation SwarmSimulation => swarmSimulation;

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

            float deltaTimeSeconds = Time.deltaTime * simulationTimeScale;

            float clampedFormationWeight = Mathf.Clamp01(formationWeight);
            float flockingWeight = 1f - clampedFormationWeight;

            if (flockingWeight > 0f)
            {
                swarmSimulation.ApplyFlockingToAllDrones(
                    separationRadiusMeters,
                    separationStrength * flockingWeight,
                    alignmentRadiusMeters,
                    alignmentStrength * flockingWeight,
                    cohesionRadiusMeters,
                    cohesionStrength * flockingWeight
                );
            }

            if (clampedFormationWeight > 0f && activeFormationDefinition != null && activeFormationDefinition.SlotCount > 0)
            {
                GetFormationAnchor(out Vector3 anchorPosition, out Quaternion anchorRotation);

                float effectiveFormationStrength = formationStrength * clampedFormationWeight;

                swarmSimulation.ApplyFormationSteeringToAllDrones(
                    activeFormationDefinition,
                    anchorPosition,
                    anchorRotation,
                    effectiveFormationStrength
                );
            }
            
            swarmSimulation.UpdateAllDrones(deltaTimeSeconds);
        }

        /// <summary>
        /// Sets the active formation definition used for formation steering.
        /// Passing null disables formation steering until a new formation is set.
        /// </summary>
        /// <param name="formationDefinition"></param>
        public void SetActiveFormation(FormationDefinition formationDefinition)
        {
            activeFormationDefinition = formationDefinition;
        }

        private void GetFormationAnchor(out Vector3 anchorPosition, out Quaternion anchorRotation)
        {
            if (formationAnchorTransform != null)
            {
                anchorPosition = formationAnchorTransform.position;
                anchorRotation = formationAnchorTransform.rotation;
            }
            else
            {
                anchorPosition = transform.position;
                anchorRotation = transform.rotation;
            }
        }

        /// <summary>
        /// Sets the blend weight between flocking and formation.
        /// Value will be clamped to the [0, 1] range
        /// </summary>
        /// <param name="weight">Desired formation weight in [0, 1]</param>
        public void SetFormationWeight(float weight)
        {
            formationWeight = Mathf.Clamp01(weight);
        }

        private void OnDrawGizmosSelected()
        {
            if (swarmSimulation == null) return;

            var states = swarmSimulation.DroneStates;
            if (states == null || states.Count == 0) return;

            for (int i = 0; i < states.Count; i++)
            {
                DroneState state = states[i];
                Vector3 position = state.positionMeters;

                if (drawNeighborRadiusGizmos)
                {
                    if (separationRadiusMeters > 0f)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireSphere(position, separationRadiusMeters);
                    }

                    if (alignmentRadiusMeters > 0f)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawWireSphere(position, alignmentRadiusMeters);
                    }

                    if (cohesionRadiusMeters > 0f)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawWireSphere(position, cohesionRadiusMeters);
                    }
                }

                if (drawSeparationVectors && state.lastSeparationSteeringMetersPerSecond != Vector3.zero)
                {
                    Gizmos.color = Color.red;
                    Vector3 end = position + state.lastSeparationSteeringMetersPerSecond * gizmoVectorScale;
                    Gizmos.DrawLine(position, end);
                }

                if (drawAlignmentVectors && state.lastAlignmentSteeringMetersPerSecond != Vector3.zero)
                {
                    Gizmos.color = Color.green;
                    Vector3 end = position + state.lastAlignmentSteeringMetersPerSecond * gizmoVectorScale;
                    Gizmos.DrawLine(position, end);
                }

                if (drawCohesionVectors && state.lastCohesionSteeringMetersPerSecond != Vector3.zero)
                {
                    Gizmos.color = Color.blue;
                    Vector3 end = position + state.lastCohesionSteeringMetersPerSecond * gizmoVectorScale;
                    Gizmos.DrawLine(position, end);
                }
            }
        }
    }
}
