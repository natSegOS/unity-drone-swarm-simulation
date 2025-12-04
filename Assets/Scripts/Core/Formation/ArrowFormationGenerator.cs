using System;
using System.Collections.Generic;
using UnityEngine;

namespace DroneSwarmSimulation.Core.Formation
{
    /// <summary>
    /// Provides factory methods for creating arrow-shaped formations.
    /// The arrow points along the local +Z ais and lines in the XZ plane
    /// </summary>
    public static class ArrowFormationGenerator
    {
        public static FormationDefinition CreateArrowFormation(
            string formationName,
            int slotCount,
            float shaftLengthMeters,
            float headLengthMeters,
            float headWidthMeters)
        {
            if (slotCount < 3) throw new ArgumentException("slotCount must be at least 3 for an arrow formation.", nameof(slotCount));
            if (shaftLengthMeters <= 0f) throw new ArgumentException("shaftLengthMeters must be greater than zero for an arrow formation.", nameof(shaftLengthMeters));
            if (headLengthMeters <= 0f) throw new ArgumentException("headLengthMeters must be greater than zero for an arrow formation", nameof(headLengthMeters));
            if (headWidthMeters <= 0f) throw new ArgumentException("headWidthMeters must be greater than zero for an arrow formation.", nameof(headWidthMeters));

            var slots = new List<FormationSlot>(slotCount);

            const int tipSlotCount = 1;
            int remainingSlots = slotCount - tipSlotCount;

            int shaftSlots = Mathf.Max(1, remainingSlots / 3);
            int headSlotsTotal = remainingSlots - shaftSlots;

            if (headSlotsTotal < 2 && remainingSlots >= 2)
            {
                headSlotsTotal = 2;
                shaftSlots = remainingSlots - headSlotsTotal;
            }

            int headSlotsPerSide = headSlotsTotal / 2;
            int extraHeadSlot = headSlotsTotal % 2;

            int slotIndex = 0;

            // Tip slot
            Vector3 tipOffset = new Vector3(0f, 0f, 0.05f * headLengthMeters);
            Vector3 tipForward = Vector3.forward;

            slots.Add(new FormationSlot(
                slotIndex: slotIndex++,
                localOffsetMeters: tipOffset,
                localForwardDirection: tipForward
            ));

            // Shaft slot
            if (shaftSlots > 0)
            {
                float shaftStep = shaftLengthMeters / shaftSlots;

                for (int i = 0; i < shaftSlots; i++)
                {
                    float z = -shaftStep * (i + 1);

                    Vector3 shaftOffset = new Vector3(0f, 0f, z);
                    Vector3 shaftForward = Vector3.forward;

                    slots.Add(new FormationSlot(
                        slotIndex: slotIndex++,
                        localOffsetMeters: shaftOffset,
                        localForwardDirection: shaftForward
                    ));
                }
            }

            // Head slots
            if (headSlotsTotal > 0)
            {
                Vector3 leftArmDirection = new Vector3(-headWidthMeters, 0f, -headLengthMeters).normalized;
                Vector3 rightArmDirection = new Vector3(headWidthMeters, 0f, -headLengthMeters).normalized;

                int segmentsPerArm = Mathf.Max(1, headSlotsPerSide);
                float armStep = headLengthMeters / segmentsPerArm;

                if (extraHeadSlot > 0)
                {
                    Vector3 extraOffset = new Vector3(0f, 0f, 0.5f * headLengthMeters);
                    slots.Add(new FormationSlot(
                        slotIndex: slotIndex++,
                        localOffsetMeters: extraOffset,
                        localForwardDirection: Vector3.forward
                    ));
                }

                // left arm
                for (int i = 0; i < headSlotsPerSide; i++)
                {
                    float distanceAlongArm = armStep * (i + 1);
                    Vector3 offset = leftArmDirection * distanceAlongArm;

                    slots.Add(new FormationSlot(
                        slotIndex: slotIndex++,
                        localOffsetMeters: offset,
                        localForwardDirection: Vector3.forward
                    ));
                }

                // right arm
                for (int i = 0; i < headSlotsPerSide; i++)
                {
                    float distanceAlongArm = armStep * (i + 1);
                    Vector3 offset = rightArmDirection * distanceAlongArm;

                    slots.Add(new FormationSlot(
                        slotIndex: slotIndex++,
                        localOffsetMeters: offset,
                        localForwardDirection: Vector3.forward
                    ));
                }
            }

            return new FormationDefinition(formationName, slots);
        }
    }
}
