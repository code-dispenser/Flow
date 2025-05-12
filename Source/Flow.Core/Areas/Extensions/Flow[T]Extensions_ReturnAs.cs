using Flow.Core.Areas.Returns;

namespace Flow.Core.Areas.Extensions;

public static partial class FlowExtensions
{

    /// <summary>
    /// Transforms both the success and failure values of a flow using the specified functions.
    /// </summary>
    /// <typeparam name="TIn">The type of the input flow value.</typeparam>
    /// <typeparam name="TOut">The type of the output flow value.</typeparam>
    /// <param name="thisFlow">The input flow.</param>
    /// <param name="onFailure">The function to transform the failure value.</param>
    /// <param name="onSuccess">The function to transform the success value.</param>
    /// <returns>A new flow with the transformed success or failure value.</returns>
    public static Flow<TOut> ReturnAs<TIn, TOut>(this Flow<TIn> thisFlow, Func<Failure, Failure> onFailure, Func<TIn, TOut> onSuccess)

        => thisFlow.Map(onFailure, onSuccess);


    /// <summary>
    /// Transforms both the success and failure values of a task flow using the specified functions.
    /// </summary>
    /// <typeparam name="TIn">The type of the input flow value.</typeparam>
    /// <typeparam name="TOut">The type of the output flow value.</typeparam>
    /// <param name="thisFlow">The task representing the input flow.</param>
    /// <param name="onFailure">The function to transform the failure value.</param>
    /// <param name="onSuccess">The function to transform the success value.</param>
    /// <returns>A task representing the asynchronous operation that returns a new flow with the transformed success or failure value.</returns>
    public static async Task<Flow<TOut>> ReturnAs<TIn, TOut>(this Task<Flow<TIn>> thisFlow, Func<Failure, Failure> onFailure, Func<TIn, TOut> onSuccess)

        => (await thisFlow.ConfigureAwait(false)).Map(onFailure, onSuccess);


    /// <summary>
    /// Transforms the success value of a task flow using the specified function.
    /// </summary>
    /// <typeparam name="TIn">The type of the input flow value.</typeparam>
    /// <typeparam name="TOut">The type of the output flow value.</typeparam>
    /// <param name="thisFlow">The task representing the input flow.</param>
    /// <param name="onSuccess">The function to transform the success value.</param>
    /// <returns>A task representing the asynchronous operation that returns a new flow with the transformed success value.</returns>
    public static async Task<Flow<TOut>> ReturnAs<TIn, TOut>(this Task<Flow<TIn>> thisFlow, Func<TIn, TOut> onSuccess)

        => (await thisFlow.ConfigureAwait(false)).Map(onSuccess);

}
