using System;
using System.Collections.Generic;
using AI.BehaviorTree.Runtime.Context;

public class BtValidator
{
    private const string ScriptName = nameof(BtValidator);  
    private readonly BtContext _context;
    private readonly List<string> _errors = new();
    
    public static BtValidator Require(BtContext context) => new(context);
    private BtValidator(BtContext context) => _context = context;

    public BtValidator Movement()
    {
        if (_context.Movement == null)
            _errors.Add($"[{ScriptName}] Movement logic missing.");
        return this;
    }

    public BtValidator Rotation()
    {
        if (_context.Rotation == null)
            _errors.Add($"[{ScriptName}] Rotation logic missing.");
        return this;
    }

    public BtValidator Targeting(string targetProfileKey)
    {
        // Check profile exists in the blackboard's TargetingProfiles
        if (_context.AgentProfiles.TargetingProfiles == null)
        {
            _errors.Add($"[{ScriptName}] TargetingProfiles dictionary missing.");
            return this;
        }
        if (!_context.AgentProfiles.TargetingProfiles.TryGetValue(targetProfileKey, out var targetingData))
        {
            _errors.Add($"[{ScriptName}] TargetingProfile '{targetProfileKey}' missing.");
            return this;
        }

        // Check style is present in registry
        if (string.IsNullOrWhiteSpace(targetingData.Style.ToString()))
        {
            _errors.Add($"[{ScriptName}] TargetingProfile '{targetProfileKey}' has no style.");
            return this;
        }
        if (!TargetResolverRegistry.TryGetValue(targetingData.Style, out var resolver))
        {
            _errors.Add($"[{ScriptName}] Targeting style '{targetingData.Style}' for profile '{targetProfileKey}' missing from registry.");
        }
        return this;
    }

    public BtValidator Timers()
    {
        if (!_context.TimeExecutionManager)
            _errors.Add($"[{ScriptName}] Timer system missing.");
        return this;
    }

    public BtValidator RequireChild(IBehaviorNode child)
    {
        if (child == null)
            _errors.Add($"[{ScriptName}] Child node is missing.");
        return this;
    }

    public BtValidator Children(List<IBehaviorNode> children, int min = 1)
    {
        if (children == null || children.Count < min)
        {
            _errors.Add($"[{ScriptName}] Missing or invalid children. Expected at least {min}, got {children?.Count ?? 0}.");
        }
        return this;
    }
    
    public BtValidator Effects()
    {
        if (!_context.Blackboard.StatusEffectManager)
            _errors.Add($"[{ScriptName}] StatusEffectManager missing.");
        return this;
    }
    
    public BtValidator Health()
    {
        throw new Exception("Health Validator is not yet implemented.");
    }

    public BtValidator Blackboard()
    {
        if (_context.Blackboard == null)
            _errors.Add($"[{ScriptName}] Blackboard missing.");
        return this;
    }

    public BtValidator KeyExists<T>(string key)
    {
        if (_context.Blackboard == null)
        {
            _errors.Add($"[{ScriptName}] Blackboard is null.");
            return this;
        }

        if (!_context.Blackboard.TryGet<T>(key, out _))
        {
            _errors.Add($"[{ScriptName}] Blackboard is missing required key '{key}' of type {typeof(T).Name}.");
        }

        return this;
    }

    
    public bool Check(out string error)
    {
        error = _errors.Count > 0 ? string.Join("\n", _errors) : null;
        return _errors.Count == 0;
    }
}


public static class BtValidatorExtensions
{
    public static BtValidator RequireChild(this BtValidator validator, IBehaviorNode child)
        => validator.RequireChild(child);
}
