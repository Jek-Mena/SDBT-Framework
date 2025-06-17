using System;
using UnityEngine;

/// <summary>
/// [ARCHITECTURAL NOTE -- 2025-06-17]
/// This module is NOT registered globally like other context modules.
/// Instead, it is *inserted at the start* of the builder pipeline
/// via ContextBuilderFactory.CreateWithBtConfig(config) for each entity.
/// 
/// Purpose: Injects the specific ConfigData (BtConfig) for the entity into the blackboard
/// so that all downstream context builder modules and plugins have access to the right config.
/// 
/// DO NOT register this module in ContextModuleRegistration.RegisterAll()!
/// It is only for per-entity usage.
/// </summary>
public class EntityConfigInjectionModule : IContextBuilderModule
{
    private static readonly string _scriptName = nameof(EntityConfigInjectionModule);
    private readonly ConfigData _config;

    public EntityConfigInjectionModule(ConfigData config)
    {
        _config = config ?? throw new ArgumentNullException($"[{_scriptName}] BtConfig is null: {nameof(config)}");
    }

    public void Build(BtContext context)
    {
        var blackboard = context.Blackboard;
        
        if (blackboard == null) 
            throw new ArgumentNullException($"[{_scriptName}] Blackboard is null: {nameof(blackboard)}");

        if (_config != null)
        {
            blackboard.Set(PluginMetaKeys.Core.BtConfig.Plugin, _config);
            Debug.Log($"[{_scriptName}] {PluginMetaKeys.Core.BtConfig.Plugin} injected into blackboard.");
        }
        else
        {
            Debug.LogError($"[{_scriptName}] No {PluginMetaKeys.Core.BtConfig.Plugin} JSON provided. Is {PluginMetaKeys.Core.BtConfig.Plugin} been cleared somewhere or forgotten?");
        }
    }
}