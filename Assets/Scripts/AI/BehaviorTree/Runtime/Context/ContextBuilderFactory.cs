using Newtonsoft.Json.Linq;

public static class ContextBuilderFactory
{
    public static IContextBuilder CreateWithBtConfig(ConfigData config)
    {
        if (BtServices.ContextBuilder is not BtBlackboardBuilder baseBuilder)
            throw new System.Exception("BtServices.ContextBuilder must be BtBlackboardBuilder.");

        var builder = baseBuilder.Clone();
        builder.InsertModuleAtStart(new BtConfigContextBuilderModule(config));
        return builder;
    }
}