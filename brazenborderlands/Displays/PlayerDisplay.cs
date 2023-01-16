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
            Rows = EffectiveCellsHeight() / Consts.YScaleText;
            Columns = 4;
            ColumnTabs = new List<int> { 0, 
                (int)((float)EffectiveCellsWidth()*0.2),
                (int)((float)EffectiveCellsWidth()*0.4),
                (int)((float)EffectiveCellsWidth()*0.775)
            };
            RowTabs = new List<int> { };
            for (int i = 0; i < EffectiveCellsHeight(); i+= Consts.YScaleText)
            {
                RowTabs.Add(i);
            }
            Contents = new string[Columns, Rows];

        }

        public override void Draw()
        {
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
            Contents[3, 2] = "Usage";
            Contents[3, 3] = "--------";
            for (int i = 0; i < Math.Min(Program.player.Inventory.NumItems(), Rows - 5); i++)
            {
                Contents[2, i + 4] = Program.player.Inventory.Items[i].Name;
                Contents[3, i + 4] = Program.player.Inventory.Items[i].IsEquipped ? 
                    Enum.GetName(typeof(EquipmentSlot), Program.player.Inventory.Items[i].Slot) : "             ";
            }
            if (Program.player.Inventory.NumItems() > Rows - 5)
            {
                Contents[2, Rows - 1] = "...";
            }
            else
            {
                Contents[2, Rows - 1] = "   ";
            }
            base.Draw();
        }
    }
}
