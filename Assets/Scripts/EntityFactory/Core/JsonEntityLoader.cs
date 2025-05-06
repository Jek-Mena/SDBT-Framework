using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class JsonEntityLoader
{
    private const string JsonFolderPath = "";
    //private const string JsonFolderPath = "Data/Units/Enemies/Standard/";
    private readonly Dictionary<string, BaseEntityData> _cache = new();

    public BaseEntityData Load(string path)
    {
        if (_cache.TryGetValue(path, out var cached))
            return cached;

        var jsonAsset = Resources.Load<TextAsset>($"{JsonFolderPath}{path}");
        if (jsonAsset == null)
            throw new System.Exception($"JSON not found at: Resources/{JsonFolderPath}{path}.json");

        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new ComponentEntryObjectConverter());

        var data = JsonConvert.DeserializeObject<BaseEntityData>(jsonAsset.text, settings);

        _cache[path] = data;
        return data;
    }

    private BaseEntityData Merge(BaseEntityData parent, BaseEntityData child)
    {
        var result = JsonConvert.DeserializeObject<BaseEntityData>(
            JsonConvert.SerializeObject(parent)
        );

        result.id = child.id ?? parent.id;
        result.prefab= child.prefab ?? parent.prefab;

        if (child.components!= null)
            result.components.AddRange(child.components);

        return result;
    }
}