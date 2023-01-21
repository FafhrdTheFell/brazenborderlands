using System;
using System.Collections.Generic;
using System.Text;

namespace brazenborderlands
{
    internal class PlayerDisplay : TabularDisplay
    {
        public PlayerDisplay(int cellsWidth, int cellsHeight, int cellsXOffset, int cellsYOffset) :
            base(cellsWidth, cellsHeight, cellsXOffset, cellsYOffset)
        {
            YLines = EffectiveCellsHeight() / Consts.YScaleText;
            XColumns = 4;
            XTabs = new List<int> { 0, 
                (int)((float)EffectiveCellsWidth()*0.2),
                (int)((float)EffectiveCellsWidth()*0.4),
                (int)((float)EffectiveCellsWidth()*0.775)
            };
            //YTabs = new List<int> { };
            //for (int i = 0; i < EffectiveCellsHeight(); i+= Consts.YScaleText)
            //{
            //    YTabs.Add(i);
            //}
            RowsAutoSpacing();
            InitContents();
            //Contents = new string[XColumns, YLines];

        }

        public override void Draw()
        {
            if (!Dirty)
            {
                return;
            }
            if (GlobalDirty)
            {
                InitContents();
            }
            Contents[0, 0] = Program.player.Name;
            Contents[2, 0] = Program.player.Species + " " + Program.player.Class + " (lvl " + Program.player.Level.ToString() + ")";
            Contents[3, 0] = "H: " + Program.player.Health + "   ";
            Contents[0, 2] = "Brawn";
            Contents[1, 2] = Program.player.Brawn().ToString();
            Contents[0, 3] = "Reflexes";
            Contents[1, 3] = Program.player.Reflexes().ToString();
            Contents[0, 4] = "Ego";
            Contents[1, 4] = Program.player.Ego().ToString();
            Contents[0, 6] = "Acc / Dam";
            Contents[1, 6] = Program.player.MeleeAttack().Accuracy(Program.player).ToString() + " / " +
                Program.player.MeleeAttack().Damage(Program.player).ToString() + "  ";
            Contents[0, 7] = "Def / Soak";
            Contents[1, 7] = Program.player.Defense().ToString() + " / " +
                Program.player.Soak().ToString() + "  ";
            Contents[2, 2] = "Inventory";
            Contents[2, 3] = "------------";
            for (int i = 0; i < Math.Min(Program.player.Inventory.NumItems(), YLines - 5); i++)
            {
                Contents[2, i + 4] = Program.player.Inventory.Items[i].NameString();
                //Contents[3, i + 4] = Program.player.Inventory.Items[i].IsEquipped ? 
                //    Enum.GetName(typeof(EquipmentSlot), Program.player.Inventory.Items[i].Slot) : "             ";
            }
            if (Program.player.Inventory.NumItems() > YLines - 5)
            {
                Contents[2, YLines - 1] = "...";
            }
            else
            {
                Contents[2, YLines - 1] = "   ";
            }
            base.Draw();
        }
    }
}
