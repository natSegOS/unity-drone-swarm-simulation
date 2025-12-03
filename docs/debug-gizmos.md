# Debug Gizmos for Swarm Flocking

**Related Jira Story:** DSS-31  
**Related Systems:** `DroneState`, `SwarmSimulation`, `SwarmManager`

---

## 1. Purpose

The debug gizmos provide a visual way to inspect:

- The **neighbor radius** used for flocking behaviors.
- The **separation steering vector** per drone.
- The **alignment steering vector** per drone.
- The **cohesion steering vector** per drone.

These tools are used only in the editor for debugging and tuning.

---

## 2. Responsibilities

### DroneState

- Store per-frame debug steering vectors for:
  - Separation
  - Alignment
  - Cohesion

### SwarmSimulation

- Populate the debug steering vectors whenever:
  - `ApplySeparationToAllDrones`
  - `ApplyAlignmentToAllDrones`
  - `ApplyCohesionToAllDrones`
  - `ApplyFlockingToAllDrones` is called.

### SwarmManager

- Draw editor-only gizmos to show, for each drone:
  - Neighbor radius (wire spheres).
  - Steering vectors (lines) for separation, alignment, cohesion.

---

## 3. Usage

- In the `SwarmManager` inspector:
  - Toggle per-feature gizmos on/off.
  - Adjust a `gizmoVectorScale` to control vector length for readability.

- Gizmos are drawn in `OnDrawGizmosSelected` so they appear when the manager is selected in the Scene view.

---
