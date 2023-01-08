using System;
using System.Collections.Generic;
using System.Text;

namespace brazenborderlands
{
    public class Consts
    {


        public static int MapHeight = 60; //100;
        public static int MapWidth = 100; //200;

        public static int WindowWidthPixels = 1280;
        public static int WindowHeightPixels = 720;

        //public static int TermBlockWidthPx = 8;
        //public static int TermBlockHeightPx = 8;
        public static int TermBlockWidthPx = 4;
        public static int TermBlockHeightPx = 4;

        public static int GlyphFontWidthPx = 16;
        public static int GlyphFontHeightPx = 24;

        public static int TextFontWidth = 8;
        public static int TextFontHeight = 16;

        // height and width of term in # of blocks
        public static int TermHeightBlocks = WindowHeightPixels / TermBlockHeightPx; // 90
        public static int TermWidthBlocks = WindowWidthPixels / TermBlockWidthPx; // 160

        private static int TermHeightTenths = TermHeightBlocks / 10;
        private static int TermWidthTenths = TermWidthBlocks / 10;

        public static int LocationDisplayWidth = 6 * TermWidthTenths;
        public static int CharacterDisplayHeight = 4 * TermHeightTenths;

        public static int XScaleGlyphs = GlyphFontWidthPx / TermBlockWidthPx;
        public static int YScaleGlyphs = GlyphFontHeightPx / TermBlockHeightPx;

        public static int XScaleText = TextFontWidth / TermBlockWidthPx;
        public static int YScaleText = TextFontHeight / TermBlockHeightPx;

        public static bool FullScreenDefault = false;
    }
}
