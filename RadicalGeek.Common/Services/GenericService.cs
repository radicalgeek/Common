using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceProcess;

namespace RadicalGeek.Common.Services
{
    public sealed class GenericService<TInterface, TImplementation> : ServiceBase
        where TInterface : class
        where TImplementation : class, TInterface, new()
    {
        private void InitializeComponent()
        {
            ServiceName = string.Format("GenericService{0}", typeof(TInterface).Name);
        }

        public static GenericService<TInterface, TImplementation> Start(ushort? httpPort = null, ushort? tcpPort = null, bool exposeWsdl = false, bool enableMex = false)
        {
            GenericService<TInterface, TImplementation> service = new GenericService<TInterface, TImplementation>(httpPort, tcpPort, exposeWsdl, enableMex);
            if (!Environment.UserInteractive)
                Run(service);
            else
            {
                Console.WriteLine("Starting Service for {0} : {1}", typeof(TImplementation).Name, typeof(TInterface).Name);
                service.OnStart(null);
                Console.WriteLine("Service Started");
            }
            return service;
        }

        private readonly ushort? httpPort;
        private readonly ushort? tcpPort;
        private ServiceHost host;
        private readonly string machineName = Environment.MachineName;
        private readonly bool exposeWsdl;
        private readonly bool enableMex;

        public GenericService(ushort? httpPort = null, ushort? tcpPort = null, bool exposeWsdl = false, bool enableMex = false)
        {
            this.httpPort = httpPort;
            this.tcpPort = tcpPort;
            this.exposeWsdl = exposeWsdl;
            this.enableMex = enableMex;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (host == null)
            {
                host = new ServiceHost(typeof(TImplementation));

                if (exposeWsdl) AddWsdlBehaviour();
                if (tcpPort.HasValue) AddTcpEndpoint();
                if (httpPort.HasValue) AddHttpEndpoint();

                AddNamedPipesEndpoint();

                if (Environment.UserInteractive && Console.Out != null)
                    foreach (ServiceEndpoint serviceEndpoint in host.Description.Endpoints)
                        Console.WriteLine("{0} at {1}", serviceEndpoint.Binding.GetType().Name, serviceEndpoint.Address);
            }
            host.Open();
        }

        private void AddWsdlBehaviour()
        {
            ServiceMetadataBehavior serviceMetadataBehavior = new ServiceMetadataBehavior();
            serviceMetadataBehavior.HttpGetEnabled = true;
            serviceMetadataBehavior.HttpGetUrl =
                    new Uri(string.Format("http://{0}:{1}/wsdl", machineName, httpPort));
            serviceMetadataBehavior.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;

            host.Description.Behaviors.Add(serviceMetadataBehavior);
        }

        private void AddHttpEndpoint()
        {
            BasicHttpBinding httpBinding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            httpBinding.TransferMode = TransferMode.Buffered;
            host.AddServiceEndpoint(typeof(TInterface), httpBinding, new Uri(string.Format("http://{0}:{1}", machineName, httpPort)));

            if (enableMex)
            {
                Binding mexHttpBinding = MetadataExchangeBindings.CreateMexHttpBinding();
                host.AddServiceEndpoint(typeof(TInterface), mexHttpBinding,
                                        new Uri(string.Format("http://{0}:{1}/mex", machineName,
                                                              httpPort)));
            }
        }

        private void AddNamedPipesEndpoint()
        {
            NetNamedPipeBinding pipeBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            pipeBinding.MaxConnections = 100;
            pipeBinding.TransferMode = TransferMode.Buffered;
            host.AddServiceEndpoint(typeof(TInterface), pipeBinding, new Uri(string.Format("net.pipe://{0}/{1}", machineName, typeof(TInterface).Name)));
            if (enableMex)
            {
                Binding mexNamedPipeBinding = MetadataExchangeBindings.CreateMexNamedPipeBinding();
                host.AddServiceEndpoint(typeof(TInterface), mexNamedPipeBinding,
                                        new Uri(string.Format("net.pipe://{0}/{1}/mex", machineName,
                                                              typeof(TInterface).Name)));
            }
        }

        private void AddTcpEndpoint()
        {
            NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.None);
            tcpBinding.ListenBacklog = 100;
            tcpBinding.MaxConnections = 100;
            tcpBinding.TransferMode = TransferMode.Buffered;
            host.AddServiceEndpoint(typeof(TInterface), tcpBinding, new Uri(string.Format("net.tcp://{0}:{1}", machineName, tcpPort)));
        }

        protected override void OnStop()
        {
            host.Close();
        }
    }
}
