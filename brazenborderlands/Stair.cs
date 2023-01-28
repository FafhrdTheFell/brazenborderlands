using System;
using System.Collections.Generic;
using System.Text;

namespace brazenborderlands
{
    internal class Stair : Furnishing, IPathBetween
    {
        public bool StairDown { get; set; }
        public override bool IsWalkable { get => true; set { } }
        public override bool IsInteractable { get => true; set { } }
        public override bool IsTransparent { get => !StairDown; set { } }
        public int TerminusX { get; set; }
        public int TerminusY { get; set; }
        public int EntranceX { get => this.x; set => this.x = value; }
        public int EntranceY { get => this.y; set => this.y = value; }
        public Stair(bool down)
        {
            this.StairDown = down;
            if (down)
            {
                Glyph.Character = TileFinder.TileGridLookupUnicode(18, 2);
                Glyph.BGCharacter = TileFinder.TileGridLookupUnicode(37, 9);
                Glyph.BGColor = "black";
                Glyph.Background = true;
                Name = "stairs down";
            }
            else
            {
                Glyph.Character = TileFinder.TileGridLookupUnicode(18, 1);
                Name = "stairs up";
            }
            Glyph.Color = "white";
        }
        public Stair() { }
    }
}
