using System.Threading.Tasks;

namespace OSS.DataFlow.Event
{
    /// <summary>
    /// 事件处理器
    /// </summary>
    /// <typeparam name="TIn">输入参数类型</typeparam>
    /// <typeparam name="TOut">输出参数类型</typeparam>
    public class FlowEventProcessor<TIn, TOut> : InternalBaseFlowEventProcessor<TIn, TOut>
    {
        private readonly IFlowEvent<TIn, TOut> _event;

        /// <inheritdoc />
        public FlowEventProcessor(IFlowEvent<TIn, TOut> eventInstance, FlowEventOption option) : base(option)
        {
            _event = eventInstance;
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

        internal override Task<TOut> InterEventExecute(TIn input)
        {
            return _event.Execute(input);
        }
        internal override Task InterEventFailed(TIn input)
        {
            return _event.Failed(input);
        }
    }

    /// <summary>
    /// 事件处理器
    /// </summary>
    /// <typeparam name="TIn">输入参数类型</typeparam>
    public class FlowEventProcessor<TIn> : InternalBaseFlowEventProcessor<TIn, Empty>
    {
        private readonly IFlowEvent<TIn> _event;

        /// <inheritdoc />
        public FlowEventProcessor(IFlowEvent<TIn> eventInstance, FlowEventOption option) : base(option)
        {
            _event = eventInstance;
        }

        /// <summary>
        ///  执行
        /// </summary>
        /// <param name="input">输入参数</param>
        /// <returns></returns>
        public Task Process(TIn input)
        {
            return InterProcess(new FlowEventInput<TIn>()
            {
                input = input
            });
        }

        internal override async Task<Empty> InterEventExecute(TIn input)
        {
            await _event.Execute(input);
            return Empty.Default;
        }
        internal override Task InterEventFailed(TIn input)
        {
            return _event.Failed(input);
        }
    }




    internal readonly struct EventProcessResp<TOut>
    {
        public bool is_success { get; }

        public EventProcessResp(bool isSuccess, TOut res)
        {
            is_success = isSuccess;
            result     = res;
        }

        /// <summary>
        ///  返回结果
        /// </summary>
        public TOut result { get; }
    }
}
