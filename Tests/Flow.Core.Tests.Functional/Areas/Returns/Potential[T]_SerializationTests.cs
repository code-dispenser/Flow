using Flow.Core.Areas.Returns;
using Flow.Core.Tests.SharedDataAndFixtures.Common.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using ProtoBuf;
using System.Text.Json;

namespace Flow.Core.Tests.Functional.Areas.Returns;

public class Potential_SerializationTests
{

    [Fact]
    public void Protobuf_net_serializer_should_be_able_to_serialize_and_deserialize_a_potential_without_a_value_correctly()
    {

        var potentialWithoutValue = Potential<Person>.WithoutValue();
        var deserializedPotential = Serializer.DeepClone<Potential<Person>>(potentialWithoutValue);

        using (new AssertionScope())
        {
            deserializedPotential.Should().Match<Potential<Person>>(m => m.HasValue == false && m.HasNoValue == true);
            deserializedPotential.Reduce(new Person("FirstName", "Surname", 42)).Should().Match<Person>(p => p.FirstName == "FirstName" && p.Surname == "Surname" && p.Age == 42);
        }
    }

    [Fact]
    public void Protobuf_net_serializer_should_be_able_to_serialize_and_deserialize_a_potential_with_a_value_correctly()
    {
        Potential<Person> potentialWitValue = new Person("John", "Doe", 25);
        var           deserializedPotential = Serializer.DeepClone(potentialWitValue);

        using (new AssertionScope())
        {
            deserializedPotential.Should().Match<Potential<Person>>(r => r.HasValue == true && r.HasNoValue == false);
            deserializedPotential.Reduce(new Person("FirstName", "Surname", 42)).Should().Match<Person>(p => p.FirstName == "John" && p.Surname == "Doe" && p.Age == 25);
        }
    }

    [Fact]
    public void Json_serializer_should_be_able_to_serialize_and_deserialize_a_potential_without_a_value_correctly()
    {

        var potentialWithoutValue = Potential<Person>.WithoutValue();
        var jsonString            = JsonSerializer.Serialize(potentialWithoutValue);
        var deserializedPotential     = JsonSerializer.Deserialize<Potential<Person>>(jsonString)!;

        using (new AssertionScope())
        {
            deserializedPotential.Should().Match<Potential<Person>>(m => m.HasValue == false && m.HasNoValue == true);
            deserializedPotential.Reduce(new Person("FirstName", "Surname", 42)).Should().Match<Person>(p => p.FirstName == "FirstName" && p.Surname == "Surname" && p.Age == 42);
        }

    }

    [Fact]
    public void Json_serializer_should_be_able_to_serialize_and_deserialize_a_potential_with_a_value_correctly()
    {

        Potential<Person> potentialWithValue    = new Person("John", "Doe", 25);
        var               jsonString            = JsonSerializer.Serialize(potentialWithValue);
        var               deserializedPotential = JsonSerializer.Deserialize<Potential<Person>>(jsonString)!;

        using (new AssertionScope())
        {
            deserializedPotential.Should().Match<Potential<Person>>(m => m.HasValue == true && m.HasNoValue == false);
            deserializedPotential.Reduce(new Person("FirstName", "Surname", 42)).Should().Match<Person>(p => p.FirstName == "John" && p.Surname == "Doe" && p.Age == 25);
        }

    }
}
