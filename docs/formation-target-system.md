# Formation Target System

**Related Jira Story:** DSS-57  
**Related Systems:** `FormationDefinition`, `FormationSlot`, (later) `SwarmSimulation`, `SwarmManager`

---

## 1. Purpose

The formation target system represents **where each drone should be** when the swarm is in a specific formation (circle, line, arrow, etc.).

It provides:

- A way to describe **formation slots** (individual target positions/orientations).
- A way to group slots into a **formation definition**.
- A clean API that future generators (circle, line, arrow) and steering logic can use.

This story only defines the **data structures and core API**.  
Actual shape generation and steering toward formations are handled by later stories.

---

## 2. Key Concepts

### 2.1 Formation Anchor

Formations are defined **relative to an anchor** (a point/orientation in world space).

- The anchor will live in scene-level logic (e.g., `SwarmManager`).
- Slots store **local offsets** relative to the anchor, not absolute world positions.
- World target position = `anchorPosition + localOffset`.

### 2.2 FormationSlot

Represents a **single position in the formation**:

- A local offset from the formation anchor.
- A local forward direction (which way a drone should face).
- A slot index to keep them ordered.

### 2.3 FormationDefinition

Represents a **full formation**:

- A name (e.g., "Circle", "Line", "Arrow").
- A list of `FormationSlot` entries.
- Read-only access to slots for steering/assignment logic.

---

## 3. Responsibilities

### FormationSlot

- Store immutable **target information** for one drone in a formation:
  - `slotIndex`
  - `localOffsetMeters`
  - `localForwardDirection`
- Be simple, serializable, and safe to copy.

### FormationDefinition

- Own a collection of `FormationSlot` instances.
- Provide read-only access to slots in a stable order.
- Provide helper properties such as `SlotCount`.

Future stories will extend this system with:

- Formation generators (circle, line, arrow).
- Logic for assigning drones to slots.
- Steering behaviors that move drones toward their assigned slots.

---

## 4. Non-Responsibilities

The formation target system does **not**:

- Update drone physics or velocities.
- Apply flocking behaviors.
- Decide which drone goes to which slot.
- Know anything about Unity `Transform` components.
- Manage the formation anchor position/orientation (that’s a higher-level concern).

It is purely a **target representation layer**.

---

## 5. Public API (Initial)

### FormationSlot

```csharp
public readonly struct FormationSlot
{
    public int slotIndex { get; }
    public Vector3 localOffsetMeters { get; }
    public Vector3 localForwardDirection { get; }
}

### FormationDefinition
```csharp
public sealed class FormationDefinition
{
    public string formationName { get; }
    public int SlotCount { get; }
    public IReadOnlyList<FormationSlot> Slots { get; }
}
```
