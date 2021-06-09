﻿
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSS.DataFlow;

namespace OSS.Tools.Tests.DataStack
{
    [TestClass]
    public class DataStackTests
    {

        private static readonly IDataPublisher<MsgData> _normalFlowPublisher =
            DataFlowFactory.CreateFlow("normal_flow", new MsgPoper());

        [TestMethod]
        public async Task DataStackTest()
        {
            var pushRes = await _normalFlowPublisher.Publish(new MsgData() {name = "test"});
            Assert.IsTrue(pushRes);

            await Task.Delay(2000);
        }


        private static readonly IDataPublisher<MsgData> _delegateFlowpusher = DataFlowFactory.CreateFlow<MsgData>(
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
            var pushRes = await _delegateFlowpusher.Publish(new MsgData() {name = "test"});
            Assert.IsTrue(pushRes);
            await Task.Delay(2000);
        }


        [TestMethod]
        public async Task DataPublisherAndSubscriberTest()
        {
            var msgKey = "Publisher-Subscriber";

            var publisher =
                DataFlowFactory.CreatePublisher<MsgData>(msgKey, new DataPublisherOption() {SourceName = "NewSource"});
            DataFlowFactory.ReceiveSubscriber<MsgData>(msgKey, async (data) =>
            {
                await Task.Delay(1000);
                Assert.IsTrue(data.name == "test");
                return true;
            }, new DataFlowOption() {SourceName = "NewSource"});

            var pushRes = await publisher.Publish(new MsgData() {name = "test"});
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
