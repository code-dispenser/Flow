using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flow.Core.Demos.AppClient.Common.Interceptors;

public class HttpClientDelegatingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await base.SendAsync(request, cancellationToken);
            /*
                * We could process the response here and check for status codes and add failures if not already added by the server. 
                * or just let our custom extension do it so the delegatinghandler can just work as normal for any traffic
            */
            return response;
        }
        catch
        {
            // Run the call without the server running and let this be thrown, which will be caught by to our custom extension.
            throw;
        }
    }


}
