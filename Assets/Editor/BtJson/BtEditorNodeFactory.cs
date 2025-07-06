using Keys;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Editor.BtJson
{
    public static class BtEditorNodeFactory
    {
        public static JObject CreateNewNode(string nodeType)
        {
            if (!BtNodeSchemaRegistry.TryGet(nodeType, out var schemaObj))
            {
                Debug.LogError($"No schema found for node type '{nodeType}'");
                return JObject.FromObject(new { type = nodeType });
            }
        
            var config = new JObject();
            foreach (var field in schemaObj.GetFields())
            {
                if (field.DefaultValue != null)
                    config[field.Key] = field.DefaultValue.DeepClone();
            }
        
            var node = new JObject
            {
                [BtJsonFields.Type] = nodeType,
                [BtJsonFields.ConfigField] = config
            };
        
            // Add empty children array if node is composite or decorator
            if (schemaObj.SupportsChildren)
                node[BtJsonFields.Children] = new JArray();

            return node;
        }
    }
}