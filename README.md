# OSS.DataFlow中间件

系统重构解耦的过程涉及不同领域服务分拆，或同一服务下实时响应部分和非响应部分分拆，分解后的各部分通过异步消息的流转传递，完成整体的业务逻辑，但是频繁的在业务层面直接调用不同消息队列的SDK，不够简洁  
OSS.Dataflow主要实现异步消息传递的过程抽象，在业务层面提供消息发布订阅的统一抽象接口，在业务逻辑分支之间，以简单的调用完成消息的传递，和具体的消息存储触发实现无关。  
同时，在底层的存储和触发层面提取接口，能够在系统的全局适配具体的消息基础设施。在这些接口之上，实现事件处理器，通过消息的重复投放，实现事件执行的容错补充机制。

## 消息流业务使用

使用时可以通过Nuget直接安装，也可以通过命令行： **Install-Package OSS.DataFlow**  
组件的使用非常简单，只需要关注：

  a. 消息发布者接口，由组件注册时返回，供业务方法调用传入消息体。  
  b. 消息订阅（消费）者接口实现或委托方法，在组件注册时传入。

具体示例：  
  a. 消息的发布订阅独立调用示例  
```csharp
	// 全局初始化，注入订阅者实现
	const string msgPSKey = "Publisher-Subscriber-MsgKey";
	DataFlowFactory.RegisterSubscriber<MsgData>(msgPSKey, async (data) =>
            {
                // 当前通过注入消费的委托方法，也可通过接口实现
                // DoSomething(data);
                return true;
            });

	//	获取发布者接口
	private static readonly IDataPublisher publisher = DataFlowFactory.CreatePublisher(); 

	//  业务方法中发布消息
	await publisher.Publish(msgPSKey,new MsgData() {name = "test"});
```
  b.  消息的流式调用示例  
```csharp
	// 直接注册消费实现并获取消息发布接口
	private static readonly IDataPublisher _delegateFlowpusher = 
        DataFlowFactory.RegisterFlow<MsgData>("delegate_flow",async (data) =>
            {
                // 当前通过注入消费的委托方法，也可通过接口实现
                // DoSomething(data);
                return true;
            });

	// 业务方法中发布消息
    await _delegateFlowpusher.Publish("normal_flow",new MsgData() {name = "test"});
```
如上，只需要获取发布者，并注入消费实现，即可完成整个消息的异步消费处理，同一个消息key可以注册多个消费实现，当有消息进入消费时，会并发处理。

## 消息底层存储适配扩展

前边介绍了业务接口的使用，和具体消息队列或数据库等隔离，这是对接业务层面的使用。因为业务场景不同，不同的项目对消息的响应速度和处理机制又各有需求，所以 OSS.DataFlow 同样提供了对接消息产品的扩展接口，方便使用者适配已有消息基础设施。  

1. 消息存储适配接口  
对于事件消息处理，需要关注两件事情：接收存储 和 消费触发。在类库中提供了 DataFlowManager 消息流管理类，用户可以通过实现IDataPublisherProvider接口，完成具体的存储实现。  
同时在不同的消息产品触发消费时（比如数据库定时任务或者RabbitMQ消费）， 调用通知方法（NotifySubscriber ），来触发通过类库注册的具体的业务订阅处理。  
```csharp
    // 消息流核心部件管理者
    public static class DataFlowManager
    {
        /// <summary>
        /// 自定义 数据流发布（存储）实现的 提供者
        /// </summary>
        public static IDataPublisherProvider PublisherProvider { get; set; }

        /// <summary>
        ///  通过自定义消息触发机制通知订阅者
        ///     调用时请做异常拦截，防止脏数据导致 msgData 类型错误
        /// </summary>
        /// <param name="msgDataKey"></param>
        /// <param name="msgData">消息内容，自定义触发时，请注意和注册订阅者的消费数据类型转换安全</param>
        /// <returns></returns>
        public static Task<bool> NotifySubscriber(string msgDataKey, object msgData)
        {
            ....
        }
    }
```
关于 IDataPublisherProvider
```csharp
 	public interface IDataPublisherProvider
    {
        /// <summary>
        /// 数据发布者
        /// </summary>
        /// <param name="option"></param>
        /// <returns> 返回消息发布接口实现 </returns>
        IDataPublisher CreatePublisher(DataPublisherOption option);
    }

	/// <summary>
    ///  数据的发布者
    /// </summary>
    public interface IDataPublisher
    {
        /// <summary>
        /// 推进数据(存储具体消息队列或者数据库实现)
        /// </summary>
        /// <param name="dataKey"></param>
        /// <param name="data"></param>
        /// <returns>是否推入成功</returns>
        Task<bool> Publish<TData>(string dataKey,TData data);
    }
```
可以看到 IDataPublisher 接口负责具体的存储实现，可以根据 DataPublisherOption 的 source_name 业务属性实现对不同业务需求返回不同的具体实现。  

 2. 默认实现介绍

借助.Net 自身的内存消息队列，在类库中提供了默认的内部消息存储转发实现（内存级别），使用者可以自行实现扩展相关接口并进行全局配置。  
内置的.Net Core消息队列， 设置了默认1个队列，最大并发为32线程。 如果需要可以通过设置DataPublisherOption的source_name，类库将会为每个source_name 创建独立的内存队列。  

## 回流（重复执行）事件处理器
在有些比较重要的业务处理中，如果发生异常（如网络超时）等操作，会要求过段时间后重复执行进行错误补偿，借助OSS.DataFlow 类库的消息存储和转发接口，提供了  FlowEventProcessor<TIn, TOut>   的事件处理器，在事件处理器内部完成了异常拦截重试的封装。  
具体的过程就是当异常发生时，处理器通过将入参包装（FlowEventInput<TIn>）通过消息流保存，具体的重试触发实现间隔，由开发者根据 MsgKey和参数 FlowEventInput 自行定制实现即可。  

一. 使用示例：  
```csharp
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

	//  单元测试方法
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
       Assert.IsTrue(countPara.count==10);// 默认消息队列实现是内存级，countPara引用不变
   	}
```
二. 使用介绍  
上例中 TestCount为默认入参和出参类型。 继承至 IFlowEvent<TestCount, TestCount>  的 TestEvent 为具体执行事件，其定义如下：  
```csharp
 	public interface IFlowEvent<in TIn,TOut>
    {
        /// <summary>
        /// 具体事件执行
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<TOut> Execute(TIn input);

        /// <summary>
        ///   最终失败执行方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Failed(TIn input);
    }
```
FlowEventOption为当前事件执行重试等参数项。其定义如下：  
```csharp
 	public class FlowEventOption
    {
        /// <summary>
        ///  保存事件消息的key(用来保存消息到消息流订阅重试)
        /// </summary>
        public string event_msg_key { get; set; }

        /// <summary>
        ///  异常时 推送消息到消息流，通过订阅方式重试运行次数,默认：0
        /// </summary>
        public int flow_retry_times { get; set; } = 1;

        /// <summary>
        /// 异常时 在推送消息流之前，在当前执行方法内部直接串联循环重试次数，默认：1
        /// </summary>
        public int func_retry_times { get; set; } = 0;

        /// <summary>
        ///  消息流的可选项
        /// </summary>
        public DataFlowOption flow_option { get; set; } 
    }
```
