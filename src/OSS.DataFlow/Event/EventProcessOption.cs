namespace OSS.DataFlow.Event
{
    /// <summary>
    ///  事件执行参数
    /// </summary>
    public class EventProcessOption
    {
        /// <summary>
        ///  保存事件消息的key(用来消息重试)
        /// </summary>
        public string event_msg_key { get; set; }

        /// <summary>
        ///   重试运行次数,默认不重试运行
        /// </summary>
        public int retry_times { get; set; } = 0;

        /// <summary>
        ///  直接循环次数
        /// </summary>
        public int loop_times { get; set; } = 1;
    }
}