using System;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RadicalGeek.Common.Services;

namespace RadicalGeek.Common.UnitTests
{
    [TestClass]
    public class ServiceClientGeneratorTests
    {
        [TestMethod]
        public void CreateNamedPipesClient()
        {
            IMyService myService = NamedPipesServiceClient<IMyService>.Get();
        }

        [TestMethod]
        public void CreateNetTcpClient()
        {
            IMyService myService = NetTcpServiceClient<IMyService>.Get("localhost", 123);
        }

        [TestMethod]
        public void CreateBasicHttpClientWithNoPath()
        {
            IMyService myService = BasicHttpServiceClient<IMyService>.Get("localhost", 321);
        }
        
        [TestMethod]
        public void CreateBasicHttpClientWithUri()
        {
            IMyService myService = BasicHttpServiceClient<IMyService>.Get(new Uri("http://localhost:2343/services/blah.svc"));
        }
        
        [TestMethod]
        public void CreateWsHttpClientWithNoPath()
        {
            IMyService myService = WsHttpServiceClient<IMyService>.Get("localhost", 3210);
        }
        
        [TestMethod]
        public void CreateWsHttpClientWithUri()
        {
            IMyService myService = WsHttpServiceClient<IMyService>.Get(new Uri("http://localhost:2343/services/blah.svc"));
        }
    }

    [ServiceContract]
    public interface IMyService
    {
        [OperationContract]
        void Hello();
        [OperationContract]
        int World(string hello);
        [OperationContract]
        int Complex(string hello, int world, char[] blahblahblah);
    }
}
