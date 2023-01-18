using RogueSharp.DiceNotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace brazenborderlands
{
    internal class Weapon : Item, IWeapon 
    {
        private string _drawingGlyph;
        private string _drawingColor;
        private string _name;
        private string _description;
        public override string DrawingGlyph { get { return _drawingGlyph ?? Item.DefaultMeleeWeaponGlyph(WeaponType); } set { _drawingGlyph = value; } }
        public override string DrawingColor { get { return _drawingColor ?? Item.DefaultMaterialColors(Material); } set { _drawingColor = value; } }
        public int PctAccuracyBrawn { get; set; }
        public int PctAccuracyReflexes { get; set; }
        public int PctAccuracyEgo { get; set; }
        public int PctDamageBrawn { get; set; }
        public int PctDamageReflexes { get; set; }
        public int PctDamageEgo { get; set; }
        public int BaseDamage { get; set; }
        public int BaseAccuracy { get; set; }
        public MeleeWeaponType WeaponType { get; set; }
        public Material Material { get; set; }
        public override string Name
        {
            get { return _name ?? MaterialString() + " " + WeaponTypeString(); }
            set { _name = value; }
        }
        public override string Description
        {
            get { return _description ?? DescriptionString(); }
            set { _description = value; }
        }


        public Weapon(MeleeWeaponType weaponType) : this(weaponType, Material.UnknownMaterial) { }
        public Weapon(MeleeWeaponType weaponType, Material material)
        {
            Material = material;
            WeaponType = weaponType;
            Template = MakeTemplate("Weapon", WeaponTypeString(), MaterialString());
            switch (weaponType)
            {
                case MeleeWeaponType.Fist:
                    Init(EquipmentSlot.PrimaryHand, EquipmentType.BodyPart, 50, 50, 0, 50, 30, 0); break;
                case MeleeWeaponType.Claw:
                    Init(EquipmentSlot.PrimaryHand, EquipmentType.BodyPart, 50, 50, 0, 50, 50, 0); break;
                case MeleeWeaponType.Broadsword:
                    Init(EquipmentSlot.PrimaryHand, EquipmentType.OneHandMeleeWeapon, 50, 50, 0, 50, 50, 0); break;
                case MeleeWeaponType.Dagger:
                    Init(EquipmentSlot.PrimaryHand, EquipmentType.OneHandMeleeWeapon, 40, 80, 0, 40, 40, 0); break;
                case MeleeWeaponType.Greatsword:
                    Init(EquipmentSlot.BothHands, EquipmentType.TwoHandMeleeWeapon, 75, 25, 0, 95, 40, 0); break;
                case MeleeWeaponType.Hammer:
                    Init(EquipmentSlot.PrimaryHand, EquipmentType.OneHandMeleeWeapon, 65, 45, 0, 60, 30, 0); break;
                case MeleeWeaponType.HandAxe:
                    Init(EquipmentSlot.PrimaryHand, EquipmentType.OneHandMeleeWeapon, 60, 40, 0, 60, 40, 0); break;
                case MeleeWeaponType.Maul:
                    Init(EquipmentSlot.BothHands, EquipmentType.TwoHandMeleeWeapon, 75, 20, 0, 100, 35, 0); break;
                case MeleeWeaponType.Morningstar:
                    Init(EquipmentSlot.PrimaryHand, EquipmentType.OneHandMeleeWeapon, 40, 60, 0, 60, 50, 0); break;
                case MeleeWeaponType.Scimitar:
                    Init(EquipmentSlot.PrimaryHand, EquipmentType.OneHandMeleeWeapon, 40, 60, 0, 50, 50, 0); break;
                case MeleeWeaponType.Spear:
                    Init(EquipmentSlot.PrimaryHand, EquipmentType.OneHandMeleeWeapon, 50, 60, 0, 40, 50, 0); break;
                case MeleeWeaponType.Staff:
                    Init(EquipmentSlot.BothHands, EquipmentType.OneHandMeleeWeapon, 50, 60, 0, 50, 50, 0); break;

                default:
                    throw new NotImplementedException("No WeaponType definition for " + Template);

            }
            switch (material)
            {
                case Material.Bronze:
                    BaseAccuracy -= 1; BaseDamage -= 3; break;
                case Material.Elfmetal:
                    PctAccuracyEgo += 20; PctDamageEgo += 20; break;
                case Material.IronWood:
                    PctAccuracyBrawn -= 5; PctAccuracyReflexes += 10; BaseDamage -= 2; break;
                case Material.Steel:
                    PctAccuracyBrawn += 5; PctAccuracyReflexes += 5; PctDamageBrawn += 10;
                    BaseDamage += 1; break;
                case Material.Adamantine:
                    PctAccuracyBrawn += 8; PctAccuracyReflexes += 8; PctDamageBrawn += 15;
                    BaseDamage += 2; break;
            }
        }
        public void Init(EquipmentSlot slot, EquipmentType type, int pctAB, int pctAR, int pctAE, int pctDB, int pctDR, int pctDE)
        {
            Slot = slot;
            Type = type;
            PctAccuracyBrawn = pctAB;
            PctAccuracyReflexes = pctAR;
            PctAccuracyEgo = pctAE;
            PctDamageBrawn = pctDB;
            PctDamageReflexes = pctDR;
            PctDamageEgo = pctDE;
            Name ??= MaterialString() + " " + WeaponTypeString();
        }
        public override int Rarity()
        {
            int w = ItemProperties.WeaponRarity.ContainsKey(WeaponType) ? ItemProperties.WeaponRarity[WeaponType] : 0;
            int m = ItemProperties.MaterialRarity.ContainsKey(Material) ? ItemProperties.MaterialRarity[Material] : 0;
            return Math.Max(w, m) + (w>0 && m>0 ? 1 : 0);
        }
        public virtual void Attack(Actor attacker, Actor target)
        {
            if (attacker is Player)
            {
                Program.logDisplay.AppendEntry(attacker.Name + " attacks with " + Name + " ");
            }
            else
            {
                Program.logDisplay.AppendEntry(attacker.Name  + " attacks ");
            }

            int excesstohit = ToHitRoll(attacker, target, null);
            if (excesstohit <= 0)
            {
                Program.logDisplay.AppendEntry("and missed.");
                Program.logDisplay.WriteBufferAsEntry();
                return;
            }
            excesstohit = Math.Max(excesstohit - 8, 0);
            int finaldamage = DamageRoll(attacker, target, excesstohit, null);
            if (finaldamage <= 0)
            {
                Program.logDisplay.AppendEntry("and failed to penetrate.");
                Program.logDisplay.WriteBufferAsEntry();
                return;
            }
            int woundpoints = Math.Max(0, Helpers.RollXSidedDie(finaldamage) - Helpers.RollXSidedDie(target.Soak()));
            if (target.IsStunned) { woundpoints = finaldamage; }
            int painpoints = finaldamage - woundpoints;
            target.Painpoints += painpoints;
            target.Woundpoints += woundpoints;
            if (target.IsDead)
            {
                Program.logDisplay.AppendEntry("for " + finaldamage.ToString() + " damage, killing it.");
            }
            else if (target.IsStunned)
            {
                Program.logDisplay.AppendEntry("for " + finaldamage.ToString() + " damage. " + target.Name + " is stunned.");
            }
            else
            {
                Program.logDisplay.AppendEntry("for " + finaldamage.ToString() + " damage.");
            }
            Program.logDisplay.WriteBufferAsEntry();
        }
        protected virtual int ToHitRoll(Actor attacker, Actor defender, int? defense)
        {
            int def = defense ?? defender.Defense();
            if (defender.IsStunned) { def = 0; }
            int result = Rules.OpposedRoll(Accuracy(attacker), def);
            return result;
        }
        protected virtual int DamageRoll(Actor attacker, Actor defender, int excesstohit, int? soak)
        {
            int s = soak ?? defender.Soak();
            int dam = Damage(attacker);
            int soaked = Dice.Roll("2d" + s.ToString() + "k1");
            int damageroll = Dice.Roll("1d" + dam.ToString());
            int result;
            if (soaked > damageroll)
            {
                result = Math.Max(0, excesstohit - (soaked - damageroll) / 2);
            }
            else
            {
                result = damageroll + excesstohit - soaked;
            }
            //System.Console.WriteLine("soaked {0} roll {1} result {2}", soaked.ToString(), damageroll.ToString(), result.ToString());
            return result;
        }
        public virtual int Accuracy(Actor wielder)
        {
            return BaseAccuracy + (wielder.Brawn() * PctAccuracyBrawn + wielder.Reflexes() * PctAccuracyReflexes + wielder.Ego() * PctAccuracyEgo) / 100;
        }
        public virtual int Damage(Actor wielder)
        {
            return BaseDamage + (wielder.Brawn() * PctDamageBrawn + wielder.Reflexes() * PctDamageReflexes + wielder.Ego() * PctDamageEgo) / 100;
        }
        public string MaterialString()
        {
            return Enum.GetName(typeof(Material), Material);
        }
        public string WeaponTypeString()
        {
            return Enum.GetName(typeof(MeleeWeaponType), WeaponType);
        }
        public string DescriptionString()
        {
            return "Wielding : " + Accuracy(Program.player).ToString() + " Accuracy, " + Damage(Program.player).ToString() + " Damage";
        }
        public static Weapon RandomWeapon(int rarity)
        {
            int generatedRarity = 999;
            Weapon weapon = null;
            int tries = 0;
            while(!(generatedRarity == rarity) && (tries < 50))
            {
                tries++;
                MeleeWeaponType w = Helpers.RandomEnumValue<MeleeWeaponType>();
                while (ItemProperties.InnateWeapons.Contains(w))
                {
                    w = Helpers.RandomEnumValue<MeleeWeaponType>();
                }
                Material m = Helpers.RandomEnumValue<Material>();
                while (!ItemProperties.WeaponMaterials.Contains(m))
                {
                    m = Helpers.RandomEnumValue<Material>();
                }
                weapon = new Weapon(w, m);
                generatedRarity = weapon.Rarity();
            }
            return weapon;
        }
    }
}
