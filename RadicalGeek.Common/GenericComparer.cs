using System;
using System.Collections.Generic;

namespace RadicalGeek.Common
{
    public class GenericComparer<T> : EqualityComparer<T>, IComparer<T>
    {
        private Func<T, T, bool> equals = (t1, t2) => (t1.GetHashCode() == t2.GetHashCode());
        private Func<T, int> getHashCode = t => t.GetHashCode();
        private Func<T, T, int> compare = (t1, t2) => (t1.GetHashCode() == t2.GetHashCode() ? 0 : t1.GetHashCode() > t2.GetHashCode() ? 1 : -1);

        public GenericComparer()
        {
        }

        public Func<T,T,int>  CompareMethod
        {
            get { return compare; }
            set { compare = value; }
        }

        public Func<T, T, bool> EqualsMethod
        {
            get { return equals; }
            set { equals = value; }
        }

        public Func<T, int> GetHashCodeMethod
        {
            get { return getHashCode; }
            set { getHashCode = value; }
        }

        public GenericComparer(Func<T,T,int> compare)
        {
            this.compare = compare;
        }

        public GenericComparer(Func<T, int> getHashCode)
        {
            this.getHashCode = getHashCode;
        }

        public GenericComparer(Func<T,T,bool> equals)
        {
            this.equals = equals;
        }  

        public GenericComparer(Func<T, int> getHashCode, Func<T, T, bool> equals)
            : this(getHashCode)
        {
            this.equals = equals;
        }

        public override bool Equals(T x, T y)
        {
            return equals(x, y);
        }

        public override int GetHashCode(T obj)
        {
            return getHashCode(obj);
        }

        public int Compare(T x, T y)
        {
            return compare(x, y);
        }
    }
}
