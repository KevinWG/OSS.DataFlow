
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace OSS.DataFlow.Inter.Queue
{
    internal static class InterQueueHub
    {
        #region 内部队列

        #region 内部默认队列

        private static readonly ExecutionDataflowBlockOptions options = new ExecutionDataflowBlockOptions()
        {
            MaxDegreeOfParallelism = 32
        };

        private static readonly ActionBlock<InterData> _defaultDataQueue = new ActionBlock<InterData>(InterConsumer, options);

        #endregion

        private static readonly ConcurrentDictionary<string, ActionBlock<InterData>> _sourceDataQueueMaps = new ConcurrentDictionary<string, ActionBlock<InterData>>();

        private static ActionBlock<InterData> GetQueue(string sourceName)
        {
            if (string.IsNullOrEmpty(sourceName))
            {
                return _defaultDataQueue;
            }

            return _sourceDataQueueMaps.TryGetValue(sourceName, out var q) ? q : _defaultDataQueue;
        }

        internal static void RegisterQueue(string sourceName)
        {
            if (string.IsNullOrEmpty(sourceName) || _sourceDataQueueMaps.ContainsKey(sourceName))
                return;

            if (_sourceDataQueueMaps.TryAdd(sourceName, new ActionBlock<InterData>(InterConsumer, options)))
                return;
        }


        #endregion

        #region 推送（生产）消息

        public static Task<bool> Publish(string msgFlowKey, object msg, string sourcename)
        {
            return Task.FromResult(GetQueue(sourcename).Post(new InterData(msgFlowKey, msg)));
        }

        #endregion

        #region 订阅（消费消息）
        
        internal static async Task InterConsumer(InterData data)
        {
            try
            {
                await DataFlowManager.NotifySubscriber(data.msg_key, data.data).ConfigureAwait(false);
            }
            catch
            {
            }
        }

        #endregion
    }

    internal readonly struct InterData
    {
        public InterData(string msgKey, object data)
        {
            msg_key   = msgKey;
            this.data = data;
        }

        public string msg_key { get; }

        public object data { get; }
    }
}
