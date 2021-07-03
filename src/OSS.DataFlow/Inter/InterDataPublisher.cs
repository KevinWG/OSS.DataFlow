using OSS.DataFlow.Inter.Queue;
using System.Threading.Tasks;

namespace OSS.DataFlow
{
    /// <summary>
    ///  默认发布者
    /// </summary>
    internal class InterDataPublisher : IDataPublisher
    {
        private readonly DataPublisherOption _option;
        //private readonly string              _dataTypeKey;

        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="option"></param>
        public InterDataPublisher(DataPublisherOption option)
        {
            _option      = option;
            //_dataTypeKey = dataTypeKey;

            InterQueueHub.RegisterQueue(option?.SourceName);
        }

        /// <summary>
        ///   发布数据
        /// </summary>
        /// <param name="dataTypeKey"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<bool> Publish<TData>(string dataTypeKey,TData data)
        {
            return InterQueueHub.Publish(dataTypeKey, data, _option?.SourceName);
        }
    }
}