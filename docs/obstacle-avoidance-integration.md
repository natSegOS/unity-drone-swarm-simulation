# Obstacle Avoidance Integration

**Related Jira Story:** DSS-93  
**Related Systems:** `SwarmManager`, `SwarmSimulation`, `ObstacleDefinition`

---

## 1. Purpose

Integrate the obstacle avoidance steering behavior into the main swarm update
loop so that drones:

- Continue to flock and form shapes.
- Also react to obstacles by steering away from them.

This story wires the already implemented `ApplyObstacleAvoidanceToAllDrones`
method into `SwarmManager.Update()` and exposes basic tuning parameters.

---

## 2. Behavior Ordering

Within `SwarmManager.Update()` the steering behaviors are applied in this order:

1. **Flocking** (separation, alignment, cohesion)
2. **Formation steering** (if active)
3. **Obstacle avoidance**

Obstacle avoidance is applied **last** so that it has a high priority:
it is allowed to override or distort flocking/formation if necessary to avoid
collisions.

After steering has been applied, the swarm simulation is advanced via
`UpdateAllDrones(deltaTimeSeconds)`.

---

## 3. Parameters

Two new parameters are added to `SwarmManager`:

- `obstacleAvoidanceRadiusMeters`
  - Base radius around each drone within which obstacles influence avoidance.

- `obstacleAvoidanceStrength`
  - Scalar multiplier on the avoidance steering.

These parameters are passed directly into:

```csharp
swarmSimulation.ApplyObstacleAvoidanceToAllDrones(
    obstacleAvoidanceRadiusMeters,
    obstacleAvoidanceStrength);
