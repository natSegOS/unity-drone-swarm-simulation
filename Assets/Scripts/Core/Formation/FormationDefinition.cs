using System;
using System.Collections.Generic;
using UnityEngine;

namespace DroneSwarmSimulation.Core.Formation
{
    /// <summary>
    /// Represents a full formation as a named, ordered collection of formation slots.
    /// The formation is defined in local space relative to an external anchor
    /// </summary>
    public sealed class FormationDefinition
    {
        /// <summary>
        /// Human-readable name for this formation (e.g., "Circle", "Line", "Arrow")
        /// </summary>
        public string formationName { get; }

        private readonly List<FormationSlot> slots = new List<FormationSlot>();

        /// <summary>
        /// Number of slots defined in this formation
        /// </summary>
        public int SlotCount => slots.Count;

        /// <summary>
        /// Read-only view over all slots in this formation.
        /// Slots are ordered by their slotIndex at construction time
        /// </summary>
        public IReadOnlyList<FormationSlot> Slots => slots;

        /// <summary>
        /// Creates an empty formation definition with the given name.
        /// Slots can be added later via the constructor overloads or factory methods
        /// </summary>
        /// <param name="formationName">Human-readable name for the formation</param>
        /// <exception cref="ArgumentException"></exception>
        public FormationDefinition(string formationName)
        {
            if (string.IsNullOrWhiteSpace(formationName)) throw new ArgumentException("formationName must be a non-empty string.", nameof(formationName));
            this.formationName = formationName;
        }

        /// <summary>
        /// Creates a formation definitino from an existing collection of slots.
        /// Slots are copies and sorted by slotIndex.
        /// </summary>
        /// <param name="formationName">Name of the formation</param>
        /// <param name="sourceSlots">Collection of slots to include in the formation</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FormationDefinition(string formationName, IEnumerable<FormationSlot> sourceSlots) : this(formationName)
        {
            if (sourceSlots == null) throw new ArgumentNullException(nameof(sourceSlots));

            slots.AddRange(sourceSlots);
            slots.Sort((a, b) => a.slotIndex.CompareTo(b.slotIndex));
        }

        /// <summary>
        /// Attempts to get a slot by its index in the formation list
        /// </summary>
        /// <param name="slotIndex">Zero-based index into the slot list</param>
        /// <param name="slot">The slot found at the index, if any</param>
        /// <returns>True if the index was valid and the slot was returned; false otherwise</returns>
        public bool TryGetSlotAt(int slotIndex, out FormationSlot slot)
        {
            if (slotIndex < 0 || slotIndex >= slots.Count)
            {
                slot = default;
                return false;
            }

            slot = slots[slotIndex];
            return true;
        }

        /// <summary>
        /// Returns a slot by index and throws if the index is invalid.
        /// Convenient method for callers that require a slot to exist.
        /// </summary>
        /// <param name="slotIndex">Zero-based index into the slot list</param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public FormationSlot GetSlotAt(int slotIndex)
        {
            if (!TryGetSlotAt(slotIndex, out var slot))
            {
                throw new IndexOutOfRangeException($"No formation slot at index {slotIndex} in formation '{formationName}");
            }

            return slot;
        }
    }
}
