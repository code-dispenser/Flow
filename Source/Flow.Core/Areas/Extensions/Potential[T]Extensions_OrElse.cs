using Flow.Core.Areas.Returns;

namespace Flow.Core.Areas.Extensions;

public static partial class PotentialExtensions
{
    public static Potential<T> OrElse<T>(this Potential<T> potential, Func<Potential<T>> onNone) where T : notnull

        => potential.HasValue ? potential : onNone();

    public static Potential<T> OrElse<T>(this Potential<T> potential, Func<T> onNone) where T : notnull

        => potential.HasValue ? potential : onNone();//uses implicit conversion so null will be none.
}
