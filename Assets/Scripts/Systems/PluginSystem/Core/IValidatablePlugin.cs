public interface IValidatablePlugin
{
    /// <summary>
    /// Validates the plugin's config payload at runtime.
    /// </summary>
    /// <param name="entry">The ComponentEntry config block.</param>
    void Validate(ComponentEntry entry);
}