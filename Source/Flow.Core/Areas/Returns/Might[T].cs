using Flow.Core.Common.Models;
using ProtoBuf;
using System.Text.Json.Serialization;

namespace Flow.Core.Areas.Returns;


[ProtoContract]
public readonly record struct Might<T> where T : notnull
{
    private readonly T _value;

    [JsonInclude]
    [ProtoMember(1)]
    private T Value { get => _value; init => _value = value; }

    [ProtoMember(2)]
    public bool HasValue { get; }
    public bool HasNoValue => !HasValue;


    public Might(T value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));

        _value = value;
        HasValue = true;
    }
    [JsonConstructor]
    private Might(T Value, bool hasValue)
    {
        _value   = Value;
        HasValue = hasValue;
    }

    public static implicit operator Might<T>(T value) => value is null ? default : new Might<T>(value);
    public static implicit operator Might<T>(None _) => default;

    public static Might<T> WithoutValue() => default;
    public static Might<T> WithValue(T value) => new Might<T>(value);


    public T GetValueOr(T orValue)

        => HasValue ? _value : orValue;

    public void Match(Action act_onNoValue, Action<T> act_onValue)
    {
        if (HasValue) act_onValue(_value); else act_onNoValue();
    }
    public TOut Match<TOut>(Func<TOut> onNoValue, Func<T, TOut> onValue)

        => HasValue ? onValue(_value) : onNoValue();

    public Might<TOut> Map<TOut>(Func<T, TOut> onValue) where TOut : notnull

        => HasValue ? new Might<TOut>(onValue(_value)) : default;

    public Might<TOut> Bind<TOut>(Func<T, Might<TOut>> onValue) where TOut : notnull

        => HasValue ? onValue(_value) : default;

    public override string ToString()

        => HasValue ? $"Might({_value})" : "Ø";
}
