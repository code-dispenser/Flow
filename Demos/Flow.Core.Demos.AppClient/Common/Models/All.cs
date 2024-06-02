
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Flow.Core.Demos.AppClient.Common.Models;

public record class Person
{
    [ProtoMember(1)] public Address Address  { get; }
    [ProtoMember(2)] public string FirstName { get; }
    [ProtoMember(3)] public string Surname   { get; }
    [ProtoMember(4)] public DateOnly DOB     { get; }

    private Person() {}//for grpc 

    [JsonConstructor]
    public Person(string firstName, string surname, DateOnly dob, Address address)

        => (FirstName, Surname, DOB, Address) = (firstName, surname, dob, address);
       
}

[ProtoContract]
public record class Address
{
    [ProtoMember(1)] public string AddressLine { get; }
    [ProtoMember(2)] public string TownCity    { get; }
    [ProtoMember(3)] public string PostCode    { get; }
   
    private Address() { }

    [JsonConstructor]
    public Address(string addressLine, string townCity, string postCode)

        => (AddressLine, TownCity, PostCode) = (addressLine, townCity, postCode);
}
