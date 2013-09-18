using System.Collections.Generic;

namespace RadicalGeek.Common.Numbers
{
    public static class NumbersExtensionMethods
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)")]
        public static string ToOrdinal(this int number)
        {
            Dictionary<int, string> ordinals = new Dictionary<int, string> { { 1, "st" }, { 2, "nd" }, { 3, "rd" } };
            string ordinal = "th";
            int numMod100 = number % 100;

            if (numMod100 < 10 || numMod100 > 20)
            {
                int numMod10 = number % 10;
                if (ordinals.ContainsKey(numMod10))
                    ordinal = ordinals[numMod10];
            }

            return string.Format("{0}{1}", number, ordinal);
        }

        /// <summary>
        /// Returns x percent of the given value, e.g. 25.PercentOf(4) = 1, and var interest = interestRate.PercentOf(principal)
        /// </summary>
        /// <param name="whole"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public static decimal PercentOf(this decimal percentage, decimal whole)
        {
            return whole * (percentage / 100);
        }

        /// <summary>
        /// Returns what percentage of a given whole this would be, e.g. 1.AsPercentOf(4) = 25, 1.AsPercentOf(20) = 5
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="whole"></param>
        /// <returns></returns>
        public static decimal AsPercentOf(this decimal candidate, decimal whole)
        {
            return (candidate / whole) * 100;
        }
    }
}
