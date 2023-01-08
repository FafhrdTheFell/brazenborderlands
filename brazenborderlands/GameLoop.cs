using System;
using System.Collections.Generic;
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

        public void InventoryLoop()
        {
            Program.inventoryDisplay.Dirty = true;
            Program.inventoryDisplay.GlobalDirty = true;
            Program.inventoryDisplay.Draw();
            bool inventoryActive = true;
            while (inventoryActive)
            {
                term.Refresh();
                int r = term.Read();
                // shift does not work with numpad on my laptop...
                bool shiftdown = (term.State(term.TK_SHIFT) == 1);
                switch (r)
                {
                    case term.TK_0:
                        inventoryActive = false; break;
                }
            }
            Program.inventoryDisplay.Hide();
            term.Refresh();
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
