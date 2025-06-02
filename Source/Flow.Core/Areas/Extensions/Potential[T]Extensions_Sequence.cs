using Flow.Core.Areas.Returns;

namespace Flow.Core.Areas.Extensions;

public static partial class PotentialSequence
{
    public static Potential<IEnumerable<T>> Sequence<T>(this IEnumerable<Potential<T>> potentials) where T : notnull
    {
        if (potentials is null) throw new ArgumentNullException(nameof(potentials));

        List<T> list = [];

        foreach (var potential in potentials)
        {
            if (potential.HasNoValue) return Potential<IEnumerable<T>>.WithoutValue();

            list.Add(potential.Reduce(default!));

        }

        return Potential<IEnumerable<T>>.WithValue(list);
    }

}
