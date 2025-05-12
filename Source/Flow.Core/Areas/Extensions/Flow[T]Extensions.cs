using Flow.Core.Areas.Returns;

namespace Flow.Core.Areas.Extensions;

/// <summary>
/// Extension methods that basically wrap the Map, Bind and Match method on the <see cref="Flow{T}"/> class,
/// enabling a more fluent, declarative approach. 
/// </summary>
public static partial class FlowExtensions
{

    #region Then Methods

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

    #endregion


    #region OnSuccess Methods

    /// <summary>
    /// Transforms the success value of a flow using the specified function if the flow is successful.
    /// </summary>
    /// <typeparam name="TIn">The type of the input flow value.</typeparam>
    /// <typeparam name="TOut">The type of the output flow value.</typeparam>
    /// <param name="thisFlow">The input flow.</param>
    /// <param name="onSuccess">The function to transform the success value.</param>
    /// <returns>A new flow with the transformed success value or the original failure value.</returns>
    public static Flow<TOut> OnSuccess<TIn, TOut>(this Flow<TIn> thisFlow, Func<TIn, TOut> onSuccess)

        => thisFlow.Map<TOut>(onSuccess);


    /// <summary>
    /// Executes the specified action if the flow is successful.
    /// </summary>
    /// <typeparam name="T">The type of the flow value.</typeparam>
    /// <param name="thisFlow">The input flow.</param>
    /// <param name="act_onSuccess">The action to execute if the flow is successful.</param>
    /// <returns>The original flow.</returns>
    public static Flow<T> OnSuccess<T>(this Flow<T> thisFlow, Action<T> act_onSuccess)
    {
        thisFlow.Match(_ => { }, success => act_onSuccess(success));
        return thisFlow;
    }


    /// <summary>
    /// Transforms the success value of a task flow using the specified asynchronous function if the flow is successful.
    /// </summary>
    /// <typeparam name="TIn">The type of the input flow value.</typeparam>
    /// <typeparam name="TOut">The type of the output flow value.</typeparam>
    /// <param name="thisFlow">The task representing the input flow.</param>
    /// <param name="onSuccess">The asynchronous function to transform the success value.</param>
    /// <returns>A task representing the asynchronous operation that returns a new flow with the transformed success value or the original failure value.</returns>
    public static async Task<Flow<TOut>> OnSuccess<TIn, TOut>(this Task<Flow<TIn>> thisFlow, Func<TIn,Task<Flow<TOut>>> onSuccess)

        => await (await thisFlow.ConfigureAwait(false)).Bind(onSuccess).ConfigureAwait(false);



    /// <summary>
    /// Transforms the success value of a flow using the specified asynchronous function if the flow is successful.
    /// </summary>
    /// <typeparam name="TIn">The type of the input flow value.</typeparam>
    /// <typeparam name="TOut">The type of the output flow value.</typeparam>
    /// <param name="thisFlow">The input flow.</param>
    /// <param name="onSuccess">The asynchronous function to transform the success value.</param>
    /// <returns>A task representing the asynchronous operation that returns a new flow with the transformed success value or the original failure value.</returns>
    public static async Task<Flow<TOut>> OnSuccess<TIn, TOut>(this Flow<TIn> thisFlow, Func<TIn, Task<Flow<TOut>>> onSuccess)

        => await thisFlow.Bind(success => onSuccess(success)).ConfigureAwait(false);


    /// <summary>
    /// Transforms the success value of a task flow using the specified function if the flow is successful.
    /// </summary>
    /// <typeparam name="TIn">The type of the input flow value.</typeparam>
    /// <typeparam name="TOut">The type of the output flow value.</typeparam>
    /// <param name="thisFlow">The task representing the input flow.</param>
    /// <param name="onSuccess">The function to transform the success value.</param>
    /// <returns>A task representing the asynchronous operation that returns a new flow with the transformed success value or the original failure value.</returns>
    public static async Task<Flow<TOut>> OnSuccess<TIn, TOut>(this Task<Flow<TIn>> thisFlow, Func<TIn, Flow<TOut>> onSuccess)
        
        => (await thisFlow.ConfigureAwait(false)).Bind(onSuccess);


    /// <summary>
    /// Executes the specified action if the task flow is successful.
    /// </summary>
    /// <typeparam name="T">The type of the flow value.</typeparam>
    /// <param name="thisFlow">The task representing the input flow.</param>
    /// <param name="act_onSuccess">The action to execute if the flow is successful.</param>
    /// <returns>A task representing the asynchronous operation that returns the original flow.</returns>
    public static async Task<Flow<T>> OnSuccess<T>(this Task<Flow<T>> thisFlow, Action<T> act_onSuccess)
    {
        var awaitedFlow = await thisFlow.ConfigureAwait(false);

        awaitedFlow.Match(act_onFailure: _ => { }, success => act_onSuccess(success));

        return awaitedFlow;
    }

    /// <summary>
    /// Executes the specified async action if the task flow is successful.
    /// </summary>
    /// <typeparam name="T">The type of the flow value.</typeparam>
    /// <param name="thisFlow">The task representing the input flow.</param>
    /// <param name="onSuccess">The async action to execute if the flow is successful.</param>
    /// <returns>A task representing the asynchronous operation that returns the original flow.</returns>
    public static async Task<Flow<T>> OnSuccess<T>(this Task<Flow<T>> thisFlow, Func<T, Task> onSuccess)
    {
        var awaitedFlow = await thisFlow.ConfigureAwait(false);

        await awaitedFlow.Match(_ => Task.CompletedTask, onSuccess: onSuccess).ConfigureAwait(false);

        return awaitedFlow;
    }

    #endregion



    #region OnFailure Methods


    /// <summary>
    /// Transforms the failure value of a flow using the specified function if the flow is a failure.
    /// </summary>
    /// <typeparam name="T">The type of the flow value.</typeparam>
    /// <param name="thisFlow">The input flow.</param>
    /// <param name="onFailure">The function to transform the failure value.</param>
    /// <returns>A new flow with the transformed failure value or the original success value.</returns>
    public static Flow<T> OnFailure<T>(this Flow<T> thisFlow, Func<Failure, Flow<T>> onFailure)
    {
  
        var failureValue = thisFlow.Match(failure => failure, success => Failure.CreateNoFailure());

        if (thisFlow.IsSuccess) return thisFlow;//moved from start to here due to code coverage, either that or abuse the match function thisFlow.Match(failure => onFailure(failure), _ => thisFlow);


        return onFailure(failureValue);
    }


    /// <summary>
    /// Executes the specified action if the flow is a failure.
    /// </summary>
    /// <typeparam name="T">The type of the flow value.</typeparam>
    /// <param name="thisFlow">The input flow.</param>
    /// <param name="act_onFailure">The action to execute if the flow is a failure.</param>
    /// <returns>The original flow.</returns>
    public static Flow<T> OnFailure<T>(this Flow<T> thisFlow, Action<Failure> act_onFailure)
    {
        thisFlow.Match(failure => act_onFailure(failure), _ => { });

        return thisFlow;
    }


    /// <summary>
    /// Executes the specified action if the task flow is a failure.
    /// </summary>
    /// <typeparam name="T">The type of the flow value.</typeparam>
    /// <param name="thisFlow">The task representing the input flow.</param>
    /// <param name="act_onFailure">The action to execute if the flow is a failure.</param>
    /// <returns>A task representing the asynchronous operation that returns the original flow.</returns>
    public static async Task<Flow<T>> OnFailure<T>(this Task<Flow<T>> thisFlow, Action<Failure> act_onFailure)
    {
        var awaitedFlow = await thisFlow.ConfigureAwait(false);

        awaitedFlow.Match(act_onFailure, _ => { });

        return awaitedFlow;
    }

    /// <summary>
    /// Executes the specified asynchronous function if the task flow is a failure.
    /// </summary>
    /// <typeparam name="T">The type of the flow value.</typeparam>
    /// <param name="thisFlow">The task representing the input flow.</param>
    /// <param name="onFailure">The asynchronous function to execute if the flow is a failure.</param>
    /// <returns>A task representing the asynchronous operation that returns the original flow.</returns>
    public static async Task<Flow<T>> OnFailure<T>(this Task<Flow<T>> thisFlow, Func<Failure, Task> onFailure)
    {
        var awaitedFlow = await thisFlow.ConfigureAwait(false);

        await awaitedFlow.Match(onFailure, _ => Task.CompletedTask).ConfigureAwait(false);

        return awaitedFlow;
    }


    /// <summary>
    ///     Transforms the failure value of the flow if it is a failure, using the specified mapping function. If the flow is successful, it is returned unchanged.
    /// </summary>
    /// <param name="thisFlow">The current flow instance.</param>
    /// <param name="onFailure">A function to transform the failure instance.</param>
    /// <typeparam name="T">The value type of the flow.</typeparam>
    /// <returns>
    ///     A flow containing the transformed failure if the original flow was a failure; otherwise, the original successful flow.
    /// </returns>
    public static Flow<T> OnFailure<T>(this Flow<T> thisFlow, Func<Failure, Failure> onFailure)

        => thisFlow.Map(onFailure, success => success);

    /// <summary>
    ///     Transforms the failure value of the flow if the awaited flow is a failure, using the specified mapping function. If the flow is successful, it is returned unchanged.
    /// </summary>
    /// <param name="thisFlow">The task representing the asynchronous flow operation.</param>
    /// <param name="onFailure">A function to transform the failure instance.</param>
    /// <typeparam name="T">The value type of the flow.</typeparam>
    /// <returns>
    ///     A task that represents the asynchronous operation, containing a new failed flow if the original flow was a failure; otherwise, the original successful flow.
    /// </returns>
    /// <remarks>
    ///     This overload allows modification of a failed flow's failure value without introducing asynchronous behavior into the mapping function. The success value is left untouched.
    /// </remarks>
    public static async Task<Flow<T>> OnFailure<T>(this Task<Flow<T>> thisFlow, Func<Failure, Failure> onFailure)

        => (await thisFlow.ConfigureAwait(false)).Map(onFailure, success => success);



    #endregion


    //#region OnSuccessWithRetry Methods

    //public static async Task<Flow<TOut>> OnSuccessWithRetry<TIn, TOut>(this Flow<TIn> thisFlow, Func<TIn, Task<Flow<TOut>>> onSuccess, int maxAttempts = 3, int retryIntervalMs = 500)
    //{

    //    if (thisFlow.IsFailure) return Flow<TOut>.Failed(thisFlow.Match(failure => failure, success => Failure.CreateNoFailure()));

    //    Flow<TOut> attemptedFlow = Failure.CreateNoFailure();

    //    while (maxAttempts > 0) 
    //    {
    //        attemptedFlow = await thisFlow.Bind(onSuccess);

    //        if (attemptedFlow.IsSuccess) return attemptedFlow;

    //        var failure = attemptedFlow.Match(failure => failure, _ => Failure.CreateNoFailure());

    //        if (false == failure.CanRetry) return attemptedFlow;

    //        if (retryIntervalMs > 0) await Task.Delay(retryIntervalMs);

    //        maxAttempts--;
    //    }

    //    return attemptedFlow;
    //}




    //#endregion

}

