using Flow.Core.Areas.Returns;

namespace Flow.Core.Areas.Extensions;

public static partial class FlowExtensions
{
    /// <summary>
    /// Converts an object to a <see cref="Flow{TOut}"/> using the specified function.
    /// </summary>
    /// <typeparam name="T">The type of the input object.</typeparam>
    /// <typeparam name="TOut">The type of the output flow.</typeparam>
    /// <param name="thisObject">The input object.</param>
    /// <param name="toFlow">The function to convert the input object to a flow.</param>
    /// <returns>A new flow of type <typeparamref name="TOut"/>.</returns>
    public static Flow<TOut> Then<T, TOut>(this T thisObject, Func<T, Flow<TOut>> toFlow)

        => toFlow(thisObject);


    /// <summary>
    /// Converts a task result to a <see cref="Flow{TOut}"/> using the specified asynchronous function.
    /// </summary>
    /// <typeparam name="T">The type of the input object.</typeparam>
    /// <typeparam name="TOut">The type of the output flow.</typeparam>
    /// <param name="thisObject">The task representing the input object.</param>
    /// <param name="toFlow">The asynchronous function to convert the input object to a flow.</param>
    /// <returns>A task representing the asynchronous operation that returns a new flow of type <typeparamref name="TOut"/>.</returns>
    public static async Task<Flow<TOut>> Then<T, TOut>(this Task<T> thisObject, Func<T, Task<Flow<TOut>>> toFlow)

        => await toFlow(await thisObject);
}
