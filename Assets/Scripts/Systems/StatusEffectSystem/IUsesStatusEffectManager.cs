using Systems.StatusEffectSystem.Component;

public interface IUsesStatusEffectManager
{
    void OnDomainBlocked(string domain);
    void OnDomainUnblocked(string domain);
    void Dispose();
}