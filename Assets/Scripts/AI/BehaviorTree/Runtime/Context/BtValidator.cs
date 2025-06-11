using System;
using System.Collections.Generic;

public class BtValidator
{
    public static BtValidator Require(BtContext context) => new(context);
    private BtValidator(BtContext context) => _context = context;
    
    private readonly BtContext _context;
    private readonly List<string> _errors = new();

    public BtValidator Movement()
    {
        if (_context.Movement == null)
            _errors.Add("[BtValidator] Movement logic missing.");
        return this;
    }

    public BtValidator Rotation()
    {
        if (_context.Rotation == null)
            _errors.Add("[BtValidator] Rotation logic missing.");
        return this;
    }

    public BtValidator Targeting()
    {
        if (_context.TargetingData == null)
            _errors.Add("[BtValidator] TargetingData missing.");
        if (_context.TargetResolver == null)
            _errors.Add("[BtValidator] TargetResolver missing.");
        return this;
    }

    public BtValidator Timers()
    {
        if (!_context.Timers)
            _errors.Add("[BtValidator] Timer system missing.");
        return this;
    }

    public BtValidator RequireChild(IBehaviorNode child)
    {
        if (child == null)
            _errors.Add("[BtValidator] Child node is missing.");
        return this;
    }

    public BtValidator Children(List<IBehaviorNode> children, int min = 1)
    {
        if (children == null || children.Count < min)
        {
            _errors.Add($"[BtValidator] Missing or invalid children. Expected at least {min}, got {children?.Count ?? 0}.");
        }
        return this;
    }
    
    public BtValidator Effects()
    {
        if (!_context.Blackboard?.StatusEffectManager)
            _errors.Add("[BtValidator] StatusEffectManager missing.");
        return this;
    }
    
    public BtValidator Health()
    {
        throw new Exception("Health Validator is not yet implemented.");
    }

    public BtValidator Blackboard()
    {
        if (_context.Blackboard == null)
            _errors.Add("[BtValidator] Blackboard missing.");
        return this;
    }

    public BtValidator KeyExists<T>(string key)
    {
        if (_context.Blackboard == null)
        {
            _errors.Add("[BtValidator] Blackboard is null.");
            return this;
        }

        if (!_context.Blackboard.TryGet<T>(key, out _))
        {
            _errors.Add($"[BtValidator] Blackboard is missing required key '{key}' of type {typeof(T).Name}.");
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
