# Obstacle Avoidance Behavior

**Related Jira Story:** DSS-92  
**Related Systems:** `SwarmSimulation`, `ObstacleCollection`, `ObstacleDefinition`, (later) `SwarmManager`

---

## 1. Purpose

Obstacle avoidance is a steering behavior that keeps drones from colliding
with static spherical obstacles in the environment.

For each drone, the behavior:

- Looks for nearby obstacles.
- Computes a steering vector that pushes the drone away from obstacles.
- Scales the steering based on how close the drone is to each obstacle.

This is similar in spirit to separation, but using obstacle centers instead of
neighbor drones.

---

## 2. Inputs

The obstacle avoidance behavior is controlled by:

- `avoidanceRadiusMeters`  
  Maximum distance at which obstacles influence a drone.

- `avoidanceStrength`  
  Scalar multiplier applied to the avoidance steering before it is added to
  the drone's velocity.

The simulation uses the existing `ObstacleCollection` for obstacle positions
and radii.

---

## 3. Algorithm Summary

For each drone:

1. Read its current world position.

2. For each obstacle:
   - Compute distance `d` between drone and obstacle center.
   - Compute an effective "influence radius" as:
     - `influenceRadius = avoidanceRadiusMeters + obstacle.radiusMeters`.
   - If `d` is greater than `influenceRadius`, ignore this obstacle.
   - Otherwise:
     - Compute a unit vector pointing away from the obstacle center:
       - `away = (dronePosition - obstacleCenter) / d`
     - Compute how close the drone is to the obstacle relative to the influence radius:
       - `distanceFactor = 1 - (d / influenceRadius)` clamped to `[0, 1]`.
     - Accumulate `away * distanceFactor`.

3. Average the accumulated avoidance directions over all contributing
   obstacles (if any).

4. Multiply by `avoidanceStrength` to obtain a steering vector.

5. Add the steering vector to the drone's velocity.

This behavior only modifies velocity; positions are still updated by
`DroneState.UpdateState(deltaTimeSeconds)`.

---

## 4. Public API

A new method is added to `SwarmSimulation`:

```csharp
public void ApplyObstacleAvoidanceToAllDrones(
    float avoidanceRadiusMeters,
    float avoidanceStrength)
