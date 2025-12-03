# Circle Formation Generator

**Related Jira Story:** DSS-58  
**Related Systems:** `FormationDefinition`, `FormationSlot`, (later) `SwarmManager`

---

## 1. Purpose

The circle formation generator creates a `FormationDefinition` whose slots are arranged
evenly around a circle in the XZ plane, centered around the formation anchor.

This formation is:

- Simple and visually clear.
- Useful for demonstrations and transitions.
- A reusable building block for Phase 3 formations.

---

## 2. Inputs

The generator requires:

- `formationName` (string)
- `slotCount` (int)
- `radiusMeters` (float)

Constraints:

- `slotCount > 0`
- `radiusMeters > 0`

If constraints are not met, the generator will throw an `ArgumentException`.

---

## 3. Output

The generator returns a `FormationDefinition` with:

- `SlotCount == slotCount`
- Slots with:
  - `localOffsetMeters` on a circle of radius `radiusMeters` in the XZ plane:
    - Y = 0
    - Evenly spaced angles in `[0, 2π)`
  - `localForwardDirection` pointing **outward from the center**:
    - From origin toward the slot's local offset

The slots are ordered by `slotIndex` from `0` to `slotCount - 1`.

---

## 4. API Shape

A single static method will be added:

```csharp
public static class CircleFormationGenerator
{
    public static FormationDefinition CreateCircleFormation(
        string formationName,
        int slotCount,
        float radiusMeters)
}
