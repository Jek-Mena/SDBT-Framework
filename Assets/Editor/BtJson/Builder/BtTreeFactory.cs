using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;

namespace Editor.BtJson.Builder
{
    public static class BtTreeFactory
    {
        public static JObject BuildTree(string preset)
        {
            return preset switch
            {
                "BasicChase" => BtTreeSamples.BasicChase(),
                "MoveAndWait" => BtTreeSamples.MoveAndWait(),
                _ => throw new System.Exception($"Unknown preset: {preset}")
            };
        }
    }
}