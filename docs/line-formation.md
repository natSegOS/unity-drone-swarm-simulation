# Line Formation Generator

**Related Jira Story:** DSS-59  
**Related Systems:** `FormationDefinition`, `FormationSlot`, (later) `SwarmManager`

---

## 1. Purpose

The line formation generator creates a `FormationDefinition` whose slots are arranged
evenly along a straight line in the XZ plane, centered around the formation anchor.

This formation is:

- Simple and visually clear.
- Useful for "parade" or "flight line" style visuals.
- A good counterpart to the circle and arrow formations.

---

## 2. Inputs

The generator requires:

- `formationName` (string)
- `slotCount` (int)
- `spacingMeters` (float)

Constraints:

- `slotCount > 0`
- `spacingMeters > 0`

If constraints are not met, the generator will throw an `ArgumentException`.

---

## 3. Output

The generator returns a `FormationDefinition` with:

- `SlotCount == slotCount`
- Slots laid out along the **local X axis**, centered at the origin:
  - Y = 0
  - Z = 0
  - X positions are spaced by `spacingMeters`
  - The entire line is centered so that:
    - For odd `slotCount`, one slot is at X = 0.
    - For even `slotCount`, the center point lies between the two middle slots.

- `localForwardDirection` for each slot points along the local +Z axis (`Vector3.forward`),
  so all drones in the line face the same direction.

Slots are ordered by `slotIndex` from `0` to `slotCount - 1`.

---

## 4. API Shape

A single static method will be added:

```csharp
public static class LineFormationGenerator
{
    public static FormationDefinition CreateLineFormation(
        string formationName,
        int slotCount,
        float spacingMeters)
}
