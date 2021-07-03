using System.Threading.Tasks;

namespace OSS.DataFlow
{
    /// <summary>
    ///  数据的发布者
    /// </summary>
    public interface IDataPublisher
    {
        /// <summary>
        /// 推进数据
        /// </summary>
        /// <param name="dataKey"></param>
        /// <param name="data"></param>
        /// <returns>是否推入成功</returns>
        Task<bool> Publish<TData>(string dataKey,TData data);
    }
}
