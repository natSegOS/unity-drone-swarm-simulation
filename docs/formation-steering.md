# Formation Assignment and Steering

**Related Jira Story:** DSS-61  
**Related Systems:** `FormationDefinition`, `SwarmSimulation`, `DroneState`, (later) `SwarmManager`

---

## 1. Purpose

Formation steering pulls drones toward **explicit formation slots**:

- Each drone is assigned a **target slot** in a `FormationDefinition`.
- The slot is defined in local space relative to a **formation anchor**.
- The simulation computes a steering vector that nudges each drone toward its assigned slot.

This provides the low-level mechanism needed for:

- Circle, line, and arrow formations.
- Blending between flocking and formation modes in later stories.

---

## 2. Assignment Strategy (Initial)

For the initial implementation, assignment is **index-based**:

- `SwarmSimulation.DroneStates[i]` is assigned to `FormationDefinition.Slots[i % SlotCount]`.

This approach is:

- Deterministic.
- Simple to implement.
- Sufficient for early visuals.

More advanced strategies (e.g., nearest-slot assignment, dynamic re-mapping) can be introduced later if needed.

---

## 3. Coordinate System

Formation slots are defined in **local space**:

- `FormationSlot.localOffsetMeters` and `localForwardDirection` are local to the formation anchor.

World-space target position is computed as:

```csharp
worldTargetPosition = anchorPosition + anchorRotation * slot.localOffsetMeters;
