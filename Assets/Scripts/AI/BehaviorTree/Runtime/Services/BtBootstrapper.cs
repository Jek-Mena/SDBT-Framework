using UnityEngine;

public static class BtBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        // Call to load/caches all assets/configs
        GameAssets.Bootstrap("Data/Units/Enemies/Standard", "Prefabs/Enemies/Standard");
        // or pass multiple folders, or call multiple times for different entity types.
        
        // Register all templates
        // Loads all JSON templates from the Resources folder and registers them with the BtRegistry.
        BtConfigRegistrationList.Initialize();
        
        // Register all node factories
        // Initializes the Behavior Tree system by setting up default nodes,
        BtNodeRegistrationList.Initialize();
        
        // Set up and register context builder and context modules
        var btBlackboardBuilder = new BtContextBuilder();
        ContextModuleRegistrationList.RegisterAll(btBlackboardBuilder);
    }
}

