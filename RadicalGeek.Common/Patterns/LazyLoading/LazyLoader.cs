using System;

namespace RadicalGeek.Common.Patterns.LazyLoading
{
    [Obsolete("When using .Net 4 or above, consider whether you want to use this (fairly lightweight) or the built-in Lazy<T> (readonly)")]
    public class LazyLoader<T>
    {
        private bool isAssigned;
        private T value;
        private Func<T> initialiser;
        private readonly object lockObj = new object();

        public LazyLoader(Func<T> initialiser)
        {
            if (initialiser == null) throw new ArgumentNullException("initialiser");
            this.initialiser = initialiser;
        }

        private LazyLoader(T value)
        {
            isAssigned = true;
            this.value = value;
        }

        public static implicit operator T(LazyLoader<T> source)
        {
            return source.GetValue();
        }

        public static implicit operator LazyLoader<T>(T source)
        {
            return new LazyLoader<T>(source);
        }

        public override string ToString()
        {
            return GetValue().ToString();
        }

        private T GetValue()
        {
            lock (lockObj)
            {
                if (!isAssigned)
                {
                    value = initialiser();
                    initialiser = null;
                    isAssigned = true;
                }

                return value;
            }
        }
    }
}
