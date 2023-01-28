using System;
using System.Collections.Generic;
using System.Text;
using term = BearLib.Terminal;

namespace brazenborderlands
{
    public class Glyph
    {
        /// <summary>
        /// Character is '[U+E' + [hexadecimal representation of tile]  + ']'.
        /// </summary>
        public string Character { get; set; }
        public string Color { get; set; }
        public string BGCharacter { get; set; }
        public string BGColor { get; set; }
        public bool Outline { get; set; }
        public bool Background { get; set; }
        public Glyph() { }
        public Glyph(string character, string color)
        {
            Character = character;
            Color = color;
        }
        public Glyph(string character, string color, string bgCharacter, string bgColor) : 
            this(character, color, bgCharacter, bgColor, false, false)
        { }
        public Glyph(string character, string color, string bgCharacter, string bgColor, bool outline, bool background)
        {
            Character = character;
            Color = color;
            BGCharacter = bgCharacter;
            BGColor = bgColor;
            Outline = outline;
            Background = background;
        }
        public void DrawAt(int x, int y, bool inFOV)
        {
            DrawAt(x, y, inFOV, 0, 0);
        }
        public void DrawAt(int x, int y, bool inFOV, int offsetX, int offsetY)
        {
            string globalOffset = (offsetX != 0 || offsetY != 0) ? "[offset=" + offsetX.ToString() + "," + offsetY.ToString() + "]" : "";
            string tint = (inFOV ? "light" : "darker");
            if (Background)
            {
                term.Composition(true);
                term.Print(x, y, globalOffset + ColoredString(BGCharacter, BGColor, tint));
            }
            if (Outline)
            {
                term.Composition(true);
                for (int dx = offsetX - 1; dx <= offsetX + 1; dx++)
                {
                    for (int dy = offsetY - 1; dy <= offsetY + 1; dy++)
                    {
                        string offset = "[offset=" + dx.ToString() + "," + dy.ToString() + "]";
                        term.Print(x,y, offset + ColoredString(Character, "Black"));
                    }
                }
            }
            term.Print(x, y, globalOffset + ColoredString(Character, Color, tint));
            if (Outline || Background)
            {
                term.Composition(false);
            }
        }

        private string ColoredString(string text, string color)
        {
            return "[color=" + color + "]" + text + "[/color]";
        }
        private string ColoredString(string text, string color, string tint)
        {
            string tintstring = (tint == "" ? "" : tint + " ");
            return "[color=" + tintstring + color + "]" + text + "[/color]";
        }
    }
}
