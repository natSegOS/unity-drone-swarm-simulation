using UnityEngine;
using UnityEngine.InputSystem;
using DroneSwarmSimulation.Core.Formation;
using DroneSwarmSimulation.Core.Simulation;

namespace DroneSwarmSimulation.Presentation
{
    /// <summary>
    /// Simple command interface for switching between different formations
    /// at runtime using keyboard input. Intended for debugging and demo use
    /// </summary>
    public class FormationCommandController : MonoBehaviour
    {
        [Header("References")]

        [Tooltip("SwarmManager responsible for managing swarm simulation and blending")]
        [SerializeField]
        private SwarmManager swarmManager;

        [Header("General Settings")]

        [Tooltip("Formation weight to apply when activating a formation (0 = pure flocking, 1 = pure formation)")]
        [Range(0f, 1f)]
        [SerializeField]
        private float targetFormationWeight = 1.0f;

        [Header("Circle Formation Settings")]

        [Tooltip("Radius of the circle formation in meters")]
        [SerializeField]
        private float circleRadiusMeters = 20f;

        [Header("Line Formation Settings")]

        [Tooltip("Spacing between drones in the line formation, in meters")]
        [SerializeField]
        private float lineSpacingMeters = 5f;

        [Header("Arrow Formation Settings")]

        [Tooltip("Total shaft length of the arrow formation, in meters")]
        [SerializeField]
        private float arrowShaftLengthMeters = 40f;

        [Tooltip("Length of each arrow head arm, in meters")]
        [SerializeField]
        private float arrowHeadLengthMeters = 20f;

        [Tooltip("Width of the arrow head at its base, in meters")]
        [SerializeField]
        private float arrowHeadWidthMeters = 30f;

        private void Awake()
        {
            if (swarmManager == null)
            {
                Debug.LogWarning("FormationCommandController has no SwarmManager reference assigned. Formation commands will not function.");
            }
        }

        public void OnDisableFormation(InputAction.CallbackContext context)
        {
            if (swarmManager == null) return;

            SwarmSimulation swarmSimulation = swarmManager.SwarmSimulation;
            if (swarmSimulation == null || swarmSimulation.DroneStates == null) return;

            int droneCount = swarmSimulation.DroneStates.Count;
            if (droneCount == 0) return;

            if (!context.performed) return;

            swarmManager.SetActiveFormation(null);
            swarmManager.SetFormationWeight(0f);
        }

        public void OnCircleFormation(InputAction.CallbackContext context)
        {
            if (swarmManager == null) return;

            SwarmSimulation swarmSimulation = swarmManager.SwarmSimulation;
            if (swarmSimulation == null || swarmSimulation.DroneStates == null) return;

            int droneCount = swarmSimulation.DroneStates.Count;
            if (droneCount == 0) return;

            if (!context.performed) return;

            FormationDefinition formation = CircleFormationGenerator.CreateCircleFormation(
                formationName: "Circle",
                slotCount: droneCount,
                radiusMeters: circleRadiusMeters
            );

            swarmManager.SetActiveFormation(formation);
            swarmManager.SetFormationWeight(targetFormationWeight);
        }

        public void OnLineFormation(InputAction.CallbackContext context)
        {
            if (swarmManager == null) return;

            SwarmSimulation swarmSimulation = swarmManager.SwarmSimulation;
            if (swarmSimulation == null || swarmSimulation.DroneStates == null) return;

            int droneCount = swarmSimulation.DroneStates.Count;
            if (droneCount == 0) return;

            if (!context.performed) return;

            FormationDefinition formation = LineFormationGenerator.CreateLineFormation(
                formationName: "Line",
                slotCount: droneCount,
                spacingMeters: lineSpacingMeters
            );

            swarmManager.SetActiveFormation(formation);
            swarmManager.SetFormationWeight(targetFormationWeight);
        }

        public void OnArrowFormation(InputAction.CallbackContext context)
        {
            if (swarmManager == null) return;

            SwarmSimulation swarmSimulation = swarmManager.SwarmSimulation;
            if (swarmSimulation == null || swarmSimulation.DroneStates == null) return;

            int droneCount = swarmSimulation.DroneStates.Count;
            if (droneCount == 0) return;

            if (!context.performed) return;

            FormationDefinition formation = ArrowFormationGenerator.CreateArrowFormation(
                formationName: "Arrow",
                slotCount: droneCount,
                shaftLengthMeters: arrowShaftLengthMeters,
                headLengthMeters: arrowHeadLengthMeters,
                headWidthMeters: arrowHeadWidthMeters
            );

            swarmManager.SetActiveFormation(formation);
            swarmManager.SetFormationWeight(targetFormationWeight);
        }
    }
}

public class FormationCommandController
{
    
}
