
using System.Threading.Tasks;

namespace OSS.DataFlow
{
    internal class InterDataSubscriberWrap<TData> : ISubscriberWrap
    {
        private readonly IDataSubscriber<TData> _subscriber;
        //internal InterDataSubscriberWrap(Func<TData, Task<bool>> subscribeFunc)
        //{
        //    _subscriber = new InterDataFuncSubscriber<TData>(subscribeFunc);
        //}

        internal InterDataSubscriberWrap(IDataSubscriber<TData> subscribe)
        {
            _subscriber = subscribe;
        }

        Task<bool> ISubscriberWrap.Subscribe(object data)
        {
            return _subscriber.Subscribe((TData) data);
        }
    }

    internal interface ISubscriberWrap
    {
        Task<bool> Subscribe(object data);
    }
}