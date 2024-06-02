using Flow.Core.Areas.Returns;
using Flow.Core.Demos.AppClient.Common.Models;
using Flow.Core.Demos.Contracts.Common.CustomFailures;
using Flow.Core.Demos.Contracts.Utilities;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace Flow.Core.Demos.AppClient;

public class Checking_Serialization_Examples
{
    /*
        * If you are going to use Grpc code first or web api with json then, IMHO, its advisable to make a quick console application as a sandbox to make sure
        * that your types can be serialized and deserialized correctly without all the overhead of servers and services that complicate matters.
        * Most of the time just using the following is all you need to identify any problems
        * 
        * Flow and its associated Failures can be serialized by grpc and json, the T in Flow<T> may not be!. 
    */ 


    public void GeneralJsonTypeExamples()
    {

        var person = new Person("Maria", "Anders", new DateOnly(1992, 11, 13), new Address("Obere Str. 57", "Berlin", "12209"));

        var jsonString   = JsonSerializer.Serialize(person);
        var clonedPerson = JsonSerializer.Deserialize<Person>(jsonString);

        Console.WriteLine($"Person: {person}\r\n");
        Console.WriteLine($"Person json string: {person}\r\n");
        Console.WriteLine($"Person clone: {clonedPerson}\r\n");

        Flow<Person> flowPerson = person;

        var jsonFlowString   = JsonSerializer.Serialize(flowPerson);
        var clonedFlowPerson = JsonSerializer.Deserialize<Flow<Person>>(jsonFlowString);

        Console.WriteLine($"Flow<Person>: {flowPerson}\r\n");
        Console.WriteLine($"Flow<Person> json string: {jsonFlowString}\r\n");

        Console.WriteLine($"Flow<Person> clone: {clonedFlowPerson}\r\n");
        clonedFlowPerson?.Match(failure => Console.WriteLine(failure),success => Console.WriteLine($"Flow<Person> clone success value: {success}\r\n"));
    }
    public void GeneralGrpcTypeExamples()
    {

        var person = new Person("Maria", "Anders", new DateOnly(1992, 11, 13), new Address("Obere Str. 57", "Berlin", "12209"));

        var clonedPerson = ProtoBuf.Serializer.DeepClone<Person>(person);//this does a serialize and deserialize.

        Console.WriteLine($"Person: {person}\r\n");
        Console.WriteLine($"Person clone: {clonedPerson}\r\n");

        Flow<Person> flowPerson = person;
        var clonedFlowPerson    = ProtoBuf.Serializer.DeepClone<Flow<Person>>(flowPerson);

        Console.WriteLine($"Flow<Person>: {flowPerson}\r\n");
        flowPerson?.Match(failure => Console.WriteLine(failure), success => Console.WriteLine($"Flow<Person> success value: {success}\r\n"));

        Console.WriteLine($"Flow<Person> clone: {clonedFlowPerson}\r\n");
        clonedFlowPerson?.Match(failure => Console.WriteLine(failure), success => Console.WriteLine($"Flow<Person> clone success value: {success}\r\n"));
    }

    /*
        * The flow library has approximately 23 failure types that inherit from the base abstract class Failure, this includes NoFailure (start of numbering) 
        * and UnknownFailure (end of type discriminator numbering).Non of the derived types add any addition properties, they all use the same base type properties. 
        
        * To ensure serialization and polymorphic serialization work, both json and protobuf-net attributes have been added to the available failure types. 
        * Custom failure type discriminator numbering should start at 200 or higher to avoid clashes with the types in the library.
        *             * 
        * NB. The base property Exception purposely does not have attributes and is not serialized, its only purpose is to store the exception that may have prompted 
        * a conversion to a failure, for logging at some point prior to any serialization across process boundaries etc.
     */
    public void CustomFailuresViaJsonExample()
    {
        /*
            * As well as adding the appropriate json attributes you will also need to add the custom failure type to the json type resolver. I have made an example that can be used to get you started.
            * You can either create a class derived from the DefaultJsonTypeInfoResolver or use the DefaultJsonTypeInfoResolver and the WithAddedModifier. The code for both is basically the same.
             
            * The example adds new derived types (custom failures) if not found on the base Failure type (discriminators less than 200)). There may well be better ways
            * to achieve this but this approach works if you absolutely need custom types. The SubTypeID on the base type was added so you could have sub categories of the built-in
            * failure types, that could then be mapped to enumerations etc to negate custom failures.

        */

        /*
            * The type resolver needs to be added for both serialization (to add the type discriminator value) and deserialization (to use the discriminator value)
            * The easiest way I found was just to create a static helper class, placing it in the contracts project that's referenced by both the client and server.
        */

        var notApprovedFailure = new NotApprovedFailure("Bad credit rating", "Accounts");
        //This is a custom failure not part of the library see Flow.Core.Demos.Contracts project Common\CustomFailures\NotApprovedFailure.cs

        var customTypeResolver = Flow.Core.Demos.Contracts.Utilities.CustomFailureHelper.GetJsonCustomFailureTypeResolver();
        /*
            * I found that I needed to add PropertyNameCaseInsensitive = true, to ensure correct deserialization from the serialized web api content.
            * despite the internal types working fine with no serializer options, other wise the failure is null.
            *
            * Remove the json options to see serialization exceptions
        */
        

       // var jsonOptions = new JsonSerializerOptions() { TypeInfoResolver = customTypeResolver, PropertyNameCaseInsensitive = true };
        var jsonOptions = new JsonSerializerOptions() { TypeInfoResolver = new DefaultJsonTypeInfoResolver().WithAddedModifier(CustomFailureHelper.AddCustomFailuresToJsonResolver) , PropertyNameCaseInsensitive = true };


        var test = notApprovedFailure.GetType().IsAssignableFrom(typeof(Failure));

        test = notApprovedFailure is Failure;
        test = notApprovedFailure.GetType().IsAssignableTo(typeof(Failure));

        var failureString = JsonSerializer.Serialize(notApprovedFailure,jsonOptions);

        var cloned = JsonSerializer.Deserialize<NotApprovedFailure>(failureString, jsonOptions);


        var failedPersonFlow = Flow<Person>.Failed(notApprovedFailure);

        var jsonString = JsonSerializer.Serialize(failedPersonFlow, jsonOptions);

        Flow<Person> clonedFailure = JsonSerializer.Deserialize<Flow<Person>>(jsonString, jsonOptions)!;

        Console.WriteLine($"Custom failure json string (note the $type discriminator): {jsonString}\r\n");

        clonedFailure.Match(failure => Console.WriteLine($"Cloned custom failure content: {failure}\r\n"), success => Console.WriteLine(success));
    }

    public void CustomFailuresViaGrpcExample()
    {
        /*
            * As well as adding the appropriate proto attributes you will also need to add your custom failures to the RuntimeTypeModel so that the protobuf serializer
            * knows how to serialize and deserialize the polymorphic failure types. Start your discriminator (field number for the type) numbering at 200 or higher.
            * 
            * The types need to be added to the RuntimeTypeModel on both the server (for serialization) and client (for deserialization) 
            * The easiest way I found was just to create a static helper class, placing it in the contracts project that's referenced by both the client and server.
        */

        /*
            * You need to add the types at start up before anything else. I have already done this for the previous examples
            * so just look at start of the Main method in the program file you will see CustomFailureHelper.AddCustomFailuresToGrpcRuntimeModel();
            * that uses the helper class in the Contracts project. all the code does is:
            * 
            * RuntimeTypeModel.Default.Add(typeof(Failure), true).AddSubType(200, typeof(NotApprovedFailure));
            
        */

        var notApprovedFailure = new NotApprovedFailure("Bad credit rating", "Accounts");
        //This is a custom failure not part of the library see Flow.Core.Demos.Contracts project Common\CustomFailures\NotApprovedFailure.cs

        var failedPersonFlow = Flow<Person>.Failed(notApprovedFailure);

        Flow<Person> clonedFailure = ProtoBuf.Serializer.DeepClone<Flow<Person>>(failedPersonFlow);   
              
        clonedFailure.Match(failure => Console.WriteLine($"Cloned custom failure content: {failure}\r\n"), success => Console.WriteLine(success));
    }

}
