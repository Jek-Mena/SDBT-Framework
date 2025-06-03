using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class TimedExecutionDataBuilder
{
    /// <summary>
    /// Build TimedExecutionData from config.
    /// Handles defaults, validation, and domain-specific quirks.
    /// </summary>
    public static TimedExecutionData FromConfig(JObject config, string context)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config), $"[{context}] Config cannot be null for TimedExecutionData.");

        var data = new TimedExecutionData
        {
            Label = JsonUtils.RequireString(config, BtConfigFields.Common.Label, context),
            Duration = JsonUtils.RequireFloat(config, BtConfigFields.Common.Duration, context),
            StartDelay = JsonUtils.GetFloatOrDefault(config, BtConfigFields.Common.StartDelay, 0, context),
            Interruptible = JsonUtils.GetBoolOrDefault(config, BtConfigFields.Common.Interruptible, true, context),
            FailOnInterrupt = JsonUtils.GetBoolOrDefault(config, BtConfigFields.Common.FailOnInterrupt, true, context),
            ResetOnExit = JsonUtils.GetBoolOrDefault(config, BtConfigFields.Common.ResetOnExit, true, context),
        };

        // Optionally handle a "mode" if present
        if (config.TryGetValue("mode", out var modeToken) && Enum.TryParse(modeToken.ToString(), true, out TimeExecutionMode mode))
            data.Mode = mode;

        if (data.Duration < 0f)
            throw new ArgumentException($"[{context}] 'duration' must be >= 0.");

        return data;
    }
}