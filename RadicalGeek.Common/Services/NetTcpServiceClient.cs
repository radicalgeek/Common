using System;
using System.ServiceModel;

namespace RadicalGeek.Common.Services
{
    public abstract class NetTcpServiceClient<TInterface> : GenericServiceClientBaseClass<TInterface> where TInterface : class
    {
        protected NetTcpServiceClient(string hostAddress, int tcpPort)
            : base(() =>
                      {
                          NetTcpBinding binding = new NetTcpBinding
                              {
                                  TransferMode = TransferMode.Buffered,
                                  MaxBufferSize = 65536,
                                  MaxReceivedMessageSize = 104857600
                              };
                          binding.Security.Mode = SecurityMode.None;
                          return binding;
                      },
                  new EndpointAddress(string.Format("net.tcp://{0}:{1}", hostAddress, tcpPort)))
        {
        }

        private static readonly Lazy<Type> client = new Lazy<Type>(ServiceClientGenerator.GetClient<TInterface, NetTcpServiceClient<TInterface>>);

        public static TInterface Get(string hostAddress, int tcpPort)
        {
            return (TInterface)Activator.CreateInstance(client.Value, hostAddress, tcpPort);
        }
    }
}