using System;
using System.Collections.Generic;
using System.Linq;
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
                bool ctrldown = (term.State(term.TK_CONTROL) == 1);
                System.Console.WriteLine(r.ToString() + " " + shiftdown.ToString() + " " + ctrldown.ToString());
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
                if (Program.inventoryDisplay.Mode == Displays.InventoryDisplay.InventoryMode.None &&
                    r == term.TK_U) Program.inventoryDisplay.Mode = Displays.InventoryDisplay.InventoryMode.Use;
                else if (Program.inventoryDisplay.Mode == Displays.InventoryDisplay.InventoryMode.None &&
                    r == term.TK_E) Program.inventoryDisplay.Mode = Displays.InventoryDisplay.InventoryMode.Equip;
                else if (Program.inventoryDisplay.Mode == Displays.InventoryDisplay.InventoryMode.None &&
                    r == term.TK_D) Program.inventoryDisplay.Mode = Displays.InventoryDisplay.InventoryMode.Drop;
                else if (Program.inventoryDisplay.Mode == Displays.InventoryDisplay.InventoryMode.Equip)
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

        public bool LookLoop()
        {
            bool lookActive = true;
            int x = Program.player.x;
            int y = Program.player.y;
            int oldx = x;
            int oldy = y;
            int monsterIndex = -1;
            List<Monster> monstersInFOV = Program.location.Monsters.Where(m => Program.location.FOV.IsInFov(m.x,m.y)).ToList();
            Program.locationDisplay.HighlightCells.Add(Program.location.Map.GetCell(x, y));
            Program.locationDisplay.Dirty = true;
            while (lookActive)
            {
                Program.locationDisplay.Draw();
                term.Refresh();
                KeyPress keyPress = new KeyPress();
                switch (keyPress.Command)
                {
                    case Player.PlayerCommand.Move:
                        x += keyPress.moveDx;
                        y += keyPress.moveDy;
                        Program.locationDisplay.Dirty = true;
                        Program.locationDisplay.HighlightCells = new List<RogueSharp.ICell>
                            { Program.location.Map.GetCell(x, y) };
                        break;
                    case Player.PlayerCommand.Exit:
                        lookActive = false; break;
                    case Player.PlayerCommand.Select:
                        Program.location.LookAt(x, y);
                        lookActive = false;
                        break;
                    case Player.PlayerCommand.Next:
                        if (monstersInFOV.Count == 0) break;
                        monsterIndex += 1;
                        if (monsterIndex >= monstersInFOV.Count) monsterIndex = 0;
                        x = monstersInFOV[monsterIndex].x;
                        y = monstersInFOV[monsterIndex].y;
                        break;
                    case Player.PlayerCommand.Previous:
                        if (monstersInFOV.Count == 0) break;
                        monsterIndex -= 1;
                        if (monsterIndex < 0) monsterIndex = monstersInFOV.Count - 1;
                        x = monstersInFOV[monsterIndex].x;
                        y = monstersInFOV[monsterIndex].y;
                        break;
                }
                if (oldx != x || oldy != y)
                {
                    Program.locationDisplay.Dirty = true;
                    Program.locationDisplay.HighlightCells = new List<RogueSharp.ICell>
                            { Program.location.Map.GetCell(x, y) };
                    oldx = x; oldy = y;
                }
            }
            Program.locationDisplay.HighlightCells = new List<RogueSharp.ICell>();
            Program.locationDisplay.Dirty = true;
            return true;
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
