using System;
using System.Collections.Generic;

namespace RadicalGeek.Common.Collections
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "It inherits from List, not Collection.")]
    public class PagingList<T> : List<T>
    {
        private readonly object[] arguments;
        private readonly int pageLength;
        private readonly Func<object[], int, int, List<T>> populationDelegate;
        private int currentPage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageLength"></param>
        /// <param name="populationDelegate">A function taking an array of arguments, start record, end record and list object to populate</param>
        /// <param name="arguments"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public PagingList(int pageLength, Func<object[], int, int, List<T>> populationDelegate, params object[] arguments)
        {
            this.pageLength = pageLength;
            this.populationDelegate = populationDelegate;
            this.arguments = arguments;
            Populate();
        }

        public new T this[int index]
        {
            get
            {
                if (index < pageLength * currentPage)
                {
                    // go back to previous page
                    if (currentPage > 0)
                    {
                        currentPage--;
                        Populate();
                    }
                    else throw new ArgumentOutOfRangeException("index");
                }
                if (index >= pageLength * (currentPage + 1))
                {
                    // go forward to next page
                    if (Count == pageLength)
                    {
                        currentPage++;
                        Populate();
                    }
                    else throw new ArgumentOutOfRangeException("index");
                }
                return base[index % pageLength];
            }
        }

        private void Populate()
        {
            // if pageLength = 10 and currentPage = 0
            // 0, 9
            // if pageLength = 10 and currentPage = 1
            // 10, 19
            Clear();
            int startRecord = currentPage * pageLength;
            int endRecord = ((currentPage + 1) * pageLength) - 1;
            AddRange(populationDelegate(arguments, startRecord, endRecord));
        }
    }
}
