using System;
using System.ServiceModel;

namespace RadicalGeek.Common.Services
{
    public abstract class NamedPipesServiceClient<TInterface> : GenericServiceClientBaseClass<TInterface> where TInterface : class
    {
        protected NamedPipesServiceClient()
            : base(() =>
                       {
                           NetNamedPipeBinding binding = new NetNamedPipeBinding
                               {
                                   TransferMode = TransferMode.Buffered,
                                   MaxBufferSize = 65536,
                                   MaxReceivedMessageSize = 104857600
                               };
                           binding.Security.Mode = NetNamedPipeSecurityMode.None;
                           return binding;
                       },
                   new EndpointAddress(string.Format("net.pipe://{0}/{1}", Environment.MachineName,
                                                     typeof(TInterface).Name)))
        { }

        private static readonly Lazy<Type> client = new Lazy<Type>(ServiceClientGenerator.GetClient<TInterface, NamedPipesServiceClient<TInterface>>);

        public static TInterface Get()
        {
            return (TInterface)Activator.CreateInstance(client.Value);
        }
    }
}
