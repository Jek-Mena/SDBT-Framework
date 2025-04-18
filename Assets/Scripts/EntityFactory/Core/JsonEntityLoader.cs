using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class JsonEntityLoader
{
    private const string JsonFolderPath = "Data/Units/Enemies/Standard/";
    private readonly Dictionary<string, BaseEntityData> _cache = new();

    public BaseEntityData Load(string fileName)
    {
        if (_cache.TryGetValue(fileName, out var cached))
            return cached;

        var jsonAsset = Resources.Load<TextAsset>($"{JsonFolderPath}{fileName}");
        if (jsonAsset == null)
            throw new System.Exception($"JSON not found at: Resources/{JsonFolderPath}{fileName}.json");

        var data = JsonConvert.DeserializeObject<BaseEntityData>(jsonAsset.text);

        _cache[fileName] = data;
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