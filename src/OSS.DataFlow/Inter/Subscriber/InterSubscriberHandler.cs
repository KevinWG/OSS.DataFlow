using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.DataFlow.Inter.Queue
{
    /// <summary>
    ///  订阅者处理器
    /// </summary>
    internal class InterSubscriberHandler
    {
        private readonly object _lockObj = new object();
        private readonly Dictionary<string, IList<ISubscriberWrap>> subKeyMaps = new Dictionary<string, IList<ISubscriberWrap>>();
        
        internal bool RegisterSubscriber(string msgDataTypeKey, ISubscriberWrap sbWrap)
        {
            IList<ISubscriberWrap> listValue;
            lock (_lockObj)
            {
                if (!subKeyMaps.ContainsKey(msgDataTypeKey))
                {
                    listValue = new List<ISubscriberWrap>
                    {
                        sbWrap
                    };
                    subKeyMaps.Add(msgDataTypeKey, listValue);
                }
                else
                {
                    if (!subKeyMaps.TryGetValue(msgDataTypeKey, out listValue))
                    {
                        return false;
                    }
                    listValue.Add(sbWrap);
                }
            }
            return true;
        }

        /// <summary>
        ///  通知订阅者
        ///     自定义触发，手动调用时请做异常拦截，防止脏数据导致 msgData 类型错误
        /// </summary>
        /// <param name="msgDataKey"></param>
        /// <param name="msgData">消息内容，自定义触发时，请注意和注册订阅者的消费数据类型转换安全</param>
        /// <returns></returns>
        public async Task<bool> NotifySubscriber(string msgDataKey, object msgData)
        {
            if (!subKeyMaps.TryGetValue(msgDataKey, out var listValue))
                return false;

            if (listValue.Count == 1)
                return await listValue[0].Subscribe(msgData).ConfigureAwait(false);

            // 有多个订阅者时，并发执行
            var tasks = listValue.Select(async s => await s.Subscribe(msgData).ConfigureAwait(false));
            var res   = await Task.WhenAll(tasks);

            return res.Any(r => r);
        }

    }
}
