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
            Rows = 10;
            Columns = 4;
            ColumnTabs = new List<int> { 0, 
                (int)((float)EffectiveCellsWidth()*0.2),
                (int)((float)EffectiveCellsWidth()*0.4),
                (int)((float)EffectiveCellsWidth()*0.775)
            };
            RowTabs = new List<int> { 0,
                (int)((float)EffectiveCellsHeight()*0.2),
                (int)((float)EffectiveCellsHeight()*0.275),
                (int)((float)EffectiveCellsHeight()*0.35),
                (int)((float)EffectiveCellsHeight()*0.425),
                (int)((float)EffectiveCellsHeight()*0.5),
                (int)((float)EffectiveCellsHeight()*0.575),
                (int)((float)EffectiveCellsHeight()*0.65),
                (int)((float)EffectiveCellsHeight()*0.725),
                (int)((float)EffectiveCellsHeight()*0.8)
            };
            Contents = new string[Columns, Rows];

        }

        public override void Draw()
        {
            Contents[0, 0] = Program.player.Name;
            Contents[2, 0] = Program.player.Species + " " + Program.player.Class + " (lvl " + Program.player.Level.ToString() + ")";
            Contents[3, 0] = "H: " + Program.player.Health + "   ";
            Contents[0, 1] = "Brawn";
            Contents[1, 1] = Program.player.Brawn().ToString();
            Contents[0, 2] = "Reflexes";
            Contents[1, 2] = Program.player.Reflexes().ToString();
            Contents[0, 3] = "Ego";
            Contents[1, 3] = Program.player.Ego().ToString();
            Contents[0, 5] = "Acc / Dam";
            Contents[1, 5] = Program.player.MeleeAttack().Accuracy(Program.player).ToString() + " / " +
                Program.player.MeleeAttack().Damage(Program.player).ToString() + "  ";
            Contents[0, 6] = "Def / Soak";
            Contents[1, 6] = Program.player.Defense().ToString() + " / " +
                Program.player.Soak().ToString() + "  ";
            Contents[2, 1] = "Inventory";
            Contents[2, 2] = "------------";
            Contents[3, 1] = "Usage";
            Contents[3, 2] = "--------";
            for (int i = 0; i < Program.player.Inventory.NumItems(); i++)
            {
                Contents[2, i + 3] = Program.player.Inventory.Items[i].Name;
                Contents[3, i + 3] = Program.player.Inventory.Items[i].IsEquipped ? 
                    Enum.GetName(typeof(EquipmentSlot), Program.player.Inventory.Items[i].Slot) : "             ";
            }
            base.Draw();
        }
    }
}
