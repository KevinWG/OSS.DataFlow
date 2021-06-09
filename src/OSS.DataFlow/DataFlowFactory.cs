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
        /// 数据流的提供者
        /// </summary>
        public static IDataFlowProvider FlowProvider { get; set; }

        /// <summary>
        ///  创建数据流
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="subscriber">数据订阅者</param>
        /// <param name="flowKey"> 流key ( 默认对应实现是 Task.Factory.StartNew 传递数据实现 ) </param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static IDataPublisher<TData> CreateFlow<TData>(string flowKey, IDataSubscriber<TData> subscriber,  DataFlowOption option=null) 
        {
            var pusher = FlowProvider?.CreateFlow(flowKey, subscriber,  option);
            return pusher ?? new InterDataFlow<TData>(flowKey,subscriber, option);
        }

        /// <summary>
        ///  创建数据流
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="subscribeFunc"> 订阅数据流消息的委托方法</param>
        /// <param name="flowKey"> 流key ( 默认对应实现是 Task.Factory.StartNew 传递数据实现 ) </param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static IDataPublisher<TData> CreateFlow<TData>(string flowKey, Func<TData, Task<bool>> subscribeFunc, DataFlowOption option = null)
        {
            var poper = new InterDataFuncSubscriber<TData>(subscribeFunc);

            var pusher = FlowProvider?.CreateFlow(flowKey, poper,  option);
            return pusher ?? new InterDataFlow<TData>(flowKey,poper, option);
        }


        /// <summary>
        /// 数据流的提供者
        /// </summary>
        public static IDataPublisherProvider PublisherProvider { get; set; }

        /// <summary>
        /// 创建单向数据发布者
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="publisherKey"> 流key  </param>
        /// <param name="option"></param>
        /// <returns> 返回当前流的发布接口实现 </returns>
        public static IDataPublisher<TData> CreatePublisher<TData>(string publisherKey, DataPublisherOption option = null)
        {
            var pusher = PublisherProvider?.CreatePublisher<TData>(publisherKey, option);
            return pusher ?? new InterDataPublisher<TData>(publisherKey, option);
        }


        /// <summary>
        /// 数据流订阅者的接收器
        /// </summary>
        public static IDataSubscriberReceiver SubscriberReceiver { get; set; }
        
        /// <summary>
        /// 接收数据订阅者
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="subscriber"></param>
        /// <param name="subscriberKey"> 流key  </param>
        /// <param name="option"></param>
        /// <returns> 是否接收成功 </returns>
        public static void ReceiveSubscriber<TData>(string subscriberKey, IDataSubscriber<TData> subscriber, DataFlowOption option = null)
        {
            if (SubscriberReceiver == null)
            {
                InterQueueHub.RegisterSubscriber(subscriberKey, subscriber);
            }
            else
            {
                SubscriberReceiver.Receive(subscriberKey, subscriber, option);
            }
        }

        /// <summary>
        /// 接收数据订阅者
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="subscribeFunc"></param>
        /// <param name="subscriberKey"> 流key  </param>
        /// <param name="option"></param>
        /// <returns> 是否接收成功 </returns>
        public static void ReceiveSubscriber<TData>(string subscriberKey, Func<TData, Task<bool>> subscribeFunc, DataFlowOption option = null)
        {
            var poper = new InterDataFuncSubscriber<TData>(subscribeFunc); 
            ReceiveSubscriber(subscriberKey, poper,  option);
        }
    }

    

}
