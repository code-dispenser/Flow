using Flow.Core.Areas.Extensions;
using Flow.Core.Areas.Returns;

namespace Flow.Core.Demos.AppClient;

public class Flow_Basics_Examples
{

    /*
        * Flow<T> is a simple structure that holds either a success value or a failure value. It serves as a return value that can be chained.
        * While the SuccessValue and FailureValue properties are private, accessing the values is still possible when needed.
        * The flow of values is crucial, hence the library name.
        * 
        * Flow provides methods like map, bind, and match to transform the values inside the structure, return flows, and access the success or failure value.
        * It also exposes public properties IsSuccess and IsFailure to indicate the type of value held in Flow.
        * 
        * Several extension methods are available to use Flow in a more fluent or DSL-like manner, utilizing the underlying map, bind, and match methods.
        * Developers can create their own extension methods using these functions to create custom DSLs.
        * 
        * Flow resembles a monadic structure, especially due to the Bind function. It can be thought of as a box containing a value inside it.
        * Map allows passing functions to transform the value in the box and returns a box with the transformed value.
        * Bind allows passing functions that return a box containing the transformation of the existing value.
        * 
        * The output from Map and Bind is a box that can be further chained with other functions returning boxes (Flow).
        * 
        * Match allows providing functions for both success and failure cases. If it's a success, the success function is executed and returns a value; otherwise, the
        * failure function is executed and returns a value.
    */

    public void RunPuttingValuesInAFlowExamples()
    {
        /*
            * To put values into a flow, use static methods or implicit conversions (operators). A success value cannot be null or a type derived from Failure, and
            * a failure cannot be null; it must be a type derived from Failure. Otherwise, an exception will be raised.
        */

        var explicitFlowSuccess = Flow<int>.Success(42);
        var explicitFlowFailure = Flow<int>.Failed(new Failure.ApplicationFailure("Some reason for the failure"));

        Flow<int> implicitFlowSuccess = 42;
        Flow<int> implicitFlowFailure = new Failure.ApplicationFailure("Some reason for the failure");


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
        /*
            * The finally extension method is just a wrapper over match. 
             
            * Flow is short-circuiting if its a failure then success is not executed, if its a success then failure is not executed
            * When chaining, the same flow goes from start to finish unless changed by a function, so if you have five chained onSuccess's and the first one fails that failed
            * Flow gets passed to each, but each onSuccess will do nothing other than to pass it on to the next etc.

            * Then          - Take some object/value and chain a flow returning function that gets passed the object/value
            * OnSuccess     - execute only on success, pass the current success value to an action and return the current flow or execute a flow returning function
            * OnSuccessTry  - a variant of the above that is wrapped in a try catch that you supply the exception handler to use in the catch block.
            * OnFailure     - execute only on failure, pass the current failure to an action and return the current flow or execute a flow returning function
            * ReturnAs      - just a wrapper around Map to make things easier to read in circumstances where you are changing the type of value in flow.
            * Finally       - return the value from the flow via two functions, one func for if its a failure the other func for success. A wrapper around Match.
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
