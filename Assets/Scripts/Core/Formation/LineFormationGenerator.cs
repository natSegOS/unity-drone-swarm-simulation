using System;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;

namespace DroneSwarmSimulation.Core.Formation
{
    /// <summary>
    /// Provides factory methods for creating line formations.
    /// Slots are arranged along the local X axis in the XZ plane,
    /// centered around the formation anchor
    /// </summary>
    public static class LineFormationGenerator
    {
        public static FormationDefinition CreateLineFormation(
            string formationName,
            int slotCount,
            float spacingMeters)
        {
            if (slotCount <= 0) throw new ArgumentException("slotCount must be greater than zero for a line formation.", nameof(slotCount));
            if (spacingMeters <= 0f) throw new ArgumentException("spacingMeters must be greater than zero for a line formation.", nameof(spacingMeters));

            var slots = new List<FormationSlot>(slotCount);
            float totalWidth = spacingMeters * (slotCount - 1);

            float startX = -totalWidth * 0.5f;
            
            for (int i = 0; i < slotCount; i++)
            {
                float x = startX + i * spacingMeters;
                Vector3 localOffset = new Vector3(x, 0f, 0f);
                Vector3 localForward = Vector3.forward;

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
