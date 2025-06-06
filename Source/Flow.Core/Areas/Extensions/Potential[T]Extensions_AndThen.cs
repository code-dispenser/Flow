using Flow.Core.Areas.Returns;

namespace Flow.Core.Areas.Extensions;

/// <summary>
/// Provides extension methods for working with <see cref="Potential{T}"/> values.
/// </summary>
public static partial class PotentialExtensions
{
    /// <summary>
    /// Applies a transformation function to the value of the potential if it exists,
    /// returning a new <see cref="Potential{TOut}"/>. This is equivalent to monadic bind.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <param name="potential">The input potential.</param>
    /// <param name="onValue">A function to transform the input value into a new potential.</param>
    /// <returns>
    /// A new <see cref="Potential{TOut}"/> resulting from the transformation function if a value exists;
    /// otherwise, an empty <see cref="Potential{TOut}"/>.
    /// </returns>
    public static Potential<TOut> AndThen<TIn, TOut>(this Potential<TIn> potential, Func<TIn, Potential<TOut>> onValue) where TIn : notnull where TOut : notnull
        
        => potential.Bind(onValue);

    /// <summary>
    /// Applies a transformation function to the value of the potential if it exists,
    /// returning a new <see cref="Potential{TOut}"/> with the transformed value.
    /// This is equivalent to functor map.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <typeparam name="TOut">The type of the transformed value.</typeparam>
    /// <param name="potential">The input potential.</param>
    /// <param name="onValue">A function to transform the input value.</param>
    /// <returns>
    /// A new <see cref="Potential{TOut}"/> containing the transformed value if a value exists;
    /// otherwise, an empty <see cref="Potential{TOut}"/>.
    /// </returns>
    public static Potential<TOut> AndThen<TIn, TOut>(this Potential<TIn> potential, Func<TIn, TOut> onValue) where TIn : notnull where TOut : notnull

        => potential.Map(onValue);
}
