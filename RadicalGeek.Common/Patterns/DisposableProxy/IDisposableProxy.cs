using System;
using System.Linq.Expressions;

namespace RadicalGeek.Common.Patterns.DisposableProxy
{
    public interface IDisposableProxy<TDisposable> where TDisposable : IDisposable
    {
        TDisposable CreateDisposableImplementation();
        TResult Call<TResult>(Func<TDisposable, TResult> func);
        void Call(Action<TDisposable> action);
    }
}
