using Flow.Core.Areas.Returns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow.Core.Areas.Extensions;

public static partial class FlowExtensions
{

    /// <summary>
    /// Tries to perform an operation on the success value of a flow, handling any exceptions,
    /// and wraps the result into a new flow.
    /// </summary>
    /// <typeparam name="TIn">The type of the input flow value.</typeparam>
    /// <typeparam name="TOut">The type of the output flow value.</typeparam>
    /// <param name="thisFlow">The input flow.</param>
    /// <param name="operationToTry">The operation to perform on the success value.</param>
    /// <param name="exceptionHandler">The function to handle any exceptions that occur during the operation.</param>
    /// <returns>
    /// A new flow containing the result of the operation if successful, or the result of the exception handler if an exception occurs.
    /// </returns>
    public static Flow<TOut> OnSuccessTry<TIn,TOut>(this Flow<TIn> thisFlow, Func<TIn, TOut> operationToTry, Func<Exception, Flow<TOut>> exceptionHandler)
    {
        try
        {
            return thisFlow.Bind(success => Flow<TOut>.Success(operationToTry(success)));
        }
        catch (Exception ex) { return exceptionHandler(ex); }
    }

    /// <summary>
    /// Tries to perform an asynchronous operation on the success value of a flow, handling any exceptions,
    /// and wraps the result into a new flow.
    /// </summary>
    /// <typeparam name="TIn">The type of the input flow value.</typeparam>
    /// <typeparam name="TOut">The type of the output flow value.</typeparam>
    /// <param name="thisFlow">The input flow.</param>
    /// <param name="operationToTry">The asynchronous operation to perform on the success value.</param>
    /// <param name="exceptionHandler">The function to handle any exceptions that occur during the operation.</param>
    /// <returns>
    /// A task that returns a new flow containing the result of the operation if successful, 
    /// or the result of the exception handler if an exception occurs.
    /// </returns>
    public static async Task<Flow<TOut>> OnSuccessTry<TIn, TOut>(this Flow<TIn> thisFlow, Func<TIn, Task<TOut>> operationToTry, Func<Exception, Flow<TOut>> exceptionHandler)
    {
        try
        {
            return await thisFlow.Match(failure => Task.FromResult(Flow<TOut>.Failed(failure)),
                                  async success => Flow<TOut>.Success(await operationToTry(success).ConfigureAwait(false))).ConfigureAwait(false);

        }
        catch (Exception ex) { return exceptionHandler(ex); }
    }


    /// <summary>
    /// Tries to perform an asynchronous operation on the success value of a task flow, handling any exceptions.
    /// </summary>
    /// <typeparam name="TIn">The type of the input flow value.</typeparam>
    /// <typeparam name="TOut">The type of the output flow value.</typeparam>
    /// <param name="thisFlow">The task representing the input flow.</param>
    /// <param name="operationToTry">The asynchronous operation to try on the success value.</param>
    /// <param name="exceptionHandler">The function to handle any exceptions that occur.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns a new flow with the result of the operation or the handled exception.
    /// </returns>
    public static async Task<Flow<TOut>> OnSuccessTry<TIn, TOut>(this Task<Flow<TIn>> thisFlow, Func<TIn, Task<Flow<TOut>>> operationToTry, Func<Exception, Flow<TOut>> exceptionHandler)
    {
        try
        {
            return await (await thisFlow.ConfigureAwait(false)).Bind<TOut>(operationToTry).ConfigureAwait(false);
        }
        catch (Exception ex) { return exceptionHandler(ex); }
    }

    /// <summary>
    /// Attempts to transform the success value using the specified function, returning a failure if an exception occurs.
    /// </summary>
    /// <typeparam name="TIn">The type of the input flow value.</typeparam>
    /// <typeparam name="TOut">The type of the resulting flow value.</typeparam>
    /// <param name="thisFlow">The flow to transform if successful.</param>
    /// <param name="operationToTry">The function to apply to the success value.</param>
    /// <param name="exceptionHandler">A function that returns a failure flow if an exception occurs.</param>
    /// <returns>A transformed flow if successful; otherwise, a failure flow.</returns>
    public static Flow<TOut> OnSuccessTry<TIn, TOut>(this Flow<TIn> thisFlow, Func<TIn, Flow<TOut>> operationToTry, Func<Exception, Flow<TOut>> exceptionHandler)
    {
        try
        {
            return thisFlow.Bind<TOut>(operationToTry);
        }
        catch (Exception ex) { return exceptionHandler(ex); }
    }
}
