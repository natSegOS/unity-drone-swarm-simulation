# Swarm Simulation

## Purpose
The Swarm Simulation system manages a large set of autonomous drones by:
- Holding a collection of their internal simulation states
- Updating each drone each frame
- Acting as the swarm's root-level simulation context
- Preparing the infrastructure for flocking behaviors (alignment, cohesion, separation)

## Responsibilities
### SwarmSimulation (Core)
- Owns DroneState instances
- Updates drone simulation each frame
- Provides read-only acess to DroneStates
- Ensures deterministic update order

### SwarmManager (Unity / MonoBehaviour)
- Creates and owns SwarmSimulation instances
- Runs the swarm update loop
- Scene-level configuration
- Acts as the bridge between Unity and simulation

## Non-Responsibilities
### SwarmSimulation must not:
- Move Unity transforms
- Interact with MonoBehaviours
- Spawn or destroy GameObjects
- Handle rendering, VFX, or input
- Handle flocking behaviors not explicitly assigned

### SwarmManager must not:
- Implement flocking math
- Modify simulation rules directly
- Hold low-level per-drone logic
- Replace DroneController functionality
- Recreate functionalty already handled by DroneState

