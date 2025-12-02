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

        /// <summary>
        /// Core simulation object that owns and updates all DroneStates
        /// </summary>
        private SwarmSimulation swarmSimulation;

        private void Awake()
        {
            swarmSimulation = new SwarmSimulation();
        }

        private void Update()
        {
            if (swarmSimulation == null) return;

            float deltaTimeSeconds = Time.deltaTime;
            
            swarmSimulation.UpdateAllDrones(deltaTimeSeconds);
        }
    }
}
