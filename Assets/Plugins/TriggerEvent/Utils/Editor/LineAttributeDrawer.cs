using UnityEditor;
using UnityEngine;

namespace PsychoGarden.Utils
{
    /// <summary>
    /// Drawer for LineAttribute to render a horizontal line in the inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(LineAttribute))]
    public class LineAttributeDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            LineAttribute line = (LineAttribute)attribute;

            // Calculate position for the line
            float y = position.y + line.Padding / 2f;
            Rect lineRect = new Rect(position.x, y, position.width, line.Thickness);

            // Draw the line
            EditorGUI.DrawRect(lineRect, line.Color);
        }

        public override float GetHeight()
        {
            LineAttribute line = (LineAttribute)attribute;
            return line.Padding + line.Thickness;
        }
    }
}
