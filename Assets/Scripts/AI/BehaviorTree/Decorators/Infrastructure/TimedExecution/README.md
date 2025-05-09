# TimedExecution Infrastructure

This module provides runtime infrastructure for time-based behavior tree execution.
It supports decorators and actions like Pause, Timeout, and Cooldown by tracking duration,
interruptibility, and execution policies.

Used in:
- PauseNode (Action)
- TimeoutDecorator (Decorator)
- CooldownDecorator (Decorator)