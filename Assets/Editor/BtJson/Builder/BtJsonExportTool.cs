using System.IO;
using UnityEditor;

namespace Editor.BtJson.Builder
{
    public static class BtJsonExportTool
    {
        private const string BtSavePath = "Assets/Resources/Data/BTs/";

        [MenuItem("Tools/Behavior Tree/Export MoveToTarget")]
        public static void ExportMoveToTarget()
        {
            var json = BtTreeFactory.BuildTree("BasicChase").ToString();
            File.WriteAllText($"{BtSavePath}basic_chase.json", json);
        }

        [MenuItem("Tools/Behavior Tree/Export MoveAndWait")]
        public static void ExportMoveAndWait()
        {
            var json = BtTreeFactory.BuildTree("MoveAndWait").ToString();
            File.WriteAllText($"{BtSavePath}movewait.json", json);
        }
    }
}