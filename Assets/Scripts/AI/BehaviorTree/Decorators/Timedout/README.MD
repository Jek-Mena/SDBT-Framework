# TimeoutDecoratorNode

This folder contains the implementation for a behavior tree decorator that limits the execution time of its child node.

## Purpose

`TimeoutDecoratorNode` ensures that its child node has a limited time window to return success. If the child runs longer than the configured duration, the decorator forces completion by returning `Success`.

Useful for:
- Interrupting long-running tasks (e.g., movement, animation, interaction)
- Failsafes for non-terminating child nodes
- Timeboxing conditional behaviors

## Components

- **TimeoutDecoratorNode.cs**  
  Implements the decorator node. Delegates timing to `ITimedExecutionNode`.

- **TimeoutDecoratorNodeFactory.cs**  
  Responsible for constructing the node from JSON configuration. Injects duration and key.

- **TimeoutDecoratorNodePlugin.cs**  
  Runtime plugin for injecting `TimedExecutionData` and hooking into the blackboard.

## Depends On

- `TimedExecutionMono` or any runtime implementation of `ITimedExecutionNode`
- Config data: `TimedExecutionData` (key, duration, interrupt flags)

## Behavior

- Starts a timer on first tick.
- Ticks child each frame.
- Returns `Success` when either:
  - The child returns `Success`
  - The duration has elapsed
