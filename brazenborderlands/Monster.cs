using RogueSharp.DiceNotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace brazenborderlands
{
    internal class Monster : Actor
    {
        public enum MonsterKind
        {
            Monster,
            Gremlin,
            Raider
        }
        public enum MonsterAttribute
        {
            Small,
            Minion
        }
        public List<MonsterKind> KindsThatUseTechnology = new List<MonsterKind>()
        { 
            MonsterKind.Raider
        };
        internal static string DefaultMonsterTile(Monster.MonsterKind monsterKind)
        {
            switch (monsterKind)
            {
                case Monster.MonsterKind.Gremlin:
                    return TileFinder.TileGridLookupUnicode(11, 4, TileFinder.TileSheet.Monsters);
                case Monster.MonsterKind.Raider:
                    return TileFinder.TileGridLookupUnicode(13, 2, TileFinder.TileSheet.Monsters);
                default:
                    return TileFinder.TileGridLookupUnicode(17, 19, TileFinder.TileSheet.Monsters);
            }
        }
        internal static string DefaultMonsterColor(Monster.MonsterKind monsterKind)
        {
            switch (monsterKind)
            {
                case Monster.MonsterKind.Gremlin:
                    return "green";
                case Monster.MonsterKind.Raider:
                    return "red";
                default:
                    return "red";
            }
        }
        public MonsterKind Kind { get; set; }
        public override int SoakBase { get => LevelNorm(Level)/2; set { } }
        public Monster() { }
        public Monster(int level, MonsterKind monsterKind, params MonsterAttribute[] monsterAttributes)
        {
            Name = Enum.GetName(typeof(MonsterKind), monsterKind);
            Kind = monsterKind;
            Level = level;
            InitByLevelNorm(level);
            JiggleAttributes();
            if (KindsThatUseTechnology.Contains(monsterKind))
            {
                InitEquipment();
            }
            Modify(monsterAttributes);
            Glyph.Character = DefaultMonsterTile(Kind);
            Glyph.Color = DefaultMonsterColor(Kind);
            Glyph.Outline = true;
        }
        public void Modify(params MonsterAttribute[] monsterAttributes)
        {
            foreach (MonsterAttribute attribute in monsterAttributes)
            {
                switch(attribute)
                {
                    case MonsterAttribute.Small:
                        BrawnBase -= 3;
                        ReflexesBase += 2;
                        break;
                    case MonsterAttribute.Minion:
                        HealthMax = HealthMax / 2;
                        break;
                }
            }
        }
        public void InitEquipment()
        {
            Weapon w = Weapon.RandomWeapon(Rules.RarityRoll(Level));
            Armor a = Armor.RandomArmor(Rules.RarityRoll(Level));
            Inventory.Add(a);
            Inventory.Equip(0);
            Inventory.Add(w);
            Inventory.Equip(1);
        }
        public override bool Act()
        {
            if (IsStunned) { return true; }
            int dx = Math.Sign(Program.player.x - this.x);
            int dy = Math.Sign(Program.player.y - this.y);
            bool moved = Program.location.Move(this, dx, dy);
            if (!moved)
            {
                moved = Program.location.Move(this, dx, 0);
            }
            if (!moved)
            {
                moved = Program.location.Move(this, 0, dy);
            }
            // else wait
            return true;
        }
        public override Weapon MeleeAttack()
        {
            Weapon claw = new Weapon(MeleeWeaponType.Claw);
            return claw;
        }
        public override int Soak()
        {
            return SoakBase;
        }
        public static Monster RandomMonster(int level)
        {
            List<MonsterKind> monsterKinds = Enum.GetValues(typeof(MonsterKind)).Cast<MonsterKind>().ToList();
            MonsterKind m = monsterKinds[Dice.Roll("1d" + monsterKinds.Count.ToString() + "-1")];
            return new Monster(level, m);
        }
    }
}
