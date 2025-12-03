using UnityEngine;

namespace DroneSwarmSimulation.Core.Formation
{
    /// <summary>
    /// Represents a single slot (target position and orientation) in a formation,
    /// defined in local space relative to a formation anchor
    /// </summary>
    [System.Serializable]
    public readonly struct FormationSlot
    {
        /// <summary>
        /// Logical index of this slot within the formation
        /// Can be used ordering or assignment
        /// </summary>
        public int slotIndex { get; }

        /// <summary>
        /// Local-space offset of thjis slot from the formation anchor, in meters
        /// </summary>
        public Vector3 localOffsetMeters { get; }

        /// <summary>
        /// Local-space forward direction for a drone occupying this slot, normalized
        /// </summary>
        public Vector3 localForwardDirection { get; }

        public FormationSlot(
            int slotIndex,
            Vector3 localOffsetMeters,
            Vector3 localForwardDirection)
        {
            this.slotIndex = slotIndex;
            this.localOffsetMeters = localOffsetMeters;
            
            if (localForwardDirection == Vector3.zero)
            {
                this.localForwardDirection = Vector3.forward;
            }
            else
            {
                this.localForwardDirection = localForwardDirection.normalized;
            }
        }
    }
}
