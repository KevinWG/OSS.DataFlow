using System.Threading.Tasks;

namespace OSS.DataFlow.Event
{
    /// <summary>
    ///  消息流 事件
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    public interface IFlowEvent<in TIn>
    {
        /// <summary>
        /// 具体事件执行
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Execute(TIn input);

        /// <summary>
        ///   失败方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Failed(TIn input);
    }

    /// <summary>
    /// 失败后循环重试执行事件接口
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public interface IFlowEvent<in TIn,TOut>
    {
        /// <summary>
        /// 具体事件执行
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<TOut> Execute(TIn input);

        /// <summary>
        ///   失败方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Failed(TIn input);
    }
    
}