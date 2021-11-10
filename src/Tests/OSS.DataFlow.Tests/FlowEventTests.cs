
using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSS.DataFlow.Event;

namespace OSS.Tools.Tests.DataStack
{
    [TestClass]
    public class FlowEventTests
    {
        [TestMethod]
        public async Task DataStackTest()
        {
            var o = new FlowEventOption()
            {
                event_msg_key = "Test_flow_event_msg",
                flow_retry_times = 4, // 经过消息流重试次数
                func_retry_times = 1  //  当前执行方法内部直接串联循环重试次数
            };
            var flowProcessor = new FlowEventProcessor<TestCount,TestCount>(new TestEvent(), o);
            
            var countPara    = new TestCount() {count = 0};

            var countRes = await flowProcessor.Process(countPara);
            Assert.IsNull(countRes); // 首次抛出异常，拦截返回空

            await Task.Delay(5000);  // 异步消息队列消费缓冲

            // 总执行次数 = (flow_retry_times+1)*(func_retry_times+1) = (4+1)*(1+1) = 10
            Assert.IsTrue(countPara.count==10);// 默认消息队列实现是内存级，引用不变
        }
        
    }

    // 具体执行事件
    public class TestEvent:IFlowEvent<TestCount, TestCount>
    {
        public Task<TestCount> Execute(TestCount input)
        {
            input.count++;
            if (input.count < 10)
            {
                throw new ArgumentException("小于当前要求的条件");
            }
            return Task.FromResult(input);
        }

        public Task Failed(TestCount input)
        {
             Console.WriteLine(input.count);
             return Task.CompletedTask;
        }
    }
    public class TestCount
    {
        public int count { get; set; }
    }
}
