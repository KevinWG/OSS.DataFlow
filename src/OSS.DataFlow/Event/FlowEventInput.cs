namespace OSS.DataFlow.Event
{
    /// <summary>
    ///  事件的输入参数
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    public class FlowEventInput<TIn>
    {
        /// <summary>
        ///  循环执行次数
        /// </summary>
        public int circulate_times { get; set; }

        /// <summary>
        /// 输入参数
        /// </summary>
        public TIn input { get; set; }
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