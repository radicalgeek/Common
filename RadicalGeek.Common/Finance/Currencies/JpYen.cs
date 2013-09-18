namespace RadicalGeek.Common.Finance.Currencies
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Jp", Justification = "Jp is not an acronym.")]
    public class JpYen : Currency
    {
        public override string Symbol
        {
            get { return "¥"; }
        }

        public override string IsoCode
        {
            get { return "JPY"; }
        }
    }
}