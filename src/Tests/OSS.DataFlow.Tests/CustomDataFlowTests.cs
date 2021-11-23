
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSS.DataFlow;

namespace OSS.Tools.Tests.DataStack
{
    // 定义自定义消息存储适配器
    public class CustomMsgStorageProvider : IDataPublisherProvider
    {
        public IDataPublisher CreatePublisher(DataPublisherOption option)
        {
            return option.source_name == "CustomStorageQueue" ? new CustomMsgStoragePublisher() : null;
        }
    }
    //自定义存储的发布接口实现，可以自定义其他实现，如： RabbitMQ，Redis 等
    public class CustomMsgStoragePublisher : IDataPublisher
    {
        public Task<bool> Publish<TData>(string dataKey, TData data)
        {
            // 自定义实现消息插入实现

            // 这里为了测试直接内部执行 通知订阅者，正常情况在消息触发时，调用此方法
            return DataFlowManager.NotifySubscriber(dataKey, data);
        }
    }
    [TestClass]
    public class CustomDataFlowTests
    {
        private static readonly IDataPublisher _customMsgPublisher ;
        static CustomDataFlowTests()
        {
            //  正常应该在程序全局入口处注册，这里为了测试
            DataFlowManager.PublisherProvider = new CustomMsgStorageProvider();

            _customMsgPublisher = DataFlowFactory.RegisterFlow<MsgData>("custom-storage-msg", async (data) =>
            {
                //  执行订阅业务, 这里是委托方法的形式，也可以传入继承 IDataSubscriber<MsgData> 接口的实例
                await Task.Delay(1000);
                Assert.IsTrue(data.name == "test");
                return true;
            }, new DataPublisherOption { source_name = "CustomStorageQueue" });
        }
        [TestMethod]
        public async Task CustomMsgStorageFuncTest()
        {
            var pushRes = await _customMsgPublisher.Publish("custom-storage-msg", new MsgData() { name = "test" });
            Assert.IsTrue(pushRes);
            await Task.Delay(2000);
        }
    }
  
}
