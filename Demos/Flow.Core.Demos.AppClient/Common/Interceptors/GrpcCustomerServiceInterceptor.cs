using Flow.Core.Areas.Returns;
using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Flow.Core.Demos.AppClient.Common.Interceptors;

public class GrpcCustomerServiceInterceptor : Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var call = continuation(request, context);

        return new AsyncUnaryCall<TResponse>(ResponseHandler(call.ResponseAsync), call.ResponseHeadersAsync, call.GetStatus, call.GetTrailers, call.Dispose);
    }

    private async Task<TResponse> ResponseHandler<TResponse>(Task<TResponse> serviceResponse)
    {
        try
        {
            return await serviceResponse;
        }
        catch (RpcException rpEx)
        {
            if (IsFlow(typeof(TResponse)) == true)
            {
                var typeParam        = typeof(TResponse).GetGenericArguments()[0];
                var createFailedFlow = typeof(GrpcCustomerServiceInterceptor)
                                            .GetMethod("CreateFailedFlow", BindingFlags.NonPublic | BindingFlags.Instance)!
                                            .MakeGenericMethod(typeParam);
                /*
                    * Check type of exception and create appropriate failure 
                */
                return (TResponse)createFailedFlow.Invoke(this, [new Failure.MessagingFailure("A problem occurred",exception:rpEx)])!;
            }

            throw new ApplicationException();
        }
    }
    private Flow<T> CreateFailedFlow<T>(Failure failure) => Flow<T>.Failed(failure);

    private bool IsFlow(Type responseType)

        => responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Flow<>);
}
