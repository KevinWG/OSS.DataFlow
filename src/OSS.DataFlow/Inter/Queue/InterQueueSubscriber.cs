using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace OSS.DataFlow.Inter.Queue
{
    internal static class InterQueueSubscriber
    {
        private static readonly ConcurrentDictionary<Type, InterSubscriberMultiHandler> _keySubscriberMaps =
            new ConcurrentDictionary<Type, InterSubscriberMultiHandler>();

        public static async Task InterSubscribe(InterData data)
        {
            try
            {
                if (_keySubscriberMaps.TryGetValue(data.msg.GetType(), out var subscriber))
                {
                    await subscriber.Subscribe(data.data_type_key, data.msg).ConfigureAwait(false);
                }
            }
            catch
            {
            }
        }

        internal static bool RegisterSubscriber<TData>(string msgFlowKey, IDataSubscriber<TData> subscriber)
        {
            _keySubscriberMaps.AddOrUpdate(typeof(TData), (t) =>
                {
                    var handler = new InterSubscriberMultiHandler();
                    handler.RegisterSubscriber(msgFlowKey, new InterDataSubscriberWrap<TData>(subscriber));
                    return handler;
                },
                (t, handler) =>
                {
                    handler.RegisterSubscriber(msgFlowKey, new InterDataSubscriberWrap<TData>(subscriber));
                    return handler;
                });
            return true;
        }
    }
}
