using System;
using System.Collections.Generic;
using System.Linq;

namespace RadicalGeek.Common.Validation
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Luhn", Justification = "It is spelled correctly.")]
    public static class Luhn
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Luhn", Justification = "It is spelled correctly.")]
        public static bool LuhnCheck(this string cardNumber)
        {
            return LuhnCheck(cardNumber.Select(c => c - '0').ToArray());
        }

        static readonly int[] results = new[] { 0, 2, 4, 6, 8, 1, 3, 5, 7, 9 };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Luhn", Justification = "It is spelled correctly.")]
        private static bool LuhnCheck(this int[] digits)
        {
            if (digits == null) throw new ArgumentNullException("digits");
            for (int i = digits.Length % 2; i < digits.Length; i += 2)
                digits[i] = results[digits[i]];
            return digits.Sum() % 10 == 0;
        }

        private static IEnumerable<int> AddLuhnChecksum(int[] digits)
        {
            int[] result = new int[digits.Length + 1];
            int cs = 0;
            while (true)
            {
                digits.CopyTo(result, 0);
                result[digits.Length] = cs;
                if (result.LuhnCheck())
                {
                    digits.CopyTo(result, 0);
                    return result;
                }
                cs++;
            }
        }

        public static string AddLuhnChecksum(this string number)
        {
            return new string(AddLuhnChecksum(number.Select(c => c - '0').ToArray()).Select(c => (char)(c + '0')).ToArray());
        }
    }
}
