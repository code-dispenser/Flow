using Flow.Core.Common.Models;
using ProtoBuf;
using System.Text.Json.Serialization;

namespace Flow.Core.Areas.Returns;


/// <summary>
/// Represents an optional value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the encapsulated value. Must be non-nullable.</typeparam>
[ProtoContract]
public readonly record struct Potential<T> where T : notnull
{
    private readonly T _value;

    [JsonInclude]
    [ProtoMember(1)]
    private T Value { get => _value; init => _value = value; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Potential{T}"/> has a value.
    /// </summary>
    [ProtoMember(2)]
    public bool HasValue { get; }
    /// <summary>
    /// Gets a value indicating whether this <see cref="Potential{T}"/> has no value.
    /// </summary>
    public bool HasNoValue => !HasValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="Potential{T}"/> struct with the specified value.
    /// </summary>
    /// <param name="value">The non-null value to encapsulate.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
    public Potential(T value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));

        _value = value;
        HasValue = true;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="Potential{T}"/> struct.
    /// Used for deserialization.
    /// </summary>
    /// <param name="value">The value to encapsulate.</param>
    /// <param name="hasValue">Indicates whether the potential has a value.</param>
    [JsonConstructor]
    private Potential(T value, bool hasValue)
    {
        _value   = value;
        HasValue = hasValue;
    }
    /// <summary>
    /// Implicitly converts a non-null value to a <see cref="Potential{T}"/>.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    public static implicit operator Potential<T>(T value) => value is null ? default : new Potential<T>(value);
    
    /// <summary>
    /// Implicitly converts a <see cref="None"/> value to an empty <see cref="Potential{T}"/>.
    /// </summary>
    /// <param name="_">The none value.</param>
    public static implicit operator Potential<T>(None _) => default;

    /// <summary>
    /// Returns a <see cref="Potential{T}"/> instance with no value.
    /// </summary>
    public static Potential<T> WithoutValue() => default;

    /// <summary>
    /// Returns a <see cref="Potential{T}"/> instance containing the specified value.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    public static Potential<T> WithValue(T value) => new Potential<T>(value);

    /// <summary>
    /// Returns the encapsulated value if present; otherwise, returns the specified fallback.
    /// </summary>
    /// <param name="fallback">The value to return if <see cref="HasValue"/> is false.</param>
    /// <returns>The encapsulated value or the fallback.</returns>
    public T GetValueOr(T fallback)

        => HasValue ? _value : fallback;

    /// <summary>
    /// Executes the specified action based on whether the potential has a value.
    /// </summary>
    /// <param name="act_onNoValue">Action to execute if there is no value.</param>
    /// <param name="act_onValue">Action to execute with the encapsulated value if present.</param>
    public void Match(Action act_onNoValue, Action<T> act_onValue)
    {
        if (HasValue) act_onValue(_value); else act_onNoValue();
    }

    /// <summary>
    /// Executes one of the provided functions based on whether the potential has a value.
    /// </summary>
    /// <typeparam name="TOut">The return type of the match functions.</typeparam>
    /// <param name="onNoValue">Function to execute if there is no value.</param>
    /// <param name="onValue">Function to execute with the encapsulated value if present.</param>
    /// <returns>The result of the executed function.</returns>
    public TOut Match<TOut>(Func<TOut> onNoValue, Func<T, TOut> onValue)

        => HasValue ? onValue(_value) : onNoValue();

    /// <summary>
    /// Transforms the encapsulated value using the specified function if present.
    /// </summary>
    /// <typeparam name="TOut">The type of the transformed value.</typeparam>
    /// <param name="onValue">Function to transform the value.</param>
    /// <returns>A new <see cref="Potential{TOut}"/> containing the transformed value or empty.</returns>
    public Potential<TOut> Map<TOut>(Func<T, TOut> onValue) where TOut : notnull

        => HasValue ? new Potential<TOut>(onValue(_value)) : default;

    /// <summary>
    /// Transforms the encapsulated value using the specified function if present, returning a new <see cref="Potential{TOut}"/>.
    /// </summary>
    /// <typeparam name="TOut">The type of the resulting potential.</typeparam>
    /// <param name="onValue">Function to transform the value into a new potential.</param>
    /// <returns>The resulting <see cref="Potential{TOut}"/> or an empty potential.</returns>
    public Potential<TOut> Bind<TOut>(Func<T, Potential<TOut>> onValue) where TOut : notnull

        => HasValue ? onValue(_value) : default;

    /// <summary>
    /// Returns a string that represents the current potential.
    /// </summary>
    /// <returns>A string representing the potential value or an empty symbol.</returns>
    public override string ToString()

        => HasValue ? $"Potential({_value})" : "Ø";
}
