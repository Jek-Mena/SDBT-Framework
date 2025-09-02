# SDBT (Stimuli-Driven Behavior Tree) AI Framework

**An exploratory, modular behavior tree framework for Unity developed as part of postgraduate coursework (ITA602). The focus was on runtime flexibility, stimuli-driven switching, and modular agent design, validated in sandbox scenarios rather than full production. While the prototype demonstrated clean separation of perception, decision, and action layers, profiling revealed significant GC allocation spikes from the dictionary-based blackboard. These findings motivate the current direction: refactoring toward a strongly-typed, allocation-free model and exploring DOTS/ECS patterns to achieve scalable, zero-allocation AI at higher agent counts.**

[![Unity Version](https://img.shields.io/badge/Unity-2022.3%2B-blue.svg)](https://unity.com/)
[![C# Version](https://img.shields.io/badge/C%23-9.0%2B-green.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)

## Overview

SDBT is a high-performance, modular behavior tree framework designed for real-time AI systems in Unity. Built with strict separation of concerns, it provides runtime behavior switching, stimuli-driven decision making, and allocation-aware architecture suitable for high-agent-count scenarios.SDBT is an exploratory, modular behavior tree prototype developed in Unity, focusing on runtime flexibility and stimuli-driven decision making. Rather than a finished production system, it serves as a testbed for architectural ideas such as separation of concerns, external configuration, and allocation-aware design, with the long-term aim of scaling to higher agent counts.

- **Composition over Inheritance**: Modular node system with pluggable executors
- **Data-Driven Configuration**: JSON-based behavior trees with runtime modification
- **Performance-First**: Transitioning from Dictionary-based blackboard to strongly-typed, allocation-free model
- **Runtime Transparency**: Live debugging overlays and comprehensive logging
- **Domain Separation**: Clean boundaries between perception, decision, and action layers

## 🏗️ Architecture

### Core Components

```
Scripts/AI/BehaviorTree/
├── Core/                    # Agent definitions and runtime registry
├── Runtime/                 # Blackboard, context, and execution engine
├── Nodes/                   # Behavior tree node implementations
│   ├── Actions/            # Movement, rotation, impulse actions
│   ├── Composites/         # Selector, sequence, parallel nodes
│   ├── Decorators/         # Repeaters, timers, conditions
│   └── Perception/         # Stimulus detection and fear systems
├── Executor/               # Intent-based action execution
├── Registry/               # Factory and service registration
└── Schema/                 # Node validation and configuration
```

### Data Flow

```
JSON Config → BtTreeBuilder → Node Registry → Blackboard Context → Runtime Execution
     ↓              ↓              ↓              ↓              ↓
 Behavior Tree   Factory       Runtime         Shared State    Live Agents
 Definition      Pattern       Nodes           Management      in Scene
```

## 🔧 Features (Exploratory)
> Note: This is not a production-ready framework.  
> The following items are **experimental prototypes** and profiling results from coursework exploration.  
> They represent directions tested, observed issues, and future pivots.

### Runtime Behavior Switching
- Stimuli-Driven Logic   → prototype for dynamic reactions to environmental events
- Persona Switching      → experimental hot-swap of behavior subtrees
- Modulation Curves      → tested sigmoid / Gaussian curves for smoother transitions
- Hysteresis Support     → explored methods to prevent rapid oscillation ("thrashing")

### Performance Experiments
- Allocation-Aware Design → profiling showed GC spikes from Dictionary-based blackboard
- Object Pooling          → early tests on reusing agent/node contexts
- DOTS/ECS Pivot          → findings confirm Mono/GC as a bottleneck → **hard pivot toward ECS & zero-allocation design**
- Memory Profiling        → used Unity Profiler to capture GC stalls and allocation spikes

### Developer Experience Prototypes
- Debug Overlays          → runtime blackboard + BT state visualization for scenario validation
- JSON Configuration      → behavior definitions loaded externally for rapid iteration
- Logging & Traces        → execution flow captured for analysis, not full production telemetry
- Unit Test Harness       → scenario-driven checks, lightweight and academic in scope

## 🎯 Use Cases (Exploratory)

These are **potential applications** identified during development.  
They remain conceptual until the framework matures beyond prototype.

- Game AI            → enemy behaviors, companions, NPC interactions
- Simulation Systems → agent-based modeling with modular decision logic
- Research Projects  → academic experiments with transparent runtime
- Procedural Content → future exploration of emergent or generated behaviors

## ⚡ Performance Findings

> Profiling was part of the academic cycle.  
> Results highlight clear bottlenecks and motivate the **hard pivot toward DOTS/ECS**.

**Observed (Dictionary-based Blackboard):**
- GC pressure noticeable
- Frame drops, allocation spikes

**Profiler Evidence:**
- Allocation spike during agent instantiation
- >6s stall in `UnitySynchronizationContext.ExecuteTasks`
- Repeated spikes from dictionary/list usage during BT updates

**Next Direction:**
- Replace Dictionary-based blackboard with strongly typed / struct-based model
- Embrace zero-allocation execution paths
- Hard pivot toward DOTS/ECS for scalable, allocation-free runtime

## 📚 Documentation

### Architecture Deep-Dive
- **Design Patterns**: Factory, Observer, Strategy, Dependency Injection
- **Separation of Concerns**: Perception → Decision → Action pipeline
- **Configuration System**: Profile-driven agent definitions
- **Runtime Services**: Targeting, movement, status effects

### Migration Guides
- **DOTS/ECS Integration**: Planned migration path from managed to unmanaged
- **Commercial Tool Integration**: NodeCanvas, Behavior Designer compatibility layer
- **Performance Optimization**: Memory management and allocation strategies

## 🛠️ Technical Requirements

**Minimum Requirements:**
- Unity 2022.3+
- .NET Standard 2.1
- Newtonsoft.Json package

**Recommended:**
- Odin Inspector (enhanced debugging)
- Unity DOTS packages (future performance)
- Unity Profiler (performance analysis)

## ⚠️ Academic Project Notice

**This framework was developed as part of --- coursework (Assessments 1–3).**  
It represents an **exploratory proof-of-concept** in modular AI architecture, created under academic timelines and constraints.

**Project Scope & Limitations:**
- Built to explore modular behavior tree design, not as a production tool
- Full game implementation, ML integration, and visual editor tools were out-of-scope
- Performance bottlenecks were observed (GC allocations); DOTS/ECS migration identified as a future direction
- Shared for learning and portfolio purposes, not as a licensed or supported release

**What This Demonstrates:**
- Unity C# development and modular system architecture in practice
- Performance profiling to surface GC allocation issues and motivate refactoring
- An extensible AI framework concept rooted in academic research
- Application of Clean Architecture principles in an exploratory context

## 🎓 Academic Context

This project was part of --- postgraduate coursework and served as an  
**exploratory testbed** for applying software architecture principles to real-time AI.  
The emphasis was on learning, profiling, and documenting trade-offs rather than  
delivering a production-ready system.

**Key Areas Explored:**
- Modular AI Architecture → clean separation between behavior logic and execution
- Runtime System Design   → experiments with dynamic behavior loading and hot-swapping
- Performance Profiling   → GC-aware design, highlighting allocation bottlenecks
- Data-Driven Config      → external JSON definitions with runtime validation
- Debug Tooling           → overlays and logs for transparent runtime inspection

**Skills Demonstrated:**
- Unity C# development in a performance-aware context
- Applying design patterns and clean architecture principles
- Identifying and documenting performance trade-offs in real-time systems
- Bridging academic research with practical prototyping
- Producing technical documentation to support analysis and portfolio use

## 📑 Supporting Academic Reports

This project was developed as part of --- postgraduate coursework.  
Full reports (Assessments 1–3) provide detailed planning, action steps,  
and analysis of findings.

- [Assessment 1: Project Proposal & Planning (PDF)](https://1drv.ms/b/c/943ef67e0dca000b/Ec5r1vorW7RGjmU_eXSbIjIBuPhZnhRYLN11oj2RDVxXnA?e=Z32KqX)
- [Assessment 2: Project Action Plan (PDF)](https://1drv.ms/b/c/943ef67e0dca000b/EYxlDLTHFKVLnmAmddQTiesB155P9R0Uh_gE4sHM47LVXw?e=xNGWmt)
- [Assessment 3: Project Analysis & Findings (PDF)](https://1drv.ms/b/c/943ef67e0dca000b/EVeVyqYApHNNlJoO0WQCKJABkBSaGdXO8gfixAdvMvz4gw?e=js32xV)
- [Assessment 3: Project Presentation Slides (PDF)](https://1drv.ms/b/c/943ef67e0dca000b/EbZrrTrmXeBBkDpoy7eXWA4Bigd3r-7Nr-M8BY92PtEssw?e=ZK8xgn)

*Note: Personal names and identifying details have been redacted to avoid permissions/association issues.*

---