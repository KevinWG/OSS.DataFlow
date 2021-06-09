
namespace OSS.DataFlow
{
    /// <summary>
    ///  数据流提供器
    /// </summary>
    public interface IDataFlowProvider
    {
        /// <summary>
        /// 创建一个数据流，并暴露发布接口实现
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="subscriber"> 订阅者 </param>
        /// <param name="flowKey"> 流key  </param>
        /// <param name="option"></param>
        /// <returns> 返回当前流的发布接口实现 </returns>
        IDataPublisher<TData> CreateFlow<TData>(string flowKey, IDataSubscriber<TData> subscriber,  DataFlowOption option=null);
    }
}
