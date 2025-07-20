namespace Systems.StatusEffectSystem
{
    public interface IBlocker
    {
        bool IsBlocked(string domain);
    }
}