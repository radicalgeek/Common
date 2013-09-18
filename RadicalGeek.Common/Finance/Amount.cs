using System;
using System.Globalization;
using System.Linq;
using RadicalGeek.Common.Finance.Currencies;

namespace RadicalGeek.Common.Finance
{
    public class Amount
    {
        public Currency Currency { get; private set; }
        private string Symbol { get; set; }
        public decimal Value { get; set; }

        public Amount(decimal value, Currency currency)
        {
            if (currency == null) throw new ArgumentNullException("currency");
            Currency = currency;
            Value = value;
            Symbol = currency.Symbol;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        public override string ToString()
        {
            return Value.ToString(string.Format("{0},#.00", Symbol), CultureInfo.CurrentCulture);
        }

        static readonly char[] allowedChars = "1234567890.".ToCharArray();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Convert.ToDecimal(System.String)")]
        public static Amount Parse(string text)
        {
            Currency currency = Currency.Find(text);
            string num = new string(text.Where(allowedChars.Contains).ToArray());
            return new Amount(Convert.ToDecimal(num), currency);
        }

        public static implicit operator Amount(string amount)
        {
            return Parse(amount);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validate it but don't throw an exception if it's invalid? Conflicting CA rules.")]
        public static implicit operator decimal(Amount amount)
        {
            return amount.Value;
        }
    }
}
