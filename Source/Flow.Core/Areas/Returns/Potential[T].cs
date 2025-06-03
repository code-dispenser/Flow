using Flow.Core.Common.Models;
using ProtoBuf;
using System.Text.Json.Serialization;

namespace Flow.Core.Areas.Returns;


[ProtoContract]
public readonly record struct Potential<T> where T : notnull
{
    private readonly T _value;

    [JsonInclude]
    [ProtoMember(1)]
    private T Value { get => _value; init => _value = value; }

    [ProtoMember(2)]
    public bool HasValue { get; }
    public bool HasNoValue => !HasValue;


    public Potential(T value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));

        _value = value;
        HasValue = true;
    }
    [JsonConstructor]
    private Potential(T value, bool hasValue)
    {
        _value   = value;
        HasValue = hasValue;
    }

    public static implicit operator Potential<T>(T value) => value is null ? default : new Potential<T>(value);
    public static implicit operator Potential<T>(None _) => default;

    public static Potential<T> WithoutValue() => default;
    public static Potential<T> WithValue(T value) => new Potential<T>(value);


    public T GetValueOr(T fallback)

        => HasValue ? _value : fallback;

    public void Match(Action act_onNoValue, Action<T> act_onValue)
    {
        if (HasValue) act_onValue(_value); else act_onNoValue();
    }
    public TOut Match<TOut>(Func<TOut> onNoValue, Func<T, TOut> onValue)

        => HasValue ? onValue(_value) : onNoValue();

    public Potential<TOut> Map<TOut>(Func<T, TOut> onValue) where TOut : notnull

        => HasValue ? new Potential<TOut>(onValue(_value)) : default;

    public Potential<TOut> Bind<TOut>(Func<T, Potential<TOut>> onValue) where TOut : notnull

        => HasValue ? onValue(_value) : default;

    public override string ToString()

        => HasValue ? $"Potential({_value})" : "Ø";
}
