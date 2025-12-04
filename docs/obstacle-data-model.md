# Obstacle Data Model

**Related Jira Story:** DSS-90  
**Related Systems:** `ObstacleDefinition`, `ObstacleCollection`, (later) `SwarmSimulation`, `SwarmManager`

---

## 1. Purpose

The obstacle data model defines how obstacles are represented inside the core
simulation layer.

It provides:

- A simple, extensible representation of a single obstacle.
- A container for managing a collection of obstacles.
- A clean abstraction that the simulation can query in later stories.

This story does **not** implement avoidance steering or Unity components.

---

## 2. Key Concepts

### 2.1 ObstacleDefinition

Represents a single obstacle in world space, currently modeled as a sphere:

- `obstacleIdentifier` – string identifier for debugging / logging.
- `centerPositionMeters` – world-space center position.
- `radiusMeters` – spherical radius in meters.

The spherical model is:

- Simple to reason about.
- Cheap to compute distances against.
- Sufficient for first-pass avoidance behavior.

The data model is intentionally designed so that more shapes can be added later
if needed (e.g., capsules, boxes).

### 2.2 ObstacleCollection

Represents a set of obstacles known to the simulation:

- Owns a `List<ObstacleDefinition>`.
- Exposes a read-only view via `IReadOnlyList<ObstacleDefinition>`.
- Provides methods to add and remove obstacles.

Later stories will add:

- Query helpers (e.g., closest obstacle, obstacles within a radius).
- Integration with `SwarmSimulation` and Unity MonoBehaviours.

---

## 3. Responsibilities

### ObstacleDefinition

- Store the **minimal information** needed for avoidance and debug:
  - Position
  - Radius
  - Identifier
- Provide a method to update center position when obstacles move.

### ObstacleCollection

- Maintain an internal list of obstacle definitions.
- Provide safe, read-only access to that list.
- Provide basic add/remove semantics.

---

## 4. Non-Responsibilities

The obstacle data model does **not**:

- Implement avoidance steering logic.
- Maintain spatial acceleration structures (e.g., grids, trees).
- Handle Unity-specific components or scene placement.
- Decide how obstacles are visualized.

Those concerns belong to later stories.

---

## 5. Initial Public API

### ObstacleDefinition

```csharp
public sealed class ObstacleDefinition
{
    public string obstacleIdentifier { get; }
    public Vector3 centerPositionMeters { get; private set; }
    public float radiusMeters { get; }

    public void UpdateCenterPosition(Vector3 newCenterPositionMeters);
}
