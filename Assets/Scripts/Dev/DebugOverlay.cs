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
        if (_context == null) return;
        if (!_statusEffectManager) return;

        var sb = new StringBuilder();
        sb.AppendLine($"<b>{gameObject.name}</b>");
        sb.AppendLine("=== Status Effects ===\n");

        // List all relevant domains you care about
        var allDomains = BlockedDomain.AllDomains;
        
        foreach (var domain in allDomains)
        {
            var isBlocked = _statusEffectManager.IsBlocked(domain);
            sb.AppendLine($"[{domain}]");
            sb.AppendLine(isBlocked ? "  <color=red>BLOCKED</color>" : "  OK");

            // Show ALL effects impacting this domain
            foreach (var fx in _statusEffectManager.GetActiveEffects())
            {
                if (fx.AffectsDomain(domain))
                {
                    var fxName = fx.Name;
                    var mult = fx.Multipliers.Stats.Movement; // Adapt to per-domain if you support it!
                    var remaining = fx.RemainingDuration;
                    sb.AppendLine($"  - {fxName} (x{mult:0.00}) [{remaining:0.0}s]");
                }
            }
            sb.AppendLine();
        }
        // Display the overlay in the game
        GUI.Label(new Rect(10, 10, 350, 400), sb.ToString());
    }

    
}
