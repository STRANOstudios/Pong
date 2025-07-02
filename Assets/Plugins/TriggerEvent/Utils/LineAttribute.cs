using UnityEngine;

namespace PsychoGarden.Utils
{
    /// <summary>
    /// Attribute to draw a horizontal line in the Unity Inspector.
    /// </summary>
    public class LineAttribute : PropertyAttribute
    {
        public float Thickness { get; private set; }
        public float Padding { get; private set; }
        public Color Color { get; private set; }

        // Default constructor
        public LineAttribute()
        {
            Thickness = 1f;
            Padding = 10f;
            Color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }

        // Constructor with thickness and padding
        public LineAttribute(float thickness, float padding)
        {
            Thickness = thickness;
            Padding = padding;
            Color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }

        // Constructor with Color
        public LineAttribute(float thickness, float padding, float r, float g, float b)
        {
            Thickness = thickness;
            Padding = padding;
            Color = new Color(r, g, b, 1f);
        }

        // Constructor with full Color object
        public LineAttribute(float thickness, float padding, LineColor color)
        {
            Thickness = thickness;
            Padding = padding;
            Color = color == default ? new Color(0.5f, 0.5f, 0.5f, 1f) : GetColorFromEnum(color);
        }

        private Color GetColorFromEnum(LineColor color)
        {
            return color switch
            {
                LineColor.White => Color.white,
                LineColor.Black => Color.black,
                LineColor.Gray => Color.gray,
                LineColor.LightGray => new Color(0.8f, 0.8f, 0.8f),
                LineColor.DarkGray => new Color(0.2f, 0.2f, 0.2f),

                LineColor.Red => Color.red,
                LineColor.LightRed => new Color(1f, 0.5f, 0.5f),
                LineColor.DarkRed => new Color(0.5f, 0f, 0f),

                LineColor.Green => Color.green,
                LineColor.LightGreen => new Color(0.5f, 1f, 0.5f),
                LineColor.DarkGreen => new Color(0f, 0.5f, 0f),

                LineColor.Blue => Color.blue,
                LineColor.LightBlue => new Color(0.5f, 0.5f, 1f),
                LineColor.DarkBlue => new Color(0f, 0f, 0.5f),

                LineColor.Cyan => Color.cyan,
                LineColor.LightCyan => new Color(0.5f, 1f, 1f),
                LineColor.DarkCyan => new Color(0f, 0.5f, 0.5f),

                LineColor.Magenta => Color.magenta,
                LineColor.LightMagenta => new Color(1f, 0.5f, 1f),
                LineColor.DarkMagenta => new Color(0.5f, 0f, 0.5f),

                LineColor.Yellow => Color.yellow,
                LineColor.LightYellow => new Color(1f, 1f, 0.5f),
                LineColor.DarkYellow => new Color(0.5f, 0.5f, 0f),

                LineColor.Orange => new Color(1f, 0.5f, 0f),
                LineColor.LightOrange => new Color(1f, 0.7f, 0.3f),
                LineColor.DarkOrange => new Color(0.6f, 0.3f, 0f),

                LineColor.Brown => new Color(0.4f, 0.26f, 0.13f),
                LineColor.Pink => new Color(1f, 0.75f, 0.8f),
                LineColor.Purple => new Color(0.5f, 0f, 0.5f),
                LineColor.Teal => new Color(0f, 0.5f, 0.5f),
                LineColor.Lime => new Color(0.75f, 1f, 0f),
                LineColor.Indigo => new Color(0.29f, 0f, 0.51f),
                LineColor.Gold => new Color(1f, 0.84f, 0f),
                LineColor.Silver => new Color(0.75f, 0.75f, 0.75f),

                _ => new Color(0.5f, 0.5f, 0.5f, 1f)
            };
        }
    }
}
