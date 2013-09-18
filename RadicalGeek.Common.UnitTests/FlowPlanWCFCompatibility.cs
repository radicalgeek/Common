using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Threading;
//using Dollar.Common.ProcessFlow;
using RadicalGeek.Common.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RadicalGeek.Common.Reflection;

namespace RadicalGeek.Common.UnitTests
{
//    [TestClass]
//    public class FlowPlanWcfCompatibility
//    {
//        private ServiceHost serviceHost;
//        private ServiceEndpoint endPoint;
//
//        [TestInitialize]
//        public void Init()
//        {
//            var baseAddress = new Uri("http://localhost:16008/FlowPlan");
//            serviceHost = new ServiceHost(typeof(FlowPlanService), baseAddress);
//            Binding binding = new WSHttpBinding();
//            var address = new EndpointAddress(baseAddress);
//            endPoint = serviceHost.AddServiceEndpoint(typeof(IFlowPlanContract), binding, address.Uri);
//
//            var smb = new ServiceMetadataBehavior { HttpGetEnabled = true };
//            serviceHost.Description.Behaviors.Add(smb);
//
//            foreach (OperationDescription operation in endPoint.Contract.Operations)
//                operation.Behaviors.Find<DataContractSerializerOperationBehavior>().DataContractResolver =
//                    new TypeResolver();
//        }
//
//        [TestCleanup]
//        public void TearDown()
//        {
//            if (serviceHost != null)
//                ((IDisposable)serviceHost).Dispose();
//        }
//
//        [TestMethod]
//        public void SendAndReceiveAFlowPlan()
//        {
//            using (var client = new ProxyClient(endPoint))
//            {
//                endPoint.Name = client.Endpoint.Name;
//                serviceHost.Open();
//                while (serviceHost.State != CommunicationState.Opened)
//                    Thread.Sleep(1);
//
//                Assert.IsTrue(client.TakeFlowPlan(FlowPlanTestHelper.CreateSimpleFlowPlan()));
//                FlowPlan test = client.GiveFlowPlan();
//                Assert.AreEqual(FlowPlanTestHelper.TestFlowPlanName, test.Name);
//                client.Close();
//                serviceHost.Close();
//            }
//        }
//    }

//    public class ProxyClient : ClientBase<IFlowPlanContract>, IFlowPlanContract
//    {
//        public ProxyClient(ServiceEndpoint endpoint)
//            : base(endpoint.MemberwiseClone())
//        {
//        }
//
//        public bool TakeFlowPlan(FlowPlan flowPlan)
//        {
//            return Channel.TakeFlowPlan(flowPlan);
//        }
//
//        public FlowPlan GiveFlowPlan()
//        {
//            return Channel.GiveFlowPlan();
//        }
//    }
//
//    [ServiceContract]
//    public interface IFlowPlanContract
//    {
//        [OperationContract]
//        bool TakeFlowPlan(FlowPlan flowPlan);
//        [OperationContract]
//        FlowPlan GiveFlowPlan();
//    }
//
//    class FlowPlanService : IFlowPlanContract
//    {
//        public bool TakeFlowPlan(FlowPlan flowPlan)
//        {
//            return flowPlan.Name == FlowPlanTestHelper.TestFlowPlanName;
//        }
//
//        public FlowPlan GiveFlowPlan()
//        {
//            return FlowPlanTestHelper.CreateSimpleFlowPlan();
//        }
//    }
}
