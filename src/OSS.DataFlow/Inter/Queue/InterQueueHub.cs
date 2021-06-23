
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using OSS.DataFlow.Inter.Queue;

namespace OSS.DataFlow.Inter.Queue
{
    internal static class InterQueueHub
    {
        private static readonly ExecutionDataflowBlockOptions options = new ExecutionDataflowBlockOptions()
        {
            MaxDegreeOfParallelism = 32
        }; 

        private static readonly ActionBlock<InterData> _defaultDataQueue = new ActionBlock<InterData>(InterQueueSubscriber.InterSubscribe, options);
       
        private static readonly ConcurrentDictionary<string, ActionBlock<InterData>> _sourceDataQueueMaps = new ConcurrentDictionary<string, ActionBlock<InterData>>();
      
        public static Task<bool> Publish(string msgFlowKey, object msg,string sourcename)
        {
            return Task.FromResult(GetQueue(sourcename).Post(new InterData()
            {
                data_type_key = msgFlowKey, msg = msg
            }));
        }
        
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

            if (_sourceDataQueueMaps.TryAdd(sourceName, new ActionBlock<InterData>(InterQueueSubscriber.InterSubscribe, options)))
                return;
        }
        
    }


    internal class InterData
    {
        public string data_type_key { get; set; }

        public object msg { get ; set; }
    }
}
