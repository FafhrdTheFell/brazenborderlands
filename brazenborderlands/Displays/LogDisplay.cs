using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using term = BearLib.Terminal;

namespace brazenborderlands.Displays
{
    internal class LogDisplay : BorderedDisplay
    {
        private List<string> Entries = new List<string>();
        private List<string> TypesetLines = new List<string>();
        private string EntryBuffer = string.Empty;
        private int _logBorderSpaces;

        public int LogBorderSpaces
        {
            get => _logBorderSpaces + BorderSpaces;
            set => _logBorderSpaces = value;
        }

        public int SpacingAfterParagraph { get; set; }
        public int SpacingAfterLine { get; set; }

        public LogDisplay(int cellsWidth, int cellsHeight, int cellsXOffset, int cellsYOffset) :
            base(cellsWidth, cellsHeight, cellsXOffset, cellsYOffset)
        {
            LogBorderSpaces = 1;
            SpacingAfterParagraph = 1;
            SpacingAfterLine = 0;
        }

        public void AppendEntry(string part)
        {
            EntryBuffer += part;
        }

        public void WriteBufferAsEntry()
        {
            if (EntryBuffer == "")
            {
                return;
            }
            AddEntry(EntryBuffer);
            EntryBuffer = ""; 
        }

        public void AddEntry(string entry)
        {
            Entries.Add(entry);
            List<string> newLines = WrapText(entry, HorizontalBlocks());
            for (int i = 0; i < SpacingAfterParagraph - SpacingAfterLine; i++)
            {
                newLines.Add("");
            }
            TypesetLines.AddRange(newLines);
            Dirty = true;
        }

        public override void Draw()
        {
            if (!Dirty)
            {
                return;
            }
            if (GlobalDirty)
            {
                DrawBorder();
            }
            int linesAdjust = Math.Max(0, TypesetLines.Count - VerticalBlocks());
            while ((linesAdjust > 0) && (TypesetLines[linesAdjust] == "")) 
            {
                // remove blank line
                linesAdjust--;
            }
            int linesToPrint = Math.Min(TypesetLines.Count, VerticalBlocks());
            if (linesAdjust > 0)
            {
                ClearDisplayLayer(0);
                DrawBorder();
            }
            for (int i = 0; i < linesToPrint; i++)
            {
                term.Print(DisplayX(), DisplayY(i), TypesetLines[i + linesAdjust]);
            }

            GlobalDirty = false;
            Dirty = false;
        }

        public int DisplayY(int lineNumber)
        {
            return lineNumber + YOffset + LogBorderSpaces + Consts.YScaleGlyphs * (AddBorder ? 1 : 0);
        }

        public int DisplayX()
        {
            return XOffset + LogBorderSpaces + Consts.XScaleGlyphs *(AddBorder ? 1 : 0);
        }

        public int HorizontalBlocks()
        {
            return CellsWidth - 2 * Consts.XScaleGlyphs * (AddBorder ? 1 : 0) - 2 * LogBorderSpaces;
        }

        public int VerticalBlocks()
        {
            return CellsHeight - 2 * Consts.YScaleGlyphs * (AddBorder ? 1 : 0) - 2 * LogBorderSpaces;
        }


        private List<string> WrapText(string text, int linewidth)
        {
            string[] originalLines = text.Split(new string[] { " " },
                StringSplitOptions.None);

            List<string> wrappedLines = new List<string>();

            StringBuilder actualLine = new StringBuilder();
            int actualWidth = 0;

            foreach (string word in originalLines)
            {
                actualWidth += (word.Length + 1) * Consts.XScaleText;

                if (actualWidth > linewidth)
                {
                    wrappedLines.Add(actualLine.ToString());
                    for (int i = 0; i < Consts.YScaleText - 1 + SpacingAfterLine; i++)
                    {
                        wrappedLines.Add("");
                    }
                    //for (int i = 0; i < SpacingAfterLine; i++)
                    //{
                    //}
                    actualLine.Clear();
                    actualLine.Append(word + " ");
                    actualWidth = word.Length + 1;
                }
                else
                {
                    actualLine.Append(word + " ");
                }
            }

            if (actualLine.Length > 0)
            {
                wrappedLines.Add(actualLine.ToString());
                for (int i = 0; i < Consts.YScaleText - 1 + SpacingAfterLine; i++)
                {
                    wrappedLines.Add("");
                }
            }

                for (int i = 0; i < wrappedLines.Count; i++)
            { System.Console.WriteLine(wrappedLines[i]); }
            return wrappedLines;
        }
    }
}
