using RogueSharp.DiceNotation;
using System;
using System.Collections.Generic;
using System.Text;

namespace brazenborderlands
{
    internal class Rules
    {
        public static int StandardRoll()
        {
            return Dice.Roll("1d32 + 1d12 - 23");
        }

        public static int OpposedRoll(int checkerLevel, int target)
        {
            return checkerLevel - target + StandardRoll();
        }

        public static int RarityRoll(int level)
        {
            int rolltotal = 0;
            int rolled = 6;
            while(rolled == 6)
            {
                rolled = Dice.Roll("1d6");
                rolltotal += rolled;
            }
            return (rolltotal + level - 1) / 6;
        }

    }
    internal class Helpers
    {
        public static int RollXSidedDice(int sides, int number)
        {
            return Dice.Roll(number.ToString() + "d" + sides.ToString());
        }
        public static int RollXSidedDie(int sides)
        {
            return RollXSidedDice(sides, 1);
        }
        public static int PlusOrMinus(int value)
        {
            string range = (1 + 2 * value).ToString();
            return Dice.Roll("1d" + range + "-" + value.ToString() + " -1");
        }
        public static T RandomEnumValue<T>() where T : Enum
        {
            var v = Enum.GetValues(typeof(T));
            int x = Dice.Roll("1d" + v.Length + "-1");
            return (T)v.GetValue(x);
        }
        public static int BoundsCheck(int value, int min, int max)
        {
            if (value > max || value < min)
            {
                throw new NotSupportedException("Attempt to set value " + value.ToString() +
                    "outside supported range [" + min.ToString() + "," + max.ToString() + "].");
            };
            return value;
        }
        public static int BoundsCheck(int value)
        {
            return BoundsCheck(value, 1, 99);
        }
    }
}

