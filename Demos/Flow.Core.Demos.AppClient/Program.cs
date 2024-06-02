using Flow.Core.Demos.AppClient.Common.Interceptors;
using Flow.Core.Demos.AppClient.Services;
using Flow.Core.Demos.Contracts.Areas.Customers;
using Flow.Core.Demos.Contracts.Utilities;
using Grpc.Net.ClientFactory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc.ClientFactory;
namespace Flow.Core.Demos.AppClient;

internal class Program
{

    /*
        * To see Flow be used across boundaries (serialized and deserialized) in a more realistic manor, both a client and web server are used.
        * For those like myself running Visual studio you can configure multi-startup projects or or just simply right click on the AppServer project Debug menu -> start new instance
        * leave that running and then just right click on the AppClient and choose the same option. If you only close the console at the end the server
        * will remain running, saves a few seconds of waiting.
    */ 


    private static readonly string _kestrelHost = "https://localhost:7209";  // <<<< Choose your poison for the ConfigureMicrosoftContainer.
    private static readonly string _iisHost     = "https://localhost:44357"; // <<<< Choose your poison for the ConfigureMicrosoftContainer.


    static async Task Main()
    {
        CustomFailureHelper.AddCustomFailuresToGrpcRuntimeModel();
        /*
            * Logging disabled to concentrate on our console messages.
            * NOTE Remember to add the correct host for the server you want to run, it need https for grpc!
        */
        var resolver     = ConfigureMicrosoftContainer(_iisHost); // <<<<<<< make sure this matches what you start.  

        var grpcExamples = resolver.GetRequiredService<Grpc_With_Flow_Examples>();
        var jsonExamples = resolver.GetRequiredService<Http_Json_With_Flow_Examples>();


        var flowExamples = new Flow_Basics_Examples();

        flowExamples.RunPuttingValuesInAFlowExamples();
        flowExamples.RunGettingValuesOutOfAFlowExamples();

        await Console.Out.WriteLineAsync("\r\n< < < < Code below is the client server demo, make sure the server is up and running > > > >\r\n");

        /*
            * The CustomerID ALFKI already exists and as such should cause a database constraint exception to be converted to a Failure used in the examples. 
        */
        CustomerData data = new CustomerData() { CustomerID ="ALFKI", CompanyName ="ACME", ContactName="Road", ContactTitle="Runner" };

        /*
            * To use a grpc interceptor instead of the example TryCatchGrpcResult as of the Common\Extensions folder that are chained to the end of 
            * the GrpcCustomerService methods, just uncomment the options.InterceptorRegistrations line in the ConfigureMicrosoftContainer. 
            * The interceptor will circumvent the extension method, so you can leave them attached.
        */
        await Console.Out.WriteLineAsync("\r\n< < < < Flow results via Grpc code first > > > >\r\n");

        await grpcExamples.RunCustomerSearchExample();
        await grpcExamples.RunCustomerAddExample(data);
        await grpcExamples.RunRaiseServerGrpcExceptionExample();
        await grpcExamples.RunUseCustomFailureExample();

        await Console.Out.WriteLineAsync("\r\n< < < < Flow results via web api json > > > >\r\n");

        await jsonExamples.RunCustomerSearchExample();
        await jsonExamples.RunCustomerAddExample(data);
        await jsonExamples.RunRaiseServerSideExceptionExample();
        await jsonExamples.RunUseCustomFailureExample();

        /*
            * Simple ways to check that your objects serialize ok for json or grpc code first. 
        */
        var serializableExamples = new Checking_Serialization_Examples();

        await Console.Out.WriteLineAsync("\r\n< < < < Json serialization/deserialization of types and custom failures > > > >\r\n");

        serializableExamples.GeneralJsonTypeExamples();
        serializableExamples.CustomFailuresViaJsonExample();

        await Console.Out.WriteLineAsync("\r\n< < < < Grpc serialization/deserialization of types and custom failures > > > >\r\n");

        serializableExamples.GeneralGrpcTypeExamples();
        serializableExamples.CustomFailuresViaGrpcExample();

        Console.ReadLine();
    }

    private static ServiceProvider ConfigureMicrosoftContainer(string hostPath)

        => Host.CreateApplicationBuilder()
                .Logging
                    .ClearProviders()
                .Services
                    .AddSingleton<HttpClientDelegatingHandler>()
                    .AddHttpClient<JsonCustomerService>("", config => config.BaseAddress = new Uri(hostPath + "/customers/"))
                    .AddHttpMessageHandler<HttpClientDelegatingHandler>()
                .Services
                    .AddSingleton<GrpcCustomerServiceInterceptor>()
                    .AddCodeFirstGrpcClient<IGrpcCustomerService>(options =>
                    {
                        options.Address =  new Uri(hostPath);
                        //options.InterceptorRegistrations.Add(new InterceptorRegistration(InterceptorScope.Channel, services => services.GetRequiredService<GrpcCustomerServiceInterceptor>()));
                    })
                .Services
                    .AddTransient<GrpcCustomerService>()
                    .AddTransient<Grpc_With_Flow_Examples>()
                    .AddTransient<JsonCustomerService>()
                    .AddTransient<Http_Json_With_Flow_Examples>()

                .BuildServiceProvider();


    }



