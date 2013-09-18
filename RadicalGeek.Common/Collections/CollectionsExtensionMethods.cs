using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RadicalGeek.Common.Collections
{
    public static class CollectionsExtensionMethods
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            return dict.ContainsKey(key) ? dict[key] : default(TValue);
        }

        public static bool ContainsSameItemsAs<T>(this IEnumerable<T> target, IEnumerable<T> candidate)
        {
            if (target == null) return false;
            if (candidate == null) return false;
            T[] targetArray = target.ToArray(); // Performance: Use ToArray to enumerate target only once
            T[] candidateArray = candidate.ToArray(); // Performance: Use ToArray to enumerate candidate only once
            if (targetArray.Length != candidateArray.Length) return false; // Performance: Do this on the arrays because Count() would be an extra enumeration.
            return !candidateArray.Any(item => !targetArray.Contains(item)) &&
                   !targetArray.Any(item => !candidateArray.Contains(item));
        }

        /// <summary>
        /// Executes action for each item in this IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable != null && action != null)
                lock (enumerable)
                    foreach (T t in enumerable)
                        action(t);
        }

        private struct ListSearchConfiguration
        {
            public int ListSize;
            public int LowerBound;
            public int UpperBound;
            public int SectionSize;
            public int SectionCount;
            public int InitialCutSize;
        }

        static readonly Dictionary<IList, ListSearchConfiguration> listSearchConfigCache = new Dictionary<IList, ListSearchConfiguration>();

        /// <summary>
        /// Returns an array of two integers describing the location of the item in the IList (including arrays). If the item exists, both integers will be the exact position of the value. If the item does not exist, the integers will describe the positions adjacent to where the item would be. For example, in [1, 3] searching for 2 would return [0, 1] and searching for 3 would return [1, 1] while searching for 4 would return [1, 2]
        /// </summary>
        /// <returns>int[2]</returns>
        public static int[] FindIndexBinary<T>(this IList list, T item, IComparer<T> comparer = null) where T : IComparable
        {
            if (comparer == null)
                comparer = Comparer<T>.Default;

            ListSearchConfiguration configuration = default(ListSearchConfiguration);
            if (listSearchConfigCache.ContainsKey(list))
                configuration = listSearchConfigCache[list];

            if (configuration.ListSize == 0 || list.Count > configuration.SectionCount * configuration.SectionSize ||
                Math.Abs(list.Count - configuration.ListSize) > configuration.SectionSize)
                if (listSearchConfigCache.ContainsKey(list))
                    listSearchConfigCache[list] = BuildListSearchConfiguration(list);
                else
                    listSearchConfigCache.Add(list, configuration = BuildListSearchConfiguration(list));

            if (item.CompareTo(list[configuration.LowerBound]) == -1)
                return new[] { -1, 0 };
            if (item.CompareTo(list[configuration.UpperBound]) == 1)
                return new[] { configuration.UpperBound, configuration.UpperBound + 1 };

            return FindIndexBinaryCore(list, item, comparer, configuration);
        }

        private static int[] FindIndexBinaryCore<T>(IList list, T item, IComparer<T> comparer,
                                                ListSearchConfiguration configuration) where T : IComparable
        {
            int startPoint = GetBestStartPoint(list, item, comparer, configuration);
            int[] result = new[] { 0, startPoint };
            int cutSize = configuration.InitialCutSize;
            int nextCompareResult = comparer.Compare(item, (T)list[result[1]]);
            while (true)
            {
                result[0] = result[1];

                if (nextCompareResult == 0)
                    return result;

                if (cutSize > 1)
                    cutSize /= 2;

                result[1] += cutSize * nextCompareResult;

                int compareResult = nextCompareResult;
                nextCompareResult = result[1] < configuration.LowerBound ? 1 : comparer.Compare(item, (T)list[result[1]]);

                if (compareResult == 1 &&
                    nextCompareResult == -1 &&
                    result[1] - result[0] == 1)
                    return result;
            }
        }

        private static int GetBestStartPoint<T>(IList list, T item, IComparer<T> comparer, ListSearchConfiguration configuration)
            where T : IComparable
        {
            int startPoint = configuration.UpperBound - configuration.InitialCutSize;
            for (int i = configuration.LowerBound; i <= configuration.UpperBound; i += configuration.SectionSize)
                if (comparer.Compare(item, (T)list[i]) == -1)
                {
                    startPoint = i - configuration.InitialCutSize;
                    break;
                }
            return startPoint;
        }

        private const int InitialSectionSize = 32768; // Should be a power of 2 for clean division.
        private const int MaximumSectionCount = 128; // This is the maximum granularity for GetBestStartPoint

        private static ListSearchConfiguration BuildListSearchConfiguration(IList list)
        {
            ListSearchConfiguration configuration = new ListSearchConfiguration();
            if (configuration.ListSize != list.Count)
            {
                if (list is Array)
                {
                    configuration.LowerBound = ((Array)list).GetLowerBound(0);
                    configuration.UpperBound = ((Array)list).GetUpperBound(0);
                }
                else
                {
                    configuration.LowerBound = 0;
                    configuration.UpperBound = list.Count - 1;
                }

                configuration.ListSize = list.Count;
                configuration.SectionSize = InitialSectionSize;

                while ((list.Count / configuration.SectionSize) > MaximumSectionCount)
                {
                    configuration.SectionSize *= 2;
                }
                while ((list.Count / configuration.SectionSize) < 1)
                {
                    configuration.SectionSize /= 2;
                }
                configuration.InitialCutSize = (configuration.SectionSize / 2);
                configuration.SectionCount = (list.Count / configuration.SectionSize);
            }
            return configuration;
        }
    }
}
