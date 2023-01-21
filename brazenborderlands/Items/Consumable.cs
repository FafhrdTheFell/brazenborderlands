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
        private string _drawingColor;
        public override string DrawingGlyph { get { return _drawingGlyph ?? Item.DefaultMiscGlyph(ConsumableType); } set { _drawingGlyph = value; } }
        public override string DrawingColor { get { return _drawingColor ?? Item.DefaultUsageColors(Usage); } set { _drawingColor = value; } }
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
            Description = ActionVerb(ConsumableType) + " : " + UsageEffect(Usage);
            if (ConsumableType == MiscItemType.Pebble && Usage == MiscItemUsage.HealVisible)
            {
                Name = "Nelh'aig Pebble";
            }
            if (ConsumableType == MiscItemType.Pebble && Usage == MiscItemUsage.PainVisible)
            {
                Name = "Nipn'aig Pebble";
            }
            if (ConsumableType == MiscItemType.Potion && Usage == MiscItemUsage.IncreaseBrawn)
            {
                Name = "Ogre's Blood";
            }
            if (ConsumableType == MiscItemType.Potion && Usage == MiscItemUsage.IncreaseReflexes)
            {
                Name = "Essence of Speed";
            }
            if (ConsumableType == MiscItemType.Scroll && Usage == MiscItemUsage.IncreaseMental)
            {
                Name = "Scroll of Enlightenment";
            }
        }
        public override int Rarity()
        {
            int w = ItemProperties.MiscItemUsageRarity.ContainsKey(Usage) ? ItemProperties.MiscItemUsageRarity[Usage] : 0;
            return Math.Max(w, NumUses) + (w > 0 && NumUses > 1 ? 1 : 0);
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
            if (Usage == MiscItemUsage.IncreaseBrawn)
            {
                Program.logDisplay.AppendEntry(user.Name + " feels stronger and healthier.");
                user.BrawnBase++;
            }
            if (Usage == MiscItemUsage.IncreaseMental)
            {
                Program.logDisplay.AppendEntry(user.Name + " feels more decisive.");
                user.EgoBase++;
            }
            if (Usage == MiscItemUsage.IncreaseReflexes)
            {
                Program.logDisplay.AppendEntry(user.Name + " feels quicker and more agile.");
                user.ReflexesBase++;
            }
            Program.logDisplay.WriteBufferAsEntry();
        }
        public static string ActionVerb(MiscItemType type)
        {
            switch (type)
            {
                case MiscItemType.Potion:
                    return "Imbibe";
                case MiscItemType.Pebble:
                    return "Burn Up";
                case MiscItemType.Scroll:
                    return "Read";
                default:
                    return "Jigger";
            }
        }
        public static string UsageEffect(MiscItemUsage usage)
        {
            switch (usage)
            {
                case MiscItemUsage.HealVisible:
                    return "Heal all wounds of all visible creatures";
                case MiscItemUsage.PainVisible:
                    return "Cause pain to all visible creatures";
                case MiscItemUsage.IncreaseBrawn:
                    return "Permanently increase Brawn";
                case MiscItemUsage.IncreaseReflexes:
                    return "Permanently increase Reflexes";
                case MiscItemUsage.IncreaseMental:
                    return "Permanently increase Ego";
                default:
                    return "???";
            }
        }
        public static Consumable RandomConsumable(int rarity)
        {
            int generatedRarity = -1;
            Consumable consumable = new Consumable(MiscItemType.Pebble,MiscItemUsage.HealVisible,1);
            int tries = 0;
            while ((generatedRarity < rarity) && (tries < 100))
            {
                tries++;
                MiscItemUsage u = Helpers.RandomEnumValue<MiscItemUsage>();
                MiscItemType t = Helpers.RandomEnumValue<MiscItemType>();
                while (!ItemProperties.ItemTypesByUsage[u].Contains(t))
                {
                    t = Helpers.RandomEnumValue<MiscItemType>();
                }
                int n = Rules.RarityRoll(1) + 1;
                Consumable c = new Consumable(t, u, n);
                generatedRarity = c.Rarity();
                if (c.Rarity() > consumable.Rarity()) consumable = c;
            }
            return consumable;
        }
    }
}
