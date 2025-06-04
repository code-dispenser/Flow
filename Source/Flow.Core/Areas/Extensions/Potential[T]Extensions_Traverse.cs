using Flow.Core.Areas.Returns;
using System;

namespace Flow.Core.Areas.Extensions;

public static partial class PotentialExtensions
{
    public static Potential<IEnumerable<TOut>> Traverse<TIn, TOut>(this IEnumerable<TIn> source, Func<TIn, Potential<TOut>> transformer) where TIn : notnull where TOut : notnull
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

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
