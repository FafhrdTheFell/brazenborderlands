using RogueSharp.DiceNotation;
using System;
using System.Collections.Generic;

namespace brazenborderlands
{
    enum EquipmentSlot
    {
        PrimaryHand,
        SecondaryHand,
        BothHands,
        Missile,
        Body,
        Feet,
        Hands,
        Neck,
        Head
    }

    enum EquipmentType
    {
        Possession,
        BodyPart,
        OneHandMeleeWeapon,
        TwoHandMeleeWeapon,
        Shield,
        MissileWeapon,
        BodyArmor,
        Boots,
        Gloves,
        Necklace,
        Cloak,
        Headgear
    }

    enum Material
    {
        UnknownMaterial,
        IronWood,
        Iron,
        Steel,
        Bronze,
        Elfmetal,
        Adamantine,
        Cuirbolli,
        Lamellar,
        Linothorax,
        Flexilon
    }

    enum ItemAttribute
    {
        Light,
        HighQuality,
        LowQuality,
        Solid,
        Enchanted
    }

    enum MeleeWeaponType
    {
        Broadsword,
        Dagger,
        Greatsword,
        Hammer,
        HandAxe,
        Maul,
        Morningstar,
        Scimitar,
        Spear,
        Staff,
        Claw,
        Fist
    }


    enum ArmorType
    {
        Shirt,
        Vest,
        Cuirass,
        Hauberk,
        Mail,
        Plate,
        Shield,
        Buckler,
        Helmet,
        Innate
    }

    internal interface IEquipment
    {
        public EquipmentSlot Slot { get; set; }
        public EquipmentType Type { get; set; }
        public bool IsEquipped { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Template { get; set; }
        public int Rarity();
        //public string ShortName { get; set; }
    }
    internal interface IWeapon
    {
        public int PctAccuracyBrawn { get; set; }
        public int PctAccuracyReflexes { get; set; }
        public int PctAccuracyEgo { get; set; }
        public int PctDamageBrawn { get; set; }
        public int PctDamageReflexes { get; set; }
        public int PctDamageEgo { get; set; }
        public int BaseDamage { get; set; }
        public int BaseAccuracy { get; set; }
        public void Attack(Actor wielder, Actor target);
    }
    internal interface IArmor
    {
        public int PctDefBrawn { get; set; }
        public int PctDefReflexes { get; set; }
        public int PctDefEgo { get; set; }
        public int BaseSoak { get; set; }
        public int BaseDefense { get; set; }
        public int DefenseChange(Actor wearer);
        public int SoakChange(Actor wearer);
    }
    internal class ItemProperties
    {
        public static List<MeleeWeaponType> InnateWeapons = new List<MeleeWeaponType>() { MeleeWeaponType.Claw, MeleeWeaponType.Fist };
        public static List<ArmorType> InnateArmor = new List<ArmorType>() { ArmorType.Innate };
        //public static List<Material> ArmorMaterials = new List<Material>() { Material.IronWood, Material.Iron, Material.Elfmetal,
        //    Material.Adamantine, Material.Steel, Material.Bronze, Material.Cuirbolli, Material.Lamellar, Material.Linothorax,
        //        Material.Flexilon};
        public static List<Material> WeaponMaterials = new List<Material>() { Material.IronWood, Material.Iron, Material.Elfmetal,
            Material.Adamantine, Material.Steel, Material.Bronze};
        public static Dictionary<ArmorType, List<Material>> MaterialsOfArmorType = new Dictionary<ArmorType, List<Material>>()
        {
            { ArmorType.Vest,  new List<Material>() { Material.IronWood, Material.Elfmetal,
            Material.Cuirbolli, Material.Linothorax, Material.Flexilon} },
            { ArmorType.Shirt,  new List<Material>() { Material.IronWood, Material.Elfmetal,
            Material.Cuirbolli, Material.Linothorax, Material.Flexilon} },
            { ArmorType.Cuirass, new List<Material>() { Material.IronWood, Material.Iron, Material.Elfmetal,
            Material.Adamantine, Material.Steel, Material.Bronze, Material.Cuirbolli, Material.Lamellar, Material.Flexilon} },
            { ArmorType.Hauberk, new List<Material>() { Material.IronWood, Material.Iron, Material.Elfmetal,
            Material.Adamantine, Material.Steel, Material.Bronze, Material.Cuirbolli, Material.Lamellar, Material.Flexilon} },
            { ArmorType.Plate,  new List<Material>() { Material.Iron, Material.Elfmetal,
            Material.Adamantine, Material.Steel, Material.Bronze, Material.Cuirbolli, Material.Lamellar, Material.Flexilon} },
            { ArmorType.Mail,  new List<Material>() { Material.Iron, Material.Elfmetal,
            Material.Adamantine, Material.Steel, Material.Bronze} },
            { ArmorType.Buckler, new List<Material>() { Material.IronWood, Material.Iron, Material.Elfmetal,
            Material.Adamantine, Material.Steel, Material.Bronze, Material.Cuirbolli, Material.Lamellar, Material.Flexilon} },
            { ArmorType.Shield, new List<Material>() { Material.IronWood, Material.Iron, Material.Elfmetal,
            Material.Adamantine, Material.Steel, Material.Bronze, Material.Cuirbolli, Material.Lamellar, Material.Flexilon} },
            { ArmorType.Helmet, new List<Material>() { Material.IronWood, Material.Iron, Material.Elfmetal,
            Material.Adamantine, Material.Steel, Material.Bronze, Material.Cuirbolli } }
        };
        public static Dictionary<MeleeWeaponType, int> WeaponRarity = new Dictionary<MeleeWeaponType, int>()
        {
            { MeleeWeaponType.Broadsword, 1},
            { MeleeWeaponType.Greatsword, 2},
            { MeleeWeaponType.Maul, 2 },
            { MeleeWeaponType.Scimitar, 1 },
            { MeleeWeaponType.Morningstar, 2 }
        };
        public static Dictionary<ArmorType, int> ArmorTypeRarity = new Dictionary<ArmorType, int>()
        {
            { ArmorType.Hauberk, 1},
            { ArmorType.Cuirass, 1 },
            { ArmorType.Plate, 2},
            { ArmorType.Mail, 2 },
            { ArmorType.Buckler, 2 },
            { ArmorType.Helmet, 1 }
        };
        public static Dictionary<Material, int> MaterialRarity = new Dictionary<Material, int>()
        {
            { Material.Adamantine, 3},
            { Material.Cuirbolli, 1 },
            { Material.Elfmetal, 2 },
            { Material.Flexilon, 3 },
            { Material.Lamellar, 1 },
            { Material.Steel, 2 }
        };

    }
    // an item is an object that can be picked up and added to inventory.
    // derived classes also need to reconstructable by template using Rebuild().
    // Rebuild is a fix for JSON serialization not handling desrialization
    // of objects of a derived class well.
    internal class Item : IEquipment, IEmbodied
    {
        public EquipmentSlot Slot { get; set; }
        public EquipmentType Type { get; set; }
        public bool IsEquipped { get; set; }
        public virtual string Name { get; set; }
        public string Description { get; set; }
        public string Template { get; set; }
        public virtual string DrawingGlyph { get; set; }
        public virtual string DrawingColor { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public bool IsWalkable { get => true; set { } }
        public bool IsTransparent { get => true; set { } }
        public Item() { }
        public Item Rebuild()
        {
            Item i = BuildFromTemplate(Template);
            i.Name = Name;
            i.Slot = Slot;
            i.Type = Type;
            i.IsEquipped = IsEquipped;
            i.Template = Template;
            i.DrawingColor = DrawingColor;
            i.DrawingGlyph = DrawingGlyph;
            i.x = x;
            i.y = y;
            return i;
        }
        public virtual int Rarity() { return 0; }
        protected string MakeTemplate(params string[] tokens)
        {
            string s = "";
            for (int i = 0; i < tokens.Length; i++)
            {
                s += tokens[i] + (i < tokens.Length - 1 ? "," : "");
            }
            return s;
        }
        public static Item BuildFromTemplate(string template)
        {
            string[] tokens = template.Split(',');
            switch (tokens[0])
            {
                case "Weapon":
                    return new Weapon((MeleeWeaponType)Enum.Parse(typeof(MeleeWeaponType), tokens[1]),
                        (Material)Enum.Parse(typeof(Material), tokens[2]));
                case "Armor":
                    return new Armor((ArmorType)Enum.Parse(typeof(ArmorType), tokens[1]),
                        (Material)Enum.Parse(typeof(Material), tokens[2]));
                default:
                    throw new Exception("Building " + tokens[0] + " is not implemented.");
            }
        }
        public static string DefaultMeleeWeaponGlyph(MeleeWeaponType type)
        {
            switch (type)
            {
                case MeleeWeaponType.Broadsword:
                    return TileFinder.TileGridLookupUnicode(1, 2, TileFinder.TileSheet.Items);
                case MeleeWeaponType.Dagger:
                    return TileFinder.TileGridLookupUnicode(1, 1, TileFinder.TileSheet.Items);
                case MeleeWeaponType.Greatsword:
                    return TileFinder.TileGridLookupUnicode(2, 11, TileFinder.TileSheet.Items);
                case MeleeWeaponType.Hammer:
                    return TileFinder.TileGridLookupUnicode(1, 12, TileFinder.TileSheet.Items);
                case MeleeWeaponType.HandAxe:
                    return TileFinder.TileGridLookupUnicode(1, 10, TileFinder.TileSheet.Items);
                case MeleeWeaponType.Maul:
                    return TileFinder.TileGridLookupUnicode(1, 15, TileFinder.TileSheet.Items);
                case MeleeWeaponType.Morningstar:
                    return TileFinder.TileGridLookupUnicode(2, 15, TileFinder.TileSheet.Items);
                case MeleeWeaponType.Scimitar:
                    return TileFinder.TileGridLookupUnicode(2, 10, TileFinder.TileSheet.Items);
                case MeleeWeaponType.Spear:
                    return TileFinder.TileGridLookupUnicode(2, 9, TileFinder.TileSheet.Items);
                case MeleeWeaponType.Staff:
                    return TileFinder.TileGridLookupUnicode(2, 19, TileFinder.TileSheet.Items);
                default:
                    return TileFinder.TileGridLookupUnicode(2, 1, TileFinder.TileSheet.Items);
            }
        }

        public static string DefaultArmorGlyph(ArmorType type)
        {
            switch (type)
            {
                case ArmorType.Vest:
                    return TileFinder.TileGridLookupUnicode(8, 1, TileFinder.TileSheet.Items);
                case ArmorType.Shirt:
                    return TileFinder.TileGridLookupUnicode(9, 1, TileFinder.TileSheet.Items);
                case ArmorType.Cuirass:
                    return TileFinder.TileGridLookupUnicode(8, 2, TileFinder.TileSheet.Items);
                case ArmorType.Hauberk:
                    return TileFinder.TileGridLookupUnicode(9, 4, TileFinder.TileSheet.Items);
                case ArmorType.Mail:
                    return TileFinder.TileGridLookupUnicode(8, 5, TileFinder.TileSheet.Items);
                case ArmorType.Plate:
                    return TileFinder.TileGridLookupUnicode(8, 7, TileFinder.TileSheet.Items);
                case ArmorType.Helmet:
                    return TileFinder.TileGridLookupUnicode(7, 1, TileFinder.TileSheet.Items);
                case ArmorType.Buckler:
                    return TileFinder.TileGridLookupUnicode(6, 1, TileFinder.TileSheet.Items);
                case ArmorType.Shield:
                    return TileFinder.TileGridLookupUnicode(6, 9, TileFinder.TileSheet.Items);
                default:
                    return TileFinder.TileGridLookupUnicode(3, 1, TileFinder.TileSheet.Items);
            }
        }
        public static string DefaultMaterialColors(Material material)
        {
            switch (material)
            {
                case Material.Adamantine:
                    return "Silver";
                case Material.Bronze:
                    return "Bronze";
                case Material.Cuirbolli:
                    return "MediumBrown";
                case Material.Elfmetal:
                    return "Elfmetal";
                case Material.Flexilon:
                    return "DeepBrown";
                case Material.Iron:
                    return "Iron";
                case Material.IronWood:
                    return "YoungWood";
                case Material.Lamellar:
                    return "RedLacquer";
                case Material.Linothorax:
                    return "Cotton";
                default:
                    return "Gray";
            }
        }
    }
}