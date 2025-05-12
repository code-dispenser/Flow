using Flow.Core.Areas.Returns;

namespace Flow.Core.Areas.Extensions;
public static partial class FlowExtensions
{
    /// <summary>
    /// Attempts to perform an asynchronous operation when the flow is a failure, and handles any exceptions that may occur.
    /// </summary>
    /// <typeparam name="TOut">The type of the flow value.</typeparam>
    /// <param name="thisFlow">The task representing the input flow.</param>
    /// <param name="operationToTry">The asynchronous operation to perform if the flow is a failure.</param>
    /// <param name="exceptionHandler">A function to handle any exception thrown during the operation.</param>
    /// <returns>
    /// A task representing the resulting flow, either from the successful recovery operation,
    /// the original success value, or the exception handler result.
    /// </returns>
    public static async Task<Flow<TOut>> OnFailureTry<TOut>(this Task<Flow<TOut>> thisFlow, Func<Failure, Task<Flow<TOut>>> operationToTry, Func<Exception, Flow<TOut>> exceptionHandler)
    {
        try
        {
            var awaitedFlow = await thisFlow.ConfigureAwait(false);

            return await awaitedFlow.Match(failure => operationToTry(failure), success => Task.FromResult(Flow<TOut>.Success(success))).ConfigureAwait(false);
        }
        catch (Exception ex) { return exceptionHandler(ex); }
    }


    /// <summary>
    /// Attempts to perform a synchronous operation when the flow is a failure, and handles any exceptions that may occur.
    /// </summary>
    /// <typeparam name="TOut">The type of the flow value.</typeparam>
    /// <param name="thisFlow">The input flow instance.</param>
    /// <param name="operationToTry">The operation to perform if the flow is a failure.</param>
    /// <param name="exceptionHandler">A function to handle any exception thrown during the operation.</param>
    /// <returns>
    /// A new flow resulting from the failure-handling operation, the original success value,
    /// or the result of the exception handler if an error occurs.
    /// </returns>
    public static Flow<TOut> OnFailureTry<TOut>(this Flow<TOut> thisFlow, Func<Failure, Flow<TOut>> operationToTry, Func<Exception, Flow<TOut>> exceptionHandler)
    {
        try
        {
            return thisFlow.Match(failure => operationToTry(failure), success =>  Flow<TOut>.Success(success));
        }
        catch (Exception ex) { return exceptionHandler(ex); }
    }


    /// <summary>
    /// Tries to perform an asynchronous operation on the failure value of a task-based flow, 
    /// wrapping the result in a success flow, and handles any exceptions that occur.
    /// </summary>
    /// <typeparam name="TOut">The type of the output flow value.</typeparam>
    /// <param name="thisFlow">The task representing the input flow.</param>
    /// <param name="operationToTry">The asynchronous operation to try on the failure value.</param>
    /// <param name="exceptionHandler">The function to handle any exceptions that occur during the operation.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns a new flow with the result of the operation 
    /// wrapped in a success, or the handled exception result.
    /// </returns>
    public static async Task<Flow<TOut>> OnFailureTry<TOut>(this Task<Flow<TOut>> thisFlow, Func<Failure, Task<TOut>> operationToTry, Func<Exception, Flow<TOut>> exceptionHandler)
    {
        try
        {
            var awaitedFlow = await thisFlow.ConfigureAwait(false);

            return await awaitedFlow.Match(
                                            async failure => Flow<TOut>.Success(await operationToTry(failure).ConfigureAwait(false)),
                                            success => Task.FromResult(Flow<TOut>.Success(success))).ConfigureAwait(false);
        }
        catch (Exception ex) { return exceptionHandler(ex); }
    }

}
