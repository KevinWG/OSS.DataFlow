
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSS.DataFlow;

namespace OSS.Tools.Tests.DataStack
{
    [TestClass]
    public class DataFlowTests
    {

        private static readonly IDataPublisher _normalFlowPublisher =
            DataFlow.DataFlowFactory.RegisterFlow("normal_flow", new MsgPoper());

        [TestMethod]
        public async Task DataStackTest()
        {
            var pushRes = await _normalFlowPublisher.Publish("normal_flow",new MsgData() {name = "test"});
            Assert.IsTrue(pushRes);

            await Task.Delay(2000);
        }




        private static readonly IDataPublisher _delegateFlowpusher = DataFlow.DataFlowFactory.RegisterFlow<MsgData>(
            "delegate_flow",
            async (data) =>
            {
                await Task.Delay(1000);
                Assert.IsTrue(data.name == "test");
                return true;
            });

        [TestMethod]
        public async Task DataStackFuncTest()
        {
            var pushRes = await _delegateFlowpusher.Publish("delegate_flow",new MsgData() {name = "test"});
            Assert.IsTrue(pushRes);
            await Task.Delay(2000);
        }





        [TestMethod]
        public async Task DataPublisherAndMultiSubscriberTest()
        {
            const string msgPSKey = "Publisher-Subscriber";
            var publisher = DataFlow.DataFlowFactory.CreatePublisher<MsgData>(new DataPublisherOption()
                {
                    SourceName = "NewSource"
                });


            DataFlow.DataFlowFactory.RegisterSubscriber<MsgData>(msgPSKey, async (data) =>
            {
                await Task.Delay(1000);
                Assert.IsTrue(data.name == "test");
                // DoSomething(data)   // 功能实现
                return true;// 消费成功
            });

            DataFlow.DataFlowFactory.RegisterSubscriber<MsgData>(msgPSKey, async (data) =>
            {
                await Task.Delay(1000);
                Assert.IsTrue(data.name == "test");
                return true;
            });

            var pushRes = await publisher.Publish(msgPSKey,new MsgData() {name = "test"});
            Assert.IsTrue(pushRes);

            await Task.Delay(2000);
        }
    }


    public class MsgData
    {
        public string name { get; set; }
    }


    public class MsgPoper : IDataSubscriber<MsgData>
    {
        public async Task<bool> Subscribe(MsgData data)
        {
            await Task.Delay(1000);
            Assert.IsTrue(data.name == "test");
            return true;
        }
    }
}
