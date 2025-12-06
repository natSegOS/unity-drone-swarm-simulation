using UnityEngine;
using DroneSwarmSimulation.Core.Obstacles;

namespace DroneSwarmSimulation.Presentation
{
    /// <summary>
    /// Unity component that represents a spherical obstacle in the scene
    /// and registers it with the swarm simulation via SwarmManager
    /// </summary>
    [DisallowMultipleComponent]
    public class ObstacleComponent : MonoBehaviour
    {
        [Header("Obstacle Setup")]

        [Tooltip("Optional identifier for this obstacle. If left empty, one will be generated")]
        [SerializeField]
        private string obstacleIdentifier = string.Empty;

        [Tooltip("Radius of the obstacle in meters. Should match the visual representation")]
        [SerializeField]
        private float obstacleRadiusMeters = 10f;

        [Header("References")]

        [Tooltip("Reference to the SwarmManager that manages the swarm simulation. If not assigned, one will be searched for at runtime")]
        [SerializeField]
        private SwarmManager swarmManager;

        private ObstacleDefinition obstacleDefinition;

        private void Awake()
        {
            if (swarmManager == null)
            {
                swarmManager = FindAnyObjectByType<SwarmManager>();

                if (swarmManager == null)
                {
                    Debug.LogWarning($"ObstacleComponent on '{gameObject.name}' does not have a SwarmManager set. Obstacle will not participate in avoidance until a SwarmManager is set.");
                }
            }

            if (string.IsNullOrWhiteSpace(obstacleIdentifier))
            {
                obstacleIdentifier = $"Obstacle_{gameObject.GetInstanceID()}";
            }

            if (obstacleRadiusMeters <= 0f)
            {
                obstacleRadiusMeters = 1f;
            }

            obstacleDefinition = new ObstacleDefinition(
                obstacleIdentifier,
                transform.position,
                obstacleRadiusMeters
            );

            SyncSphereColliderRadius();
        }

        private void OnEnable()
        {
            if (swarmManager != null && obstacleDefinition != null)
            {
                swarmManager.RegisterObstacle(obstacleDefinition);
            }
        }

        private void OnDisable()
        {
            if (swarmManager != null && obstacleDefinition != null)
            {
                swarmManager.UnregisterObstacle(obstacleDefinition);
            }
        }

        private void Update()
        {
            if (obstacleDefinition != null)
            {
                obstacleDefinition.UpdateCenterPosition(transform.position);
            }
        }

        private void OnValidate()
        {
            if (obstacleRadiusMeters <= 0f)
            {
                obstacleRadiusMeters = 1f;
            }

            SyncSphereColliderRadius();
        }

        /// <summary>
        /// If a SphereCollider is attached, sync its radius with the obstacle radius
        /// </summary>
        private void SyncSphereColliderRadius()
        {
            var sphereCollider = GetComponent<SphereCollider>();
            if (sphereCollider == null) return;

            float maxScale = Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            if (maxScale <= 0f)
            {
                maxScale = 1f;
            }

            sphereCollider.radius = obstacleRadiusMeters / maxScale;
        }
    }
}
