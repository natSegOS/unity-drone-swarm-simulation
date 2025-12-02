using System.Collections.Generic;
using UnityEngine;

namespace DroneSwarmSimulation.Core.Simulation
{
    /// <summary>
    /// Represents the simulation state for a swarm of drones.
    /// Owns and updates a collection of DroneState instances.
    /// </summary>
    public class SwarmSimulation
    {
        private readonly List<DroneState> droneStates = new List<DroneState>();

        /// <summary>
        /// Provides read-only access to all drone states managed by this swarm
        /// </summary>
        public IReadOnlyList<DroneState> DroneStates => droneStates;
        
        // small threshold used to avoid division by zero when drones are extremely close
        private const float minimumNeighborDistanceSquared = 0.0001f;

        /// <summary>
        /// Adds an existing DroneState instance to the swarm
        /// </summary>
        /// <param name="droneState">The drone state to add</param>
        public void AddDrone(DroneState droneState)
        {
            if (droneState == null)
            {
                Debug.LogWarning("Attempted to add a null DroneState to SwarmSimulation");
                return;
            }

            if (!droneStates.Contains(droneState))
            {
                droneStates.Add(droneState);
            }
        }

        /// <summary>
        /// Removes a drone state from this swarm, if present
        /// </summary>
        /// <param name="droneState"></param>
        public void RemoveDrone(DroneState droneState)
        {
            if (droneState == null) return;
            droneStates.Remove(droneState);
        }
        
        /// <summary>
        /// Applies a local separation steering behavior to all drones in the swarm
        /// This behavior pushes drones away from nearby neighbors within the specified radius
        /// </summary>
        /// <param name="separationRadiusMeters">Maximum distance at which neighbors influence separation, in meters</param>
        /// <param name="separationStrength">Scalar multiplier to the computed separation direction before it is added to velocity</param>
        public void ApplySeparationToAllDrones(float separationRadiusMeters, float separationStrength)
        {
            if (droneStates.Count == 0) return;
            if (separationRadiusMeters <= 0f || separationStrength <= 0f) return;
            
            float separationRadiusSquared = separationRadiusMeters * separationRadiusMeters;
            
            for (int i = 0; i < droneStates.Count; i++)
            {
                DroneState currentDrone = droneStates[i];
                Vector3 currentPosition = currentDrone.positionMeters;
                
                Vector3 accumulatedSeparation = Vector3.zero;
                int neighborCount = 0;
                
                for (int j = 0; j < droneStates.Count; j++)
                {
                    if (j == i) continue;
                    
                    DroneState neighborDrone = droneStates[j];
                    Vector3 offset = currentPosition - neighborDrone.positionMeters;
                    float distanceSquared = offset.sqrMagnitude;
                    
                    if (distanceSquared > separationRadiusSquared || distanceSquared < minimumNeighborDistanceSquared) continue;
                    
                    float distance = Mathf.Sqrt(distanceSquared);
                    Vector3 directionAway = offset / distance;
                    
                    float distanceFactor = 1f - Mathf.Clamp01(distance / separationRadiusMeters);
                    accumulatedSeparation += directionAway * distanceFactor;
                    neighborCount++;
                }
                
                if (neighborCount > 0)
                {
                    accumulatedSeparation /= neighborCount;
                    currentDrone.velocityMetersPerSecond += accumulatedSeparation * separationStrength;
                }
            }
        }

        /// <summary>
        /// Advances the simulation state of all drones in the swarm
        /// forward in time by the specified amount
        /// </summary>
        /// <param name="deltaTimeSeconds">Time step in seconds</param>
        public void UpdateAllDrones(float deltaTimeSeconds)
        {
            for (int i = 0; i < droneStates.Count; i++)
            {
                droneStates[i].UpdateState(deltaTimeSeconds);
            }
        }
    }
}
