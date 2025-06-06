using Flow.Core.Areas.Returns;

namespace Flow.Core.Areas.Extensions;

public static partial class PotentialExtensions
{
    /// <summary>
    /// Returns the original potential if it has a value; otherwise, evaluates the provided function and returns its result.
    /// </summary>
    /// <typeparam name="T">The type of the potential value.</typeparam>
    /// <param name="potential">The original potential.</param>
    /// <param name="onNone">A function that returns an alternative <see cref="Potential{T}"/> when the original has no value.</param>
    /// <returns>
    /// The original <see cref="Potential{T}"/> if it has a value; otherwise, the result of <paramref name="onNone"/>.
    /// </returns>
    public static Potential<T> OrElse<T>(this Potential<T> potential, Func<Potential<T>> onNone) where T : notnull

        => potential.HasValue ? potential : onNone();

    /// <summary>
    /// Returns the original potential if it has a value; otherwise, evaluates the provided function to obtain a fallback value.
    /// </summary>
    /// <typeparam name="T">The type of the potential value.</typeparam>
    /// <param name="potential">The original potential.</param>
    /// <param name="onNone">A function that returns a fallback value when the original has no value.</param>
    /// <returns>
    /// The original <see cref="Potential{T}"/> if it has a value; otherwise, a new <see cref="Potential{T}"/> containing the fallback value.
    /// </returns>
    public static Potential<T> OrElse<T>(this Potential<T> potential, Func<T> onNone) where T : notnull

        => potential.HasValue ? potential : onNone();//uses implicit conversion so null will be none.
}
