using System;
using System.Collections.Generic;
using System.Text;
using term = BearLib.Terminal;

namespace brazenborderlands
{
    public class Glyph
    {
        public string Character { get; set; }
        public string Color { get; set; }
        public string BGCharacter { get; set; }
        public string BGColor { get; set; }
        public Glyph() { }
        public Glyph(string character, string color)
        {
            Character = character;
            Color = color;
        }
        public Glyph(string character, string color, string bgCharacter, string bgColor)
        {
            Character = character;
            Color = color;
            BGCharacter = bgCharacter;
            BGColor = bgColor;
        }
        public void DrawAt(int x, int y, bool inFOV)
        {
            string tint = (inFOV ? "light" : "darker");
            if (BGColor != "")
            {
                term.Print(x, y, ColoredString(BGCharacter, BGColor, tint));
                term.Layer(termLayer()+1);
                term.Print(x, y, ColoredString(Character, Color, tint));
                term.Layer(termLayer()-1);
            }
            else
            {
                term.Print(x, y, ColoredString(Character, Color, tint));
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
       private int termLayer()
        {
            return term.State(term.TK_LAYER);
        }
    }
}
