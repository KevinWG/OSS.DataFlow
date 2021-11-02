using OSS.DataFlow.Inter.Queue;
using System;
using System.Threading.Tasks;

namespace OSS.DataFlow
{
    /// <summary>
    ///  数据流创建者
    /// </summary>
    public static class DataFlowFactory
    {
        /// <summary>
        /// 自定义 数据流 发布/订阅 实现的 提供者
        /// </summary>
        public static IDataFlowProvider FlowProvider { get; set; }
        /// <summary>
        /// 自定义 数据流发布实现的 提供者
        /// </summary>
        public static IDataPublisherProvider PublisherProvider { get; set; }
        /// <summary>
        /// 自定义 数据流订阅实现的 提供者
        /// </summary>
        public static IDataSubscriberProvider SubscriberProvider { get; set; }


        /// <summary>
        ///  创建数据流
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="subscriber">数据订阅者</param>
        /// <param name="flowDataTypeKey"> 流key ( 默认对应实现是 Task.Factory.StartNew 传递数据实现 ) </param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static IDataPublisher RegisterFlow<TData>(string flowDataTypeKey, IDataSubscriber<TData> subscriber,  DataFlowOption option=null) 
        {
            var pusher = FlowProvider?.RegisterFlow(flowDataTypeKey, subscriber,  option);
            return pusher ?? new InterDataFlow<TData>(flowDataTypeKey,subscriber, option);
        }

        /// <summary>
        ///  创建数据流
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="subscribeFunc"> 订阅数据流消息的委托方法</param>
        /// <param name="flowDataTypeKey"> 流key ( 默认对应实现是 Task.Factory.StartNew 传递数据实现 ) </param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static IDataPublisher RegisterFlow<TData>(string flowDataTypeKey, Func<TData, Task<bool>> subscribeFunc, DataFlowOption option = null)
        {
            var poper = new InterDataFuncSubscriber<TData>(subscribeFunc);

            var pusher = FlowProvider?.RegisterFlow(flowDataTypeKey, poper,  option);
            return pusher ?? new InterDataFlow<TData>(flowDataTypeKey,poper, option);
        }


  
        /// <summary>
        /// 创建单向数据发布者
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="option"></param>
        /// <returns> 返回当前流的发布接口实现 </returns>
        public static IDataPublisher RegisterPublisher<TData>(DataPublisherOption option = null)
        {
            var pusher = PublisherProvider?.RegisterPublisher<TData>(option);
            return pusher ?? new InterDataPublisher(option);
        }
        


        
        /// <summary>
        /// 注册 接收数据订阅者
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="subscriber"></param>
        /// <param name="dataTypeKey"> 流key  </param>
        /// <param name="option"></param>
        /// <returns> 是否接收成功 </returns>
        public static void RegisterSubscriber<TData>(string dataTypeKey, IDataSubscriber<TData> subscriber, DataFlowOption option = null)
        {
            if (SubscriberProvider == null)
            {
                InterQueueSubscriber.RegisterSubscriber(dataTypeKey, subscriber);
            }
            else
            {
                SubscriberProvider.RegisterSubscriber(dataTypeKey, subscriber, option);
            }
        }

        /// <summary>
        /// 注册 接收数据订阅者
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="subscribeFunc"></param>
        /// <param name="dataTypeKey"> 流key  </param>
        /// <param name="option"></param>
        /// <returns> 是否接收成功 </returns>
        public static void RegisterSubscriber<TData>(string dataTypeKey, Func<TData, Task<bool>> subscribeFunc, DataFlowOption option = null)
        {
            var poper = new InterDataFuncSubscriber<TData>(subscribeFunc); 
            RegisterSubscriber(dataTypeKey, poper,  option);
        }
    }

    

}
