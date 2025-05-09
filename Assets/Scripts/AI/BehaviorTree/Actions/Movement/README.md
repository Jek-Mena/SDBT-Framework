# 🧠 Behavior Tree: Movement Domain

This folder contains all Behavior Tree logic related to **movement behaviors** — including node definitions, plugin integrations, movement execution components, and supporting data structures.

---

## 🔧 Purpose

To isolate all logic that controls agent navigation, directional movement, and pathfinding-driven actions within Behavior Trees.

---

## 📦 File Overview

| File                     | Role                                          |
|--------------------------|-----------------------------------------------|
| `MoveToTargetNode.cs`    | 🟩 BT Leaf Node — triggers movement behavior toward a target |
| `MoveNodeFactory.cs`     | 🛠️ Factory for constructing `MoveToTargetNode` from JSON config |
| `NavMeshMover.cs`        | 🧱 Runtime executor — applies navmesh-based movement logic |
| `NavMeshMoverPlugin.cs`  | 🔌 Plugin that wires movement logic into spawned entities |
| `MovementData.cs`        | 📦 Serializable data config for movement properties |
| `IMovementNode.cs`       | 📐 Interface for BT nodes that issue movement commands |

---

## 🧠 Notes

- This node is a **Leaf** node — it triggers behavior, but does not control flow.
- It is expected to run **alongside** logic like `Timeout`, `Pause`, `Impulse`, etc.
- Movement is **domain-isolated** — no other action (e.g. impulse or animation) should modify navmesh directly.

---

## 📏 Design Principles

- **Keep behavior + runtime logic close**: All classes that touch movement live here.
- **Avoid hardcoded strings**: Use `JsonLiterals` for keys and config.
- **Fail-fast loading**: `NavMeshMoverPlugin` should throw if dependencies are missing.
- **Domain-first grouping**: Don’t split into `Leaves/` or `Factories/` unless complexity demands it.

---

## ✅ TODO / Future Enhancements

- [ ] Add `FleeNode` or `WanderNode` using the same NavMesh pipeline
- [ ] Support movement cancellation (interrupts) in `NavMeshMover`
- [ ] Add `MovementStateVisualizer` for debug mode
- [ ] Validate direction source (`Blackboard`, `Player`, `Target`) more robustly

---
