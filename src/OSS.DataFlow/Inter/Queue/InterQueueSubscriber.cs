

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace OSS.DataFlow.Inter.Queue
{
    internal static class InterQueueSubscriber
    {
        private static readonly ConcurrentDictionary<string, ISubscriberWrap> _keySubscriberMaps = new ConcurrentDictionary<string, ISubscriberWrap>();
        public static async Task InterSubscribe(InterData data)
        {
            try
            {
                if (_keySubscriberMaps.TryGetValue(data.data_type_key, out var subscriber))
                {
                    await subscriber.Subscribe(data.msg).ConfigureAwait(false);
                }
            }
            catch
            {
            }
        }

        internal static void RegisterSubscriber<TData>(string msgFlowKey, IDataSubscriber<TData> subscriber)
        {
            if (_keySubscriberMaps.ContainsKey(msgFlowKey))
            {
                throw new ArgumentException($"消息流key：{msgFlowKey} 已被注册");
            }

            if (_keySubscriberMaps.TryAdd(msgFlowKey, new InterDataSubscriberWrap<TData>(subscriber)))
                return;


            throw new ArgumentException($"未能注册（{msgFlowKey} ）对应的消息订阅处理!");
        }


    }
}
