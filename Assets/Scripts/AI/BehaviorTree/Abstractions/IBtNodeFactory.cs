using System;
using Newtonsoft.Json.Linq;

public interface IBtNodeFactory
{
    IBehaviorNode CreateNode(JObject jObject, Blackboard blackboard, Func<JToken, IBehaviorNode> build);
}

// =======================================================================
// TODO [Refactor Naming + Introduce Wrapper Type] - Serialized Node Input
//
// 📌 CONTEXT:
// This factory method currently accepts a parameter named `config` or 'jObject' of type JObject.
// However, this is misleading: the object represents an entire **serialized node**
// from a behavior tree, not just a configuration block. It includes:
//
//   - "btKey"     → the identifier for the node type
//   - "config"    → the node's configuration/settings (maybe null or absent)
//   - "children"  → an array of raw subnode definitions (recursive tree parts)
//
// 🧨 PROBLEM:
// - `config` as a name suggests it's only node settings — **false**
// - Nested usage of `config["config"]` is semantically confusing
// - This weak naming leaks abstraction and makes factory contracts unclear
// - As the system scales, it hampers editor tooling, validation, and code generation
//
// 🎯 GOAL:
// Refactor this signature to truthfully reflect what’s being passed,
// and introduce structure that enforces and documents expectations.
//
// ✅ ACTION PLAN:
//
// 1. 🔄 Rename Parameter in All Node Factories
//     - FROM:  `JObject config`
//     - TO:    `JObject treeNodeData` or `JObject serializedNode`
//     - EXTRACT nested config inside method as:
//         `var config = treeNodeData["config"] as JObject;`
//
// 2. 🧱 Introduce a Wrapper Type: `TreeNodeData`
//     ```csharp
//     public class TreeNodeData
//     {
//         public JObject Raw { get; }
//         public string BtKey => Raw["btKey"]?.ToString();
//         public JObject Config => Raw["config"] as JObject;
//         public JArray Children => Raw["children"] as JArray;
//
//         public TreeNodeData(JObject raw) => Raw = raw;
//     }
//     ```
//
//     - Use `TreeNodeData` to clarify intent and provide strong accessors
//     - Encapsulates validation and common parsing logic
//     - Makes editor support and JSON schema validation easier
//
// 3. ✅ Update Factory Signature
//     ```csharp
//     public IBehaviorNode CreateNode(TreeNodeData nodeData, Blackboard blackboard, Func<TreeNodeData, IBehaviorNode> buildChildNode)
//     ```
//
//     - `nodeData` makes role clear: full node input
//     - `buildChildNode` instead of `recurse` clarifies that this is a constructor, not traversal
//
// 📈 FUTURE BENEFITS:
//
// - ✅ Centralizes and standardizes access to serialized structure
// - ✅ Reduces duplication and parsing errors
// - ✅ Enables linting, debugging, and testing of trees outside Unity
// - ✅ Better DX when building visual editors, codegen tools, validators
//
// 🕒 DEFERRED UNTIL:
// - All factories are stable and ready for uniform signature updates
// - You’re ready to make a pass across all node factory implementations
//
// =======================================================================
