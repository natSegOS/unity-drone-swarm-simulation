# Separation Behavior

**Related Jira Story:** DSS-26  
**Related Systems:** `SwarmSimulation`, (later) `SwarmManager`

---

## 1. Purpose

Separation is a local avoidance behavior that prevents drones from getting too close to each other.

It ensures that:

- Each drone maintains a minimum comfortable distance from its neighbors.
- The swarm does not collapse into a single clump.
- Local collisions are reduced without enforcing any global formation or pattern.

---

## 2. Responsibilities

The separation behavior is responsible for:

1. **Detecting nearby neighbors**
   - For each drone, consider other drones within a configurable separation radius.

2. **Computing a steering direction away from neighbors**
   - For neighbors that are too close, compute a vector pointing away from them.
   - Weight contributions so closer neighbors push more strongly.

3. **Averaging neighbor influence**
   - Combine contributions from all neighbors into a single separation vector per drone.

4. **Applying the separation steering to drone velocity**
   - Modify the drone's velocity, not its position directly.
   - Let the core integration (`DroneState.UpdateState`) handle position updates.

---

## 3. Non-Responsibilities

Separation is *not* responsible for:

- Alignment (matching direction or speed with neighbors).
- Cohesion (steering toward neighbor centers).
- Obstacle avoidance (static or dynamic obstacles).
- Global formation control (shapes like circles, lines, arrows).
- Updating Unity `Transform` components (that’s handled by controllers/MonoBehaviours).

It is purely a **local, neighbor-based spacing behavior** inside the simulation.

---

## 4. Parameters

For now, separation uses the following parameters, passed in by the caller (e.g., SwarmManager later):

- `separationRadiusMeters`
  - Maximum distance at which neighbors influence separation.
- `separationStrength`
  - Scalar multiplier applied to the separation steering vector before adding it to velocity.

These will eventually be centralized into configuration (e.g., a config ScriptableObject).

---

## 5. Algorithm Summary

For each drone:

1. Initialize an accumulator vector `separation = Vector3.zero` and `neighborCount = 0`.
2. Loop over all other drones:
   - Compute offset = `currentPosition - neighborPosition`.
   - If distance is within `(0, separationRadiusMeters]`:
     - Normalize `directionAway = offset / distance`.
     - Compute a weight based on how close the neighbor is (closer ⇒ stronger).
     - Accumulate `separation += directionAway * weight`.
     - Increment `neighborCount`.
3. If `neighborCount > 0`:
   - Average `separation /= neighborCount`.
   - Apply to velocity:
     - `velocity += separation * separationStrength`.

Position is still updated later via `DroneState.UpdateState(deltaTimeSeconds)`.

---
