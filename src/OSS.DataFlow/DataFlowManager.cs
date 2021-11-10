using OSS.DataFlow.Inter.Queue;
using System.Threading.Tasks;

namespace OSS.DataFlow
{
    /// <summary>
    /// 消息流核心部件管理者
    /// </summary>
    public static class DataFlowManager
    {
        /// <summary>
        /// 自定义 数据流 发布/订阅 实现的 提供者
        /// </summary>
        public static IDataFlowProvider FlowProvider { get; set; }

        /// <summary>
        /// 自定义 数据流发布实现的 提供者
        /// </summary>
        public static IDataPublisherProvider PublisherProvider { get; set; }


        private readonly static InterSubscriberHandler _subscriberHandler = new InterSubscriberHandler();

        /// <summary>
        ///  通过自定义消息触发机制通知订阅者
        ///     调用时请做异常拦截，防止脏数据导致 msgData 类型错误
        /// </summary>
        /// <param name="msgDataKey"></param>
        /// <param name="msgData">消息内容，自定义触发时，请注意和注册订阅者的消费数据类型转换安全</param>
        /// <returns></returns>
        public static Task<bool> NotifySubscriber(string msgDataKey, object msgData)
        {
            return _subscriberHandler.NotifySubscriber(msgDataKey, msgData);
        }
        
        //  注册订阅者
        internal static bool RegisterSubscriber<TData>(string msgFlowKey, IDataSubscriber<TData> subscriber)
        {
            _subscriberHandler.RegisterSubscriber(msgFlowKey, new InterDataSubscriberWrap<TData>(subscriber));
            return true;
        }
    }
}