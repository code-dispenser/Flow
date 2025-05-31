using Flow.Core.Areas.Returns;
using Flow.Core.Tests.SharedDataAndFixtures.Common.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using ProtoBuf;
using System.Text.Json;

namespace Flow.Core.Tests.Functional.Areas.Returns;

public class Might_SerializationTests
{

    [Fact]
    public void Protobuf_net_serializer_should_be_able_to_serialize_and_deserialize_a_might_without_a_value_correctly()
    {

        var mightWithoutValue = Might<Person>.WithoutValue();
        var deserializedMight = Serializer.DeepClone<Might<Person>>(mightWithoutValue);

        using (new AssertionScope())
        {
            deserializedMight.Should().Match<Might<Person>>(m => m.HasValue == false && m.HasNoValue == true);
            deserializedMight.GetValueOr(new Person("FirstName", "Surname", 42)).Should().Match<Person>(p => p.FirstName == "FirstName" && p.Surname == "Surname" && p.Age == 42);
        }
    }

    [Fact]
    public void Protobuf_net_serializer_should_be_able_to_serialize_and_deserialize_a_might_with_a_value_correctly()
    {
        Might<Person> mightWitValue     = new Person("John", "Doe", 25);
        var           deserializedMight = Serializer.DeepClone(mightWitValue);

        using (new AssertionScope())
        {
            deserializedMight.Should().Match<Might<Person>>(r => r.HasValue == true && r.HasNoValue == false);
            deserializedMight.GetValueOr(new Person("FirstName", "Surname", 42)).Should().Match<Person>(p => p.FirstName == "John" && p.Surname == "Doe" && p.Age == 25);
        }
    }

    [Fact]
    public void Json_serializer_should_be_able_to_serialize_and_deserialize_a_might_without_a_value_correctly()
    {

        var mightWithoutValue = Might<Person>.WithoutValue();
        var jsonString        = JsonSerializer.Serialize(mightWithoutValue);
        var deserializedMight = JsonSerializer.Deserialize<Might<Person>>(jsonString)!;

        using (new AssertionScope())
        {
            deserializedMight.Should().Match<Might<Person>>(m => m.HasValue == false && m.HasNoValue == true);
            deserializedMight.GetValueOr(new Person("FirstName", "Surname", 42)).Should().Match<Person>(p => p.FirstName == "FirstName" && p.Surname == "Surname" && p.Age == 42);
        }

    }

    [Fact]
    public void Json_serializer_should_be_able_to_serialize_and_deserialize_a_might_with_a_value_correctly()
    {

        Might<Person> mightWitValue     = new Person("John", "Doe", 25);
        var           jsonString        = JsonSerializer.Serialize(mightWitValue);
        var           deserializedMight = JsonSerializer.Deserialize<Might<Person>>(jsonString)!;

        using (new AssertionScope())
        {
            deserializedMight.Should().Match<Might<Person>>(m => m.HasValue == true && m.HasNoValue == false);
            deserializedMight.GetValueOr(new Person("FirstName", "Surname", 42)).Should().Match<Person>(p => p.FirstName == "John" && p.Surname == "Doe" && p.Age == 25);
        }

    }
}
