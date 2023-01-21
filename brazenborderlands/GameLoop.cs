using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using term = BearLib.Terminal;

namespace brazenborderlands
{
    internal class GameLoop
    {
        public GameLoop() { }

        public void Run()
        {
            while (Program.active) 
            {
                //Actor[] actors = Program.location.Actors();
                //for (int i=0; i<actors.Length; i++)
                foreach (Actor a in Program.location.Actors())
                {
                    bool seenbefore = Program.location.FOV.IsInFov(a.x, a.y);
                    bool acted = a.Act();
                    if (a is Player)
                    {
                        while(!acted)
                        {
                            acted = a.Act();
                        }
                    }
                    bool seenafter = Program.location.FOV.IsInFov(a.x, a.y);
                    Program.locationDisplay.Dirty = seenafter || seenbefore;
                    if (a is Player) { Program.location.UpdatePlayerVision(); }
                    Program.locationDisplay.Draw();
                    Program.logDisplay.Draw();
                    Program.characterDisplay.Draw();
                    term.Refresh();
                }
            }
        }

        public bool InventoryLoop()
        {
            Program.inventoryDisplay.Dirty = true;
            Program.inventoryDisplay.GlobalDirty = true;
            bool inventoryActive = true;
            bool acted = false;
            while (inventoryActive)
            {
                Program.inventoryDisplay.Draw();
                term.Refresh();
                int r = term.Read();
                // shift does not work with numpad on my laptop...
                bool shiftdown = (term.State(term.TK_SHIFT) == 1);
                if (r == term.TK_ESCAPE)
                {
                    if (Program.inventoryDisplay.Mode == Displays.InventoryDisplay.InventoryMode.None)
                    {
                        inventoryActive = false;
                    }
                    else
                    {
                        Program.inventoryDisplay.Mode = Displays.InventoryDisplay.InventoryMode.None;
                    }
                }
                if (Program.inventoryDisplay.Mode == Displays.InventoryDisplay.InventoryMode.Equip)
                {
                    if (Program.inventoryDisplay.EquipChosenItem(r)) acted = true; 
                }
                else if (Program.inventoryDisplay.Mode == Displays.InventoryDisplay.InventoryMode.Drop)
                {
                    if (Program.inventoryDisplay.DropChosenItem(r)) acted = true;
                }
                else if (Program.inventoryDisplay.Mode == Displays.InventoryDisplay.InventoryMode.Use)
                {
                    if (Program.inventoryDisplay.UseChosenItem(r))
                    {
                        acted = true;
                        inventoryActive = false;
                    }
                }
                if (Program.inventoryDisplay.Mode == Displays.InventoryDisplay.InventoryMode.None &&
                    r == term.TK_U) Program.inventoryDisplay.Mode = Displays.InventoryDisplay.InventoryMode.Use;
                if (Program.inventoryDisplay.Mode == Displays.InventoryDisplay.InventoryMode.None && 
                    r == term.TK_E) Program.inventoryDisplay.Mode = Displays.InventoryDisplay.InventoryMode.Equip;
                if (Program.inventoryDisplay.Mode == Displays.InventoryDisplay.InventoryMode.None &&
                    r == term.TK_D) Program.inventoryDisplay.Mode = Displays.InventoryDisplay.InventoryMode.Drop;

                //    switch (r)
                //{
                //    case term.TK_D:
                //        Program.inventoryDisplay.Mode = Displays.InventoryDisplay.InventoryMode.Drop; break;
                //    case term.TK_E:
                //        Program.inventoryDisplay.Mode = Displays.InventoryDisplay.InventoryMode.Equip; break;
                //    case term.TK_ESCAPE:
                //        inventoryActive = false; break;
                //}
            }
            Program.inventoryDisplay.Hide();
            Program.locationDisplay.Draw();
            Program.logDisplay.Draw();
            Program.characterDisplay.Draw();
            term.Refresh();
            return acted;
        }

        public void WaitForKey()
        {
            bool waiting = true;
            while (waiting)
            {
                int z = term.Read();
                if (z == term.TK_0) { waiting = false; }
            }
        }
    }
}
