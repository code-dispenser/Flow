using Flow.Core.Areas.Returns;

namespace Flow.Core.Areas.Extensions;

public static partial class PotentialExtensions
{
    /// <summary>
    /// Transforms a sequence of <see cref="Potential{T}"/> values into a single
    /// <c>Potential&lt;IEnumerable&lt;T&gt;&gt;</c>. If any element in the sequence has no value,
    /// the result is <c>Potential&lt;IEnumerable&lt;T&gt;&gt;.WithoutValue()</c>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the input potentials.</typeparam>
    /// <param name="potentials">The sequence of potential values to combine.</param>
    /// <returns>
    /// A <c>Potential&lt;IEnumerable&lt;T&gt;&gt;</c> containing all values if all potentials have values,
    /// otherwise <c>Potential&lt;IEnumerable&lt;T&gt;&gt;.WithoutValue()</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="potentials"/> is <c>null</c>.</exception>
    public static Potential<IEnumerable<T>> Sequence<T>(this IEnumerable<Potential<T>> potentials) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(potentials);
    
        List<T> list = [];

        foreach (var potential in potentials)
        {
            if (potential.HasNoValue) return Potential<IEnumerable<T>>.WithoutValue();

            list.Add(potential.GetValueOr(default!));

        }

        return Potential<IEnumerable<T>>.WithValue(list);
    }

}
