namespace RadicalGeek.Common.Finance.Currencies
{
    public class UsDollar : Currency
    {
        public override string Symbol
        {
            get { return "$"; }
        }

        public override string IsoCode
        {
            get { return "USD"; }
        }
    }
}