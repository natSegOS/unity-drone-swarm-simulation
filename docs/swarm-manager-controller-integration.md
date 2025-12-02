# SwarmManager ↔ DroneController Integration

**Related Jira Story:** DSS-30  
**Related Systems:** `SwarmManager`, `SwarmSimulation`, `DroneController`, `DroneState`

---

## 1. Purpose

Connect the swarm-level simulation (`SwarmSimulation` owned by `SwarmManager`) with
individual visual drones (`DroneController` on GameObjects).

After this integration:

- `SwarmManager` is responsible for spawning drone GameObjects and creating their `DroneState` instances.
- `SwarmSimulation` owns and updates all `DroneState` instances.
- `DroneController` simply reflects the state of its assigned `DroneState` onto a Unity `Transform`.

---

## 2. Responsibilities

### SwarmManager

- Own a single `SwarmSimulation` instance.
- Spawn `initialDroneCount` drone GameObjects from `dronePrefab`.
- For each spawned drone:
  - Create a corresponding `DroneState` (position, velocity, identifier, forward direction).
  - Register the `DroneState` with `SwarmSimulation`.
  - Pass the `DroneState` reference into the drone's `DroneController`.
- Each frame:
  - (In later stories) apply flocking behaviors.
  - Call `SwarmSimulation.UpdateAllDrones(deltaTime)` to advance simulation.

### DroneController

- Hold a reference to a `DroneState` assigned by `SwarmManager`.
- In `Update()`:
  - Read `DroneState`'s position and forward direction.
  - Apply them to the GameObject's `Transform`.
- Optionally still support a local test mode when no external `DroneState` is assigned
  (e.g., for single-drone debugging), but in swarm mode it is driven by the swarm.

---

## 3. Data Flow

Initialization:

1. SwarmManager.Awake():
   - Create `SwarmSimulation`.
   - Spawn N drones using `dronePrefab`.
   - For each:
     - Create `DroneState`.
     - `SwarmSimulation.AddDrone(droneState)`.
     - `DroneController.Initialize(droneState)`.

Runtime:

1. SwarmManager.Update():
   - (Later) call `ApplyFlockingToAllDrones(...)`.
   - Call `UpdateAllDrones(deltaTime)`.

2. DroneController.Update():
   - Read `DroneState.positionMeters` and `DroneState.forwardDirection`.
   - Apply to `transform.position` and `transform.forward`.

---
