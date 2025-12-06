# Obstacle Prefabs and Scene Tools

**Related Jira Story:** DSS-94  
**Related Systems:** `ObstacleDefinition`, `ObstacleCollection`, `SwarmSimulation`, `SwarmManager`, `ObstacleComponent`

---

## 1. Purpose

Provide a simple way to place obstacles in the Unity scene that are connected
to the core simulation layer.

This is done by:

- Creating an `ObstacleComponent` MonoBehaviour that wraps an `ObstacleDefinition`.
- Creating a reusable obstacle prefab (sphere-based).
- Having the component automatically register/unregister with `SwarmManager`.

---

## 2. ObstacleComponent Responsibilities

`ObstacleComponent`:

- Owns an `ObstacleDefinition` instance.
- Reads obstacle configuration from the inspector:
  - Obstacle radius.
  - Optional explicit identifier.
- On enable:
  - Creates/updates its `ObstacleDefinition`.
  - Registers with `SwarmManager` (which forwards to `SwarmSimulation`).
- On disable:
  - Unregisters from `SwarmManager`.
- On update:
  - Synchronizes `ObstacleDefinition.centerPositionMeters` with `transform.position`.

This allows both static and moving obstacles to be supported.

---

## 3. SwarmManager Responsibilities

`SwarmManager`:

- Provides:

  ```csharp
  public void RegisterObstacle(ObstacleDefinition obstacle);
  public void UnregisterObstacle(ObstacleDefinition obstacle);
