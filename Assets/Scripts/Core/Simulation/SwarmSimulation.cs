using System.Collections.Generic;
using DroneSwarmSimulation.Core.Formation;
using UnityEngine;
using UnityEngine.Assertions.Must;

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
        
        // minimum speed used when aligning drones whose current speed is effectively zero
        private const float minimumAlignmentSpeedMetersPerSecond = 0.1f;
        
        // minimum speed used when applying cohesion to drones with near-zero speed
        private const float minimumCohesionSpeedMetersPerSecond = 0.1f;

        // minimum speed used when steering drones toward formation targets
        private const float minimumFormationSpeedMetersPerSecond = 0.1f;

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
                    Vector3 steering = accumulatedSeparation * separationStrength;

                    currentDrone.velocityMetersPerSecond += steering;
                    currentDrone.lastSeparationSteeringMetersPerSecond = steering;
                }
            }
        }
        
        /// <summary>
        /// Applies a local alignment steering behavior to all drones in the swarm.
        /// Drones will tend to align their direction of travel with nearby neighbors.
        /// </summary>
        /// <param name="alignmentRadiusMeters">Maximum distance at which neighbors influence alignment, in meters</param>
        /// <param name="alignmentStrength">Scalar multiplier applied to alignment steering before adding to velocity</param>
        public void ApplyAlignmentToAllDrones(float alignmentRadiusMeters, float alignmentStrength)
        {
            if (droneStates.Count == 0) return;
            if (alignmentRadiusMeters <= 0f || alignmentStrength <= 0f) return;
            
            float alignmentRadiusSquared = alignmentRadiusMeters * alignmentRadiusMeters;
            
            for (int i = 0; i < droneStates.Count; i++)
            {
                DroneState currentDrone = droneStates[i];
                Vector3 currentVelocity = currentDrone.velocityMetersPerSecond;
                Vector3 currentPosition = currentDrone.positionMeters;
                
                Vector3 sumNeighborVelocities = Vector3.zero;
                int neighborCount = 0;
                
                for (int j = 0; j < droneStates.Count; j++)
                {
                    if (j == i) continue;
                    
                    DroneState neighborDrone = droneStates[j];
                    Vector3 offset = neighborDrone.positionMeters - currentPosition;
                    float distanceSquared = offset.sqrMagnitude;
                    
                    if (distanceSquared > alignmentRadiusSquared || distanceSquared < minimumNeighborDistanceSquared) continue;
                    
                    sumNeighborVelocities += neighborDrone.velocityMetersPerSecond;
                    neighborCount++;
                }
                
                if (neighborCount == 0) continue;
                
                Vector3 averageVelocity = sumNeighborVelocities / neighborCount;
                if (averageVelocity == Vector3.zero) continue;
                
                Vector3 desiredDirection = averageVelocity.normalized;
                
                float currentSpeedMetersPerSecond = currentVelocity.magnitude;
                if (currentSpeedMetersPerSecond < minimumAlignmentSpeedMetersPerSecond)
                {
                    currentSpeedMetersPerSecond = minimumAlignmentSpeedMetersPerSecond;
                }
                
                Vector3 desiredVelocity = desiredDirection * currentSpeedMetersPerSecond;
                Vector3 alignmentSteering = desiredVelocity - currentVelocity;
                Vector3 steering = alignmentSteering * alignmentStrength;
                
                currentDrone.velocityMetersPerSecond += steering;
                currentDrone.lastAlignmentSteeringMetersPerSecond = steering;
            }
        }
        
        /// <summary>
        /// Applies a local cohesion steering behavior to all drones in the swarm.
        /// Drones will tend to move toward the center of nearby neighbors.
        /// </summary>
        /// <param name="cohesionRadiusMeters">Maximum distance at which neighbors influece cohesion, in meters</param>
        /// <param name="cohesionStrength">Scalar multiplier applied to the cohesion steering before it is added to velocity</param>
        public void ApplyCohesionToAllDrones(float cohesionRadiusMeters, float cohesionStrength)
        {
            if (droneStates.Count == 0) return;
            if (cohesionRadiusMeters <= 0f || cohesionStrength <= 0f) return;
            
            float cohesionRadiusSquared = cohesionRadiusMeters * cohesionRadiusMeters;
            
            for (int i = 0; i < droneStates.Count; i++)
            {
                DroneState currentDrone = droneStates[i];
                Vector3 currentPosition = currentDrone.positionMeters;
                Vector3 currentVelocity = currentDrone.velocityMetersPerSecond;
                
                Vector3 sumNeighborPositions = Vector3.zero;
                int neighborCount = 0;
                
                for (int j = 0; j <  droneStates.Count; j++)
                {
                    if (j == i) continue;
                    
                    DroneState neighborDrone = droneStates[j];
                    Vector3 offset = neighborDrone.positionMeters - currentPosition;
                    float distanceSquared = offset.sqrMagnitude;
                    
                    if (distanceSquared > cohesionRadiusSquared || distanceSquared < minimumNeighborDistanceSquared) continue;
                    
                    sumNeighborPositions += neighborDrone.positionMeters;
                    neighborCount++;
                }
                
                if (neighborCount == 0) continue;
                
                Vector3 centerOfNeighbors = sumNeighborPositions / neighborCount;
                Vector3 toCenter = centerOfNeighbors - currentPosition;
                
                if (toCenter == Vector3.zero) continue;
                
                Vector3 desiredDirection = toCenter.normalized;
                
                float currentSpeedMetersPerSecond = currentVelocity.magnitude;
                if (currentSpeedMetersPerSecond < minimumCohesionSpeedMetersPerSecond)
                {
                    currentSpeedMetersPerSecond = minimumCohesionSpeedMetersPerSecond;
                }
                
                Vector3 desiredVelocity = desiredDirection * currentSpeedMetersPerSecond;
                Vector3 cohesionSteering = desiredVelocity - currentVelocity;
                Vector3 steering = cohesionSteering * cohesionStrength;
                
                currentDrone.velocityMetersPerSecond += steering;
                currentDrone.lastCohesionSteeringMetersPerSecond = steering;
            }
        }
        
        public void ApplyFlockingToAllDrones(
            float separationRadiusMeters,
            float separationStrength,
            float alignmentRadiusMeters,
            float alignmentStrength,
            float cohesionRadiusMeters,
            float cohesionStrength)
        {
            if (droneStates.Count == 0) return;

            for (int i = 0; i < droneStates.Count; i++)
            {
                droneStates[i].ClearDebugSteering();
            }
            
            ApplySeparationToAllDrones(separationRadiusMeters, separationStrength);
            ApplyAlignmentToAllDrones(alignmentRadiusMeters, alignmentStrength);
            ApplyCohesionToAllDrones(cohesionRadiusMeters, cohesionStrength);
        }

        public void ApplyFormationSteeringToAllDrones(
            FormationDefinition formation,
            Vector3 anchorPosition,
            Quaternion anchorRotation,
            float formationStrength)
        {
            if (droneStates.Count == 0) return;
            if (formation == null)
            {
                Debug.LogWarning("ApplyFormationSteeringToAllDrones called with a null formation");
                return;
            }

            if (formation.SlotCount == 0) return;
            if (formationStrength <= 0) return;

            int slotCount = formation.SlotCount;

            for (int i = 0; i < droneStates.Count; i++)
            {
                DroneState currentDrone = droneStates[i];
                Vector3 currentPosition = currentDrone.positionMeters;
                Vector3 currentVelocity = currentDrone.velocityMetersPerSecond;

                int slotIndex = i % slotCount;
                FormationSlot slot = formation.GetSlotAt(slotIndex);

                Vector3 worldTargetPosition = anchorPosition + anchorRotation * slot.localOffsetMeters;
                Vector3 toTarget = worldTargetPosition - currentPosition;
                if (toTarget == Vector3.zero) continue;

                Vector3 desiredDirection = toTarget.normalized;
                float currentSpeedMetersPerSecond = currentVelocity.magnitude;
                if (currentSpeedMetersPerSecond < minimumFormationSpeedMetersPerSecond)
                {
                    currentSpeedMetersPerSecond = minimumFormationSpeedMetersPerSecond;
                }

                Vector3 desiredVelocity = desiredDirection * currentSpeedMetersPerSecond;
                Vector3 formationSteering = desiredVelocity - currentVelocity;

                currentDrone.velocityMetersPerSecond += formationSteering * formationStrength;
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
