using UnityEngine;

namespace DroneSwarmSimulation.Core.Simulation
{
    /// <summary>
    /// Represents the simulation state of a single drone, independent of Unity's scene objects
    /// </summary>
    public class DroneState
    {
        /// <summary>
        ///  Unique identifier for this drone instance (e.g., "Drone_01")
        /// </summary>
        public string droneIdentifier;
        
        /// <summary>
        /// Current position of the drone in world space, measured in meters
        /// </summary>
        public Vector3 positionMeters;
        
        /// <summary>
        /// Current velocity of the drone in meters per second
        /// </summary>
        public Vector3 velocityMetersPerSecond;
        
        /// <summary>
        /// The normalized forward direction of the drone in world space
        /// </summary>
        public Vector3 forwardDirection;

        /// <summary>
        /// Debug-only: last separation steering applied to this drone, in meters per second
        /// </summary>
        public Vector3 lastSeparationSteeringMetersPerSecond;

        /// <summary>
        /// Debug-only: last alignment steering applied to this drone, in meters per second
        /// </summary>
        public Vector3 lastAlignmentSteeringMetersPerSecond;

        /// <summary>
        /// Debug-only: last cohesion steering applied to this drone, in meters per second
        /// </summary>
        public Vector3 lastCohesionSteeringMetersPerSecond;

        /// <summary>
        /// Creates a new DroneState with the givien initial conditions
        /// </summary>
        /// <param name="droneIdentifier">Logical ID used for debugging and tracking</param>
        /// <param name="initialPositionMeters">Starting position in world space</param>
        /// <param name="initialVelocityMetersPerSecond">Starting velocity</param>
        /// <param name="initialForwardDirection">Initial forward direction (will be normalized)</param>
        public DroneState(
            string droneIdentifier,
            Vector3 initialPositionMeters,
            Vector3 initialVelocityMetersPerSecond,
            Vector3 initialForwardDirection)
        {
            this.droneIdentifier = droneIdentifier;
            this.positionMeters = initialPositionMeters;
            this.velocityMetersPerSecond = initialVelocityMetersPerSecond;

            if (initialForwardDirection == Vector3.zero)
            {
                // Default forward direction if none provided
                this.forwardDirection = Vector3.forward;
            }
            else
            {
                this.forwardDirection = initialForwardDirection;
            }

            lastSeparationSteeringMetersPerSecond = Vector3.zero;
            lastAlignmentSteeringMetersPerSecond = Vector3.zero;
            lastCohesionSteeringMetersPerSecond = Vector3.zero;
        }

        /// <summary>
        /// Advances the drone's simulation state forward in time
        /// For now, this simply moves the drone based on its current velocity
        /// </summary>
        /// <param name="deltaTimeSeconds">Amount of simulated time to advance, in seconds</param>
        public void UpdateState(float deltaTimeSeconds)
        {
            this.positionMeters += this.velocityMetersPerSecond * deltaTimeSeconds;

            if (velocityMetersPerSecond != Vector3.zero)
            {
                forwardDirection = velocityMetersPerSecond.normalized;
            }
        }

        /// <summary>
        /// Clears per-frame debug steering values. Intended to be called once per flocking step
        /// </summary>
        public void ClearDebugSteering()
        {
            lastSeparationSteeringMetersPerSecond = Vector3.zero;
            lastAlignmentSteeringMetersPerSecond = Vector3.zero;
            lastCohesionSteeringMetersPerSecond = Vector3.zero;
        }
    }
}
