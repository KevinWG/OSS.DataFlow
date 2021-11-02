namespace OSS.DataFlow
{
    /// <summary>
    ///  数据流发布者的提供器
    /// </summary>
    public interface IDataPublisherProvider
    {
        /// <summary>
        /// 创建单向数据发布者
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="option"></param>
        /// <returns> 返回当前流的发布接口实现 </returns>
        IDataPublisher RegisterPublisher<TData>(DataPublisherOption option = null);
    }
}