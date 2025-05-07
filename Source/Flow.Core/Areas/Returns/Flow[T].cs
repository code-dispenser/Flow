using Flow.Core.Common.Models;
using ProtoBuf;
using System.Text.Json.Serialization;

namespace Flow.Core.Areas.Returns;


/// <summary>
/// Represents a result of an operation that can either be a success with a value of type <typeparamref name="T"/> or a failure 
/// derived from <see cref="Failure"/>.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
[ProtoContract]
public class Flow<T>
{
    [JsonInclude][ProtoMember(1)] private Failure FailureValue  { get; } = default!;

    [JsonInclude][ProtoMember(2)] private T?      SuccessValue  { get; }


    /// <summary>
    /// Gets a value indicating whether the result is a success.
    /// </summary>
    [JsonInclude][ProtoMember(3)] public bool     IsSuccess     { get; }


    /// <summary>
    /// Gets a value indicating whether the result is a failure.
    /// </summary>
    public bool IsFailure => !IsSuccess;


    private Flow() { }

    //Used by the static methods and implicit operators to create a successful flow
    private Flow(T? successValue)
    {
        if (successValue is null || successValue is Failure) throw new ArgumentException("A successful value cannot be null or a type of failure",nameof(successValue));

        SuccessValue    = successValue;
        IsSuccess       = true;
        FailureValue    = Failure.CreateNoFailure();
    }

    //Used by the static methods and implicit operators to create a failed flow
    private Flow(T? successValue, Failure failureValue)
    {
        FailureValue = failureValue ?? throw new ArgumentNullException(nameof(failureValue),"Failure cannot be null");
        SuccessValue = successValue;
    }

    //Used by the System.Text.Json serializer to deserialize a flow
    [JsonConstructor]
    internal Flow(T? successValue, Failure failureValue, bool isSuccess)
    {
        SuccessValue = successValue;
        FailureValue = failureValue;
        IsSuccess    = isSuccess;
    }


    /// <summary>
    /// Creates a new success <see cref="Flow{T}"/> instance.
    /// </summary>
    /// <param name="successValue">The success value.</param>
    /// <returns>A new success <see cref="Flow{T}"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown if the successValue is null or a type of failure.</exception> 
    public static Flow<T> Success(T successValue)    
        
        => new Flow<T>(successValue);


    /// <summary>
    /// Creates a <see cref="Flow{None}"/> representing a successful result with no meaningful return value.
    /// </summary>
    /// <returns>A successful <see cref="Flow{None}"/> containing <see cref="None.Value"/>.</returns>
    public static Flow<None> Success()  
        
        => new Flow<None>(None.Value);


    /// <summary>
    /// Creates a <see cref="Flow{T}"/> representing a failure with the specified failure value.
    /// </summary>
    /// <param name="failureValue">The failure detail to encapsulate.</param>
    /// <returns>A <see cref="Flow{T}"/> indicating failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureValue"/> is null.</exception>
    public static Flow<T>  Failed(Failure failureValue) 
        
        => new Flow<T>(default, failureValue);


    /// <summary>
    /// Implicit conversion from a successful value to a <see cref="Flow{T}"/> instance.
    /// </summary>
    /// <param name="successValue">The success value.</param>
    /// <returns>A new flow that has succeeded with the specified value.</returns>
    public static implicit operator Flow<T>(T successValue)         
        
        => new Flow<T>(successValue);


    /// <summary>
    /// Implicit conversion from a failure value to a a <see cref="Flow{T}"/> instance.
    /// </summary>
    /// <param name="failureValue">The failure value.</param>
    /// <returns>A new flow that has failed with the specified failure value.</returns>
    public static implicit operator Flow<T>(Failure failureValue)   
        
        => new Flow<T>(default, failureValue);


    /// <summary>
    /// Executes one of the provided functions based on whether the flow is successful or failed.
    /// </summary>
    /// <param name="act_onFailure">Function to execute if the flow is a failure.</param>
    /// <param name="act_onSuccess">Function to execute if the flow is a success.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task Match(Func<Failure, Task> act_onFailure, Func<T, Task> act_onSuccess)
    
        => IsSuccess ? act_onSuccess(SuccessValue!) : act_onFailure(FailureValue);


    /// <summary>
    /// Executes the specified action depending on whether the flow is a success or a failure.
    /// </summary>
    /// <param name="act_onFailure">Action to execute if the flow is a failure.</param>
    /// <param name="act_onSuccess">Action to execute if the flow is a success.</param>
    public void Match(Action<Failure> act_onFailure, Action<T> act_onSuccess)
    {
        if (true == IsSuccess) act_onSuccess(SuccessValue!); else act_onFailure(FailureValue);
    }


    /// <summary>
    /// Executes one of the provided functions and returns the result based on whether the flow is successful or failed.
    /// </summary>
    /// <typeparam name="TOut">The type of the result.</typeparam>
    /// <param name="onFailure">Function to execute if the flow is a failure.</param>
    /// <param name="onSuccess">Function to execute if the flow is a success.</param>
    /// <returns>The result of the executed function.</returns>
    public TOut Match<TOut>(Func<Failure, TOut> onFailure, Func<T, TOut> onSuccess) 
        
        => IsSuccess ? onSuccess(SuccessValue!) : onFailure(FailureValue);


    /// <summary>
    /// Executes one of the provided asynchronous functions and returns the result based on whether the flow is successful or failed.
    /// </summary>
    /// <typeparam name="TOut">The type of the result.</typeparam>
    /// <param name="onFailure">Asynchronous function to execute if the flow is a failure.</param>
    /// <param name="onSuccess">Asynchronous function to execute if the flow is a success.</param>
    /// <returns>A task representing the asynchronous operation that returns the result of the executed function.</returns>
    public Task<TOut> Match<TOut>(Func<Failure, Task<TOut>> onFailure, Func<T, Task<TOut>> onSuccess)

        => IsSuccess ? onSuccess(SuccessValue!) : onFailure(FailureValue);



    /// <summary>
    /// Transforms the success value of the flow using the specified function if the flow is successful.
    /// </summary>
    /// <typeparam name="TOut">The type of the result.</typeparam>
    /// <param name="onSuccess">Function to transform the success value.</param>
    /// <returns>A new flow with the transformed success value or the original failure value.</returns>
    public Flow<TOut> Map<TOut>(Func<T,TOut> onSuccess)

        => IsSuccess ? Flow<TOut>.Success(onSuccess(SuccessValue!)) : Flow<TOut>.Failed(FailureValue);


    /// <summary>
    /// Transforms the success value or failure value of the flow using the specified functions.
    /// </summary>
    /// <typeparam name="TOut">The type of the result.</typeparam>
    /// <param name="onFailure">Function to transform the failure value.</param>
    /// <param name="onSuccess">Function to transform the success value.</param>
    /// <returns>A new flow with the transformed values.</returns>
    public Flow<TOut> Map<TOut>(Func<Failure, Failure> onFailure, Func<T, TOut> onSuccess)

        => IsSuccess ? Flow<TOut>.Success(onSuccess(SuccessValue!)) : Flow<TOut>.Failed(onFailure(FailureValue));


    /// <summary>
    /// Transforms the success value of the flow using the specified function if the flow is successful, returning a new flow.
    /// </summary>
    /// <typeparam name="TOut">The type of the result.</typeparam>
    /// <param name="onSuccess">Function to transform the success value into a new flow.</param>
    /// <returns>The new flow resulting from the transformation function or the original failure.</returns>
    public Flow<TOut> Bind<TOut>(Func<T, Flow<TOut>> onSuccess) 
        
        => IsSuccess ? onSuccess(SuccessValue!) : Flow<TOut>.Failed(FailureValue);


    /// <summary>
    /// Transforms the success value of the flow using the specified asynchronous function if the flow is successful, returning a new flow.
    /// </summary>
    /// <typeparam name="TOut">The type of the result.</typeparam>
    /// <param name="onSuccess">Asynchronous function to transform the success value into a new flow.</param>
    /// <returns>A task representing the asynchronous operation that returns the new flow or the original failure.</returns>
    public Task<Flow<TOut>> Bind<TOut>(Func<T, Task<Flow<TOut>>> onSuccess) 
        
        => IsSuccess ? onSuccess(SuccessValue!) : Task.FromResult(Flow<TOut>.Failed(FailureValue));


}

