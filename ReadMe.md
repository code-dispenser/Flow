[![.NET](https://github.com/code-dispenser/Flow/actions/workflows/buildandtest.yml/badge.svg?branch=main)](https://github.com/code-dispenser/Flow/actions/workflows/buildandtest.yml) [![Coverage Status](https://coveralls.io/repos/github/code-dispenser/Flow/badge.svg?branch=main)](https://coveralls.io/github/code-dispenser/Flow?branch=main) 
[![Nuget download][download-image]][download-url]

[download-image]: https://img.shields.io/nuget/dt/Flow.Core
[download-url]: https://www.nuget.org/packages/Flow.Core

<h1>
<img src="https://raw.github.com/code-dispenser/Flow/main/Assets/icon-64.png" align="center" alt="flow icon" /> Flow
</h1>
<!--
# ![icon](https://raw.githubusercontent.com/code-dispenser/Flow/main/Assets/icon-64.png) Flow 
-->
<!-- H1 for git hub, but for nuget the markdown is fine as it centers the image, uncomment as appropriate and do the same at the bottom of this file for the icon author -->

## Overview

Flow (*Flow&lt;T&gt;*) is essentially a simple, lightweight result type that allows for returning a result containing either a success or failure value. The library includes extension methods that 
ultimately wrap methods on the Flow class, such as **Map** and **Bind**. These methods facilitate the chaining of results, enabling a more declarative, functional approach.

Flow and the provided failure type classes, derived from a **Failure** base class, have all been decorated with attributes from both the **'protobuf-net'** library as well as attributes from **'System.Text.Json'**. 
This allows for the seamless flow of results across client-server boundaries using either gRPC code-first and the protobuf-net serializer or the System.Text.Json serializer.

**Note:** The type parameter **'T'** in Flow&lt;T&gt; is not automatically serializable. It is the responsibility of the developer to decorate it with any required attributes for the chosen serialization/deserialization 
processes to work as intended.

The demo project included in the repository's [source code](https://github.com/code-dispenser/Flow) shows examples of custom classes being attributed to ensure serialization/deserialization with either protobuf-net or JSON.

**Note:** For flows that do not require a return value you can use the **None** type available within the library.
```C#
using Flow.Core.Areas.Returns;
using Flow.Core.Common.Models;

Flow<None> successFlow = Flow<None>.Success();
/*
    *or using the implicit operator
*/
Flow<None> successFlow = None.Value;
```

## Installation

Download and install the latest version of the [Flow.Core](https://www.nuget.org/packages/Flow.Core) package from [nuget.org](https://www.nuget.org/) using your preferred client tool.

## Example usage

In essence, there are three stages when working with flowing results: initiating a flow and populating values into a **Flow&lt;T&gt;**, chaining together functions that return Flow instances, or interjecting results with 
actions before continuing the flow, and finally, retrieving the values from a flow.

```c#
/*
    * To put values into a flow, use static methods or implicit conversions (operators). A success value cannot be null or a type derived from Failure, and
    * a failure cannot be null; it must be a type derived from Failure. Otherwise, an exception will be raised.
*/

var explicitFlowSuccess = Flow<int>.Success(42);
var explicitFlowFailure = Flow<int>.Failed(new Failure.ApplicationFailure("Some reason for the failure"));

Flow<int> implicitFlowSuccess = 42;
Flow<int> implicitFlowFailure = new Failure.ApplicationFailure("Some reason for the failure");

public Flow<int> SomeFlowReturningMethod() => 42;

```

**Note:** For methods that don't return a Flow but you want the result of the method to initiate the flow, you can also use the extension method 'Then'. This extension passes the T value to the flow returning func parameter.
```c#
public int NumberFunction(int value) => value + 10;
public Flow<string> NumberToString(int number) => number.ToString(); 

var flow = NumberFunction(10).Then(x => NumberToString(x));//or just Then(NumberToString); 
/*
    * flow is Flow<string> with a success value of "20"
/*

```

**Note:** Flow's success and failure values are stored in private properties. Access to these values is facilitated through the **Match** method or its wrapping extension methods. This method accepts functions to execute 
upon failure or success. These functions are provided with the private values, allowing them to either return or transform them before returning. While this concept may seem unfamiliar at first, it encourages developers 
to consider appropriate actions in the event of failure, contrasting with a more imperative, traditional approach of simply enclosing code in a try-catch block and throwing exceptions, especially when the exception is more 
accurately categorized as a known (anticipated) failure rather than an exceptional case.


The following snippet is taken from the method 'RunGettingValuesOutOfAFlowExamples' within the demo program. It is recommended to peruse this project to familiarize yourself with how to work with flow.

```c#
var successFlow = Flow<int>.Success(42);
var craftyFlow  = Flow<int>.Failed(new Failure.ApplicationFailure("Bad flow."));
/*
    * The purpose of structures like Flow is to return a value, either a success or failure value, which forces you to think about both scenarios. 
    * In order to get a value out of a Flow, you need to use its Match method, which requires you to provide functions for both success and failure.
*/
int successFlowOutput = successFlow.Match(failure => 24, success => success); //lambda that just takes the success value and then outputs the success value (success => success)
int craftyFlowOutput  = craftyFlow.Match(failure => 24, success => success);  //if its a failure return the value 24 (_ => 24).

// Or we could do some stuff and then decide what values to use for success or failure.

successFlowOutput = successFlow.OnSuccess(success => Console.WriteLine($"Success value: {success}"))
                                    .OnFailure(failure => Console.WriteLine(failure.Reason))//or do some other work in here that returns a Flow
                                        .Finally(_ => 0, success => success); //return 0 or the success value

Console.WriteLine($"The value to use is: {successFlowOutput}\r\n");

craftyFlowOutput = craftyFlow.OnSuccess(success => Console.WriteLine(success))//this will be skipped as the flow is a failed flow
                                    .OnFailure(failure => Console.WriteLine($"Failure reason: {failure.Reason}"))
                                        .Finally(_ => 24, success => success); //return 24 or the success value

Console.WriteLine($"The value to use is: { craftyFlowOutput}\r\n");

craftyFlowOutput = craftyFlow.OnSuccess(success => Console.WriteLine(success))//this will be skipped as the flow is a failed flow
                                .OnFailure(failure =>
                                {
                                    Console.WriteLine($"Failure reason: {failure.Reason} > Starting another operation . . .");
                                    return 4224;//using implicit conversion could have used Flow<int>.Success(4242) or ran another flow returning function.
                                                // or even just an int returning function, which would then be implicitly converted;. be careful with non flow returning functions!!!
                                    /*
                                             
                                        * var subFlowOutput = someMethod.OnSuccess(success => DoMoreStuff(success))
                                        *                                   .OnSuccess(success => YetMoreStuff(success))
                                        *                                      .Finally(_ => 0, success => success
                                        *                                      
                                        * return subFlowOutput // or leave off the finally so the subFlowOutput is just a Flow which the outer finally can deal with   
                                        *                      // or do some other related flow work and then return Flow<int>.Success(4224) etc
                                    */

                                }).Finally(failure => 24, success => success);//return 24 or the success value



Console.WriteLine($"The value to use is: {craftyFlowOutput}");

```

As a further example from the demo, although just a simple example, it is the complete process / flow, from the client using a client side service, calling the server via gRPC code first, that uses a query handler
class to query a database, and return the results. Known / anticipated errors would be converted to a Failure and the flow returned as normal.
```c#
var searchResults = await NetworkingUtility
                            .HasInternetConnection(alwaysOn: false)// just a flag in the demo to cause random failures
                                .OnSuccess(_ => _customerService.CustomerSearch("Al"))
                                    .OnFailure(failure => Console.Out.WriteLineAsync($"{failure.GetType().Name}: {failure.Reason}"))
                                        .Finally(failure => [], success => success.SearchResults.ToList());

public async Task<Flow<CustomerSearchResponse>> CustomerSearch(string companyName)

    => await _customerService.CustomerSearch(new CustomerSearch(companyName)).TryCatchGrpcResult();

// Server side

public async Task<Flow<CustomerSearchResponse>> CustomerSearch(CustomerSearch instruction, CallContext context = default)

    => await _searchHandler.Handle(new CustomerSearchQuery(instruction.CompanyName))
                                .ReturnAs(success => new CustomerSearchResponse(success));


public class CustomerSearchQueryHandler(CustomersDbReadOnly readOnlyDB, IDbExceptionHandler exceptionHandler)
{
    private readonly CustomersDbReadOnly _readOnlyDB        = readOnlyDB;
    private readonly IDbExceptionHandler _exceptionHandler  = exceptionHandler;

    public Task<Flow<IEnumerable<CustomerSearchResult>>> Handle(CustomerSearchQuery searchCriteria)
        /*
            * Utility helper within the Flow library so you can wrap method content in a try catch that returns a flow
            * Using an injected handler specific to database exceptions for the entire app. You could have specific global handlers for file input, messaging etc
         */ 
        => FlowHandler.TryToFlow
            (
                async () => await _readOnlyDB.Customers.Where(c => c.CompanyName.Contains(searchCriteria.companyName)).Select(Customer.ProjectToCustomerSearchResult).ToListAsync(),
                exception => _exceptionHandler.Handle<IEnumerable<CustomerSearchResult>>(exception)
            );

}

```

## Extension methods and utility class FlowHandler

```c#
using Flow.Core.Areas.Extensions
using Flow.Core.Areas.Utilities;
```

Flow is short-circuiting: if it’s a failure, then the success function is not executed; if it’s a success, then the failure function is not executed. When chaining, the same flow goes from start to finish unless
changed by a function. For example, if you have five chained **OnSuccess** functions and the first one fails, the failed flow is passed to each subsequent function, but each **OnSuccess** will do nothing other 
than pass it on to the next, and so forth.

The following extension also have Task based variants/overloads.

* ***Then:*** Takes some object/value and chains a flow-returning function that gets passed the object/value.

* ***OnSuccess:*** Executes only on success, passes the current success value to an action, and returns the current flow or executes a flow-returning function.

* ***OnSuccessTry:*** A variant of OnSuccess that is wrapped in a try-catch block; you supply the exception handler to use in the catch block.

* ***OnFailure:*** Executes only on failure, passes the current failure to an action, and returns the current flow or executes a flow-returning function.

* ***ReturnAs:*** A wrapper around Map to make things easier to read in circumstances where you are changing the type of value in the flow.

* ***Finally:*** Returns the value from the flow via two functions: one for failure and the other for success. This is essentially a wrapper around Match.

There is also a helper utility, FlowHandler, which has a method **TryToFlow** that allows you to start a chain with the method content wrapped in a try-catch block. You provide the handler for the catch block (similar to OnSuccessTry). All of the above methods have been used in the simple client/server demo included in the source code.


**Note:** There are also overloads for these methods that return and/or accept tasks, enabling asynchronous flow handling.



## Failures classes and serialization

The Flow library uses an abstract base class called **Failure** that serves as the foundation for all derived failure classes. These derived classes do not add any additional properties and are merely created 
to represent a type or category of failure. You can use these classes and create subtypes of failures by utilizing the ***SubTypeID*** property (e.g., mapped to an enum), or you can create your own specific 
failure-derived types. The derived failure classes have been decorated with the appropriate protobuf-net and System.Text.Json attributes, including type discriminators added to the base class as required for 
polymorphic serialization and deserialization.

If you plan to flow results across process boundaries, involving serialization and deserialization of results, you may need to add the appropriate serializer attributes to your types and custom failure classes. 
Regarding custom failures, protobuf-net gRPC requires adding the additional custom failure types via the RuntimeTypeModel, while System.Text.Json requires a type resolver. This needs to be done on both the server 
and the client. The demo project shows how to implement this for both gRPC and JSON

The **Failure** class exposes the following properties that can be used internally and/or displayed to end users:

* ***string Reason:*** The reason for the failure.

* ***Dictionary<string, string> Details:*** Additional details regarding the failure. One example usage could be for validation failures where you may assign the key as the field name and the value as the associated error message to display. It is optional in the constructor and will be an empty dictionary if not supplied.

* ***int SubTypeID:*** An integer value to allow for subcategories of the failure type as opposed to creating a new failure type.

* ***bool CanRetry:*** A boolean indicating whether the operation can be retried. It’s a trivial process to create an extension method that accepts a flow-returning function that is then wrapped in retry logic that could use this property.

* ***DateTime? OccurredAt:*** Optional, set to the current UTC value on the server or client if not supplied.

* ***Exception? Exception:*** Optional, this property by design is not serialized by either protobuf-net or JSON.


The following derived failure types are available for use:

* ***NoFailure*** - used as a default failure value when the Flow is a success.

* NetworkFailure
* DatabaseFailure
* FileSystemFailure
* ValidationFailure
* SecurityFailure
* ConfigurationFailure
* ServiceFailure
* CloudStorageFailure
* ItemNotFoundFailure
* MessagingFailure
* GeneralFailure
* ConstraintFailure
* DomainFailure
* ApplicationFailure
* IOFailure
* HardwareFailure
* SystemFailure
* ConnectionFailure
* TaskCancellationFailure
* InternetConnectionFailure
* CacheFailure

* ***UnknownFailure*** - has been given a type discriminator of 199 to allow for additional types to be added to the library. Your custom failures, if serialized will need a value greater than 199 for its type discriminator. The same values have been assigned to both the JsonDerivedType and ProtoInclude attributes.


***All feedback, positive or negative, is welcome.***

## Acknowledgments

This library uses the attributes from the [protobuf-net](https://www.nuget.org/packages/protobuf-net) package to enable flow to be serialized using ProtoBuf gRPC code-first. 
Many thanks to Marc Gravell and the contributors who work on that project. 


<img src="https://raw.githubusercontent.com/code-dispenser/Flow/main/Assets/icon-64.png" align="middle" height="32px" alt="Flow icon" />
<a target="_blank" href="https://icons8.com/icon/hem6U5Zq0H2c/flow">Flow</a> icon by <a target="_blank" href="https://icons8.com">Icons8</a>

<!--
![icon](https://raw.githubusercontent.com/code-dispenser/Flow/main/Assets/icon-48.png) Flow [icon by Icons8](https://icons8.com)
-->
