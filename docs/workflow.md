# Workflow Overview

## Jira

- All work is represented as Jira issues (Tasks/Stories)
- Issues belong to an Epic representing the phase (e.g., Phase 0 - Project Setup)
- Each phase is planned as part of a sprint
- Issue status flow: To Do -> In Progress -> In Review -> Done

## Git & GitHub

- Default branches
    - `main` -- stable, release-ready code
    - `develop` -- integration branch for completed features
- Feature branches:
    - Named as `feature/<JiraID>-short-description`
    - Example: `feature/DSS-4-document-workflow`
- Commit messages
    - Begin with Jira ID, e.g., `DSS-2: add initial Unity project and base structure`

## Unity

- Project name: DroneSwarmSimulation
- Core folder structure under `Assets/`:
    - `Scripts/Core` -- pure logic and data structures
    - `Scripts/Presentation` -- MonoBehaviours and view logic
- Main scene: `Assets/Scenes/Main.unity`
