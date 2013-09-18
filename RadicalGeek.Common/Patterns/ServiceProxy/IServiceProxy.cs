using System;

namespace RadicalGeek.Common.Patterns.ServiceProxy
{
    public interface IServiceProxy<TContract>
    {
        TContract CreateSessionfullClient();
        TResult Call<TResult>(Func<TContract, TResult> func);
        void Call(Action<TContract> action);
    }
}
