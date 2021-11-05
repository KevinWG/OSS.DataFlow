namespace OSS.DataFlow.Event
{
    /// <summary>
    /// 事件流的输入参数
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
}