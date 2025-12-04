# Arrow Formation Generator

**Related Jira Story:** DSS-60  
**Related Systems:** `FormationDefinition`, `FormationSlot`, (later) `SwarmManager`

---

## 1. Purpose

The arrow formation generator creates a `FormationDefinition` whose slots are arranged
in a simple arrow shape:

- A **shaft** (a line of drones behind the tip).
- A **head** (two diagonal lines forming a "V" pointing forward).

This is a strong, directional visual that works well for showcasing formation control.

---

## 2. High-Level Shape

Local-space assumptions:

- The arrow points along **+Z** (forward).
- The formation anchor is at the **arrow tip**.
- The arrow lies in the **XZ plane** (Y = 0).

Conceptually:

- The **shaft** extends backward along negative Z.
- The **head** consists of two symmetric arms forming a "V" shape in the XZ plane.

All drones in the arrow face (local) **+Z**.

---

## 3. Inputs

The generator takes:

- `formationName` (string)
- `slotCount` (int)
- `shaftLengthMeters` (float)
- `headLengthMeters` (float)
- `headWidthMeters` (float)

Constraints:

- `slotCount >= 3`
- `shaftLengthMeters > 0`
- `headLengthMeters > 0`
- `headWidthMeters > 0`

If constraints are not met, the generator throws `ArgumentException`.

---

## 4. Output

The generator returns a `FormationDefinition` with:

- `SlotCount == slotCount`.
- One slot at (approximately) the **tip** (near the origin).
- Remaining slots distributed between:
  - The **shaft** along -Z.
  - The **head arms** along two diagonal lines forming a "V".

All slots use:

- `localOffsetMeters` in the XZ plane (Y = 0).
- `localForwardDirection = Vector3.forward` (arrow pointing along +Z).

Slots are ordered by `slotIndex` from `0` to `slotCount - 1`.

---

## 5. Allocation Strategy

To keep the generator simple and deterministic:

- Require `slotCount >= 3`.
- Assign:
  - 1 slot at the arrow tip.
  - A portion of remaining slots to the shaft.
  - The rest to the head arms, split equally left/right.

The exact distribution is implementation-defined but stable and symmetric.

---

## 6. API Shape

```csharp
public static class ArrowFormationGenerator
{
    public static FormationDefinition CreateArrowFormation(
        string formationName,
        int slotCount,
        float shaftLengthMeters,
        float headLengthMeters,
        float headWidthMeters)
}
