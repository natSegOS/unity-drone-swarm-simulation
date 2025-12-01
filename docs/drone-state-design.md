# DroneState

## Responsible for:
- Tracking position in world space
- Tracking velocity
- Tracking orientation
- Tracking ID
- Offering a method that updates position based on velocity and time step

## Not Responsible for:
- Reading Unity input
- Moving transforms directly
- Handling Update() / FixedUpdate() (MonoBehaviour jobs)
- Spawning prefabs, playing sounds, spawning VFX
- Knowing about Unity components at all
