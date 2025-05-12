using Flow.Core.Areas.Returns;

namespace Flow.Core.Areas.Extensions;

public static partial class FlowExtensions
{

    /// <summary>
    /// Executes one of the specified functions based on whether the flow is a success or a failure.
    /// </summary>
    /// <typeparam name="TIn">The type of the input flow value.</typeparam>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <param name="thisFlow">The input flow.</param>
    /// <param name="onFailure">The function to execute if the flow is a failure.</param>
    /// <param name="onSuccess">The function to execute if the flow is a success.</param>
    /// <returns>The result of the executed function.</returns>
    public static TOut Finally<TIn, TOut>(this Flow<TIn> thisFlow, Func<Failure, TOut> onFailure, Func<TIn, TOut> onSuccess)

        => thisFlow.Match(onFailure, onSuccess);

    /// <summary>
    /// Executes one of the specified functions based on whether the task flow is a success or a failure.
    /// </summary>
    /// <typeparam name="TIn">The type of the input flow value.</typeparam>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <param name="thisFlow">The task representing the input flow.</param>
    /// <param name="onFailure">The function to execute if the flow is a failure.</param>
    /// <param name="onSuccess">The function to execute if the flow is a success.</param>
    /// <returns>A task representing the asynchronous operation that returns the result of the executed function.</returns>
    public static async Task<TOut> Finally<TIn, TOut>(this Task<Flow<TIn>> thisFlow, Func<Failure, TOut> onFailure, Func<TIn, TOut> onSuccess)

        => (await thisFlow.ConfigureAwait(false)).Match(onFailure, onSuccess);

}
