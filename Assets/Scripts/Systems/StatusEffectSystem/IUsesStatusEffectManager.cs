public interface IUsesStatusEffectManager
{
    void SetStatusEffectManager(StatusEffectManager manager);
    void OnDomainBlocked(string domain);
    void OnDomainUnblocked(string domain);
}