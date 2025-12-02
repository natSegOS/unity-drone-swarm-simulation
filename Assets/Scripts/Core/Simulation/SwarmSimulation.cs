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

public class SwarmSimulation
{
    
}
