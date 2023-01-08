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
        public virtual int SoakBase { get => _soakBase; set { _soakBase = BoundsCheck(value); } }

        public int x { get; set; }
        public int y { get; set; }
        public virtual string DrawingGlyph { get; set; }
        public virtual string DrawingColor { get; set; }
        public bool IsWalkable { get => false; set { } }
        public bool IsTransparent { get => false; set { } }
        public string Name { get; set; }
        public int BrawnBase { get => _brawn; set { _brawn = BoundsCheck(value); HitpointBase += (value - _brawn); } }
        public int ReflexesBase { get => _reflexes; set { _reflexes = BoundsCheck(value); } }
        public int EgoBase { get => _ego; set { _ego = BoundsCheck(value); } }
        public int HitpointBase { get; set; }
        public virtual int Hitpoints { get; set; }
        public virtual int HitpointMax { get => HitpointBase + BrawnBase; set { HitpointBase += (value - BrawnBase); } }
        public bool IsDead { get => (Hitpoints <= 0); set { } }
        public Inventory Inventory { get; set; }
        public Actor() 
        {
            Inventory = new Inventory();
        }
        public Actor(int xcurrent, int ycurrent, string drawingGlyph, string drawingColor)
        {
            x = xcurrent;
            y = ycurrent;
            DrawingGlyph = drawingGlyph;
            DrawingColor = drawingColor;
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
            Hitpoints = BrawnBase;
            HitpointMax = BrawnBase;
            }

        public void JiggleAttributes()
        {
            BrawnBase += Dice.Roll("1d5 - 3");
            ReflexesBase += Dice.Roll("1d5 - 3");
            EgoBase += Dice.Roll("1d5 - 3");
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
        //public virtual Weapon MeleeAttack()
        //{
        //    Weapon fist = new Weapon(MeleeWeaponType.Fist);
        //    return fist;
        //}
        //public virtual int Defense()
        //{
        //    return Reflexes();
        //}
        //public virtual int Soak()
        //{
        //    return SoakBase;
        //}
        public virtual int BoundsCheck(int value, int min, int max)
        {
            if (value > max || value < min)
            {
                throw new NotSupportedException("Attempt to set value " + value.ToString() + 
                    "outside supported range [" + min.ToString() + "," + max.ToString() + "].");
            };
            return value;
        }
        public virtual int BoundsCheck(int value)
        {
            return BoundsCheck(value, 1, 99);
        }
    }
}
