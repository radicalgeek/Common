//using Dollar.Common.ProcessFlow;
using RadicalGeek.Common.Structures;
using RadicalGeek.Common.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RadicalGeek.Common.UnitTests
{
//    [TestClass]
//    public class FlowPlanTests
//    {
//        public class TestJunction : Junction<string, int>
//        {
//            protected override int Action(ref string data)
//            {
//                switch (data)
//                {
//                    case "Hello":
//                        return 1;
//                    case "World":
//                        return 2;
//                    default:
//                        return 0;
//                }
//            }
//        }
//
//        [TestMethod]
//        public void SerializeAndDeserializeJunction()
//        {
//            (new TestJunction()).ToXmlString().ToObject<TestJunction>();
//        }
//
//        public class IntIncrementingJunction : Junction<int, None>
//        {
//            protected override None Action(ref int data)
//            {
//                data++;
//                return None.Value;
//            }
//        }
//
//        [TestMethod]
//        public void CheckValueTypeBoxingWorks()
//        {
//            FlowPlan flowPlan = new FlowPlan("Bob");
//            flowPlan.Junctions.Add(new IntIncrementingJunction());
//            flowPlan.SetStartJunction(flowPlan.Junctions[0]);
//            DataContainer<int> dataContainer = flowPlan.CreateDataObject(0);
//            flowPlan.Junctions[0].DataKey = dataContainer.Key;
//            flowPlan.ExecuteToEnd();
//            Assert.AreEqual(1, dataContainer.Value);
//        }
//    }
}
