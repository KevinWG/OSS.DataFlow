using System.Threading.Tasks;

namespace OSS.DataFlow.Event
{
    /// <summary>
    ///  事件处理器
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public abstract class InternalBaseFlowEventProcessor<TIn, TOut>
    {
        #region 消息通信发布订阅者

        private readonly IDataPublisher _publisher;
        private async Task<bool> subscriber(FlowEventInput<TIn> eventData)
        {
            await InterProcess(eventData);
            return true;
        }

        #endregion

        private readonly FlowEventOption _option;

        /// <summary>
        /// 事件处理器构造函数
        /// </summary>
        /// <param name="option">事件处理选项</param>
        public InternalBaseFlowEventProcessor(FlowEventOption option)
        {
            _option    = option;
            _publisher = DataFlow.RegisterFlow<FlowEventInput<TIn>>(_option.event_msg_key, subscriber);
        }

        internal async Task<TOut> InterProcess(FlowEventInput<TIn> eventData)
        {
            var interRes = await InterLoopProcessing(eventData.input);
            if (interRes.is_success)
                return interRes.result;

            if (eventData.circulate_times < _option.flow_retry_times)
            {
                eventData.circulate_times += 1;
                await _publisher.Publish(_option.event_msg_key, eventData);
            }
            else
                await InterEventFailed(eventData.input);

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
                    return new EventProcessResp<TOut>(true, await InterEventExecute(input));
                }
                catch
                {
                    needRetry      =  true;
                    interLoopTimes += 1;
                }
            } while (needRetry && interLoopTimes <= _option.func_retry_times);

            return new EventProcessResp<TOut>(false, default);
        }

        internal abstract Task<TOut> InterEventExecute(TIn input);
        internal abstract Task InterEventFailed(TIn input);
    }
}