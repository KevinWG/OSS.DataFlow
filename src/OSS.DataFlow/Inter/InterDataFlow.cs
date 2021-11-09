using OSS.DataFlow.Inter.Queue;

namespace OSS.DataFlow
{
    /// <summary>
    ///  默认数据流
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    internal class InterDataFlow<TData> : InterDataPublisher
    {
        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="flowDataTypeKey"></param>
        /// <param name="subscriber"></param>
        /// <param name="option"></param>
        public InterDataFlow(string flowDataTypeKey,IDataSubscriber<TData> subscriber,DataFlowOption option):base(option)
        {
            InterQueueConsumer.RegisterSubscriber(flowDataTypeKey, subscriber);
        }
    }
}
