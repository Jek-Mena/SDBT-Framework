// Place in a Drawing.cs utility file

using UnityEngine;

namespace Dev.OverlayDebugView
{
    // TODO rename Drawing to OverlayMatrixDraw
    public static class Drawing
    {
        private static Texture2D _tex;
        public static void DrawLine(Vector2 from, Vector2 to, Color color, float width = 2f)
        {
            if (_tex == null) {
                _tex = new Texture2D(1, 1);
                _tex.SetPixel(0, 0, Color.white);
                _tex.Apply();
            }
            Matrix4x4 matrix = GUI.matrix;
            Color savedColor = GUI.color;

            GUI.color = color;
            float angle = Vector3.Angle(to - from, Vector2.right);
            if (from.y > to.y) angle = -angle;
            GUIUtility.RotateAroundPivot(angle, from);
            GUI.DrawTexture(new Rect(from.x, from.y, (to - from).magnitude, width), _tex);
            GUI.matrix = matrix;
            GUI.color = savedColor;
        }
    }
}