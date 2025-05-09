using System;
using Newtonsoft.Json.Linq;

public class ImpulseMoverNodeFactory : IBtNodeFactory
{
    public IBehaviorNode CreateNode(JObject jObject, Blackboard blackboard, Func<JToken, IBehaviorNode> build)
    {
        // Validate and extract config
        var config = JsonUtils.GetConfig(jObject, nameof(ImpulseMoverNodeFactory));

        // Validate required keys
        JsonUtils.ValidateKeysExist(config,
            JsonKeys.Impulse.ImpulseStrength,
            JsonKeys.Impulse.Tolerance,
            JsonKeys.Impulse.StateTimeout);

        // You could also auto-wire ImpulseMoverData here if you wanted.
        // ^^^^ Refer to the TODO note below.
        return new ImpulseMoverNode(); // No children — it’s a leaf node
    }
}

// TODO: [ImpulseMoverNodeFactory - Data Source Clarification]
//
// ⚠️ This node currently depends on `ImpulseMoverData` being injected
//     into the blackboard by the plugin system.
//
// 📌 Questions to resolve before expanding usage:
// - Who owns the authoritative config? Plugin or per-node?
// - Do we support per-instance override of impulse settings via JSON?
// - Should this node fail fast if `ImpulseMoverData` is missing?
// - Are multiple impulse nodes allowed to exist with different settings?
//
// 🛠 Options to consider:
// [A] Keep plugin as the source of truth ✅
//     - Simpler, more centralized
//     - Ideal if all instances use shared impulse config
//
// [B] Move config wiring into this factory ❌
//     - Enables per-node overrides via JSON
//     - Risk of duplication / drift / conflict
//
// [C] Hybrid: plugin provides default, factory overrides if JSON present
//     - More flexible, more complex
//
// 💡 Recommendation for now:
// → Leave injection to plugin
// → Add fail-fast null check on `ImpulseMoverData`
// → Revisit once feature needs dictate override behavior


/*
TODO: [Self-Describing Config Migration] — Migrate this node to use IJsonConfigurable

CONTEXT:
This node currently uses manual key validation and config extraction (e.g., JsonUtils.ValidateKeysExist, config.Value<T>).
This creates tight coupling in the factory, poor reuse, and scattered schema enforcement.

GOAL:
Implement IJsonConfigurable to make this node self-describing and fully responsible for:
  - Declaring required config keys
  - Parsing its own config
  - Allowing factory code to remain generic and reusable

STEPS:
[ ] 1. Implement IJsonConfigurable interface on this node:
        IEnumerable<string> GetRequiredKeys();
        void LoadConfig(JObject config);

[ ] 2. Move all required config key strings into GetRequiredKeys().
        Example:
          public IEnumerable<string> GetRequiredKeys() => new[] {
              JsonKeys.Impulse.ImpulseStrength,
              JsonKeys.Impulse.Tolerance,
              JsonKeys.Impulse.StateTimeout
          };

[ ] 3. Move config extraction logic into LoadConfig(JObject config).
        Use config.Value<T>(key) or config.GetValue(key).ToObject<T>() for safe extraction.
        Store values in private fields for use in Tick().

[ ] 4. Update corresponding Factory class to:
        - Instantiate the node
        - Call ValidateKeysExist(config, node.GetRequiredKeys())
        - Call node.LoadConfig(config)
        - Return the node

[ ] 5. Delete redundant config key validation and parsing logic from the factory.

OPTIONAL:
[ ] Add validation or default handling logic inside LoadConfig (e.g., clamping or default fallback).
[ ] Write unit tests to ensure that missing or invalid config keys throw descriptive exceptions.

BENEFITS:
- Config structure is co-located with the node behavior.
- No more duplicated key lists or fragile parsing code in factories.
- Enables future tooling (e.g., editor UI or JSON generator) based on declared key structure.
- Scales better as you add more nodes or support modding/dynamic configs.
*/
