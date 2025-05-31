using Flow.Core.Areas.Returns;

namespace Flow.Core.Areas.Extensions;

public static partial class MightSequence
{
    public static Might<IEnumerable<T>> Sequence<T>(this IEnumerable<Might<T>> mightHaves) where T : notnull
    {
        if (mightHaves is null) throw new ArgumentNullException(nameof(mightHaves));

        List<T> list = [];

        foreach (var might in mightHaves)
        {
            if (might.HasNoValue) return Might<IEnumerable<T>>.WithoutValue();

            list.Add(might.GetValueOr(default!));

        }

        return Might<IEnumerable<T>>.WithValue(list);
    }

}
