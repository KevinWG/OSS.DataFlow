namespace OSS.DataFlow.Event
{
    /// <summary>
    ///  事件执行参数
    /// </summary>
    public class FlowEventOption
    {
        /// <summary>
        ///  保存事件消息的key(用来保存消息到消息流订阅重试)
        /// </summary>
        public string event_msg_key { get; set; }

        /// <summary>
        ///  异常时 推送消息到消息流，通过订阅方式重试运行次数,默认：0
        /// </summary>
        public int flow_retry_times { get; set; } = 1;

        /// <summary>
        /// 异常时 在推送消息流之前，在当前执行方法内部直接串联循环重试次数，默认：1
        /// </summary>
        public int func_retry_times { get; set; } = 0;

        /// <summary>
        ///  消息流的可选项
        /// </summary>
        public DataFlowOption flow_option { get; set; } 
    }

    /// <summary>
    /// 空值
    /// </summary>
    public struct Empty
    {
        public static readonly Empty Default = new Empty();
    }
}