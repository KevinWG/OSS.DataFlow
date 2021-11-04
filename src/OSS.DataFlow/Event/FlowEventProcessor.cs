using System.Threading.Tasks;

namespace OSS.DataFlow.Event
{
    /// <summary>
    ///  事件处理器
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public abstract class FlowEventProcessor<TIn, TOut>
    {
        #region 消息通信发布订阅者

        private readonly string         _msgKey;
        private readonly IDataPublisher _publisher;

        private async Task<bool> subscriber(FlowEventInput<TIn> eventData)
        {
            await InterProcess(eventData);
            return true;
        }

        #endregion


        private readonly EventProcessOption    _option;
        private readonly IFlowEvent<TIn, TOut> _event;

        /// <summary>
        /// 事件处理器
        /// </summary>
        /// <param name="eventInstance">事件实例</param>
        /// <param name="option">事件处理选项</param>
        public FlowEventProcessor(IFlowEvent<TIn, TOut> eventInstance, EventProcessOption option)
        {
            _event  = eventInstance;
            _option = option;

            _publisher = DataFlowFactory.RegisterFlow<FlowEventInput<TIn>>(_option.event_msg_key, subscriber);
        }

        /// <summary>
        ///  执行
        /// </summary>
        /// <param name="input">输入参数</param>
        /// <returns></returns>
        public Task<TOut> Process(TIn input)
        {
            return InterProcess(new FlowEventInput<TIn>()
            {
                input = input
            });
        }


        internal async Task<TOut> InterProcess(FlowEventInput<TIn> eventData)
        {
            var interRes = await InterLoopProcessing(eventData.input);
            if (interRes.is_success)
                return interRes.result;

            eventData.circulated_times += 1;
            if (eventData.circulated_times <= _option.retry_times)
                await _publisher.Publish(_msgKey, eventData);
            else
                await _event.Failed(eventData.input);
            
            return interRes.result;
        }


        internal async Task<EventProcessResp<TOut>> InterLoopProcessing(TIn input)
        {
            var  interLoopTimes = 0;
            bool needRetry;
            do
            {
                try
                {
                    needRetry = false;
                    return new EventProcessResp<TOut>(true, await _event.Execute(input)); //await Execute(data);
                }
                catch
                {
                    needRetry      =  true;
                    interLoopTimes += 1;
                }

            } while (needRetry && interLoopTimes <= _option.loop_times);

            return new EventProcessResp<TOut>(false, default);
        }
    }

    internal readonly struct EventProcessResp<TOut>
    {
        public bool is_success { get;  }

        public EventProcessResp(bool isSuccess, TOut res)
        {
            is_success = isSuccess;
            result     = res;
        }

        /// <summary>
        ///  返回结果
        /// </summary>
        public TOut result { get;  }
    }
}
