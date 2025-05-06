public interface ISettingsProvider<T>
{
    T GetEffectiveSettings();
}