# Combined Flocking Step

**Related Jira Story:** DSS-29  
**Related Systems:** `SwarmSimulation`, (later) `SwarmManager`

---

## 1. Purpose

The combined flocking step represents a single conceptual operation:

> "Apply all local flocking behaviors (separation, alignment, cohesion) to the swarm for this simulation tick."

This provides:

- A single entry point for swarm-level steering.
- A consistent ordering of behaviors.
- A clear place to tune behavior weights and radii.

---

## 2. Responsibilities

The combined flocking step is responsible for:

1. **Delegating to individual behaviors in a defined order**
   - Separation
   - Alignment
   - Cohesion

2. **Configurable parameters per behavior**
   - Each behavior gets its own radius and strength.
   - Callers decide which behaviors are active by choosing non-zero strengths.

3. **Encapsulating flocking orchestration**
   - Callers (e.g., SwarmManager) should not need to know the details of
     invoking each behavior individually.

---

## 3. Behavior Order

For this project, the order is:

1. **Separation**
   - Highest priority: avoid collisions and overcrowding first.

2. **Alignment**
   - Then align with the movement of nearby neighbors.

3. **Cohesion**
   - Finally, gently pull drones toward local group centers.

This ordering is a reasonable heuristic and can be adjusted later if needed.

---

## 4. Parameters

For now, the combined flocking method accepts explicit parameters:

- `separationRadiusMeters`
- `separationStrength`
- `alignmentRadiusMeters`
- `alignmentStrength`
- `cohesionRadiusMeters`
- `cohesionStrength`

In later phases, these will likely be supplied via configuration (e.g., a ScriptableObject).

---

## 5. Public API

The combined flocking step is exposed via:

```csharp
public void ApplyFlockingToAllDrones(
    float separationRadiusMeters,
    float separationStrength,
    float alignmentRadiusMeters,
    float alignmentStrength,
    float cohesionRadiusMeters,
    float cohesionStrength)
