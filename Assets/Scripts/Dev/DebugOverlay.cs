using System.Text;
using UnityEngine;

public class DebugOverlay : MonoBehaviour
{
    private BtContext _context;
    private StatusEffectManager _statusEffectManager;

    public void Initialize(BtContext context)
    {
        _context = context;
    }

    public void SetStatusEffectManager(StatusEffectManager manager)
    {
        _statusEffectManager = manager;
    }
    
    void OnGUI()
    {
        if (_context == null) {return;}
        if (!_statusEffectManager) return;

        var sb = new StringBuilder();
        sb.AppendLine($"<b>{gameObject.name}</b>");
        sb.AppendLine($"Movement x {_context.Blackboard.Get<float>(BlackboardKeys.Core.Multipliers.Movement):0.00}");
        sb.AppendLine($"Attack x {_context.Blackboard.Get<float>(BlackboardKeys.Core.Multipliers.Movement):0.00}");
        sb.AppendLine($"Armor x {_context.Blackboard.Get<float>(BlackboardKeys.Core.Multipliers.Movement):0.00}");
        sb.AppendLine("Effects:");
        foreach (var fx in _statusEffectManager.GetActiveEffects())
            sb.AppendLine($"- {fx.GetType().Name} ({fx.Multipliers.Stats.Movement:0.00})");

        // Display near agent in world, or in a static overlay
        GUI.Label(new Rect(100, 10, 250, 200), sb.ToString());
    }
    
}
