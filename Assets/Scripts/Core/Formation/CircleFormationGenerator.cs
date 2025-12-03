using System;
using System.Collections.Generic;
using UnityEngine;

namespace DroneSwarmSimulation.Core.Formation
{
    /// <summary>
    /// Provides factory methods for creating circular formations.
    /// Slots are arranged evenly around a circle in the XZ plane,
    /// defined in local space relative to a formation anchor
    /// </summary>
    public static class CircleFormationGenerator
    {
        public static FormationDefinition CreateCircleFormation(
            string formationName,
            int slotCount,
            float radiusMeters)
        {
            if (slotCount <= 0) throw new ArgumentException("slotCount must be greater than zero for a circle formation.", nameof(slotCount));
            if (radiusMeters <= 0f) throw new ArgumentException("radiusMeters must be greater than zero for a circle formation.", nameof(radiusMeters));

            var slots = new List<FormationSlot>(slotCount);

            float angleStepRadians = Mathf.PI * 2f / slotCount;

            for (int i = 0; i < slotCount; i++)
            {
                float angleRadians = angleStepRadians * i;

                float x = Mathf.Cos(angleRadians) * radiusMeters;
                float z = Mathf.Sin(angleRadians) * radiusMeters;

                Vector3 localOffset = new Vector3(x, 0f, z);
                Vector3 localForward = localOffset.sqrMagnitude > 0f ? localOffset.normalized : Vector3.forward;
                
                var slot = new FormationSlot(
                    slotIndex: i,
                    localOffsetMeters: localOffset,
                    localForwardDirection: localForward
                );

                slots.Add(slot);
            }

            return new FormationDefinition(formationName, slots);
        }
    }
}
