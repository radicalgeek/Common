using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace RadicalGeek.Common.Finance.Currencies
{
    public abstract class Currency
    {
        public abstract string Symbol { get; }
        public abstract string IsoCode { get; }

        private static Collection<Currency> currencyCache;
        private static IEnumerable<Currency> CurrencyCache
        {
            get
            {
                if (currencyCache == null)
                {
                    Assembly executingAssembly = Assembly.GetExecutingAssembly();
                    Type[] types = executingAssembly.GetTypes();
                    IEnumerable<Type> currencyTypes = types.Where(t => t.IsAssignableFrom(typeof(Currency)));
                    currencyCache = new Collection<Currency>(currencyTypes.Select(Activator.CreateInstance).Cast<Currency>().ToList());
                }
                return currencyCache;
            }
        } 

        public static Currency Find(string text)
        {
            return CurrencyCache.FirstOrDefault(c => text.StartsWith(c.Symbol, StringComparison.Ordinal) || text.EndsWith(c.IsoCode, StringComparison.Ordinal));
        }
    }
}