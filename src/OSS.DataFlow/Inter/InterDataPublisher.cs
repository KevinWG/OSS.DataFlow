using System.Threading.Tasks;

namespace OSS.DataFlow
{
    /// <summary>
    ///  默认发布者
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    internal class InterDataPublisher<TData> : IDataPublisher<TData>
    {
        private readonly string              _msgKey;
        private readonly DataPublisherOption _option;

        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="msgPublisherKey"></param>
        /// <param name="option"></param>
        public InterDataPublisher(string msgPublisherKey, DataPublisherOption option)
        {
            _msgKey = msgPublisherKey;
            _option = option;

            InterQueueHub.RegisterPublisher(option?.SourceName);
        }

        /// <summary>
        ///   发布数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<bool> Publish(TData data)
        {
            return InterQueueHub.Publish(_msgKey, data, _option?.SourceName);
        }
    }
}