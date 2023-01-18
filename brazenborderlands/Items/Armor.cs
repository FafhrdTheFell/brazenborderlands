using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using RogueSharp.DiceNotation;
using System.Text;
using System.Xml.Linq;

namespace brazenborderlands
{
    internal class Armor : Item, IArmor
    {
        private string _name;
        private string _description;
        private string _drawingGlyph;
        private string _drawingColor;
        public override string DrawingGlyph { get { return _drawingGlyph ?? Item.DefaultArmorGlyph(ArmorType); } set { _drawingGlyph = value; } }
        public override string DrawingColor { get { return _drawingColor ?? Item.DefaultMaterialColors(Material); } set { _drawingColor = value; } }
        public int PctDefBrawn { get; set; }
        public int PctDefReflexes { get; set; }
        public int PctDefEgo { get; set; }
        public int PctSoakEgo { get; set; }
        public int BaseSoak { get; set; }
        public int BaseDefense { get; set; }
        public ArmorType ArmorType { get; set; }
        public Material Material { get; set; }
        public override string Name
        {
            get { return _name ?? MaterialString() + " " + ArmorTypeString(); }
            set { _name = value; }
        }
        public override string Description 
        { 
            get { return _description ?? DescriptionString(); }
            set { _description = value; }
        }


        public Armor(ArmorType armorType, Material material)
        {
            ArmorType = armorType;
            Material = material;
            Template = MakeTemplate("Armor", ArmorTypeString(), MaterialString());
            switch (armorType)
            {
                // shirt, vest -> light, hauberk, cuirass -> medium, plate, mail -> heavy
                case ArmorType.Plate:
                    Init(EquipmentSlot.Body, EquipmentType.BodyArmor, 30, -70, 0, 0, 0, 32); break;
                case ArmorType.Mail:
                    Init(EquipmentSlot.Body, EquipmentType.BodyArmor, 30, -70, 0, 0, 0, 29); break;
                case ArmorType.Hauberk:
                    Init(EquipmentSlot.Body, EquipmentType.BodyArmor, 20, -40, 0, 0, 0, 20); break;
                case ArmorType.Cuirass:
                    Init(EquipmentSlot.Body, EquipmentType.BodyArmor, 20, -40, 0, 0, 0, 18); break;
                case ArmorType.Vest:
                    Init(EquipmentSlot.Body, EquipmentType.BodyArmor, 10, -20, 0, 0, 0, 10); break;
                case ArmorType.Shirt:
                    Init(EquipmentSlot.Body, EquipmentType.BodyArmor, 10, -20, 0, 0, 0, 12); break;
                case ArmorType.Helmet:
                    Init(EquipmentSlot.Head, EquipmentType.Headgear, 0, 0, 0, 0, 0, 4); break;
                case ArmorType.Shield:
                    Init(EquipmentSlot.SecondaryHand, EquipmentType.Shield, -10, 30, 0, 0, 0, 3); break;
                case ArmorType.Buckler:
                    Init(EquipmentSlot.SecondaryHand, EquipmentType.Shield, 10, 10, 0, 0, 0, 0); break;
            }
            switch (material)
            {
                case Material.Cuirbolli:
                    Modify(ItemAttribute.Light, ItemAttribute.HighQuality); break;
                case Material.IronWood:
                    Modify(ItemAttribute.Light); break;
                case Material.Linothorax:
                    Modify(ItemAttribute.Light); break;
                case Material.Flexilon:
                    Modify(ItemAttribute.HighQuality, ItemAttribute.Solid);  break;
                case Material.Bronze:
                    Modify(ItemAttribute.LowQuality); break;
                case Material.Steel:
                    Modify(ItemAttribute.Solid); break;
                case Material.Elfmetal:
                    Modify(ItemAttribute.Light, ItemAttribute.Enchanted); break;
                case Material.Adamantine:
                    Modify(ItemAttribute.Solid, ItemAttribute.Solid); break;
            }
        //    IronWood,
        //Iron,
        //Steel,
        //Bronze,
        //Elfmetal,
        //Adamantine,
        //Cuirbolli,
        //Jazerant,
        //Lamellar,
        //Linothorax,
        //Flexilon
        }
        public void Init(EquipmentSlot slot, EquipmentType type, int pctDB, int pctDR, int pctDE, int pctSE, int baseDef, int baseSoak)
        {
            Slot= slot;
            Type= type;
            PctDefBrawn = pctDB;
            PctDefReflexes = pctDR;
            PctDefEgo = pctDE;
            PctSoakEgo = pctSE;
            BaseDefense = baseDef;
            BaseSoak = baseSoak;
        }
        public void Modify(params ItemAttribute[] attributes)
        {
            foreach (ItemAttribute attribute in attributes)
            {
                switch (attribute)
                {
                    case ItemAttribute.Light:
                        PctDefBrawn = Math.Max(0, PctDefBrawn - 5);
                        PctDefReflexes += 5;
                        BaseSoak = 9 * BaseSoak / 10;
                        break;
                    case ItemAttribute.HighQuality:
                        BaseDefense += 2;
                        BaseSoak += 2;
                        break;
                    case ItemAttribute.LowQuality:
                        BaseDefense = Math.Max(0, BaseDefense - 2);
                        BaseSoak = Math.Max(0, BaseSoak - 2);
                        break;
                    case ItemAttribute.Solid:
                        BaseSoak = 6 * BaseSoak / 5 + 1;
                        break;
                    case ItemAttribute.Enchanted:
                        BaseSoak = 108 * BaseSoak / 100;
                        PctDefEgo += 10;
                        PctSoakEgo += 10;
                        break;
                }
            }
        }
        public override int Rarity()
        {
            int a = ItemProperties.ArmorTypeRarity.ContainsKey(ArmorType) ? ItemProperties.ArmorTypeRarity[ArmorType] : 0;
            int m = ItemProperties.MaterialRarity.ContainsKey(Material) ? ItemProperties.MaterialRarity[Material] : 0;
            return a + m;
        }
        public int DefenseChange(Actor wearer)
        {
            return (wearer.Brawn() * PctDefBrawn + wearer.Reflexes() * PctDefReflexes + wearer.Ego() * PctDefEgo) / 100 + BaseDefense;
        }
        public int SoakChange(Actor wearer)
        {
            return (wearer.Ego() * PctSoakEgo) / 100 + BaseSoak;
        }
        public string MaterialString()
        {
            return Enum.GetName(typeof(Material), Material);
        }
        public string ArmorTypeString()
        {
            return Enum.GetName(typeof(ArmorType), ArmorType);
        }
        public string DescriptionString()
        {
            string dc = (DefenseChange(Program.player) > 0) ? "+" : "";
            dc += DefenseChange(Program.player).ToString();
            string sc = (SoakChange(Program.player) > 0) ? "+" : "";
            if (Slot == EquipmentSlot.Body) sc = "";
            sc += SoakChange(Program.player).ToString();
            return "Equipped : " + dc + " Defense, " + sc + " Soak";
        }
        public static Armor RandomArmor(int rarity)
        {
            int generatedRarity = 999;
            Armor armor = null;
            while (generatedRarity > rarity)
            {
                ArmorType a = Helpers.RandomEnumValue<ArmorType>();
                while (ItemProperties.InnateArmor.Contains(a))
                {
                    a = Helpers.RandomEnumValue<ArmorType>();
                }
                Material m = Helpers.RandomEnumValue<Material>();
                while (!ItemProperties.MaterialsOfArmorType[a].Contains(m))
                {
                    m = Helpers.RandomEnumValue<Material>();
                }
                armor = new Armor(a, m);
                generatedRarity = armor.Rarity();
            }
            return armor;
        }
    }
}
