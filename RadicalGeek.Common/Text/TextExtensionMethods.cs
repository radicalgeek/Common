using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using RadicalGeek.Common.Cryptography;
using Rijndael = RadicalGeek.Common.Cryptography.Rijndael;

namespace RadicalGeek.Common.Text
{
    public static class TextExtensionMethods
    {
        private static readonly List<EncryptionKey> encryptionKeys = new List<EncryptionKey>
                                                                         {
                                                                             // Nexus
                                                                             new EncryptionKey
                                                                                 {
                                                                                     Identity = EncryptionKeySet.PCI,
                                                                                     PassPhrase = "18a@777978523677719L",
                                                                                     SaltValue = "pOl79181200AA0280312",
                                                                                     InitVector = "@2BXc3D4eYF6g7L812"
                                                                                 },
                                                                             // Everything else
                                                                             new EncryptionKey
                                                                                 {
                                                                                     Identity = EncryptionKeySet.General,
                                                                                     PassPhrase = "17a@000978523677719L",
                                                                                     SaltValue = "pOl77188200AZ0280012",
                                                                                     InitVector = "@1B2c3D4e5F6g7H812"
                                                                                 }
                                                                         };

        /// <summary>
        /// Returns "s" when the value is not 1, and "" when value is 1. Useful for pluralising.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "s"),
         SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "s",
             Justification = "Makes more sense in lower case.")]
        public static string s(this int value)
        {
            return value.pl("", "s");
        }

        /// <summary>
        /// Returns "es" when the value is not 1, and "" when value is 1. Useful for pluralising.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "es")]
        public static string es(this int value)
        {
            return value.pl("", "es");
        }

        /// <summary>
        /// Returns "ies" when the value is not 1, and "y" when value is 1. Useful for pluralising.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "ies"),
         SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ies")]
        public static string ies(this int value)
        {
            return value.pl("y", "ies");
        }

        /// <summary>
        /// Returns an appropriate plural string when int is not 1, e.g. string.Format("{0} m{1}e", mouseCount, mouseCount.pl("ous", "ic"));
        /// </summary>
        /// <param name="value"></param>
        /// <param name="singular"></param>
        /// <param name="plural"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "pl",
            Justification = "Makes more sense in lower case.")]
        public static string pl(this int value, string singular, string plural)
        {
            return value == 1 ? singular : plural;
        }

        public static int CountOf(this string candidate, string pattern, bool allowOverlapping = false)
        {
            int count = 0;
            int i = 0;
            while ((i = candidate.IndexOf(pattern, i)) != -1)
            {
                i += allowOverlapping ? 1 : pattern.Length;
                count++;
            }
            return count;
        }

        [Obsolete("It is possible to find an encrypted string that decrypts to garbage with the incorrect key.")]
        public static string Decrypt(this string candidate)
        {
            string decryptedString = null;
            int i = 0;
            if (!string.IsNullOrWhiteSpace(candidate))
            {
                while (decryptedString == null && i < encryptionKeys.Count)
                {
                    try
                    {
                        decryptedString = Rijndael.Decrypt(candidate, encryptionKeys[i].PassPhrase,
                                                           encryptionKeys[i].SaltValue, encryptionKeys[i].InitVector);
                    }
                    catch (CryptographicException)
                    {
                        decryptedString = null;
                    }
                    catch (FormatException)
                    {
                        decryptedString = null;
                    }
                    i++;
                }
            }
            return decryptedString ?? candidate;
        }

        public static string Decrypt(this string candidate, EncryptionKeySet keySet)
        {
            string decryptedString = null;
            if (!string.IsNullOrWhiteSpace(candidate))
            {
                EncryptionKey encryptionKey = encryptionKeys.FirstOrDefault(k => k.Identity == keySet);
                if (encryptionKey == null)
                {
                    throw new ArgumentException(string.Format("No key set found for EncryptionKeySet {0}", keySet), "keySet");
                }

                try
                {
                    decryptedString = Rijndael.Decrypt(candidate, encryptionKey.PassPhrase,
                                                       encryptionKey.SaltValue, encryptionKey.InitVector);
                }
                catch (CryptographicException)
                {
                    decryptedString = null;
                }
                catch (FormatException)
                {
                    decryptedString = null;
                }
            }
            return decryptedString ?? candidate;
        }


        public static string MatchCharacters(this string candidate, MatchType matchType)
        {
            return MatchCharacters(candidate, matchType, false);
        }

        private static readonly int matchTypeCeiling = MatchType.MaxValue;

        public static string MatchCharacters(this string candidate, MatchType matchType, bool caseSensitive)
        {
            string chars = string.Empty;
            for (int i = 1; i <= matchTypeCeiling; i *= 2)
                if ((matchType & i) == i)
                    chars += MatchType.MatchCharacters[i];

            if (!caseSensitive) chars += chars.ToUpper();
            return MatchCharacters(candidate, chars);
        }

        public static string MatchCharacters(this string candidate, string chars)
        {
            return new string(candidate.Where(chars.Contains).ToArray());
        }

        public static string Truncate(this string candidate, int length)
        {
            return candidate.Substring(0, Math.Min(length, candidate.Length));
        }
    }
}
