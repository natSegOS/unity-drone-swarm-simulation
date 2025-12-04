# Formation Command Interface

**Related Jira Story:** DSS-63  
**Related Systems:** `SwarmManager`, `FormationCommandController`, `FormationDefinition`

---

## 1. Purpose

The formation command interface provides a simple way to switch the swarm
between different formations at runtime using keyboard input.

This is primarily a **debugging and demo** interface, not a final UI.

---

## 2. Controls

Default key bindings:

- `0` → No active formation (pure flocking, formation weight = 0).
- `1` → Circle formation.
- `2` → Line formation.
- `3` → Arrow formation.

---

## 3. Responsibilities

### FormationCommandController (MonoBehaviour)

- Reads user input each frame (e.g., keyboard keys 0–3).
- Determines which formation (if any) should be active.
- Uses formation generators to create `FormationDefinition` instances:
  - Circle
  - Line
  - Arrow
- Calls into `SwarmManager` to:
  - Set the active formation.
  - Adjust the formation blend weight.

### SwarmManager

- Exposes a method for setting the active formation.
- Exposes a method for setting formation blend weight.
- Continues to blend flocking and formation steering per DSS-62.

---

## 4. Data & Parameters

`FormationCommandController` exposes parameters for tuning via the inspector:

- Circle radius.
- Line spacing.
- Arrow shaft length, head length, head width.
- Target `formationWeight` to apply when a formation is activated.

The number of slots generated is based on the current drone count from
`SwarmManager.SwarmSimulation`.

---

## 5. Usage

- Attach `FormationCommandController` to a GameObject in the scene (e.g., AppRoot).
- Assign a reference to the scene's `SwarmManager`.
- In play mode, press keys 0–3 to switch between no formation, circle, line, and arrow.
