using Flow.Core.Areas.Returns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.Json;
using Flow.Core.Demos.Contracts.Utilities;
using Flow.Core.Areas.Extensions;
using System.Text.Json.Serialization.Metadata;
namespace Flow.Core.Demos.AppClient.Common.Extensions;

public static class JsonFlowExt
{
    //Either _serializerOptions approaches below will work, in effect they use the same code as can be seen in the Contracts project Utilities\CustomFailuresHelper.cs
    // NB you need to include PropertyNameCaseInsensitive = true
    // 
    //public static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions 
    //                                                                        {   TypeInfoResolver            = new DefaultJsonTypeInfoResolver().WithAddedModifier(CustomFailureHelper.AddCustomFailuresToJsonResolver), 
    //                                                                            PropertyNameCaseInsensitive = true 
    //                                                                        };

    public static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions { TypeInfoResolver = CustomFailureHelper.GetJsonCustomFailureTypeResolver(), PropertyNameCaseInsensitive = true };
    /*
        * Just a simple example that unwraps the response and catch exceptions such as the web server not being up or serialization issues etc.
        * The JsonSerializerOptions / TypeInfoResolver are only required for any Custom Failures; failures defined in the flow library package are fine without these options.
    */
    public static async Task<Flow<T>> TryCatchJsonResult<T>(this Task<HttpResponseMessage> @this)
    {
        try
        {
            var response = await @this;
            /*
                * If you are using and serializing a flow from the server then the failure type should have been set on the server.
                * i.e there is probably no need to check the status code, but you could do that if necessary.
                * You can intercept and change the failure if need be i.e  flowResult.ReturnAs(failure => new ConvertedFailure(), success => success);
             */
            
            Type resultType = typeof(Flow<>).MakeGenericType(typeof(T));

            return (Flow<T>)(await response.Content.ReadFromJsonAsync(resultType, _serializerOptions))!; 

        }
        catch (Exception ex)
        {
            /*
                * If connection failure/no server up and running. 
            */ 
            return Flow<T>.Failed(new Failure.UnknownFailure("A problem has occurred, please try again in a few minutes. If the problem persists please contact the system administrator.",exception:ex));
        }

    }

}
