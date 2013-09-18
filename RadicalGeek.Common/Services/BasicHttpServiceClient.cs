using System;
using System.ServiceModel;

namespace RadicalGeek.Common.Services
{
    [Obsolete("Only use this if you need to use an old-style .ASMX service")]
    public abstract class BasicHttpServiceClient<TInterface> : GenericServiceClientBaseClass<TInterface> where TInterface : class
    {
        protected BasicHttpServiceClient(string uri)
            : base(() =>
                       {
                           BasicHttpBinding binding = new BasicHttpBinding
                               {
                                   TransferMode = TransferMode.Streamed,
                                   MaxBufferSize = 65536,
                                   MaxReceivedMessageSize = 104857600
                               };
                           binding.Security.Mode = BasicHttpSecurityMode.None;
                           return binding;
                       },
                   new EndpointAddress(uri))
        {
        }

        private static readonly Lazy<Type> client = new Lazy<Type>(ServiceClientGenerator.GetClient<TInterface, BasicHttpServiceClient<TInterface>>);

        public static TInterface Get(string hostAddress, int port)
        {
            return (TInterface)Activator.CreateInstance(client.Value,string.Format("http://{0}:{1}/", hostAddress, port));
        }

        public static TInterface Get(Uri uri)
        {   
            return (TInterface)Activator.CreateInstance(client.Value, uri.ToString());
        }
    }
}