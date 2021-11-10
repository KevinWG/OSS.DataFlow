namespace OSS.DataFlow
{
    /// <summary>
    ///  数据流发布者的提供器
    /// </summary>
    public interface IDataPublisherProvider
    {
        /// <summary>
        /// 数据发布者
        /// </summary>
        /// <param name="option"></param>
        /// <returns> 返回消息发布接口实现 </returns>
        IDataPublisher CreatePublisher(DataPublisherOption option);
    }
}