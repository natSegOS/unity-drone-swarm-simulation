# Cohesion Behavior

**Related Jira Story:** DSS-28  
**Related Systems:** `SwarmSimulation`, (later) `SwarmManager`

---

## 1. Purpose

Cohesion is a local steering behavior that causes each drone to move toward the center of mass of its nearby neighbors.

It ensures that:

- Drones do not drift infinitely far apart.
- Local groups of drones tend to stay together.
- The swarm forms coherent clusters instead of purely independent agents.

---

## 2. Responsibilities

The cohesion behavior is responsible for:

1. **Sampling neighbor positions**
   - For each drone, consider other drones within a configurable cohesion radius.
   - Read each neighbor's position.

2. **Computing the local neighbor center**
   - Average positions of all neighbors to find the local center of mass.

3. **Steering toward this center**
   - Compute a direction from the current drone toward the neighbor center.
   - Apply a steering influence that nudges the drone’s velocity toward that direction.

---

## 3. Non-Responsibilities

Cohesion is *not* responsible for:

- Separation (avoiding overlap or collisions).
- Alignment (matching neighbor direction or speed).
- Obstacle avoidance.
- Directly modifying positions (positions are updated via `DroneState.UpdateState`).

Cohesion only contributes a **steering influence toward neighbors' center**.

---

## 4. Parameters

Cohesion uses the following parameters, provided by the caller (e.g., SwarmManager):

- `cohesionRadiusMeters`
  - Maximum distance at which neighbors contribute to cohesion.
- `cohesionStrength`
  - Scalar multiplier applied to the cohesion steering before it is added to velocity.

These parameters will be centralized later in configuration.

---

## 5. Algorithm Summary

For each drone:

1. Initialize `sumNeighborPositions = Vector3.zero` and `neighborCount = 0`.
2. Loop over all other drones:
   - Compute distance to neighbor.
   - If within `(0, cohesionRadiusMeters]`, accumulate:
     - `sumNeighborPositions += neighbor.positionMeters`
     - `neighborCount++`.
3. If `neighborCount > 0`:
   - Compute `centerOfNeighbors = sumNeighborPositions / neighborCount`.
   - Compute `toCenter = centerOfNeighbors - currentPosition`.
   - If `toCenter` is non-zero:
     - `desiredDirection = toCenter.normalized`.
     - Compute preferred speed (e.g., preserve current speed or use a minimum).
     - `desiredVelocity = desiredDirection * preferredSpeed`.
     - Steering = `desiredVelocity - currentVelocity`.
     - Apply cohesion: `currentVelocity += steering * cohesionStrength`.

Position is then updated later via `DroneState.UpdateState(deltaTimeSeconds)`.
