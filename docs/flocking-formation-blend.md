# Flocking–Formation Blending

**Related Jira Story:** DSS-62  
**Related Systems:** `SwarmSimulation`, `SwarmManager`, `FormationDefinition`

---

## 1. Purpose

The swarm should be able to smoothly transition between:

- Pure **flocking** (separation + alignment + cohesion)
- Pure **formation** (drones pulled strongly toward explicit formation slots)

The blending mechanism allows continuous control over this balance via a single
weight parameter.

---

## 2. Blending Model

A scalar parameter `formationWeight` in the range `[0, 1]` is used:

- `formationWeight = 0.0` → 100% flocking, 0% formation
- `formationWeight = 1.0` → 0% flocking, 100% formation
- Values in between represent a linear blend.

The corresponding **flocking weight** is:

```csharp
float flockingWeight = 1f - formationWeight;
