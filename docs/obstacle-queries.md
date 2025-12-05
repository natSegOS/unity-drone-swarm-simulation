# Obstacle Query Utilities

**Related Jira Story:** DSS-91  
**Related Systems:** `ObstacleCollection`, `SwarmSimulation`, `ObstacleDefinition`

---

## 1. Purpose

Obstacle query utilities provide the simulation with a simple way to:

- Ask which obstacles exist.
- Find the closest obstacle to a given position.
- Enumerate obstacles within a certain radius.

These queries are used as building blocks for obstacle avoidance steering.

---

## 2. Data

`SwarmSimulation` owns an `ObstacleCollection` instance:

- Exposed as a read-only property for higher-level systems (e.g., SwarmManager).
- The collection is populated externally (e.g., from Unity obstacle components).

The simulation itself does not create or destroy obstacles; it only queries them.

---

## 3. Query Helpers

The initial set of helpers includes:

1. **Closest obstacle**
   ```csharp
   public bool TryGetClosestObstacle(
       Vector3 queryPositionMeters,
       out ObstacleDefinition closestObstacle,
       out float closestDistanceMeters)
