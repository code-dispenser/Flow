using Flow.Core.Areas.Returns;
using System;

namespace Flow.Core.Areas.Extensions;

public static partial class PotentialExtensions
{
    public static Potential<IEnumerable<TOut>> Traverse<TIn, TOut>(this IEnumerable<Potential<TIn>> potentials, Func<TIn, Potential<TOut>> onValue) where TIn : notnull where TOut : notnull
    {
        if (potentials is null) throw new ArgumentNullException(nameof(potentials));

        List<TOut> list = [];

        foreach (var potential in potentials)
        {
            if (potential.HasNoValue) return Potential<IEnumerable<TOut>>.WithoutValue();

            var newPotential = onValue(potential.Reduce(default!));

            if (newPotential.HasNoValue) return Potential<IEnumerable<TOut>>.WithoutValue();

            list.Add(newPotential.Reduce(default!));
        }

        return Potential<IEnumerable<TOut>>.WithValue(list);
    }
}
