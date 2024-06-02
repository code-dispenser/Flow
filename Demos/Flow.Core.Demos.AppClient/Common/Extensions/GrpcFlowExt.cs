using Flow.Core.Areas.Returns;
using Grpc.Core;

namespace Flow.Core.Demos.AppClient.Common.Extensions;

public static class GrpcFlowExt
{
    /*
        * For more more rpc control you can use client side grpc interceptors that you can register via the AddCodeFirstGrpcClient and the GrpcClientFactoryOptions.
        * This extension is just an example/alternate way of handling unexpected grpc issues.
    */
    public static async Task<Flow<T>> TryCatchGrpcResult<T>(this Task<Flow<T>> @this)
    {
        try
        {
            return await @this;
        }
        catch (RpcException ex) 
        {
            return Flow<T>.Failed(new Failure.UnknownFailure("A problem has occurred, please try again in a few minutes. If the problem persists please contact the system administrator.",exception: ex));
        }
    }

}
