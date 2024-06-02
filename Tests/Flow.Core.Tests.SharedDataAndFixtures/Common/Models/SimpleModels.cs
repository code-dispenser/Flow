using Flow.Core.Tests.SharedDataAndFixtures.Common.Validation;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Flow.Core.Tests.SharedDataAndFixtures.Common.Models;

[ProtoContract]
public class Person
{
    [ProtoMember(1)] public string  FirstName   { get; } 
    [ProtoMember(2)] public string  Surname     { get; } 
    [ProtoMember(3)] public int     Age         { get; } 

    private Person() { }//for protobuf-net

    [JsonConstructor]
    public Person(string firstName, string surname, int age)
    {
        FirstName = Check.ThrowIfNullEmptyOrWhitespace(firstName);
        Surname   = Check.ThrowIfNullEmptyOrWhitespace(surname);
        Age       = Check.ThrowIfNotInRange(age,18,70);
   }

}
