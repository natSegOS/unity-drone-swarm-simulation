# Alignment Behavior

**Related Jira Story:** DSS-27  
**Related Systems:** `SwarmSimulation`, (later) `SwarmManager`

---

## 1. Purpose

Alignment is a local steering behavior that causes each drone to align its direction of motion with its nearby neighbors.

It ensures that:

- Drones within a local neighborhood tend to move in roughly the same direction.
- The swarm exhibits coherent group motion instead of purely random trajectories.
- The swarm feels more "flock-like" and less chaotic.

---

## 2. Responsibilities

The alignment behavior is responsible for:

1. **Sampling neighbor velocities**
   - For each drone, consider other drones within a configurable alignment radius.
   - Read each neighbor's current velocity.

2. **Computing an average direction**
   - Compute the average velocity vector of all neighbors.
   - Convert it into a normalized "desired direction" when meaningful.

3. **Steering current drone velocity toward that direction**
   - Apply a steering change that nudges the drone’s velocity toward the neighbor-average direction.
   - Preserve or smoothly adjust the current speed magnitude as appropriate.

---

## 3. Non-Responsibilities

Alignment is *not* responsible for:

- Separation (maintaining minimum distances).
- Cohesion (moving toward neighbor centers).
- Obstacle avoidance.
- Directly modifying position or transform components (position is updated by the core integration step in `DroneState`).

Alignment only contributes a **directional steering influence** on velocity.

---

## 4. Parameters

Alignment uses the following parameters, passed in by the caller (e.g., SwarmManager):

- `alignmentRadiusMeters`
  - Maximum distance at which neighbors contribute to alignment.
- `alignmentStrength`
  - Scalar multiplier applied to the alignment steering before it is added to velocity.

These parameters will eventually be centralized in shared configuration.

---

## 5. Algorithm Summary

For each drone:

1. Initialize `sumNeighborVelocities = Vector3.zero` and `neighborCount = 0`.
2. Loop over all other drones:
   - Compute distance to neighbor.
   - If within `(0, alignmentRadiusMeters]`, accumulate neighbor velocity:
     - `sumNeighborVelocities += neighbor.velocityMetersPerSecond`.
     - `neighborCount++`.
3. If `neighborCount > 0` and `sumNeighborVelocities` is non-zero:
   - Compute `averageVelocity = sumNeighborVelocities / neighborCount`.
   - Compute `desiredDirection = averageVelocity.normalized`.
   - Compute current speed `currentSpeed = currentDrone.velocityMetersPerSecond.magnitude`.
   - Compute `desiredVelocity = desiredDirection * currentSpeed` (or a minimum fallback speed).
   - Compute steering vector: `alignmentSteering = desiredVelocity - currentVelocity`.
   - Apply steering: `currentVelocity += alignmentSteering * alignmentStrength`.

Position is still updated later via `DroneState.UpdateState(deltaTimeSeconds)`.
