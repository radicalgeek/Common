using System;

namespace RadicalGeek.Common.Patterns.DisposableProxy
{
    public class DisposableProxy<T, TImplementation> : IDisposableProxy<T>
        where TImplementation : T, new()
        where T : IDisposable
    {
        public T CreateDisposableImplementation()
        {
            return Activator.CreateInstance<TImplementation>();
        }

        public TResult Call<TResult>(Func<T, TResult> func)
        {
            var implementation = CreateDisposableImplementation();
            using (implementation)
            {
                return func.Invoke(implementation);
            }
        }

        public void Call(Action<T> action)
        {
            var implementation = CreateDisposableImplementation();
            using (implementation)
            {
                action.Invoke(implementation);
            }
        }
    }
}
