public interface IAcceptsSettingsProvider<TSettings>
{
    void SetSettingsProvider(ISettingsProvider<TSettings> provider);
}