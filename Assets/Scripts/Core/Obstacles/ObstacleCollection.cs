using System.Collections.Generic;
using UnityEngine;

namespace DroneSwarmSimulation.Core.Obstacles
{
    /// <summary>
    /// Represents a collection of obstacle definitions used by the simulation.
    /// </summary>
    public sealed class ObstacleCollection
    {
        private readonly List<ObstacleDefinition> obstacles = new List<ObstacleDefinition>();

        /// <summary>
        /// Read-obly view of all obstacles currently in the collection
        /// </summary>
        public IReadOnlyList<ObstacleDefinition> Obstacles => obstacles;

        /// <summary>
        /// Adds an obstacle to the collection if it is not already present
        /// </summary>
        /// <param name="obstacle">The obstacle definition to add</param>
        public void AddObstacle(ObstacleDefinition obstacle)
        {
            if (obstacle == null)
            {
                Debug.LogWarning("Attempted to add a null obstacle to ObstacleCollection");
                return;
            }

            if (!obstacles.Contains(obstacle))
            {
                obstacles.Add(obstacle);
            }
        }

        /// <summary>
        /// Removes an obstacle from the collection, if present
        /// </summary>
        /// <param name="obstacle">The obstacle definition to remove</param>
        /// <returns>True if the obstacle was removed, false it was not present or null</returns>
        public bool RemoveObstacle(ObstacleDefinition obstacle)
        {
            if (obstacle == null) return false;
            return obstacles.Remove(obstacle);
        }

        /// <summary>
        /// Removes all obstacles from the collection
        /// </summary>
        public void Clear()
        {
            obstacles.Clear();
        }
    }
}
