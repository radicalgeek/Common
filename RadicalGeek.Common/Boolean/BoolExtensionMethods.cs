namespace RadicalGeek.Common.Boolean
{
    public static class BoolExtensionMethods
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed"),
        System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "It's clear enough what b is.")]
        public static T IfTrue<T>(this bool b, T trueResult, T falseResult = default(T))
        {
            return b ? trueResult : falseResult;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed"),
        System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "It's clear enough what b is.")]
        public static T IfFalse<T>(this bool b, T falseResult, T trueResult = default(T))
        {
            return b ? trueResult : falseResult;
        }
    }
}
