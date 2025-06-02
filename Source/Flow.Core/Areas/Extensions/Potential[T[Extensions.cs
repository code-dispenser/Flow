using Flow.Core.Areas.Returns;
using System.Runtime.CompilerServices;

namespace Flow.Core.Areas.Extensions;

public static partial class PotentialExtensions
{
    public static T GetValueOr<T>(this Potential<T> thisPotential, T orValue) where T : notnull

        => thisPotential.Reduce(orValue);
}
