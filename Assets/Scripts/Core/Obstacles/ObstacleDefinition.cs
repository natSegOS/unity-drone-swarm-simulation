using System;
using UnityEngine;

namespace DroneSwarmSimulation.Core.Obstacles
{
    /// <summary>
    /// Represents a single obstacle in world space, modeled as a sphere.
    /// </summary>
    public sealed class ObstacleDefinition
    {
        /// <summary>
        /// Human-readable identifier for this obstacle. Useful for debugging and logging
        /// </summary>
        public string obstacleIdentifier { get; }

        /// <summary>
        /// World-space center position of the obstacle, in meters
        /// </summary>
        public Vector3 centerPositionMeters { get; private set; }

        /// <summary>
        /// Spherical radius of the obstacle, in meters
        /// </summary>
        public float radiusMeters { get; }

        public ObstacleDefinition(
            string obstacleIdentifier,
            Vector3 centerPositionMeters,
            float radiusMeters)
        {
            if (string.IsNullOrWhiteSpace(obstacleIdentifier)) throw new ArgumentException("Obstacle identifier must be a non-empty string", nameof(obstacleIdentifier));
            if (radiusMeters <= 0) throw new ArgumentException("Obstacle radius must be greater than zero", nameof(radiusMeters));

            this.obstacleIdentifier = obstacleIdentifier;
            this.centerPositionMeters = centerPositionMeters;
            this.radiusMeters = radiusMeters;
        }

        /// <summary>
        /// Updates the world-space center position of this obstacle.
        /// Intended for dynamic obstacles that move over time
        /// </summary>
        /// <param name="newCenterPositionMeters">New world-space center position, in meters</param>
        public void UpdateCenterPosition(Vector3 newCenterPositionMeters)
        {
            centerPositionMeters = newCenterPositionMeters;
        }
    }
}
