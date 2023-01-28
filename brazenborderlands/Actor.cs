using RogueSharp.DiceNotation;
using RogueSharp.DiceNotation.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace brazenborderlands
{
    // Actors are embodied agents who can attack and be attacked
    internal class Actor : IEmbodied, IActs, IStats
    {
        private int _brawn;
        private int _reflexes;
        private int _ego;
        private int _woundpoints = 0;
        private int _painpoints = 0;

        private int _level;
        private int _soakBase;

        public int Level
        {
            get => _level;
            set
            {
                if (value > 99 || value < 1)
                {
                    throw new NotSupportedException("Attempt to set level outside [1,99].");
                };
                _level = value;
            }
        }
        public virtual int SoakBase { get => _soakBase; set { _soakBase = Helpers.BoundsCheck(value); } }

        public int x { get; set; }
        public int y { get; set; }
        public virtual Glyph Glyph { get; set; }
        public bool IsWalkable { get => false; set { } }
        public bool IsTransparent { get => false; set { } }
        public string Name { get; set; }
        public int BrawnBase { get => _brawn; set { _brawn = Helpers.BoundsCheck(value); } }
        public int ReflexesBase { get => _reflexes; set { _reflexes = Helpers.BoundsCheck(value); } }
        public int EgoBase { get => _ego; set { _ego = Helpers.BoundsCheck(value); } }

        // When uninjured, default max health equal to brawn plus base HP. When injured, accumulate wound points,
        // and pain points. Pain goes away over time. If pain + wounds >= Max Health, cannot act.
        // Wounds do not go away without some healing activity.
        public int HealthBase { get; set; }
        public virtual int Woundpoints { get => _woundpoints; set { _woundpoints = value; } }
        public virtual int Painpoints { get => _painpoints; set { _painpoints = value; } }
        //public virtual int Hitpoints { get => _hitpoints; set { _hitpoints = Math.Min(value, HitpointsMax - Woundpoints); } }
        public virtual int HealthMax { get => HealthBase + BrawnBase; set { HealthBase = (value - HealthBase - BrawnBase); } }
        public virtual bool IsDead { get => (HealthMax - Woundpoints <= 0); set { } }
        public bool IsStunned { get => (HealthMax - Woundpoints - Painpoints <= 0); set { } }
        public string Health { get => (HealthMax - Woundpoints - Painpoints).ToString() + "(" +
                (HealthMax - Woundpoints).ToString() + "/" + HealthMax.ToString() + ")";  set { } }
        public Inventory Inventory { get; set; }
        public Actor()
        {
            Inventory = new Inventory();
            Glyph = new Glyph();
        }
        public Actor(int xcurrent, int ycurrent, string drawingGlyph, string drawingColor)
        {
            x = xcurrent;
            y = ycurrent;
            Glyph = new Glyph(drawingGlyph, drawingColor);
            Inventory = new Inventory();
        }

        public virtual bool Act()
        {
            return true;
        }

        public virtual bool Move(int dx, int dy)
        {
            bool acted = Program.location.Move(this, dx, dy);
            if (acted)
            {
                Program.locationDisplay.Dirty = true;
            }
            return acted;

        }
        public int LevelNorm(int level)
        {
            return 15 + level * 2;
        }
        public void InitByLevelNorm(int level)
        {
            Level = level;
            int attributeBase = LevelNorm(level);
            BrawnBase = attributeBase;
            ReflexesBase = attributeBase;
            EgoBase = attributeBase;
            SoakBase = attributeBase / 2;
            HealthBase = level * 2;
        }

        public void JiggleAttributes()
        {
            BrawnBase += Helpers.PlusOrMinus(2);
            ReflexesBase += Helpers.PlusOrMinus(2);
            EgoBase += Helpers.PlusOrMinus(2);
            HealthBase = Helpers.PlusOrMinus(3);
        }

        public virtual int Brawn()
        {
            return BrawnBase;
        }
        public virtual int Reflexes()
        {
            return ReflexesBase;
        }
        public virtual int Ego()
        {
            return EgoBase;
        }
        public virtual Weapon MeleeAttack()
        {
            return Inventory.EquippedWeapon() ?? new Weapon(MeleeWeaponType.Fist);
        }
        public virtual int Defense()
        {
            int def = Reflexes();
            foreach (Armor a in Inventory.EquippedArmors())
            {
                def += a.DefenseChange(this);
            }
            return def;
        }
        public virtual int Soak()
        {
            int soak = Inventory.IsBodyArmorEquipped() ? 0 : SoakBase;
            foreach (Armor a in Inventory.EquippedArmors())
            {
                soak += a.SoakChange(this);
            }
            return soak;
        }

    }
}
