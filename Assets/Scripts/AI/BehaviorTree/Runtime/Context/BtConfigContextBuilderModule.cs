using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

public class BtConfigContextBuilderModule : IContextBuilderModule
{
    private readonly ConfigData _config;

    public BtConfigContextBuilderModule(ConfigData config)
    {
        _config = config ?? throw new ArgumentNullException($"[ConfigContextBuilder] BtConfig is null: {nameof(config)}");
    }

    public void Build(GameObject entity, Blackboard blackboard)
    {
        if (blackboard == null) 
            throw new ArgumentNullException($"[ConfigContextBuilder] Blackboard is null: {nameof(blackboard)}");

        if (_config != null)
        {
            blackboard.Set(PluginMetaKeys.Core.BtConfig.Plugin, _config);
            Debug.Log("[ConfigContextBuilder] BtConfig injected into blackboard.");
        }
        else
        {
            Debug.LogError("[ConfigContextBuilder] No BtConfig JSON provided. Is BtConfig been cleared somewhere or forgotten?");
        }
    }
}