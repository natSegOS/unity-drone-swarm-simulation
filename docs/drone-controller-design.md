# Drone Controller

## Responsible for:
- Living on a GameObject in the scene (the visual drone)
- Owning a DroneState instance
- Initializing that state (identifier, initial position/velocity)
- Calling DroneState.UpdateState each frame
- Applying the updated state to the transform

## Not Responsible for:
- Complex flocking logic (separate systems for that)
- Global configs (eventually moved into DroneConfig)
- Input handling for the whole game
- Spawning or managing other drones (SwarmManager)

