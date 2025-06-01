using Flow.Core.Areas.Returns;

namespace Flow.Core.Areas.Extensions;

public static partial class PotentialExtensions
{
    public static Potential<TOut> AndThen<TIn, TOut>(this Potential<TIn> potential, Func<TIn, Potential<TOut>> onValue) where TIn : notnull where TOut : notnull
        
        => potential.Bind(onValue);

    public static Potential<TOut> AndThen<TIn, TOut>(this Potential<TIn> potential, Func<TIn, TOut> onValue) where TIn : notnull where TOut : notnull

        => potential.Map(onValue);
}
