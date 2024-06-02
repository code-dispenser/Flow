using Flow.Core.Areas.Returns;

namespace Flow.Core.Areas.Extensions;

public static class FlowExtensions
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

        => await thisFlow.Bind(success => onSuccess(success));


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

        awaitedFlow.Match(_ => { }, success => act_onSuccess(success));

        return awaitedFlow;
    }


    #endregion

    #region OnSuccessTry Methods

    /// <summary>
    /// Tries to perform an asynchronous operation on the success value of a task flow, handling any exceptions.
    /// </summary>
    /// <typeparam name="TIn">The type of the input flow value.</typeparam>
    /// <typeparam name="TOut">The type of the output flow value.</typeparam>
    /// <param name="thisFlow">The task representing the input flow.</param>
    /// <param name="operationToTry">The asynchronous operation to try on the success value.</param>
    /// <param name="exceptionHandler">The function to handle any exceptions that occur.</param>
    /// <returns>A task representing the asynchronous operation that returns a new flow with the result of the operation or the handled exception.</returns>
    public static async Task<Flow<TOut>> OnSuccessTry<TIn, TOut>(this Task<Flow<TIn>> thisFlow, Func<TIn, Task<Flow<TOut>>> operationToTry, Func<Exception, Flow<TOut>> exceptionHandler)
    {
        try
        {
            return await (await thisFlow.ConfigureAwait(false)).Bind<TOut>(operationToTry).ConfigureAwait(false);
        }
        catch (Exception ex) { return exceptionHandler(ex); }
    }

    /// <summary>
    /// Tries to perform an operation on the success value of a flow, handling any exceptions.
    /// </summary>
    /// <typeparam name="TIn">The type of the input flow value.</typeparam>
    /// <typeparam name="TOut">The type of the output flow value.</typeparam>
    /// <param name="thisFlow">The input flow.</param>
    /// <param name="operationToTry">The operation to try on the success value.</param>
    /// <param name="exceptionHandler">The function to handle any exceptions that occur.</param>
    /// <returns>A new flow with the result of the operation or the handled exception.</returns>
    public static Flow<TOut> OnSuccessTry<TIn, TOut>(this Flow<TIn> thisFlow, Func<TIn, Flow<TOut>> operationToTry, Func<Exception, Flow<TOut>> exceptionHandler)
    {
        try
        {
            return thisFlow.Bind<TOut>(operationToTry);
        }
        catch (Exception ex) { return exceptionHandler(ex); }
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
    #endregion

    #region ReturnAs

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

    #endregion

    #region Finally Methods

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

