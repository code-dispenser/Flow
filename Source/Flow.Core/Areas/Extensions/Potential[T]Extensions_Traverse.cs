using Flow.Core.Areas.Returns;
using System;

namespace Flow.Core.Areas.Extensions;

public static partial class PotentialExtensions
{
    //public static Might<IEnumerable<TOut>> Traverse<TIn, TOut>(this IEnumerable<Might<TIn>> mightHaves, Func<TIn, Might<TOut>> onValue) where TIn : notnull where TOut : notnull
    //{
    //    if (mightHaves is null) throw new ArgumentNullException(nameof(mightHaves));

    //    List<TOut> list = [];

    //    foreach (var might in mightHaves)
    //    {
    //        if (might.HasNoValue) return Might<IEnumerable<TOut>>.WithoutValue();

    //        var newMight = onValue(might.GetValueOr(default!));

    //        if (newMight.HasNoValue) return Might<IEnumerable<TOut>>.WithoutValue();

    //        list.Add(newMight.GetValueOr(default!));
    //    }

    //    return Might<IEnumerable<TOut>>.WithValue(list);
    //}
}
