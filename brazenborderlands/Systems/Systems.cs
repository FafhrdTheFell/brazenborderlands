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
    }

}
