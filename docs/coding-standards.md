# Coding Standards

## Naming

- Classes, structs, enums, interfaces: PascalCase (e.g., DroneState, SwarmController)
- Interfaces prefixed with "I" (e.g., IDroneSteeringBehavior)
- Methods: PascalCase (e.g., UpdateState, ApplyFlocking)
- Fields and local variables: camelCase (e.g., droneMaximumSpeedMetersPerSecond)

## Magic Numbers

- Do not use "magic numbers" directly in logic
- Define constants or use ScriptableObject configuration objects (e.g., DroneConfig) for tunable values such as speeds, distances, and time thresholds

## Unity-Specific

- Simulation logic should live in Core classes that do not inherit from MonoBehavior
- MonoBehaviours should be responsible for:
    - Unity lifecycle (Start, Update, etc.)
    - Reading inputs
    - Updating transforms, visuals, and UI

## Formatting

- Use consistent indentation (4 spaces or tabs and stick with it)
- Keep methods short and focused on a single responsibility

