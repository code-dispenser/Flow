using Flow.Core.Areas.Extensions;
using Flow.Core.Areas.Returns;

namespace Flow.Core.Demos.AppClient;

public class Flow_Basics_Examples
{

/*
    * Flow<T> is just a very simple structure that holds either a success value or a failure value, but importantly it is a return value (structure)
    * that can be chained. The first question that everybody asks is, if the SuccessValue and FailureValue properties are private how do you get the values?
     
    * At some point you will most likely need the values, however, its the flow of these values which is important - hence the library name.

    * Flow has methods such as map, bind and match that can transform the values inside the flow (structure), return flows, and provide means 
    * to access the success or failure value. It also has public properties IsSuccess and IsFailure to inform which type of value is held in Flow.
     
    * There are a few extension methods that help you use flow in a more fluent/dsl way, that use the underlying map, bind and match methods. 
    * You can of course create your own extension methods using the map, bind and match functions, to form your own DSL etc
    
    * Flow is a monadic like structure (due the Bind function). It can sometimes help to think of flow (or monadic structures) as a box that contains a value inside it.
    * Map - allows you to pass functions to transform the value in the box, and then returns a box with the transformed value in it. 
    * Bind - allows you to pass functions that must return a box which can have the transformation of the exiting value inside it.
    * 
    * The key point is that the output from Map and Bind is a box, which can then be chained with other functions that return boxes (Flow).
    * 
    * Match - allows you to provide functions for both failure and successes. If its a success then that function is executed and returns the value, otherwise the
    * failure function is executed and returns a value.

*/
    public void RunPuttingValuesInAFlowExamples()
    {
        /*
            * To put values into a flow, use the static methods or implicit (operators) conversion. A success value cannot be null or a type derived from Failure and
            * a failure cannot be null and must be a type derived from Failure, otherwise an exception will be raised.
        */ 

        var explicitFlowSuccess = Flow<int>.Success(42);
        var explicitFlowFailure = Flow<int>.Failed(new Failure.ApplicationFailure("Failed"));

        Flow<int> implicitFlowSuccess = 42;
        Flow<int> implicitFlowFailure = new Failure.ApplicationFailure("Failed");

        ExceptionHandler(() => Flow<string>.Success(null!));
        ExceptionHandler(() => Flow<string>.Failed(null!));
  
        static void ExceptionHandler(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message + "\r\n"); }
        }
        /*
            * You can also pipe a value into an operation that then returns a flow using the extension Then 
        */
        var someFlow    = "Test".Then<string,string>(s => s + " Value");  // implicit conversion so needs the types specified by Then<T,TOut> in this instance

        var anotherFlow = SomeStringReturningMethod()
                                .Then(s => SomeFlowReturningMethod(s));//or simply .Then(SomeFlowReturningMethod) although I prefer the former to see what is happening; types can be inferred from method signature

        static string SomeStringReturningMethod()   => "Test";

        static Flow<string> SomeFlowReturningMethod(string someInput)   => String.Concat(someInput, " Value"); 

        _ = anotherFlow.OnSuccess(Console.WriteLine);//Voodoo magic?

    }
    public void RunGettingValuesOutOfAFlowExamples()
    {
        var successFlow = Flow<int>.Success(42);
        var craftyFlow  = Flow<int>.Failed(new Failure.ApplicationFailure("Bad flow."));
        /*
            * The purpose of structures like flow are to return a value, either a success or failure value, which forces you to think about both scenarios. 
            * In order to get a value out of a Flow you need to use its Match method, which requires you to provide functions for both success and failure.
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
        /*
            * The finally extension method is just a wrapper over match. 
             
            * Flow is short-circuiting if its a failure then success is not executed, if its a success then failure is not executed
            * When chaining, the same flow goes from start to finish unless changed by a function, so if you have five chained onSuccess's and the first one fails that failed
            * Flow gets passed to each, but each onSuccess will do nothing other than to pass it on to the next etc.

            * Then          - Take some object/value and chain a flow returning function that gets passed the object/value
            * OnSuccess     - execute only on success, pass the current success value to an action and return the current flow or execute a flow returning function
            * OnSuccessTry  - a variant of the above that is wrapped in a try catch that you supply the exception handler to use in the catch block.
            * OnFailure     - execute only on failure, pass the current failure to an action and return the current flow or execute a flow returning function
            * ReturnAs      - just a wrapper around Map to make things easier to read in circumstances where you are changing the type of thing in the box/flow.
            * Finally       - return the value from the flow via two functions, one func for if its a failure the other func for success. 
            * 
            * There is also a helper utility FlowHandler that has a method TryToFlow which allows you to start a chain with the method content wrapped in try
            * catch that you provide the handler for (like OnSuccessTry). All of the above have been used in the simple client/server demo.
            * 
            * references 
            * 
            * using Flow.Core.Areas.Returns    - this contains the Flow class and all of the defined Failure classes
            * using Flow.Core.Areas.Extensions - contains the extension methods
            * using Flow.Core.Areas.Utilities  - contains the FlowHandler utility class
        */
    }





}
