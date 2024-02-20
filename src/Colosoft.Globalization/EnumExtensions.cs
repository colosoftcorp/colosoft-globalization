using System;
using System.Collections.Generic;

namespace Colosoft
{
    internal static class EnumExtensions
    {
        public static IEnumerable<long> GetIndividualFlags(this Enum flags)
        {
            var flagsLong = Convert.ToInt64(flags, System.Globalization.CultureInfo.InvariantCulture);
            for (int i = 0; i < sizeof(long) * 8; i++)
            {
                var individualFlagPosition = (long)Math.Pow(2, i);
                var individualFlag = flagsLong & individualFlagPosition;
                if (individualFlag == individualFlagPosition)
                {
                    yield return individualFlag;
                }
            }
        }
    }
}
