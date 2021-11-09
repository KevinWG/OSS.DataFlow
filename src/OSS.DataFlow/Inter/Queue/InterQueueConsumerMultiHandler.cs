using System.Collections.Generic;
using System.Threading.Tasks;

namespace OSS.DataFlow.Inter.Queue
{
    internal class InterQueueConsumerMultiHandler
    {

        private Dictionary<string, IList<ISubscriberWrap>>
            subKeyMaps = new Dictionary<string, IList<ISubscriberWrap>>();
        private object _lockObj = new object();

        public bool RegisterSubscriber(string msgDataTypeKey, ISubscriberWrap sbWrap)
        {
            IList<ISubscriberWrap> listValue = null;

            lock (_lockObj)
            {
                if (!subKeyMaps.ContainsKey(msgDataTypeKey))
                {
                    listValue = new List<ISubscriberWrap>();
                    listValue.Add(sbWrap);

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

        public async Task<bool> Subscribe(string msgDataKey, object obj)
        {
            if (!subKeyMaps.TryGetValue(msgDataKey, out var listValue))
                return false;

            foreach (var subscriberWrap in listValue)
            {
                try
                {
                    await subscriberWrap.Subscribe(obj).ConfigureAwait(false);
                }
                catch
                {
                }
            }
            return true;
        }

    }
}
