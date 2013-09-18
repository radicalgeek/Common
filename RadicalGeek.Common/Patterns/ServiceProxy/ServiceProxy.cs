using System;

namespace RadicalGeek.Common.Patterns.ServiceProxy
{
    /// <summary>
    /// The service proxy enables you to implement service calls without having the actual service client hard-coded.
    /// Create a private IServiceProxy{IYourServiceContract} field and a constructor to inject a concrete version of the Proxy.
    /// Then create a constructor to create a ServiceProxy{IYourServiceContract, YourServiceClient} as the default.
    /// Once this implementation has been put in place you can then inject a non-service based client for testing.
    /// The Class is not sealed so you can inherit from it making the default constructor cleaner.
    /// E.G. For the BounceProxyService you could create a BounceProxyServiceProxy class that inherits from ServiceProxy{IBounceProxyService, BounceProxyServiceClient}
    /// Then Create a new instance of BounceProxyServiceProxy as the concrete implementation
    /// </summary>
    /// <typeparam name="TContract">The Contract of a service</typeparam>
    /// <typeparam name="TClientImplementation">The Type that implements TContract</typeparam>
    public class ServiceProxy<TContract, TClientImplementation> : IServiceProxy<TContract> where TClientImplementation : TContract, new()
    {
        /// <summary>
        /// Use this method if you want to use a session based service or want to control the opening and closing of the client directly.
        /// You would need to use a using statement in the same way as using a Service client.
        /// </summary>
        /// <returns>The Client Implementation of the Contract</returns>
        public TContract CreateSessionfullClient()
        {
            return new TClientImplementation();
        }

        /// <summary>
        /// Use this method when calling a service's Method that returns a result.
        /// E.G. The BounceProxyService has a method called BounceReturnedItem that returns a bool, this would be called using the following line
        /// _ServiceProxy.Call{bool}(c => c.BounceReturnedItem(returnedItem));
        /// Using this method negates the need for a using statement as it is contained within the method itself
        /// </summary>
        /// <param name="func">The Lambda expression specifying which method is to be called on the service</param>
        /// <returns></returns>
        public TResult Call<TResult>(Func<TContract, TResult> func)
        {
            var client = CreateSessionfullClient();
            using (client as IDisposable)
            {
                return func.Invoke(client);
            }
        }

        /// <summary>
        /// Use this method when calling a service's Method that returns nothing.
        /// E.G. The BounceProxyService has a method called SetConnectionString this would be called using the following line
        /// _ServiceProxy.Call(c => c.SetConnectionString(encryptedConnectionString));
        /// Using this method negates the need for a using statement as it is contained within the method itself
        /// </summary>
        /// <param name="action">The Lambda expression specifying which method is to be called on the service</param>
        public void Call(Action<TContract> action)
        {
            var client = CreateSessionfullClient();
            using (client as IDisposable)
            {
                action.Invoke(client);
            }
        }
    }
}
