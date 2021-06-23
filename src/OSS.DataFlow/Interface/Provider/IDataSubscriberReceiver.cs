namespace OSS.DataFlow
{
    /// <summary>
    ///  数据流订阅者的接收器
    /// </summary>
    public interface IDataSubscriberReceiver
    {
        /// <summary>
        /// 接收数据订阅者
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="subscriber"></param>
        /// <param name="dataTypeKey"> 流key  </param>
        /// <param name="option"></param>
        /// <returns> 是否接收成功 </returns>
        void Receive<TData>(string dataTypeKey, IDataSubscriber<TData> subscriber,  DataFlowOption option = null);
    }
}