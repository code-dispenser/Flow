using System.Numerics;
using System.Runtime.CompilerServices;

namespace Flow.Core.Tests.SharedDataAndFixtures.Common.Validation;

internal static class Check
{
    public static T ThrowIfNullEmptyOrWhitespace<T>(T argument, [CallerArgumentExpression(nameof(argument))] string argumentName = "")

        => argument is null || typeof(T).Name == "String" && string.IsNullOrWhiteSpace(argument as string)
                ? throw new ArgumentException("The argument cannot be null or empty.", argumentName)
                    : argument;

    public static T ThrowIfNotInRange<T>(T argument, T minValue, T maxValue, [CallerArgumentExpression(nameof(argument))] string argumentName = "") where T : INumber<T>

        => (argument < minValue || argument > maxValue)
                ? throw new ArgumentException($"The argument is not in range of min: {minValue} to max {maxValue}", argumentName)
                    : argument; 
}
