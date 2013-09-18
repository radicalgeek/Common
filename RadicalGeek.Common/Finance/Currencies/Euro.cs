namespace RadicalGeek.Common.Finance.Currencies
{
    public class Euro:Currency
    {
        public override string Symbol
        {
            get { return "€"; }
        }

        public override string IsoCode
        {
            get { return "EUR"; }
        }
    }
}