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
            DrawAt(x, y, inFOV, false, 0, 0);
        }
        public void DrawAt(int x, int y, bool inFOV, bool highlighted)
        {
            DrawAt(x, y, inFOV, highlighted, 0, 0);
        }
        public void DrawAt(int x, int y, bool inFOV, bool highlighted, int offsetX, int offsetY)
        {
            string globalOffset = (offsetX != 0 || offsetY != 0) ? "[offset=" + offsetX.ToString() + "," + offsetY.ToString() + "]" : "";
            // Possible values for brightness are: lightest, lighter, light, dark, darker, darkest.
            string tint = (highlighted ? "lightest" : (inFOV ? "light" : "darker"));
            if (Background)
            {
                term.Composition(true);
                term.Print(x, y, globalOffset + ColoredString(BGCharacter, BGColor, tint));
            }
            if (Outline)
            {
                term.Composition(true);
                if (highlighted) SmudgePrint(x, y, offsetX, offsetY, 2, "yellow");
                SmudgePrint(x, y, offsetX, offsetY, 1, "black");
            }
            term.Print(x, y, globalOffset + ColoredString(Character, Color, tint));
            if (Outline || Background)
            {
                term.Composition(false);
            }
        }
        private void SmudgePrint(int x, int y, int offsetX, int offsetY, int smudgeSize, string smudgeColor)
        {
            for (int dx = offsetX - smudgeSize; dx <= offsetX + smudgeSize; dx++)
            {
                for (int dy = offsetY - smudgeSize; dy <= offsetY + smudgeSize; dy++)
                {
                    string offset = "[offset=" + dx.ToString() + "," + dy.ToString() + "]";
                    term.Print(x, y, offset + ColoredString(Character, smudgeColor));
                }
            }
        }
        private string ColoredString(string text, string color)
        {
            return "[color=" + color + "]" + text + "[/color]";
        }
        private string ColoredString(string text, string color, string tint)
        {
            string tintstring = (tint == "" ? "" : tint + " ");
            // seems to be a bug using "lightest" brightness with "Slate" color -- manually
            // adjust RGB numbers to be brighter instead. Slate is 105,101,106
            if (tint == "lightest" && color == "Slate") color = "142,136,143"; 
            return "[color=" + tintstring + color + "]" + text + "[/color]";
        }
    }
}
