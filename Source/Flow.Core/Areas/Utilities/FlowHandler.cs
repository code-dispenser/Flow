using Flow.Core.Areas.Returns;

namespace Flow.Core.Areas.Utilities;

/// <summary>
/// Provides methods to handle operations that may throw exceptions and convert them to <see cref="Flow{T}" />.
/// </summary>
public static class FlowHandler
{

    /// <summary>
    /// Tries to execute a synchronous operation that returns a value of type <typeparamref name="T"/> and handles any exceptions by converting them to a <see cref="Flow{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value in the <see cref="Flow{T}"/>.</typeparam>
    /// <param name="operationToTry">The synchronous operation to execute.</param>
    /// <param name="exceptionHandler">The function to handle exceptions and convert them to a <see cref="Flow{T}"/>.</param>
    /// <returns>A <see cref="Flow{T}"/> that encapsulates the result of the operation or an exception.</returns>
    public static Flow<T> TryToFlow<T>(Func<T> operationToTry, Func<Exception, Flow<T>> exceptionHandler)
    {
        try
        {
            return operationToTry();//uses implicit conversion to Flow if not already a Flow
        }
        catch (Exception ex) { return exceptionHandler(ex); }
    }

    /// <summary>
    /// Tries to execute an operation that returns a <see cref="Flow{T}"/> and handles any exceptions by converting them to a <see cref="Flow{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value in the <see cref="Flow{T}"/>.</typeparam>
    /// <param name="operationToTry">The operation to execute.</param>
    /// <param name="exceptionHandler">The function to handle exceptions and convert them to a <see cref="Flow{T}"/>.</param>
    /// <returns>A <see cref="Flow{T}"/> representing the result of the operation or an exception.</returns>
    public static Flow<T> TryToFlow<T>(Func<Flow<T>> operationToTry, Func<Exception, Flow<T>> exceptionHandler)
    {
        try
        {
            return operationToTry();
        }
        catch (Exception ex) { return exceptionHandler(ex); }
    }


    /// <summary>
    /// Asynchronously tries to execute an operation that returns a task with a value of type <typeparamref name="T"/> and handles any exceptions by converting them to a <see cref="Flow{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value in the <see cref="Flow{T}"/>.</typeparam>
    /// <param name="operationToTry">The asynchronous operation to execute.</param>
    /// <param name="exceptionHandler">The function to handle exceptions and convert them to a <see cref="Flow{T}"/>.</param>
    /// <returns>A task representing a <see cref="Flow{T}"/> that encapsulates the result of the operation or an exception.</returns>
    public static async Task<Flow<T>> TryToFlow<T>(Func<Task<T>> operationToTry, Func<Exception, Flow<T>> exceptionHandler)
    {
        try
        {
            return await operationToTry();
        }
        catch (Exception ex) { return exceptionHandler(ex); }
    }

    /// <summary>
    /// Asynchronously tries to execute an operation that returns a task with a <see cref="Flow{T}"/> and handles any exceptions by converting them to a <see cref="Flow{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value in the <see cref="Flow{T}"/>.</typeparam>
    /// <param name="operationToTry">The asynchronous operation to execute.</param>
    /// <param name="exceptionHandler">The function to handle exceptions and convert them to a <see cref="Flow{T}"/>.</param>
    /// <returns>A task representing a <see cref="Flow{T}"/> that encapsulates the result of the operation or an exception.</returns>
    public static async Task<Flow<T>> TryToFlow<T>(Func<Task<Flow<T>>> operationToTry, Func<Exception, Flow<T>> exceptionHandler)
    {
        try
        {
            return await operationToTry();
        }
        catch (Exception ex) { return exceptionHandler(ex); }
    }

}
