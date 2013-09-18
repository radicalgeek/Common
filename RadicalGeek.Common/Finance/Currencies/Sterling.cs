namespace RadicalGeek.Common.Finance.Currencies
{
    public class Sterling:Currency
    {
        public override string Symbol
        {
            get { return "£"; }
        }

        public override string IsoCode
        {
            get { return "GBP"; }
        }
    }
}