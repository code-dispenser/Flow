using Flow.Core.Areas.Returns;
using System;

namespace Flow.Core.Areas.Extensions;

public static partial class PotentialExtensions
{
    /// <summary>
    /// Applies a transformation function to each element in the <paramref name="source"/> sequence,
    /// producing a sequence of <see cref="Potential{TOut}"/> values, and returns a
    /// <c>Potential&lt;IEnumerable&lt;TOut&gt;&gt;</c> containing all results if all transformations succeed.
    /// </summary>
    /// <typeparam name="TIn">The type of elements in the input sequence.</typeparam>
    /// <typeparam name="TOut">The type of the transformed elements.</typeparam>
    /// <param name="source">The input sequence of elements to transform.</param>
    /// <param name="transformer">A function that transforms each element into a <see cref="Potential{TOut}"/>.</param>
    /// <returns>
    /// A <c>Potential&lt;IEnumerable&lt;TOut&gt;&gt;</c> containing all transformed values if every transformation
    /// returns a value; otherwise, <c>Potential&lt;IEnumerable&lt;TOut&gt;&gt;.WithoutValue()</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is <c>null</c>.</exception>
    public static Potential<IEnumerable<TOut>> Traverse<TIn, TOut>(this IEnumerable<TIn> source, Func<TIn, Potential<TOut>> transformer) where TIn : notnull where TOut : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
    
        List<TOut> transformerResults = [];

        foreach (var item in source)
        {
            var newOption = transformer(item);

            if (newOption.HasNoValue) return Potential<IEnumerable<TOut>>.WithoutValue();

            transformerResults.Add(newOption.GetValueOr(default!));
        }

        return Potential<IEnumerable<TOut>>.WithValue(transformerResults);
    }
}
