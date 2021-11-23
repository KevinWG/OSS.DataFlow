using System;
using System.Threading.Tasks;

namespace OSS.DataFlow
{
    /// <summary>
    ///  数据流创建者
    /// </summary>
    public static class DataFlowFactory
    {
        #region 独立处理发布者

        /// <summary>
        /// 创建单向数据发布者
        /// </summary>
        /// <param name="option"></param>
        /// <returns> 返回当前流的发布接口实现 </returns>
        public static IDataPublisher CreatePublisher(DataPublisherOption option = null)
        {
            var pusher = DataFlowManager.PublisherProvider?.CreatePublisher(option);
            return pusher ?? new InterDataPublisher(option);
        }

        #endregion


        #region 独立处理订阅者

        /// <summary>
        /// 注册 接收数据订阅者
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="subscriber"></param>
        /// <param name="dataTypeKey"> 流key  </param>
        /// <returns> 是否接收成功 </returns>
        public static void RegisterSubscriber<TData>(string dataTypeKey, IDataSubscriber<TData> subscriber)
        {
            DataFlowManager.RegisterSubscriber(dataTypeKey, subscriber);
        }

        /// <summary>
        /// 注册 接收数据订阅者
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="subscribeFunc"></param>
        /// <param name="dataTypeKey"> 流key  </param>
        /// <returns> 是否接收成功 </returns>
        public static IDataSubscriber<TData> RegisterSubscriber<TData>(string dataTypeKey, Func<TData, Task<bool>> subscribeFunc)
        {
            var subscriber = new InterDataFuncSubscriber<TData>(subscribeFunc);
            RegisterSubscriber(dataTypeKey, subscriber);
            return subscriber;
        }

        #endregion



        #region 消息流

        /// <summary>
        ///  创建数据流
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="subscriber">数据订阅者</param>
        /// <param name="flowDataTypeKey"> 流key ( 默认对应实现是 Task.Factory.StartNew 传递数据实现 ) </param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static IDataPublisher RegisterFlow<TData>(string flowDataTypeKey, IDataSubscriber<TData> subscriber,
            DataPublisherOption option = null)
        {
            DataFlowManager.RegisterSubscriber(flowDataTypeKey, subscriber);
            return CreatePublisher(option);
        }

        /// <summary>
        ///  创建数据流
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="subscribeFunc"> 订阅数据流消息的委托方法</param>
        /// <param name="flowDataTypeKey"> 流key ( 默认对应实现是 Task.Factory.StartNew 传递数据实现 ) </param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static IDataPublisher RegisterFlow<TData>(string flowDataTypeKey, Func<TData, Task<bool>> subscribeFunc, DataPublisherOption option = null)
        {
            var subscriber = new InterDataFuncSubscriber<TData>(subscribeFunc);
            return RegisterFlow(flowDataTypeKey, subscriber, option);
        }

        #endregion
    }



}
