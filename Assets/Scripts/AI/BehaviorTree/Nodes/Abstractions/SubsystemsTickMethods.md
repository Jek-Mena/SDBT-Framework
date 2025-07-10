# On Subsystem Tick Methods

## Overview

All time-driven subsystems (e.g., movement, animation, effects) must expose a `void Tick(float deltaTime)` method (or equivalent).

## Rationale: Why Not Return Status?

- **BehaviorTree nodes** (decision/control layer) are required to return status (`BtStatus`) for flow control.
- **Subsystems** (execution/action layer) must not return status; their responsibility is limited to progressing ongoing actions.
- Any status or completion information must be accessed through explicit accessors (e.g., `IsAtDestination()`, `IsPlaying()`, etc.).

## Benefits of This Separation

- **Clean Layering:** Maintains a strict separation between intent/control and action/execution.
- **Reduced Coupling:** Prevents confusion or coupling between different system responsibilities.
- **Extensibility:** Adding new systems is straightforward—implement `Tick(float deltaTime)` and provide the necessary accessors.
