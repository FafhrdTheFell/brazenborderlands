using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace brazenborderlands
{
    internal class Consumable : Item, IConsumable
    {
        private string _drawingGlyph;
        public override string DrawingGlyph { get { return _drawingGlyph ?? Item.DefaultMiscGlyph(ConsumableType); } set { _drawingGlyph = value; } }
        public bool IsStackable { get; set; }
        public bool IsCharged { get; set; }
        public int NumUses { get; set; }
        public MiscItemType ConsumableType { get; set; }
        public MiscItemUsage Usage { get; set; }
        public string BaseName { get; set; }
        public override string Name
        {
            get => (NumUses == 1 ? BaseName : BaseName + "s (" + NumUses.ToString() + ")");
            set { BaseName = value; }
        }
        public override string Template
        {
            get => MakeTemplate("Consumable", ConsumableTypeString(), UsageString(), NumUses.ToString());
            set { }
        }
        public Consumable(MiscItemType miscItemType, MiscItemUsage miscItemUsage, int numUses) 
        {
            Slot = EquipmentSlot.None;
            Type = ItemType.Consumable;
            ConsumableType = miscItemType;
            Usage = miscItemUsage;
            NumUses = numUses;
            if (ConsumableType == MiscItemType.Pebble || ConsumableType == MiscItemType.Potion)
            {
                IsStackable = true;
                IsCharged = false;
            }
            if (ConsumableType == MiscItemType.Pebble && Usage == MiscItemUsage.HealVisible)
            {
                Name = "Nelh'aig Pebble";
                Description = "Burn Up : Heal all wounds of all visible creatures";
            }

            if (ConsumableType == MiscItemType.Pebble && Usage == MiscItemUsage.PainVisible)
            {
                Name = "Nipn'aig Pebble";
                Description = "Burn up : Cause pain to all visible creatures";
            }
        }
        public string ConsumableTypeString()
        {
            return Enum.GetName(typeof(MiscItemType), ConsumableType);
        }
        public string UsageString()
        {
            return Enum.GetName(typeof(MiscItemUsage), Usage);
        }
        public void Apply(Actor user, Actor target)
        {
            FieldOfView fov = new FieldOfView(Program.location.Map);
            fov.ComputeFov(user.x, user.y, Program.location.FOVRadius, true);
            IEnumerable<Actor> targets = Program.location.Actors().Where(a => fov.IsInFov(a.x,a.y));
            Program.logDisplay.AppendEntry("The " + BaseName + " flashes and disappears. ");
            if (Usage == MiscItemUsage.PainVisible)
            {
                int pain = (user.Brawn() + user.Ego()) / 3;
                Program.logDisplay.AppendEntry(user.Name + " suffers " + pain.ToString() + " pain points. ");
                foreach (Actor a in targets)
                {
                    a.Painpoints += pain;
                    if (a.IsStunned) Program.logDisplay.AppendEntry(a.Name + " is stunned. ");
                }
            }
            if (Usage == MiscItemUsage.HealVisible)
            {
                foreach (Actor a in targets)
                {
                    if (a.Woundpoints > 0 && a is Player) Program.logDisplay.AppendEntry(user.Name + " heals " +
                        user.Woundpoints.ToString() + " wound points. ");
                    if (a.Woundpoints > 0 && a is Monster) Program.logDisplay.AppendEntry(user.Name + " is healed. ");
                    a.Woundpoints = 0;
                }
            }
            Program.logDisplay.WriteBufferAsEntry();
        }
    }
}
