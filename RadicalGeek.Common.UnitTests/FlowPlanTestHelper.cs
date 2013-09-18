//using Dollar.Common.ProcessFlow;

namespace RadicalGeek.Common.UnitTests
{
//    public class FlowPlanTestHelper
//    {
//        public static string TestFlowPlanName = "Test FlowPlan";
//
//        public static FlowPlan CreateSimpleFlowPlan()
//        {
//            FlowPlan plan = new FlowPlan(TestFlowPlanName);
//            DataContainer<OperationData> integer = plan.CreateDataObject(new OperationData(123));
//            SimpleJunction simpleJunction = plan.CreateFlowJunction<SimpleJunction>(integer);
//            simpleJunction.AddExitPath(true, simpleJunction);
//            return plan;
//        }
//
//        public class SimpleJunction : Junction<OperationData, bool>
//        {
//            protected override bool Action(ref OperationData data)
//            {
//                return data.Value++ % 2 > 0;
//            }
//        }
//    }

    public class OperationData // Note: In real usage it is not necessary to box single value types, the DataContainer and Junction classes handle this.
    {
        public decimal Value;

        public OperationData(decimal i)
        {
            Value = i;
        }

        public OperationData()
        {
        }
    }
}