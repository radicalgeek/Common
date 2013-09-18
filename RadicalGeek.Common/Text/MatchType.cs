using System;
using System.Collections.Generic;
using System.Linq;

namespace RadicalGeek.Common.Text
{
    public struct MatchType
    {
        private readonly MatchTypeEnum value;

        [Flags]
        private enum MatchTypeEnum
        {
            Numeric = 1,
            Alphabetic = 2,
            Address = 4,
            Space = 8,
            Protocol = 16
        }

        /// <summary>
        /// 0123456789
        /// </summary>
        public static MatchType Numeric = new MatchType(MatchTypeEnum.Numeric);
        /// <summary>
        /// abcdefghijklmnopqrstuvwxyz
        /// </summary>
        public static MatchType Alphabetic = new MatchType(MatchTypeEnum.Alphabetic);
        /// <summary>
        /// Equivalent to Numeric | Alphabetic
        /// </summary>
        public static MatchType Alphanumeric = new MatchType(MatchTypeEnum.Alphabetic | MatchTypeEnum.Numeric);
        /// <summary>
        /// Equivalent to Alphabetic | Space
        /// </summary>
        public static MatchType Words = new MatchType(MatchTypeEnum.Alphabetic | MatchTypeEnum.Space);
        /// <summary>
        /// .-@
        /// </summary>
        public static MatchType Domain = new MatchType(MatchTypeEnum.Address);
        /// <summary>
        /// Equivalent to Numeric | Alphabetic | Address
        /// </summary>
        public static MatchType Email = new MatchType(MatchTypeEnum.Alphabetic | MatchTypeEnum.Numeric | MatchTypeEnum.Address);
        /// <summary>
        /// space, tab, carriage return, newline
        /// </summary>
        public static MatchType Space = new MatchType(MatchTypeEnum.Space);
        /// <summary>
        /// :/
        /// </summary>
        public static MatchType Protocol = new MatchType(MatchTypeEnum.Protocol);
        /// <summary>
        /// Equivalent to Numeric | Alphabetic | Address | Protocol
        /// </summary>
        public static MatchType Url = new MatchType(MatchTypeEnum.Numeric | MatchTypeEnum.Alphabetic | MatchTypeEnum.Address | MatchTypeEnum.Protocol);

        public static readonly Dictionary<int, string> MatchCharacters =
            new Dictionary<int, string>
                {
                    {1, "0123456789"},
                    {2, "abcdefghijklmnopqrstuvwxyz"},
                    {4, ".-_@"},
                    {8, " \t\r\n"},
                    {16, ":/"}
                };

        public static int MaxValue = Enum.GetValues(typeof(MatchTypeEnum)).Cast<int>().Max();

        private MatchType(MatchTypeEnum value)
            : this()
        {
            this.value = value;
        }

        public static implicit operator int(MatchType value)
        {
            return (int)value.value;
        }

        public static implicit operator MatchType(int value)
        {
            return new MatchType((MatchTypeEnum)value);
        }
    }
}